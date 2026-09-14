namespace MaintenanceDesk.Api.Domain;

/// <summary>
/// Where a maintenance request is in its lifecycle.
/// </summary>
/// <remarks>
/// The normal flow is
/// Submitted -> Triaged -> Assigned -> InProgress -> Resolved -> Closed.
/// Cancelled is reachable from any state before Resolved. Transitions are
/// guarded; the guard is the single piece of real logic in this project.
/// </remarks>
public enum MaintenanceStatus
{
    Submitted = 1,
    Triaged = 2,
    Assigned = 3,
    InProgress = 4,
    Resolved = 5,
    Closed = 6,
    Cancelled = 7
}
