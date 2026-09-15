using MaintenanceDesk.Api.Domain;

namespace MaintenanceDesk.Api.Dtos;

public record MaintenanceRequestDto(
    Guid Id,
    Guid UnitId,
    Guid ReportedByResidentId,
    string Title,
    string Description,
    MaintenanceCategory Category,
    MaintenancePriority Priority,
    MaintenanceStatus Status,
    DateTimeOffset ReportedAt,
    DateTimeOffset ResponseDeadline,
    Guid? AssignedTechnicianId,
    DateTimeOffset? ResolvedAt,
    string? ResolutionNotes)
{
    public static MaintenanceRequestDto From(MaintenanceRequest request) => new(
        Id: request.Id,
        UnitId: request.UnitId,
        ReportedByResidentId: request.ReportedByResidentId,
        Title: request.Title,
        Description: request.Description,
        Category: request.Category,
        Priority: request.Priority,
        Status: request.Status,
        ReportedAt: request.ReportedAt,
        ResponseDeadline: request.ResponseDeadline,
        AssignedTechnicianId: request.AssignedTechnicianId,
        ResolvedAt: request.ResolvedAt,
        ResolutionNotes: request.ResolutionNotes);
}
