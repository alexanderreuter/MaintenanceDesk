using MaintenanceDesk.Api.Domain;
using static MaintenanceDesk.Api.Domain.MaintenanceStatus;

namespace MaintenanceDesk.Tests.Domain;

public class MaintenanceStatusTransitionsTests
{
    public static readonly TheoryData<MaintenanceStatus, MaintenanceStatus> AllowedStatusChanges = new()
    {
        { Submitted, Triaged },
        { Submitted, Cancelled },
        { Triaged, Cancelled },
        { Assigned, InProgress },
        { Assigned, Cancelled },
        { InProgress, Resolved },
        { InProgress, Cancelled },
        { Resolved, InProgress },
        { Resolved, Closed },
    };

    [Theory]
    [MemberData(nameof(AllowedStatusChanges))]
    public void CanChangeStatus_AllowsEveryChangeInTheStatusFlow(MaintenanceStatus from, MaintenanceStatus to)
    {
        var allowed = MaintenanceStatusTransitions.CanChangeStatus(from, to);

        Assert.True(allowed, $"{from} -> {to} should be allowed.");
    }

    [Fact]
    public void CanChangeStatus_RejectsEveryOtherChange()
    {
        var specified = AllowedStatusChanges
            .Select(row => ((MaintenanceStatus)row[0], (MaintenanceStatus)row[1]))
            .ToHashSet();

        var wronglyAllowed = new List<string>();

        foreach (var from in Enum.GetValues<MaintenanceStatus>())
        {
            foreach (var to in Enum.GetValues<MaintenanceStatus>())
            {
                if (!specified.Contains((from, to)) && MaintenanceStatusTransitions.CanChangeStatus(from, to))
                {
                    wronglyAllowed.Add($"{from} -> {to}");
                }
            }
        }

        Assert.Empty(wronglyAllowed);
    }

    [Theory]
    [InlineData(Submitted, false)]
    [InlineData(Triaged, true)]
    [InlineData(Assigned, true)]
    [InlineData(InProgress, true)]
    [InlineData(Resolved, false)]
    [InlineData(Closed, false)]
    [InlineData(Cancelled, false)]
    public void CanAssign_OnlyAfterTriageAndBeforeResolution(MaintenanceStatus status, bool expected)
    {
        var canAssign = MaintenanceStatusTransitions.CanAssign(status);

        Assert.Equal(expected, canAssign);
    }
}
