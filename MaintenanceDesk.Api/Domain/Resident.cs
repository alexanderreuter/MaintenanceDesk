namespace MaintenanceDesk.Api.Domain;

/// <summary>
/// A person living in a unit, who reports maintenance requests. Reference data;
/// seeded, with no HTTP surface of its own.
/// </summary>
/// <remarks>
/// The domain term is "tenant", but that word means something else in a backend
/// context, so this is a Resident throughout.
/// </remarks>
public class Resident
{
    public Guid Id { get; set; }

    public Guid UnitId { get; set; }

    public required string FullName { get; set; }

    public required string Email { get; set; }

    /// <summary>
    /// Optional: not every resident leaves a phone number.
    /// </summary>
    public string? PhoneNumber { get; set; }
}
