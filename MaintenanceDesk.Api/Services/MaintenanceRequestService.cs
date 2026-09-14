using MaintenanceDesk.Api.Data;
using MaintenanceDesk.Api.Domain;
using Microsoft.EntityFrameworkCore;
using RequestResult = MaintenanceDesk.Api.Services.ServiceResult<MaintenanceDesk.Api.Domain.MaintenanceRequest>;

namespace MaintenanceDesk.Api.Services;

public class MaintenanceRequestService(MaintenanceDeskDbContext db, TimeProvider timeProvider)
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

        // TODO (task 6): return Conflict for transitions the status flow does not allow.

        request.Status = newStatus;

        if (newStatus == MaintenanceStatus.Resolved)
        {
            request.ResolvedAt = timeProvider.GetUtcNow();
            request.ResolutionNotes = resolutionNotes;
        }

        await db.SaveChangesAsync(cancellationToken);

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

        var technicianExists = await db.Technicians.AnyAsync(t => t.Id == technicianId, cancellationToken);

        if (!technicianExists)
        {
            return RequestResult.Invalid($"Technician '{technicianId}' does not exist.");
        }

        // TODO (task 6): return Conflict when the request's status does not allow assignment.

        request.AssignedTechnicianId = technicianId;

        // First assignment moves the request along; reassigning later leaves the status alone.
        if (request.Status == MaintenanceStatus.Triaged)
        {
            request.Status = MaintenanceStatus.Assigned;
        }

        await db.SaveChangesAsync(cancellationToken);

        return RequestResult.Success(request);
    }
}
