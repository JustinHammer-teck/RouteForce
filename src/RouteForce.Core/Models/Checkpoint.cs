using RouteForce.Core.Enums;

namespace RouteForce.Core.Models;

public class Checkpoint
{
    public int Id { get; set; }
    public string CheckpointName { get; set; }
    public string AddressLine { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string ContactName { get; set; }
    public string ContactPhone { get; set; }
    public string ContactEmail { get; set; }
    public CheckpointType CheckpointType { get; set; }
    public int? ManagedByBusinessID { get; set; }
    public bool RequiresConfirmation { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string Notes { get; set; } 
}