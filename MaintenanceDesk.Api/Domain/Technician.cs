namespace MaintenanceDesk.Api.Domain;

/// <summary>
/// A repair worker a maintenance request can be assigned to. Reference data;
/// seeded, with no HTTP surface of its own.
/// </summary>
public class Technician
{
    public Guid Id { get; set; }

    public required string FullName { get; set; }

    public required string Email { get; set; }

    /// <summary>
    /// The technician's line of work, for example "Plumber". Free text: nothing
    /// in scope matches a trade to a request category, so an enum would be
    /// precision without a purpose.
    /// </summary>
    public required string Trade { get; set; }
}
