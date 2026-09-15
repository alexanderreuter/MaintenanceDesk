using System.ComponentModel.DataAnnotations;

namespace MaintenanceDesk.Api.Dtos;

public record AssignTechnicianDto
{
    [Required]
    public Guid? TechnicianId { get; init; }
}
