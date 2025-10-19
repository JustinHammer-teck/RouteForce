using RouteForce.Core.Enums;

namespace RouteForce.Application.Common.DTOs;

public class OrderListItemDto
{
    public int Id { get; set; }
    public string TrackingNumber { get; set; } = string.Empty;
    public string ProductReferenceId { get; set; } = string.Empty;
    public string ReceiverName { get; set; } = string.Empty;
    public string ReceiverEmail { get; set; } = string.Empty;
    public string DeliveryAddress { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? EstimatedDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
    public string? CurrentCheckpoint { get; set; }
    public int CompletedCheckpoints { get; set; }
    public int TotalCheckpoints { get; set; }
}
