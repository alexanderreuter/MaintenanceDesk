namespace MaintenanceDesk.Api.Domain;

public class Unit
{
    public Guid Id { get; set; }

    public Guid PropertyId { get; set; }

    public required string Designation { get; set; }

    public int Floor { get; set; }
}
