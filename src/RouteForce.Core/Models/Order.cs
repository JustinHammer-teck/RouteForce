using RouteForce.Core.Enums;

namespace RouteForce.Core.Models;

public class Order
{
    public int Id { get; set; }
    public string TrackingNumber { get; set; }
    public int BusinessId { get; set; }
    public Business Business { get; set; }
    public int PersonalReceiverId { get; set; }
    public PersonalReceiver PersonalReceiver { get; set; }
    public int? DeliveryServiceTemplateId { get; set; }
    public Address DeliveryAddress { get; set; }
    public int? SelectedDeliveryAddressId { get; set; }
    public DeliveryAddress SelectedDeliveryAddress { get; set; }
    public int DeliveryCheckpointId { get; set; }
    public Checkpoint DeliveryCheckpoint { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Created;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? EstimatedDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
    public string Notes { get; set; } = string.Empty;

    public string ProductReferenceId { get; set; }

    public ICollection<RouteCheckpoint> RouteCheckpoints { get; set; } = new List<RouteCheckpoint>();
    public ICollection<WebhookToken> WebhookTokens { get; set; } = new List<WebhookToken>();
    
    
}
