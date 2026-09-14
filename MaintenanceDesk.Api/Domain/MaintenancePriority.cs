namespace MaintenanceDesk.Api.Domain;

/// <summary>
/// How urgent a maintenance request is. The response deadline is derived from
/// this when the request is created.
/// </summary>
public enum MaintenancePriority
{
    Low = 1,
    Normal = 2,
    High = 3,
    Emergency = 4
}
