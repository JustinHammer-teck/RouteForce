using RouteForce.Application.Common.Interfaces;
using RouteForce.Core.Enums;
using RouteForce.Core.Models;

namespace RouteForce.Application.Service.WebhookToken;

public class WebhookService : IWebhookService
{
    private readonly IApplicationDbContext _context;

    public WebhookService(IApplicationDbContext context)
    {
        _context = context;
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
    }
}