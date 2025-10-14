using System.Security.Cryptography;
using RouteForce.Core.Enums;

namespace RouteForce.Core.Models;

public class WebhookToken
{
    public int Id { get; set; }
    public Token Token { get; set; }
    public int OrderID { get; set; }
    public int? IssuedToBusinessID { get; set; }
    public int? IssuedToPersonalReceiverID { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool IsActive { get; set; } = true;
    public int UsageLimit { get; set; } = 1;
    public int UsedCount { get; set; } = 0;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}

public record Token(string Value, TokenType Type, IssuedToType IssuedToType)
{
    public static Token Create(TokenType type, IssuedToType issuedToType)
    {
        var tokenValue = GenerateToken();
        return new Token(tokenValue, type, issuedToType);
    }

    public bool Matches(string providedToken) => Value == providedToken;

    private static string GenerateToken()
    {
        var bytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }
}