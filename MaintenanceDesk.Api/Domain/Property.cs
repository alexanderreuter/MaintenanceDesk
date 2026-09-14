namespace MaintenanceDesk.Api.Domain;

/// <summary>
/// A building managed by the property manager. Reference data; seeded, with no
/// HTTP surface of its own.
/// </summary>
public class Property
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public required string StreetAddress { get; set; }

    /// <summary>
    /// Held as text, not a number: postal codes can carry leading zeros and
    /// separators, and are never arithmetic.
    /// </summary>
    public required string PostalCode { get; set; }

    public required string City { get; set; }
}
