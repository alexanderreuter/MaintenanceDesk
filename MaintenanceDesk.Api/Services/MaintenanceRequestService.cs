using MaintenanceDesk.Api.Data;
using MaintenanceDesk.Api.Domain;
using MaintenanceDesk.Api.Events;
using Microsoft.EntityFrameworkCore;
using RequestResult = MaintenanceDesk.Api.Services.ServiceResult<MaintenanceDesk.Api.Domain.MaintenanceRequest>;

namespace MaintenanceDesk.Api.Services;

public class MaintenanceRequestService(
    MaintenanceDeskDbContext db,
    TimeProvider timeProvider,
    IEventPublisher events,
    ILogger<MaintenanceRequestService> logger)
{
    public const int MaxPageSize = 100;

    public static TimeSpan ResponseTime(MaintenancePriority priority) => priority switch
    {
        MaintenancePriority.Emergency => TimeSpan.FromHours(4),
        MaintenancePriority.High => TimeSpan.FromHours(24),
        MaintenancePriority.Normal => TimeSpan.FromDays(5),
        MaintenancePriority.Low => TimeSpan.FromDays(14),

        _ => throw new ArgumentOutOfRangeException(nameof(priority), priority, "Unknown priority."),
    };

    public async Task<RequestResult> CreateAsync(
        Guid residentId,
        string title,
        string description,
        MaintenanceCategory category,
        MaintenancePriority priority,
        CancellationToken cancellationToken = default)
    {
        // The unit comes from the resident, so a request can never name a unit its reporter doesn't live in.
        var unitId = await db.Residents
            .Where(r => r.Id == residentId)
            .Select(r => (Guid?)r.UnitId)
            .SingleOrDefaultAsync(cancellationToken);

        if (unitId is null)
        {
            return RequestResult.Invalid($"Resident '{residentId}' does not exist.");
        }

        var now = timeProvider.GetUtcNow();

        var request = new MaintenanceRequest
        {
            UnitId = unitId.Value,
            ReportedByResidentId = residentId,
            Title = title,
            Description = description,
            Category = category,
            Priority = priority,
            Status = MaintenanceStatus.Submitted,
            ReportedAt = now,
            ResponseDeadline = now + ResponseTime(priority),
        };

        db.MaintenanceRequests.Add(request);
        await db.SaveChangesAsync(cancellationToken);

        await PublishAsync(new MaintenanceRequestEvent(request.Id, MaintenanceRequestEventType.Created, now));
        await PublishAsync(
            new MaintenanceRequestEvent(request.Id, MaintenanceRequestEventType.DeadlineReached, request.ResponseDeadline),
            deliverAt: request.ResponseDeadline);

        return RequestResult.Success(request);
    }

    public Task<MaintenanceRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.MaintenanceRequests
            .AsNoTracking()
            .SingleOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<PagedResult<MaintenanceRequest>> ListAsync(
        MaintenanceStatus? status,
        MaintenancePriority? priority,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);

        var query = db.MaintenanceRequests.AsNoTracking();

        if (status is not null)
        {
            query = query.Where(r => r.Status == status.Value);
        }

        if (priority is not null)
        {
            query = query.Where(r => r.Priority == priority.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(r => r.ReportedAt)
            .ThenBy(r => r.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<MaintenanceRequest>(items, page, pageSize, totalCount);
    }

    public async Task<RequestResult> ChangeStatusAsync(
        Guid id,
        MaintenanceStatus newStatus,
        string? resolutionNotes,
        CancellationToken cancellationToken = default)
    {
        var request = await db.MaintenanceRequests.SingleOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (request is null)
        {
            return RequestResult.NotFound($"Maintenance request '{id}' does not exist.");
        }

        if (!MaintenanceStatusTransitions.CanChangeStatus(request.Status, newStatus))
        {
            return RequestResult.Conflict(DescribeRejectedStatusChange(request.Status, newStatus));
        }

        request.Status = newStatus;

        if (newStatus == MaintenanceStatus.Resolved)
        {
            request.ResolvedAt = timeProvider.GetUtcNow();
            request.ResolutionNotes = resolutionNotes;
        }
        else if (newStatus != MaintenanceStatus.Closed)
        {
            // Resolution details only exist while resolved or closed; reopening clears them.
            request.ResolvedAt = null;
            request.ResolutionNotes = null;
        }

        await db.SaveChangesAsync(cancellationToken);

        await PublishAsync(new MaintenanceRequestEvent(request.Id, MaintenanceRequestEventType.StatusChanged, timeProvider.GetUtcNow()));

        return RequestResult.Success(request);
    }

    public async Task<RequestResult> AssignAsync(
        Guid id,
        Guid technicianId,
        CancellationToken cancellationToken = default)
    {
        var request = await db.MaintenanceRequests.SingleOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (request is null)
        {
            return RequestResult.NotFound($"Maintenance request '{id}' does not exist.");
        }

        if (!MaintenanceStatusTransitions.CanAssign(request.Status))
        {
            return RequestResult.Conflict(request.Status == MaintenanceStatus.Submitted
                ? "Triage the request before assigning a technician."
                : $"A request that is {request.Status} cannot be assigned a technician.");
        }

        var technicianExists = await db.Technicians.AnyAsync(t => t.Id == technicianId, cancellationToken);

        if (!technicianExists)
        {
            return RequestResult.Invalid($"Technician '{technicianId}' does not exist.");
        }

        request.AssignedTechnicianId = technicianId;

        // First assignment moves the request along; reassigning leaves the status alone.
        var statusChanged = request.Status == MaintenanceStatus.Triaged;
        if (statusChanged)
        {
            request.Status = MaintenanceStatus.Assigned;
        }

        await db.SaveChangesAsync(cancellationToken);

        if (statusChanged)
        {
            await PublishAsync(new MaintenanceRequestEvent(request.Id, MaintenanceRequestEventType.StatusChanged, timeProvider.GetUtcNow()));
        }

        return RequestResult.Success(request);
    }

    // The change is already saved, so a failed publish is logged rather than failing (and inviting a duplicate retry).
    // Closing this gap properly takes a transactional outbox. No request token, event should go out even if the client left.
    private async Task PublishAsync(MaintenanceRequestEvent @event, DateTimeOffset? deliverAt = null)
    {
        try
        {
            if (deliverAt is null)
            {
                await events.PublishAsync(@event);
            }
            else
            {
                await events.ScheduleAsync(@event, deliverAt.Value);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to publish {EventType} for maintenance request {RequestId}.", @event.Type, @event.RequestId);
        }
    }

    private static string DescribeRejectedStatusChange(MaintenanceStatus from, MaintenanceStatus to)
    {
        if (to == MaintenanceStatus.Assigned)
        {
            return "A request becomes Assigned by assigning a technician, not by changing its status.";
        }

        var allowed = MaintenanceStatusTransitions.AllowedNextStatuses(from);

        return allowed.Count == 0
            ? $"Cannot change status from {from} to {to}. {from} is a final status."
            : $"Cannot change status from {from} to {to}. Allowed from {from}: {string.Join(", ", allowed)}.";
    }
}
