using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using RouteForce.Application.Common.Interfaces;

namespace RouteForce.Infrastructure.External.EmailSevices;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    public async Task SendDeliveryConfirmationEmailAsync(
        string recipientEmail,
        string recipientName,
        string trackingNumber,
        string confirmationUrl,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
            message.To.Add(new MailboxAddress(recipientName, recipientEmail));
            message.Subject = $"Confirm Delivery - Order #{trackingNumber}";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = CreateEmailTemplate(recipientName, trackingNumber, confirmationUrl)
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();

            await client.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort, _emailSettings.EnableSsl, cancellationToken);

            if (!string.IsNullOrEmpty(_emailSettings.Username))
            {
                await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password, cancellationToken);
            }

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation(
                "Delivery confirmation email sent successfully to {Email} for order {TrackingNumber}",
                recipientEmail, trackingNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to send delivery confirmation email to {Email} for order {TrackingNumber}",
                recipientEmail, trackingNumber);
            throw;
        }
    }

    private string CreateEmailTemplate(string recipientName, string trackingNumber, string confirmationUrl)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border-radius: 5px; margin-top: 20px; }}
        .button {{ display: inline-block; padding: 15px 30px; background-color: #4CAF50; color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
        .footer {{ margin-top: 30px; padding-top: 20px; border-top: 1px solid #ddd; font-size: 12px; color: #666; text-align: center; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>RouteForce Delivery</h1>
        </div>
        <div class=""content"">
            <h2>Hello {recipientName},</h2>
            <p>Your order <strong>#{trackingNumber}</strong> has been delivered to your address.</p>
            <p>Please confirm that you have received this order by clicking the button below:</p>
            <div style=""text-align: center;"">
                <a href=""{confirmationUrl}"" class=""button"">Confirm Delivery</a>
            </div>
            <p style=""margin-top: 20px; font-size: 14px; color: #666;"">
                Or copy and paste this link into your browser:<br>
                <a href=""{confirmationUrl}"">{confirmationUrl}</a>
            </p>
            <p style=""margin-top: 30px; padding: 15px; background-color: #fff3cd; border-left: 4px solid #ffc107;"">
                <strong>Note:</strong> This confirmation link will expire in 7 days.
            </p>
        </div>
        <div class=""footer"">
            <p>This is an automated message from RouteForce. Please do not reply to this email.</p>
            <p>If you did not expect this delivery, please contact our support team immediately.</p>
        </div>
    </div>
</body>
</html>";
    }
}