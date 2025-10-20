using RouteForce.Application.Common.DTOs;

namespace RouteForce.Application.Service.RouteCheckpoint;

public interface IRouteCheckpointService
{
    Task CreateCheckpointsForNewOrder(
        int orderId,
        int businessId,
        int warehouseCheckpointId,
        int deliveryCheckpointId,
        CreateOrderRequest request);
}