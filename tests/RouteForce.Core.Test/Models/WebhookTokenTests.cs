using RouteForce.Core.Enums;
using RouteForce.Core.Models;

namespace RouteForce.Core.Test.Models;

[TestFixture]
public class WebhookTokenTests
{
    #region Token Immutability Tests

    [Test]
    public void Token_RecordType_IsImmutable()
    {
        // Arrange
        var token = new Token("DEL-12345678", TokenType.DeliveryConfirmation, IssuedToType.Business);

        // Act & Assert - Attempting to modify properties should result in compile error
        // This test verifies the record type behavior
        Assert.That(token.Value, Is.EqualTo("DEL-12345678"));
        Assert.That(token.Type, Is.EqualTo(TokenType.DeliveryConfirmation));
        Assert.That(token.IssuedToType, Is.EqualTo(IssuedToType.Business));
    }

    [Test]
    public void Token_WithExpression_CreatesNewInstance()
    {
        // Arrange
        var originalToken = new Token("DEL-12345678", TokenType.DeliveryConfirmation, IssuedToType.Business);

        // Act
        var modifiedToken = originalToken with { Type = TokenType.PersonalReceiverConfirmation };

        // Assert
        Assert.That(originalToken.Type, Is.EqualTo(TokenType.DeliveryConfirmation), "Original token should remain unchanged");
        Assert.That(modifiedToken.Type, Is.EqualTo(TokenType.PersonalReceiverConfirmation), "Modified token should have new type");
        Assert.That(modifiedToken.Value, Is.EqualTo(originalToken.Value), "Value should be copied");
        Assert.That(ReferenceEquals(originalToken, modifiedToken), Is.False, "Should be different instances");
    }

    [Test]
    public void Token_Create_GeneratesCorrectFormat()
    {
        // Arrange & Act
        var token = Token.Create(TokenType.DeliveryConfirmation, IssuedToType.Business);

        // Assert
        Assert.That(token.Value, Does.Match(@"^DEL-[a-f0-9]{8}$"), "Token should match format PREFIX-XXXXXXXX");
    }

    [Test]
    [TestCase(TokenType.DeliveryConfirmation, "DEL")]
    [TestCase(TokenType.PersonalReceiverConfirmation, "RCV")]
    [TestCase(TokenType.DeliveryCheckpoint, "DEC")]
    [TestCase(TokenType.StorageConfirmation, "STR")]
    public void Token_Create_GeneratesCorrectPrefix(TokenType tokenType, string expectedPrefix)
    {
        // Arrange & Act
        var token = Token.Create(tokenType, IssuedToType.Business);

        // Assert
        Assert.That(token.Value, Does.StartWith(expectedPrefix), $"Token should start with {expectedPrefix}");
    }

    [Test]
    public void Token_Create_GeneratesUniqueTokens()
    {
        // Arrange & Act
        var token1 = Token.Create(TokenType.DeliveryConfirmation, IssuedToType.Business);
        var token2 = Token.Create(TokenType.DeliveryConfirmation, IssuedToType.Business);
        var token3 = Token.Create(TokenType.DeliveryConfirmation, IssuedToType.Business);

        // Assert
        Assert.That(token1.Value, Is.Not.EqualTo(token2.Value), "Tokens should be unique");
        Assert.That(token2.Value, Is.Not.EqualTo(token3.Value), "Tokens should be unique");
        Assert.That(token1.Value, Is.Not.EqualTo(token3.Value), "Tokens should be unique");
    }

    [Test]
    public void Token_Equality_TwoTokensWithSameValues_AreEqual()
    {
        // Arrange
        var token1 = new Token("DEL-12345678", TokenType.DeliveryConfirmation, IssuedToType.Business);
        var token2 = new Token("DEL-12345678", TokenType.DeliveryConfirmation, IssuedToType.Business);

        // Act & Assert
        Assert.That(token1, Is.EqualTo(token2), "Records with same values should be equal");
        Assert.That(token1 == token2, Is.True, "Equality operator should return true");
    }

    [Test]
    public void Token_Equality_DifferentValues_AreNotEqual()
    {
        // Arrange
        var token1 = new Token("DEL-12345678", TokenType.DeliveryConfirmation, IssuedToType.Business);
        var token2 = new Token("DEL-87654321", TokenType.DeliveryConfirmation, IssuedToType.Business);

        // Act & Assert
        Assert.That(token1, Is.Not.EqualTo(token2), "Records with different values should not be equal");
        Assert.That(token1 == token2, Is.False, "Equality operator should return false");
    }

    [Test]
    public void Token_Matches_ReturnsTrue_ForExactMatch()
    {
        // Arrange
        var token = new Token("DEL-12345678", TokenType.DeliveryConfirmation, IssuedToType.Business);

        // Act
        var result = token.Matches("DEL-12345678");

        // Assert
        Assert.That(result, Is.True, "Should match exact token value");
    }

    [Test]
    public void Token_Matches_ReturnsFalse_ForDifferentToken()
    {
        // Arrange
        var token = new Token("DEL-12345678", TokenType.DeliveryConfirmation, IssuedToType.Business);

        // Act
        var result = token.Matches("DEL-87654321");

        // Assert
        Assert.That(result, Is.False, "Should not match different token value");
    }

    [Test]
    public void Token_Matches_IsCaseSensitive()
    {
        // Arrange
        var token = new Token("DEL-12345678", TokenType.DeliveryConfirmation, IssuedToType.Business);

        // Act
        var result = token.Matches("del-12345678");

        // Assert
        Assert.That(result, Is.False, "Token matching should be case-sensitive");
    }

    #endregion

    #region WebhookToken Behavior Tests

    [Test]
    public void WebhookToken_DefaultValues_AreSetCorrectly()
    {
        // Arrange & Act
        var webhookToken = new WebhookToken();

        // Assert
        Assert.That(webhookToken.IsActive, Is.True, "IsActive should default to true");
        Assert.That(webhookToken.UsageLimit, Is.EqualTo(1), "UsageLimit should default to 1");
        Assert.That(webhookToken.UsedCount, Is.EqualTo(0), "UsedCount should default to 0");
        Assert.That(webhookToken.CreatedDate, Is.Not.EqualTo(default(DateTime)), "CreatedDate should be set");
    }

    [Test]
    public void WebhookToken_Token_CannotBeReassignedAfterInitialization()
    {
        // Arrange
        var token = Token.Create(TokenType.DeliveryConfirmation, IssuedToType.Business);
        var webhookToken = new WebhookToken { Token = token };

        // Act & Assert
        // This would cause a compile error if attempted:
        // webhookToken.Token = Token.Create(TokenType.DeliveryConfirmation, IssuedToType.Business);

        Assert.That(webhookToken.Token, Is.EqualTo(token), "Token should remain the same");
    }

    [Test]
    public void WebhookToken_WithTokenCreate_IntegrationTest()
    {
        // Arrange
        var token = Token.Create(TokenType.PersonalReceiverConfirmation, IssuedToType.PersonalReceiver);
        var expirationDate = DateTime.UtcNow.AddDays(30);

        // Act
        var webhookToken = new WebhookToken
        {
            Token = token,
            OrderId = 123,
            ExpirationDate = expirationDate,
            IssueType = WebHookIssueType.ReceiveConfirmation,
            IsActive = true,
            UsageLimit = 1,
            UsedCount = 0
        };

        // Assert
        Assert.That(webhookToken.Token.Type, Is.EqualTo(TokenType.PersonalReceiverConfirmation));
        Assert.That(webhookToken.Token.IssuedToType, Is.EqualTo(IssuedToType.PersonalReceiver));
        Assert.That(webhookToken.Token.Value, Does.StartWith("RCV-"));
        Assert.That(webhookToken.OrderId, Is.EqualTo(123));
        Assert.That(webhookToken.IsActive, Is.True);
    }

    [Test]
    public void WebhookToken_TokenMatches_WorksCorrectly()
    {
        // Arrange
        var token = Token.Create(TokenType.DeliveryConfirmation, IssuedToType.Business);
        var webhookToken = new WebhookToken { Token = token };

        // Act
        var matchesCorrectToken = webhookToken.Token.Matches(token.Value);
        var matchesWrongToken = webhookToken.Token.Matches("WRONG-12345678");

        // Assert
        Assert.That(matchesCorrectToken, Is.True, "Should match its own token value");
        Assert.That(matchesWrongToken, Is.False, "Should not match different token value");
    }

    [Test]
    public void WebhookToken_TokenRemains_ImmutableThroughoutLifecycle()
    {
        // Arrange
        var token = Token.Create(TokenType.DeliveryConfirmation, IssuedToType.Business);
        var originalTokenValue = token.Value;

        var webhookToken = new WebhookToken
        {
            Token = token,
            OrderId = 1,
            ExpirationDate = DateTime.UtcNow.AddDays(30)
        };

        // Act - Simulate usage
        webhookToken.UsedCount++;
        webhookToken.LastUsedDate = DateTime.UtcNow;
        webhookToken.IsActive = false;

        // Assert
        Assert.That(webhookToken.Token.Value, Is.EqualTo(originalTokenValue),
            "Token value should remain unchanged despite webhook state changes");
        Assert.That(webhookToken.Token.Type, Is.EqualTo(TokenType.DeliveryConfirmation),
            "Token type should remain unchanged");
    }

    #endregion
}
