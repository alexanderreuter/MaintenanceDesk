namespace MaintenanceDesk.Worker.Api;

public record MaintenanceRequestSummary(
    Guid Id,
    string Title,
    string Status,
    string Priority,
    DateTimeOffset ResponseDeadline,
    Guid? AssignedTechnicianId);
