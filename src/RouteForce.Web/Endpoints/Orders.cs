using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RouteForce.Application.Common.DTOs;
using RouteForce.Application.Common.Interfaces;
using RouteForce.Core.Enums;
using RouteForce.Core.Models;
using RouteForce.Web.Configurations;
using RouteForce.Web.Pages.Admin;

namespace RouteForce.Web.Endpoints;

public class Orders : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization();
        groupBuilder.MapGet("", GetOrders);
        groupBuilder.MapGet("register/form", CreatePage);
        groupBuilder.MapGet("register", RegisterForm);
        groupBuilder.MapPost("register", Register).DisableAntiforgery();
    }

    private async Task<IResult> GetOrders(
        IApplicationDbContext context,
        HttpContext httpContext,
        [AsParameters] GetOrdersRequest request)
    {
        var businessIdClaim = httpContext.User.FindFirst("BusinessId");

        if (businessIdClaim == null || !int.TryParse(businessIdClaim.Value, out var businessId))
        {
            return Results.Unauthorized();
        }

        var pageSize = request.GetValidPageSize();
        var pageNumber = request.GetValidPageNumber();

        var query = context.Orders
            .AsNoTracking()
            .Include(o => o.PersonalReceiver)
            .Include(o => o.RouteCheckpoints)
                .ThenInclude(rc => rc.Checkpoint)
            .Where(o => o.BusinessId == businessId);
        
        var totalCount = await query.CountAsync();

        var orders = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync()
            .ConfigureAwait(false);

        var orderDtos = orders.Select(o =>
        {
            var completedCheckpoints = o.RouteCheckpoints.Count(rc => rc.Status == RouteCheckPointStatus.Confirmed);
            var totalCheckpoints = o.RouteCheckpoints.Count;
            var currentCheckpoint = o.RouteCheckpoints
                .OrderBy(rc => rc.SequenceNumber)
                .FirstOrDefault(rc => rc.Status == RouteCheckPointStatus.Pending);

            return new OrderListItemDto
            {
                Id = o.Id,
                TrackingNumber = o.TrackingNumber,
                ProductReferenceId = o.ProductReferenceId,
                ReceiverName = o.PersonalReceiver.Name,
                ReceiverEmail = o.PersonalReceiver.Email,
                DeliveryAddress = $"{o.DeliveryAddress.AddressLine}, {o.DeliveryAddress.City}, {o.DeliveryAddress.State} {o.DeliveryAddress.PostalCode}",
                Status = o.Status,
                CreatedDate = o.CreatedDate,
                EstimatedDeliveryDate = o.EstimatedDeliveryDate,
                ActualDeliveryDate = o.ActualDeliveryDate,
                CurrentCheckpoint = currentCheckpoint?.Checkpoint.Name,
                CompletedCheckpoints = completedCheckpoints,
                TotalCheckpoints = totalCheckpoints
            };
        }).ToList();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new RazorComponentResult<_OrdersList>(new
        {
            Orders = orderDtos,
            TotalCount = totalCount,
            CurrentPage = pageNumber,
            TotalPages = totalPages,
            PageSize = pageSize
        });
    }

    private IResult CreatePage()
    {
        return new RazorComponentResult<CreateOrder>(new
        {
            FormStep = 1 
        });
    }

    private async Task<IResult> RegisterForm(
        IApplicationDbContext context,
        HttpContext httpContext)
    {
        var businessIdClaim = httpContext.User.FindFirst("BusinessId");

        if (businessIdClaim == null || !int.TryParse(businessIdClaim.Value, out var businessId))
        {
            return Results.Unauthorized();
        }

        var receivers = await context.PersonalReceivers
            .AsNoTracking()
            .Where(r => r.CreatedByBusinessId == businessId)
            .ToListAsync()
            .ConfigureAwait(false);

        var receiverIds = receivers.Select(r => r.Id).ToList();
        var addresses = await context.DeliveryAddresses
            .AsNoTracking()
            .Where(a => receiverIds.Contains(a.PersonalReceiverId))
            .ToListAsync()
            .ConfigureAwait(false);

        return new RazorComponentResult<_RegisterOrderForm>(new
        {
            Receivers = receivers,
            DeliveryAddresses = addresses
        });
    }

    private async Task<IResult> Register(
        [FromForm] CreateOrderRequest request,
        IApplicationDbContext context,
        HttpContext httpContext)
    {
        var businessIdClaim = httpContext.User.FindFirst("BusinessId");

        if (businessIdClaim == null || !int.TryParse(businessIdClaim.Value, out var businessId))
        {
            return Results.Unauthorized();
        }

        var receiver = await context.PersonalReceivers
            .AsNoTracking()
            .Include(r => r.SavedAddresses)
            .FirstOrDefaultAsync(r => 
                r.Id == request.PersonalReceiverId && 
                r.CreatedByBusinessId == businessId);

        if (receiver == null)
        {
            return Results.BadRequest("Invalid receiver");
        }

        DeliveryAddress? deliveryAddress = null;
        if (request.SelectedDeliveryAddressId.HasValue)
        {
            deliveryAddress = await context.DeliveryAddresses
                .FirstOrDefaultAsync(a => 
                    a.Id == request.SelectedDeliveryAddressId.Value && 
                    a.PersonalReceiverId == request.PersonalReceiverId);

            if (deliveryAddress == null)
            {
                return Results.BadRequest("Invalid delivery address");
            }
        }
        else
        {
            deliveryAddress = receiver.SavedAddresses.FirstOrDefault(a => a.IsDefault);
            if (deliveryAddress == null)
            {
                return Results.BadRequest("No delivery address selected");
            }
        }

        var checkpoint = await context.Checkpoints
            .FirstOrDefaultAsync(c =>
                c.CheckpointType == CheckpointType.DeliveryAddress &&
                c.ManagedByBusinessId == businessId &&
                c.Address.AddressLine == deliveryAddress.Address.AddressLine);

        if (checkpoint == null)
        {
            return Results.BadRequest("Delivery checkpoint not found");
        }

        var trackingNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

        var warehouseCheckpoint = await context.Checkpoints
            .FirstOrDefaultAsync(c =>
                c.ManagedByBusinessId == businessId &&
                c.CheckpointType == CheckpointType.Warehouse &&
                c.IsActive);

        if (warehouseCheckpoint == null)
        {
            return Results.BadRequest("No active warehouse found. Please set up a warehouse checkpoint first.");
        }

        var order = new Order
        {
            TrackingNumber = trackingNumber,
            BusinessId = businessId,
            PersonalReceiverId = request.PersonalReceiverId,
            SelectedDeliveryAddressId = deliveryAddress.Id,
            DeliveryAddress = deliveryAddress.Address,
            DeliveryCheckpointId = checkpoint.Id,
            ProductReferenceId = request.ProductReferenceId,
            Status = OrderStatus.Created,
            EstimatedDeliveryDate = request.EstimatedDeliveryDate,
            Notes = request.Notes ?? string.Empty,
            CreatedDate = DateTime.UtcNow
        };

        await context.Orders.AddAsync(order);
        await context.SaveChangesAsync().ConfigureAwait(false);

        var startCheckpoint = new RouteCheckpoint
        {
            OrderId = order.Id,
            CheckpointId = warehouseCheckpoint.Id,
            SequenceNumber = 1,
            Status = RouteCheckPointStatus.Pending,
            ExpectedArrival = DateTime.UtcNow,
            Notes = "Order pickup from warehouse"
        };

        var endCheckpoint = new RouteCheckpoint
        {
            OrderId = order.Id,
            CheckpointId = checkpoint.Id,
            SequenceNumber = 2,
            Status = RouteCheckPointStatus.Pending,
            ExpectedArrival = request.EstimatedDeliveryDate ?? DateTime.UtcNow.AddDays(3),
            Notes = "Final delivery to receiver"
        };

        await context.RouteCheckpoints.AddAsync(startCheckpoint);
        await context.RouteCheckpoints.AddAsync(endCheckpoint);
        await context.SaveChangesAsync().ConfigureAwait(false);

        var deliveryToken = new WebhookToken
        {
            Token = Token.Create(TokenType.DeliveryConfirmation, IssuedToType.Business),
            OrderId = order.Id,
            ExpirationDate = DateTime.UtcNow.AddDays(30),
            IsActive = true,
            UsageLimit = 999,
            UsedCount = 0,
            CreatedDate = DateTime.UtcNow
        };

        var receiverToken = new WebhookToken
        {
            Token = Token.Create(TokenType.PersonalReceiverConfirmation, IssuedToType.PersonalReceiver),
            OrderId = order.Id,
            ExpirationDate = DateTime.UtcNow.AddDays(30),
            IsActive = true,
            UsageLimit = 1,
            UsedCount = 0,
            CreatedDate = DateTime.UtcNow
        };

        await context.WebhookTokens.AddAsync(deliveryToken);
        await context.WebhookTokens.AddAsync(receiverToken);
        await context.SaveChangesAsync().ConfigureAwait(false);

        return Results.Ok();
    }
}