namespace MaintenanceDesk.Api.Domain;

public class Resident
{
    public Guid Id { get; set; }

    public Guid UnitId { get; set; }

    public required string FullName { get; set; }

    public required string Email { get; set; }

    // Optional
    public string? PhoneNumber { get; set; }
}
