using RouteForce.Application.Common.DTOs;
using RouteForce.Application.Common.Interfaces;
using RouteForce.Application.Service.RouteCheckpoint;
using RouteForce.Application.Service.WebhookToken;
using RouteForce.Core.Enums;
using RouteForce.Core.Models;

namespace RouteForce.Application.Service.Order;

public class OrderService : IOrderService
{
    private readonly IApplicationDbContext _context;
    private readonly IWebhookService _webhookService;
    private readonly IRouteCheckpointService _checkpointService;

    public OrderService(IApplicationDbContext context, 
        IWebhookService webhookService, 
        IRouteCheckpointService checkpointService)
    {
        _context = context;
        _webhookService = webhookService;
        _checkpointService = checkpointService;
    }

    public async Task CreateNewOrderAsync(CreateOrderRequest request, 
        int businessId,
        DeliveryAddress deliveryAddress,
        Core.Models.Checkpoint deliveryCheckpoint,
        Core.Models.Checkpoint warehouseCheckpoint
        )
    {
        var trackingNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        var order = new Core.Models.Order
        {
            TrackingNumber = trackingNumber,
            BusinessId = businessId,
            PersonalReceiverId = request.PersonalReceiverId,
            SelectedDeliveryAddressId = deliveryAddress.Id,
            DeliveryAddress = deliveryAddress.Address,
            DeliveryCheckpointId = deliveryCheckpoint.Id,
            ProductReferenceId = request.ProductReferenceId,
            Status = OrderStatus.Created,
            EstimatedDeliveryDate = request.EstimatedDeliveryDate,
            Notes = request.Notes ?? string.Empty,
            CreatedDate = DateTime.UtcNow
        };

        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync().ConfigureAwait(false);

        await _checkpointService.CreateCheckpointsForNewOrder(
            order.Id, 
            businessId, 
            warehouseCheckpoint.Id,
            deliveryAddress.Id, 
            request).ConfigureAwait(false);
        
        await _webhookService.CreateDeliveryToken(order.Id);
    }
}