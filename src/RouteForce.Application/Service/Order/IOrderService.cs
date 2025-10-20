using RouteForce.Application.Common.DTOs;
using RouteForce.Core.Models;

namespace RouteForce.Application.Service.Order;

public interface IOrderService
{
    Task CreateNewOrderAsync(
        CreateOrderRequest request,
        int businessId,
        DeliveryAddress deliveryAddress,
        Core.Models.Checkpoint deliveryCheckpoint,
        Core.Models.Checkpoint warehouseCheckpoint
    );
}