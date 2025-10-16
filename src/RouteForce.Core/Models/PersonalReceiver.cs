namespace RouteForce.Core.Models;

public class PersonalReceiver
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public int CreatedByBusinessId { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string Notes { get; set; } = string.Empty;

    public Business CreatedByBusiness { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<DeliveryAddress> SavedAddresses { get; set; } = new List<DeliveryAddress>();
}