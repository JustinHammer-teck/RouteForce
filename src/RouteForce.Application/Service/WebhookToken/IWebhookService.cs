namespace RouteForce.Application.Service.WebhookToken;

public interface IWebhookService
{
    Task CreateDeliveryToken(int orderId);
    Task CreateReceiverToken(int orderId);
}