namespace RouteForce.Application.Common.Settings;

public sealed class WebhookSettings
{
    public const string SectionName = "WebhookSettings";

    public string BaseUrl { get; set; } = string.Empty;
    public int TokenExpirationDays { get; set; }
    public int DefaultUsageLimit { get; set; }
}
