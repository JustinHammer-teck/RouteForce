namespace RouteForce.Core.Models;

public class DeliveryAddress
{
    public int Id { get; set; }
    public int PersonalReceiverId { get; set; }
    public string Label { get; set; } = string.Empty;
    public Address Address { get; set; }
    public bool IsDefault { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string Notes { get; set; } = string.Empty;
    public PersonalReceiver PersonalReceiver { get; set; }
}
