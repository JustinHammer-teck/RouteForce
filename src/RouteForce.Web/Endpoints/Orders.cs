using Htmx;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RouteForce.Application.Common.DTOs;
using RouteForce.Application.Common.Interfaces;
using RouteForce.Application.Service.Order;
using RouteForce.Application.Service.WebhookToken;
using RouteForce.Core.Enums;
using RouteForce.Core.Models;
using RouteForce.Web.Configurations;
using RouteForce.Web.Pages.Admin;
using RouteForce.Web.Pages.Order;

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
        groupBuilder.MapGet("confirm-receive", ConfirmReceive).AllowAnonymous();
        groupBuilder.MapGet("update-checkpoint", UpdateCheckpoint).AllowAnonymous();
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
            .Include(o => o.WebhookTokens)
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

            var deliveryToken = o.WebhookTokens
                .FirstOrDefault(t => t.Token.Type == TokenType.DeliveryConfirmation && t.IsActive)
                ?.Token.Value;

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
                TotalCheckpoints = totalCheckpoints,
                DeliveryToken = deliveryToken
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
        HttpContext httpContext,
        IWebhookService webhookService,
        IOrderService orderService
        )
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

        var deliveryCheckpoint = await context.Checkpoints
            .FirstOrDefaultAsync(c =>
                c.CheckpointType == CheckpointType.DeliveryAddress &&
                c.ManagedByBusinessId == businessId &&
                c.Address.AddressLine == deliveryAddress.Address.AddressLine);

        if (deliveryCheckpoint == null)
        {
            return Results.BadRequest("Delivery checkpoint not found");
        }

        var warehouseCheckpoint = await context.Checkpoints
            .FirstOrDefaultAsync(c =>
                c.ManagedByBusinessId == businessId &&
                c.CheckpointType == CheckpointType.Warehouse &&
                c.IsActive);

        if (warehouseCheckpoint == null)
        {
            return Results.BadRequest("No active warehouse found. Please set up a warehouse checkpoint first.");
        }

        await orderService.CreateNewOrderAsync(
            request, 
            businessId, 
            deliveryAddress, 
            deliveryCheckpoint, 
            warehouseCheckpoint);

        httpContext.Response.Htmx(x => x.Redirect("/"));
        return Results.Ok();
    }

    private IResult ConfirmReceive([FromQuery] string token, [FromQuery] bool success = false)
    {
        return new RazorComponentResult<ConfirmReceive>(new
        {
            Token = token,
            Success = success
        });
    }
    
    private IResult UpdateCheckpoint([FromQuery] string token, [FromQuery] bool success = false)
    {
        return new RazorComponentResult<UpdateCheckPoint>(new
        {
            Token = token,
            Success = success
        });
    }
}