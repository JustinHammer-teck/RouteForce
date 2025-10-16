using RouteForce.Core.Enums;

namespace RouteForce.Core.Models;

public class RouteCheckpoint
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; }
    public int CheckpointId { get; set; }
    public Checkpoint Checkpoint { get; set; }
    public int SequenceNumber { get; set; }
    public RouteCheckPointStatus Status { get; set; } = RouteCheckPointStatus.Pending;
    public int? ConfirmedByTokenId { get; set; }
    public WebhookToken ConfirmedByToken { get; set; }
    public DateTime? ConfirmationTimestamp { get; set; }
    public DateTime? ExpectedArrival { get; set; }
    public DateTime? ActualArrival { get; set; }
    public string Notes { get; set; } = string.Empty;
}