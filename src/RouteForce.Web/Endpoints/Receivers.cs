using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RouteForce.Application.Common.DTOs;
using RouteForce.Application.Common.Interfaces;
using RouteForce.Core.Enums;
using RouteForce.Core.Models;
using RouteForce.Web.Configurations;
using RouteForce.Web.Pages.Admin;
using RouteForce.Web.Sessions;

namespace RouteForce.Web.Endpoints;

public class Receivers : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization();
        groupBuilder.MapGet("register", RegisterForm);
        groupBuilder.MapPost("register", Register).DisableAntiforgery();
    }

    private IResult RegisterForm()
    {
        return new RazorComponentResult<_RegisterReceiverForm>();
    }

    private async Task<IResult> Register(
        [FromForm] CreateReceiverRequest request,
        IApplicationDbContext context,
        HttpContext httpContext,
        SessionManager sessionManager
        )
    {
        var createReceiverRequest = JsonSerializer.Serialize(request);
        sessionManager.SetSessionValue("CreateReceiverRequest ", createReceiverRequest);
        
        var businessIdClaim = httpContext.User.FindFirst("BusinessId");

        if (businessIdClaim == null || !int.TryParse(businessIdClaim.Value, out var businessId))
        {
            return Results.Unauthorized();
        }

        var receiver = new PersonalReceiver
        {
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone,
            Notes = request.Notes ?? string.Empty,
            CreatedByBusinessId = businessId,
            CreatedDate = DateTime.UtcNow
        };

        await context.PersonalReceivers.AddAsync(receiver);
        await context.SaveChangesAsync().ConfigureAwait(false);

        var address = new Address(
            request.AddressLine,
            request.City,
            request.State,
            request.PostalCode,
            request.Country,
            request.Latitude,
            request.Longitude
        );

        var deliveryAddress = new DeliveryAddress
        {
            PersonalReceiverId = receiver.Id,
            Label = request.AddressLabel ?? "Primary",
            Address = address,
            IsDefault = true,
            IsActive = true,
            CreatedDate = DateTime.UtcNow,
            Notes = string.Empty
        };

        await context.DeliveryAddresses.AddAsync(deliveryAddress);
        await context.SaveChangesAsync().ConfigureAwait(false);

        var checkpoint = new Checkpoint
        {
            Name = $"{receiver.Name} - {deliveryAddress.Label}",
            Address = address,
            ContactPoint = new ContactPoint(receiver.Name, receiver.Phone, receiver.Email),
            CheckpointType = CheckpointType.DeliveryAddress,
            ManagedByBusinessId = businessId,
            RequiresConfirmation = true,
            IsActive = true,
            CreatedDate = DateTime.UtcNow,
            Notes = $"Delivery checkpoint for {receiver.Name}"
        };

        await context.Checkpoints.AddAsync(checkpoint);
        await context.SaveChangesAsync().ConfigureAwait(false);

        return new RazorComponentResult<_RegisterOrderForm>();
    }
}