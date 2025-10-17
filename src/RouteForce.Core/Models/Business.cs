namespace RouteForce.Core.Models;

public class Business
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int BusinessOwnerId { get; set; }
    public Address BusinessAddress { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string Notes { get; set; } = string.Empty;
    
    public User BusinessOwner { get; set; }
    public ICollection<Checkpoint> Warehouses { get; set; } = new List<Checkpoint>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<User> Users { get; set; } = new List<User>();
}
