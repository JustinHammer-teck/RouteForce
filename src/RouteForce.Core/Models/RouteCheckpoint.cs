using RouteForce.Core.Enums;

namespace RouteForce.Core.Models;

public class RouteCheckpoint
{
    public int Id { get; set; }
    public int OrderID { get; set; }
    public int CheckpointID { get; set; }
    public int ManagedByBusinessID { get; set; }
    public int SequenceNumber { get; set; }
    public RouteCheckPointStatus Status { get; set; } = RouteCheckPointStatus.Pending;
    public int ConfirmedByTokenID { get; set; }
    public DateTime? ConfirmationTimestamp { get; set; }
    public DateTime? ExpectedArrival { get; set; }
    public DateTime? ActualArrival { get; set; }
}