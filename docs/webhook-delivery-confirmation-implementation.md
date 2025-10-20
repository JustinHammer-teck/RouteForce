# Webhook Token Delivery Confirmation System - Implementation Plan

## 🎯 Overview

Enable PersonalReceivers to confirm delivery by clicking a secure one-time URL sent via email, with explicit confirmation button (following industry best practices - no auto-execution).

## 📊 User Flow

```
1. Admin creates order in system
   ↓
2. System generates cryptographically secure WebhookToken
   ↓
3. System sends email to PersonalReceiver with confirmation URL
   ↓
4. PersonalReceiver clicks URL: /webhook/confirm/{token}
   ↓
5. GET request validates token → Stores in session → Shows confirmation page
   ↓
6. PersonalReceiver sees order details + "Confirm Delivery" button
   ↓
7. PersonalReceiver clicks button → POST request (CSRF protected)
   ↓
8. System validates session token → Updates order status → Consumes token
   ↓
9. Order.Status = Delivered, Order.ActualDeliveryDate = now
   ↓
10. Shows success page
```

## 🔐 Security Design

### Why This Approach?

**Rejected Approach: Auto-submit on GET**
- ❌ Email scanners would trigger false confirmations
- ❌ Trains users to accept dangerous auto-executing code
- ❌ Not industry standard for legal/financial confirmations

**Chosen Approach: Explicit Confirmation (Option A)**
- ✅ Industry standard (Amazon, UPS, FedEx, DocuSign pattern)
- ✅ Prevents email scanner false positives
- ✅ Creates audit trail with explicit user action
- ✅ No dangerous auto-execute behavior
- ✅ Clear user understanding of action

### Security Measures

1. **Token Security**
   - Cryptographically secure generation (32 bytes, Base64 URL-safe)
   - One-time use (UsageLimit = 1)
   - Expiration (configurable, default 7 days)
   - Active status flag

2. **Request Flow Security**
   - GET: Read-only, validates token, creates session
   - POST: State-changing, requires CSRF token
   - Session-based (token → session → confirmation)

3. **Audit Trail**
   - Log IP address on confirmation
   - Log timestamp of all token operations
   - Track usage count and attempts

4. **Protection Against Attacks**
   - CSRF protection (ASP.NET antiforgery)
   - Rate limiting on validation attempts
   - Token consumed after single use
   - Session timeout (30 minutes)

## 📋 Implementation Tasks

### 1. Database Layer Updates

**Files to modify:**
- `src/RouteForce.Application/Common/Interfaces/IApplicationDbContext.cs`
- `src/RouteForce.Infrastructure/Persistent/ApplicationDbContext.cs`

**Changes:**
```csharp
// Add to IApplicationDbContext
DbSet<Order> Orders { get; }
DbSet<PersonalReceiver> PersonalReceivers { get; }

// Add to ApplicationDbContext
public DbSet<Order> Orders { get; set; }
public DbSet<PersonalReceiver> PersonalReceivers { get; set; }
```

**Migration:**
- Run: `dotnet ef migrations add AddOrdersAndPersonalReceivers`
- Run: `dotnet ef database update`

### 2. Email Service (Infrastructure Layer)

**Create files:**
- `src/RouteForce.Application/Common/Interfaces/IEmailService.cs`
- `src/RouteForce.Infrastructure/Services/EmailService.cs`
- `src/RouteForce.Infrastructure/Models/EmailSettings.cs`

**IEmailService interface:**
```csharp
public interface IEmailService
{
    Task SendDeliveryConfirmationEmailAsync(
        string recipientEmail,
        string recipientName,
        string trackingNumber,
        string confirmationUrl);
}
```

**EmailService implementation:**
- Use MailKit (already installed)
- SMTP configuration from appsettings
- HTML email template
- Error handling and logging

**Register in DI:**
```csharp
// In Infrastructure/ServiceConfigurations.cs
services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
services.AddScoped<IEmailService, EmailService>();
```

**Configuration (appsettings.json):**
```json
{
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "EnableSsl": true,
    "Username": "",
    "Password": "",
    "FromEmail": "noreply@routeforce.com",
    "FromName": "RouteForce Delivery"
  }
}
```

### 3. Webhook Token Service (Application Layer)

**Create file:**
- `src/RouteForce.Application/Services/WebhookTokenService.cs`

**Methods:**
```csharp
public class WebhookTokenService
{
    Task<WebhookToken> GenerateDeliveryConfirmationTokenAsync(
        int orderId,
        int personalReceiverId,
        int expirationDays = 7);

    Task<TokenValidationResult> ValidateTokenAsync(string tokenValue);

    Task<TokenConsumptionResult> ConsumeTokenAsync(
        string tokenValue,
        string ipAddress);
}

public record TokenValidationResult(
    bool IsValid,
    int? OrderId,
    string? ErrorMessage);

public record TokenConsumptionResult(
    bool Success,
    int? OrderId,
    string? ErrorMessage);
```

**Logic:**
- Generate: Create Token, set expiration, usage limit
- Validate: Check expiration, active status, usage count
- Consume: Increment UsedCount, deactivate if single-use, log IP

**Register in DI:**
```csharp
// In Application/ServiceConfigurations.cs
services.AddScoped<WebhookTokenService>();
```

### 4. Session Support (Web Layer)

**Modify file:**
- `src/RouteForce.Web/ServiceConfigurations.cs`
- `src/RouteForce.Web/Program.cs`

**Add session services:**
```csharp
// In ServiceConfigurations.cs AddWebApplication method
services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".RouteForce.Webhook.Session";
});
```

**Add middleware:**
```csharp
// In Program.cs (BEFORE UseAuthentication)
app.UseSession();
```

### 5. Webhook Endpoints (Web Layer)

**Create file:**
- `src/RouteForce.Web/Endpoints/Webhooks.cs`

**Implementation:**
```csharp
public class Webhooks : EndpointGroupBase
{
    public override string? GroupName => "webhook";

    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet("confirm/{token}", ShowConfirmation)
            .AllowAnonymous();

        groupBuilder.MapPost("confirm", ProcessConfirmation)
            .AllowAnonymous();
    }

    public async Task<RazorComponentResult> ShowConfirmation(
        string token,
        WebhookTokenService tokenService,
        IApplicationDbContext context,
        HttpContext httpContext)
    {
        var validation = await tokenService.ValidateTokenAsync(token);

        if (!validation.IsValid)
        {
            return new RazorComponentResult<TokenError>(
                new { ErrorMessage = validation.ErrorMessage });
        }

        var order = await context.Orders
            .Include(o => o.PersonalReceiver)
            .Include(o => o.DeliveryAddress)
            .FirstOrDefaultAsync(o => o.Id == validation.OrderId);

        // Store token in session for POST
        httpContext.Session.SetString("WebhookToken", token);

        return new RazorComponentResult<DeliveryConfirmation>(
            new { Order = order });
    }

    [ValidateAntiForgeryToken]
    public async Task<IResult> ProcessConfirmation(
        WebhookTokenService tokenService,
        IApplicationDbContext context,
        HttpContext httpContext)
    {
        var token = httpContext.Session.GetString("WebhookToken");

        if (string.IsNullOrEmpty(token))
            return Results.BadRequest("Invalid session");

        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
        var result = await tokenService.ConsumeTokenAsync(token, ipAddress);

        if (!result.Success)
            return Results.BadRequest(result.ErrorMessage);

        var order = await context.Orders.FindAsync(result.OrderId);
        order.Status = OrderStatus.Delivered;
        order.ActualDeliveryDate = DateTime.UtcNow;

        await context.SaveChangesAsync();

        // Clear session
        httpContext.Session.Remove("WebhookToken");

        return Results.Ok(new { Message = "Delivery confirmed successfully!" });
    }
}
```

### 6. Razor Components (Web Layer)

**Create files:**
- `src/RouteForce.Web/Pages/Webhook/DeliveryConfirmation.razor`
- `src/RouteForce.Web/Pages/Webhook/ConfirmationSuccess.razor`
- `src/RouteForce.Web/Pages/Webhook/TokenError.razor`

**DeliveryConfirmation.razor:**
```razor
@using RouteForce.Core.Models

@{
    var order = (Order)Model?.Order;
}

<div class="container mx-auto p-6 max-w-md">
    <h1 class="text-2xl font-bold mb-4">Confirm Delivery</h1>

    <div class="bg-white rounded-lg shadow p-6 mb-4">
        <h2 class="text-lg font-semibold mb-2">Order Details</h2>
        <p><strong>Tracking Number:</strong> @order.TrackingNumber</p>
        <p><strong>Product:</strong> @order.ProductReferenceId</p>
        <p><strong>Recipient:</strong> @order.PersonalReceiver.Name</p>
        <p><strong>Delivery Address:</strong> @order.SelectedDeliveryAddress?.FullAddress</p>
    </div>

    <div class="bg-blue-50 border border-blue-200 rounded p-4 mb-4">
        <p class="text-sm text-blue-800">
            By confirming, you acknowledge that you have received this order.
        </p>
    </div>

    <form hx-post="/webhook/confirm"
          hx-swap="outerHTML"
          hx-target="body">
        <button type="submit"
                class="w-full bg-green-600 text-white py-3 rounded-lg hover:bg-green-700">
            ✓ Yes, I Received This Order
        </button>
    </form>

    <p class="text-sm text-gray-600 mt-4 text-center">
        Not you? <a href="/support" class="text-blue-600">Report an issue</a>
    </p>
</div>
```

**TokenError.razor:**
```razor
<div class="container mx-auto p-6 max-w-md text-center">
    <div class="text-red-600 text-6xl mb-4">⚠️</div>
    <h1 class="text-2xl font-bold mb-4">Invalid or Expired Link</h1>
    <p class="text-gray-600 mb-4">@Model?.ErrorMessage</p>
    <p class="text-sm text-gray-500">
        If you believe this is an error, please contact support.
    </p>
</div>
```

### 7. Order Creation Integration

**Modify file:**
- Where admin creates orders (likely in Admin endpoints or Business endpoints)

**After order creation:**
```csharp
// Generate webhook token
var token = await webhookTokenService.GenerateDeliveryConfirmationTokenAsync(
    order.Id,
    order.PersonalReceiverId,
    expirationDays: 7);

// Generate confirmation URL
var baseUrl = configuration["WebhookSettings:BaseUrl"];
var confirmationUrl = $"{baseUrl}/webhook/confirm/{token.Token.Value}";

// Send email
await emailService.SendDeliveryConfirmationEmailAsync(
    order.PersonalReceiver.Email,
    order.PersonalReceiver.Name,
    order.TrackingNumber,
    confirmationUrl);
```

### 8. Configuration

**Add to appsettings.json:**
```json
{
  "WebhookSettings": {
    "BaseUrl": "https://yourapp.com",
    "TokenExpirationDays": 7,
    "DefaultUsageLimit": 1
  }
}
```

## 🧪 Testing Scenarios

### Happy Path
1. Admin creates order
2. Email sent to PersonalReceiver
3. PersonalReceiver clicks link
4. Sees confirmation page with order details
5. Clicks "Confirm Delivery"
6. Order status updated to Delivered
7. Sees success message

### Error Cases
1. **Expired Token**
   - Token older than 7 days
   - Show: "This confirmation link has expired"

2. **Already Used Token**
   - Token UsedCount >= UsageLimit
   - Show: "This order has already been confirmed"

3. **Invalid Token**
   - Token doesn't exist in database
   - Show: "Invalid confirmation link"

4. **Inactive Token**
   - Token.IsActive = false
   - Show: "This confirmation link is no longer valid"

5. **Email Sending Failure**
   - SMTP error
   - Log error, retry mechanism, notify admin

### Security Tests
1. **CSRF Protection**
   - POST without antiforgery token → 400 Bad Request

2. **Session Hijacking**
   - POST without valid session → 400 Bad Request

3. **Token Reuse**
   - Second confirmation attempt → Already used error

4. **Email Scanner**
   - GET request only → No status change ✓

## 📈 Future Enhancements

1. **Reminder Emails**
   - Send reminder if not confirmed after 3 days
   - Configurable reminder schedule

2. **QR Code**
   - Generate QR code in email
   - Quick scan on mobile

3. **Multi-language Support**
   - Email templates in multiple languages
   - Based on PersonalReceiver preference

4. **SMS Fallback**
   - Send SMS if email fails
   - Configurable per business

5. **Photo Upload**
   - Allow PersonalReceiver to upload delivery photo
   - Visual proof of delivery

6. **Geolocation**
   - Capture delivery location
   - Verify against expected address

## 🔧 Development Setup

### Email Testing (Development)
Use one of these for testing email:
- **Mailtrap** (https://mailtrap.io) - Free email testing
- **Papercut SMTP** - Local SMTP server
- **MailHog** - Local email testing tool

Configuration example (Mailtrap):
```json
{
  "EmailSettings": {
    "SmtpHost": "smtp.mailtrap.io",
    "SmtpPort": 2525,
    "Username": "your-mailtrap-username",
    "Password": "your-mailtrap-password"
  }
}
```

### Local Testing
```bash
# Run migrations
dotnet ef migrations add AddWebhookTokenSupport
dotnet ef database update

# Run web app
dotnet run --project src/RouteForce.Web

# Test URL (replace {token} with actual token)
curl http://localhost:5000/webhook/confirm/{token}
```

## 📚 References

### Security Best Practices
- [OWASP - Unvalidated Redirects](https://owasp.org/www-project-web-security-testing-guide/)
- [Email Link Best Practices](https://www.twilio.com/blog/email-link-best-practices)
- [Magic Links Security](https://auth0.com/blog/magic-links/)

### Industry Examples
- Amazon delivery confirmations
- UPS/FedEx proof of delivery
- DocuSign document signing flow

## ✅ Acceptance Criteria

- [ ] PersonalReceivers receive email with confirmation link
- [ ] Link validates token (expiration, usage, active status)
- [ ] Confirmation page shows order details clearly
- [ ] Explicit "Confirm Delivery" button (no auto-execute)
- [ ] CSRF protection on confirmation POST
- [ ] Order status updates to "Delivered" on confirmation
- [ ] ActualDeliveryDate set to current timestamp
- [ ] Token consumed (cannot be reused)
- [ ] Audit trail (IP address, timestamp) logged
- [ ] Email scanners do NOT trigger false confirmations
- [ ] Error handling for all edge cases
- [ ] Mobile-friendly UI
- [ ] Professional email template
- [ ] Development email testing configured

---

**Document Version:** 1.0
**Last Updated:** 2025-10-17
**Status:** Ready for Implementation
