using System.ComponentModel.DataAnnotations;
using MaintenanceDesk.Api.Domain;
using MaintenanceDesk.Api.Services;

namespace MaintenanceDesk.Api.Dtos;

public record MaintenanceRequestQuery
{
    public MaintenanceStatus? Status { get; init; }

    public MaintenancePriority? Priority { get; init; }

    // Upper bound keeps (Page - 1) * PageSize from overflowing an int.
    [Range(1, int.MaxValue / MaintenanceRequestService.MaxPageSize)]
    public int Page { get; init; } = 1;

    [Range(1, MaintenanceRequestService.MaxPageSize)]
    public int PageSize { get; init; } = 20;
}
