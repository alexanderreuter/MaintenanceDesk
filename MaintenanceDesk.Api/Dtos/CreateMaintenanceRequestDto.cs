using System.ComponentModel.DataAnnotations;
using MaintenanceDesk.Api.Domain;

namespace MaintenanceDesk.Api.Dtos;

public record CreateMaintenanceRequestDto
{
    // Value types are nullable here so a missing field fails [Required] instead of binding to its default.
    [Required]
    public Guid? ResidentId { get; init; }

    [Required]
    [StringLength(200)]
    public string Title { get; init; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; init; } = string.Empty;

    [Required]
    public MaintenanceCategory? Category { get; init; }

    [Required]
    public MaintenancePriority? Priority { get; init; }
}
