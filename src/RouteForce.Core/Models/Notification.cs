using RouteForce.Core.Enums;

namespace RouteForce.Core.Models;

public class Notification
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string RecipientEmail { get; set; } = string.Empty;
    public string RecipientPhone { get; set; } = string.Empty;
    public string WebhookConfirmationUrl { get; set; } = string.Empty;
    public NotificationStatus Status { get; set; } = NotificationStatus.Pending;
    public NotificationDeliveryMethod DeliveryMethod { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? SentDate { get; set; }
    public string Message { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    
    public Order Order { get; set; }
}