using RouteForce.Core.Enums;

namespace RouteForce.Core.Model;

public class Order
{
    public int OrderId { get; set; }
    public int TrackingId { get; set; }
    public int SenderBusinessId { get; set; } // not null
    public int ReceiverId { get; set; } //not null
    public int SenderCheckPointId { get; set; }
    public int ReceiverCheckPointId { get; set; }
    public OrderStatus Status { get; set; } //Enum: OnGoing, Fulfill, Canceled
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime ExpectedDeliveryDate { get; set; }
    public DateTime ActualDeliveryDate { get; set; }
    public int CurrentRouteCheckPointId { get; set; }
    public string PublicTrackingUrl { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    
}