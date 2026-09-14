namespace MaintenanceDesk.Api.Domain;

public class Technician
{
    public Guid Id { get; set; }

    public required string FullName { get; set; }

    public required string Email { get; set; }

    public required string Trade { get; set; }
}
