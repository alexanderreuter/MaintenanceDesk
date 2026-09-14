namespace MaintenanceDesk.Api.Domain;

public class MaintenanceRequest
{
    public Guid Id { get; set; }

    public Guid UnitId { get; set; }

    public Guid ReportedByResidentId { get; set; }

    public required string Title { get; set; }

    public required string Description { get; set; }

    public MaintenanceCategory Category { get; set; }

    public MaintenancePriority Priority { get; set; }

    public MaintenanceStatus Status { get; set; }

    /// <summary>
    /// When the resident submitted the request. Set on create. 
    /// DateTimeOffset rather than DateTime throughout: this runs in Azure
    /// later, where the host's local time is not the reader's, and an instant
    /// without an offset is ambiguous.
    /// </summary>
    public DateTimeOffset ReportedAt { get; set; }

    /// <summary>
    /// The SLA clock. Derived from <see cref="Priority"/> when the request is
    /// created; the derivation lives in the service layer, not here.
    /// </summary>
    public DateTimeOffset ResponseDeadline { get; set; }

    public Guid? AssignedTechnicianId { get; set; }

    public DateTimeOffset? ResolvedAt { get; set; }

    public string? ResolutionNotes { get; set; }
}
