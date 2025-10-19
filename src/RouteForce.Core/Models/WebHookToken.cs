using RouteForce.Core.Enums;

namespace RouteForce.Core.Models;

public class WebhookToken
{
    public int Id { get; set; }
    /**
     * For security the Token should only be Created once
     * the user can not change the value
     * Right now i'm not enforce in the DB yet (but should be)
     */
    public Token Token { get; init; }
    public int OrderId { get; set; }
    public Order Order { get; set; }
    public DateTime ExpirationDate { get; set; }
    public WebHookIssueType IssueType { get; set; }
    public bool IsActive { get; set; } = true;
    public int UsageLimit { get; set; } = 1;
    public int UsedCount { get; set; } = 0;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? LastUsedDate { get; set; }
}

// Token Should Immutable for better management and
// I Don't really want order people just change it could cause big bug
// This is just a naive implementation to test the feature 
public record Token(string Value, TokenType Type, IssuedToType IssuedToType)
{
    public static Token Create(TokenType type, IssuedToType issuedToType)
    {
        var prefix = type switch
        {
            TokenType.DeliveryConfirmation => "DEL",
            TokenType.DeliveryCheckpoint => "DEC",
            TokenType.PersonalReceiverConfirmation => "RCV",
            TokenType.StorageConfirmation => "STR",
            _ => "TKN"
        };

        var tokenValue = $"{prefix}-{Guid.NewGuid().ToString("N")[..8]}";
        return new Token(tokenValue, type, issuedToType);
    }

    public bool Matches(string providedToken)
    {
        return Value == providedToken;
    }
}