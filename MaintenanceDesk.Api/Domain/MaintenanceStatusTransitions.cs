using static MaintenanceDesk.Api.Domain.MaintenanceStatus;

namespace MaintenanceDesk.Api.Domain;

public static class MaintenanceStatusTransitions
{
    private static readonly IReadOnlyDictionary<MaintenanceStatus, MaintenanceStatus[]> AllowedStatusChanges =
        new Dictionary<MaintenanceStatus, MaintenanceStatus[]>
        {
            [Submitted] = [Triaged, Cancelled],
            [Triaged] = [Cancelled],
            [Assigned] = [InProgress, Cancelled],
            [InProgress] = [Resolved, Cancelled],
            [Resolved] = [InProgress, Closed],
            [Closed] = [],
            [Cancelled] = [],
        };

    public static IReadOnlyList<MaintenanceStatus> AllowedNextStatuses(MaintenanceStatus from) =>
        AllowedStatusChanges.TryGetValue(from, out var allowed) ? allowed : [];

    public static bool CanChangeStatus(MaintenanceStatus from, MaintenanceStatus to) =>
        AllowedNextStatuses(from).Contains(to);

    public static bool CanAssign(MaintenanceStatus status) =>
        status is Triaged or Assigned or InProgress;
}
