using RouteForce.Application.Common.DTOs;
using RouteForce.Application.Common.Interfaces;
using RouteForce.Core.Enums;
using RouteForce.Core.Models;

namespace RouteForce.Application.Service.RouteCheckpoint;

public class RouteCheckpointService : IRouteCheckpointService
{
    private readonly IApplicationDbContext _context;

    public RouteCheckpointService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CreateCheckpointsForNewOrder(
        int orderId,
        int businessId,
        int warehouseCheckpointId,
        int deliveryCheckpointId,
        CreateOrderRequest request)
    {
        var routeCheckpoints = new List<Core.Models.RouteCheckpoint>();
        var sequenceNumber = 1;

        var startCheckpoint = new Core.Models.RouteCheckpoint
        {
            OrderId = orderId,
            CheckpointId = warehouseCheckpointId,
            SequenceNumber = sequenceNumber++,
            Status = RouteCheckPointStatus.Pending,
            ExpectedArrival = DateTime.UtcNow,
            Notes = "Order pickup from warehouse"
        };
        routeCheckpoints.Add(startCheckpoint);

        if (!string.IsNullOrWhiteSpace(request.IntermediateCheckpointName) &&
            !string.IsNullOrWhiteSpace(request.AddressLine))
        {
            var intermediateCheckpoint = new Core.Models.Checkpoint
            {
                Name = request.IntermediateCheckpointName,
                Address = new Address(
                    request.AddressLine,
                    request.City ?? "",
                    request.State ?? "",
                    request.PostalCode ?? "",
                    request.Country ?? "",
                    request.Latitude,
                    request.Longitude
                ),
                ContactPoint = new ContactPoint("", "", ""),
                CheckpointType = CheckpointType.TransitHub,
                ManagedByBusinessId = businessId,
                RequiresConfirmation = false,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                Notes = "Auto-created intermediate checkpoint"
            };

            await _context.Checkpoints.AddAsync(intermediateCheckpoint);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            var intermediateRouteCheckpoint = new Core.Models.RouteCheckpoint
            {
                OrderId = orderId,
                CheckpointId = intermediateCheckpoint.Id,
                SequenceNumber = sequenceNumber++,
                Status = RouteCheckPointStatus.Pending,
                ExpectedArrival = DateTime.UtcNow.AddDays(1),
                Notes = "Intermediate stop"
            };
            routeCheckpoints.Add(intermediateRouteCheckpoint);
        }

        var endCheckpoint = new Core.Models.RouteCheckpoint
        {
            OrderId = orderId,
            CheckpointId = deliveryCheckpointId,
            SequenceNumber = sequenceNumber,
            Status = RouteCheckPointStatus.Pending,
            ExpectedArrival = request.EstimatedDeliveryDate ?? DateTime.UtcNow.AddDays(3),
            Notes = "Final delivery to receiver"
        };
        routeCheckpoints.Add(endCheckpoint);

        await _context.RouteCheckpoints.AddRangeAsync(routeCheckpoints);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }
}