using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RouteForce.Application.Common.Interfaces;
using RouteForce.Core.Enums;
using RouteForce.Web.Configurations;

namespace RouteForce.Web.Endpoints;

public class Webhooks : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.AllowAnonymous();
        groupBuilder.MapPost("confirm-receive", ConfirmReceive);
        groupBuilder.MapPost("update-checkpoint", UpdateCheckpoint);
    }

    private async Task<IResult> ConfirmReceive(
        [FromForm] string token,
        IApplicationDbContext context)
    {
        var webhookToken = await context.WebhookTokens
            .Include(t => t.Order)
            .ThenInclude(o => o.RouteCheckpoints)
            .FirstOrDefaultAsync(t => t.Token.Value == token)
            .ConfigureAwait(false);

        if (webhookToken == null || !webhookToken.IsActive ||
            webhookToken.ExpirationDate < DateTime.UtcNow ||
            webhookToken.UsedCount >= webhookToken.UsageLimit)
        {
            return Results.BadRequest("Invalid or expired token");
        }

        if (webhookToken.Token.Type != TokenType.PersonalReceiverConfirmation)
        {
            return Results.BadRequest("Invalid token type");
        }

        var finalCheckpoint = webhookToken.Order.RouteCheckpoints
            .OrderByDescending(rc => rc.SequenceNumber)
            .First();

        finalCheckpoint.Status = RouteCheckPointStatus.Confirmed;
        finalCheckpoint.ActualArrival = DateTime.UtcNow;
        finalCheckpoint.ConfirmedByTokenId = webhookToken.Id;
        finalCheckpoint.ConfirmationTimestamp = DateTime.UtcNow;

        webhookToken.Order.Status = OrderStatus.Delivered;
        webhookToken.Order.ActualDeliveryDate = DateTime.UtcNow;

        webhookToken.UsedCount++;
        webhookToken.LastUsedDate = DateTime.UtcNow;

        await context.SaveChangesAsync().ConfigureAwait(false);

        return Results.Redirect($"/order/confirm-delivery?success=true&token={token}");
    }

    private async Task<IResult> UpdateCheckpoint(
        [FromForm] string token,
        [FromForm] int sequenceNumber,
        [FromForm] string? notes,
        IApplicationDbContext context)
    {
        var webhookToken = await context.WebhookTokens
            .Include(t => t.Order)
            .ThenInclude(o => o.RouteCheckpoints)
            .FirstOrDefaultAsync(t => t.Token.Value == token)
            .ConfigureAwait(false);

        if (webhookToken == null || !webhookToken.IsActive ||
            webhookToken.ExpirationDate < DateTime.UtcNow)
        {
            return Results.BadRequest("Invalid or expired token");
        }

        if (webhookToken.Token.Type != TokenType.DeliveryConfirmation)
        {
            return Results.BadRequest("Invalid token type");
        }

        var checkpoint = webhookToken.Order.RouteCheckpoints
            .FirstOrDefault(rc => rc.SequenceNumber == sequenceNumber);

        if (checkpoint == null)
        {
            return Results.BadRequest("Checkpoint not found");
        }

        checkpoint.Status = RouteCheckPointStatus.Confirmed;
        checkpoint.ActualArrival = DateTime.UtcNow;
        checkpoint.ConfirmedByTokenId = webhookToken.Id;
        checkpoint.ConfirmationTimestamp = DateTime.UtcNow;
        if (!string.IsNullOrEmpty(notes))
        {
            checkpoint.Notes = notes;
        }

        webhookToken.UsedCount++;
        webhookToken.LastUsedDate = DateTime.UtcNow;

        var allConfirmed = webhookToken.Order.RouteCheckpoints.All(rc => rc.Status == RouteCheckPointStatus.Confirmed);
        if (allConfirmed)
        {
            webhookToken.Order.Status = OrderStatus.Delivered;
            webhookToken.Order.ActualDeliveryDate = DateTime.UtcNow;
        }
        else if (webhookToken.Order.RouteCheckpoints.Any(rc => rc.Status == RouteCheckPointStatus.Confirmed))
        {
            webhookToken.Order.Status = OrderStatus.InTransit;
        }

        await context.SaveChangesAsync().ConfigureAwait(false);

        return Results.Redirect($"/order/update-checkpoint?token={token}&success=true");
    }
}
