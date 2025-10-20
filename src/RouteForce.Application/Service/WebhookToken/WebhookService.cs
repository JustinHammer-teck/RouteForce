using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RouteForce.Application.Common.Interfaces;
using RouteForce.Application.Common.Settings;
using RouteForce.Core.Enums;
using RouteForce.Core.Models;

namespace RouteForce.Application.Service.WebhookToken;

public class WebhookService : IWebhookService
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly WebhookSettings _webhookSettings;

    public WebhookService(
        IApplicationDbContext context,
        IEmailService emailService,
        IOptions<WebhookSettings> webhookSettings)
    {
        _context = context;
        _emailService = emailService;
        _webhookSettings = webhookSettings.Value;
    }

    public async Task CreateDeliveryToken(int orderId)
    {
        var deliveryToken = new Core.Models.WebhookToken
        {
            Token = Token.Create(TokenType.DeliveryConfirmation, IssuedToType.Business),
            OrderId = orderId,
            ExpirationDate = DateTime.UtcNow.AddDays(30),
            IssueType = WebHookIssueType.DeliveryCheckpoint,
            IsActive = true,
            UsageLimit = 999,
            UsedCount = 0,
            CreatedDate = DateTime.UtcNow,
        };

        await _context.WebhookTokens.AddAsync(deliveryToken);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }
    
    public async Task CreateReceiverToken(int orderId)
    {
        var order = await _context.Orders
            .Include(o => o.PersonalReceiver)
            .FirstOrDefaultAsync(o => o.Id == orderId)
            .ConfigureAwait(false);

        if (order == null)
        {
            throw new InvalidOperationException($"Order with ID {orderId} not found.");
        }

        var receiverToken = new Core.Models.WebhookToken
        {
            Token = Token.Create(TokenType.PersonalReceiverConfirmation, IssuedToType.PersonalReceiver),
            OrderId = orderId,
            ExpirationDate = DateTime.UtcNow.AddDays(30),
            IssueType = WebHookIssueType.ReceiveConfirmation,
            IsActive = true,
            UsageLimit = 1,
            UsedCount = 0,
            CreatedDate = DateTime.UtcNow,
        };

        await _context.WebhookTokens.AddAsync(receiverToken);
        await _context.SaveChangesAsync().ConfigureAwait(false);

        var tokenValue = receiverToken.Token.Value;
        var confirmationUrl = $"{_webhookSettings.BaseUrl}/orders/confirm-receive?token={tokenValue}";

        await _emailService.SendDeliveryConfirmationEmailAsync(
            order.PersonalReceiver.Email,
            order.PersonalReceiver.Name,
            order.TrackingNumber,
            confirmationUrl,
            CancellationToken.None
        ).ConfigureAwait(false);
    }
}