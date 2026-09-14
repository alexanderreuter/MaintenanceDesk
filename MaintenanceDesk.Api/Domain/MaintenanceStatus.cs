namespace MaintenanceDesk.Api.Domain;

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
