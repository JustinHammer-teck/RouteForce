using RouteForce.Core.Enums;

namespace RouteForce.Core.Models;


public record Address(
    string AddressLine,
    string City,
    string State,
    string PostalCode,
    string Country,
    decimal? Latitude,
    decimal? Longitude);

public record ContactPoint(string Name, string Phone, string Email);

public class Checkpoint
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Address Address { get; set; }
    public ContactPoint ContactPoint { get; set; }
    public CheckpointType CheckpointType { get; set; }
    public int? ManagedByBusinessId { get; set; }
    public int? DeliveryServiceTemplateId { get; set; }
    public bool RequiresConfirmation { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string Notes { get; set; } = string.Empty;
    public Business ManagedByBusiness { get; set; }
    public ICollection<RouteCheckpoint> RouteCheckpoints { get; set; } = new List<RouteCheckpoint>();
}