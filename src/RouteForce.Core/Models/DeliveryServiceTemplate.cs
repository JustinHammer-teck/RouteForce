namespace RouteForce.Core.Models;

public class DeliveryServiceTemplate
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public int EstimatedDeliveryDays { get; set; }
    public string ServiceCode { get; set; }
    public ICollection<Checkpoint> ServiceCheckpoints { get; set; } = new List<Checkpoint>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
