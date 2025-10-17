namespace RouteForce.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendDeliveryConfirmationEmailAsync(
        string recipientEmail,
        string recipientName,
        string trackingNumber,
        string confirmationUrl,
        CancellationToken cancellationToken = default); 
}