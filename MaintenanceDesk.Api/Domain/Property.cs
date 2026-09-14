namespace MaintenanceDesk.Api.Domain;

public class Property
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public required string StreetAddress { get; set; }

    // Held as text, not a number: postal codes can carry leading zeros and separators, and are never arithmetic.
    public required string PostalCode { get; set; }

    public required string City { get; set; }
}
