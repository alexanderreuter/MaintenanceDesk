using System.ComponentModel.DataAnnotations;
using MaintenanceDesk.Api.Domain;

namespace MaintenanceDesk.Api.Dtos;

public record ChangeStatusDto
{
    [Required]
    public MaintenanceStatus? Status { get; init; }

    [StringLength(2000)]
    public string? ResolutionNotes { get; init; }
}
