using Htmx;
using Htmx.Net.Toast.Enums;
using Htmx.Net.Toast.Extensions;
using Htmx.Net.Toast.Notyf.Enums;
using Htmx.Net.Toast.Notyf.Models;
using Htmx.TagHelpers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.FileProviders;
using RouteForce.Web.Pages.Home;
using RouteForce.Web.Sessions;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAntiforgery();
builder.Services.AddCors(options =>
{
    options.AddPolicy("customcorspolicy",
        policy =>
        {
            policy.WithOrigins("*")
                .WithHeaders(HtmxRequestHeaders.Keys.All)
                .WithExposedHeaders(HtmxResponseHeaders.Keys.All);
        });
});

builder.Services.AddAuthorization();
builder.Services.AddTransient<HtmxRequestContext>();
builder.Services.AddNotyf(config =>
{
    config.Duration = 2000;
    config.Dismissable = true;
    config.Position = NotyfPosition.BottomRight;
    config.Ripple = true;
    config.CustomTypes = new List<NotyfNotificationOptions>
    {
        // Create a new notification type called "rawr" with some sensible purple defaults
        new()
        {
            Type = ToastNotificationType.Custom("rawr"),
            BackgroundColor = "#5928a7",
            Dismissible = true,
            Duration = 5000,
            Icon =
                "<svg xmlns=\"http://www.w3.org/2000/svg\" fill=\"none\" viewBox=\"0 0 24 24\" stroke-width=\"1.5\" stroke=\"currentColor\" style=\"width: 1.25em; height: 1.25em;\">\r\n  <path stroke-linecap=\"round\" stroke-linejoin=\"round\" d=\"M15.59 14.37a6 6 0 01-5.84 7.38v-4.8m5.84-2.58a14.98 14.98 0 006.16-12.12A14.98 14.98 0 009.631 8.41m5.96 5.96a14.926 14.926 0 01-5.841 2.58m-.119-8.54a6 6 0 00-7.381 5.84h4.8m2.581-5.84a14.927 14.927 0 00-2.58 5.84m2.699 2.7c-.103.021-.207.041-.311.06a15.09 15.09 0 01-2.448-2.448 14.9 14.9 0 01.06-.312m-2.24 2.39a4.493 4.493 0 00-1.757 4.306 4.493 4.493 0 004.306-1.758M16.5 9a1.5 1.5 0 11-3 0 1.5 1.5 0 013 0z\" />\r\n</svg>\r\n",
            Message = "This is a default message",
            Ripple = true
        }
    };
});

builder.Services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", options =>
{
    options.LoginPath = "/Account/login";
    options.AccessDeniedPath = "/Account/Denied";
    options.ExpireTimeSpan = TimeSpan.FromHours(1);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();

    var distPath = Path.Combine(builder.Environment.ContentRootPath, "dist");
    if (Directory.Exists(distPath))
    {
        var distProvider = new PhysicalFileProvider(distPath);
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = distProvider,
            RequestPath = ""
        });
    }
}

app.UseCors("customcorspolicy");
app.MapHtmxAntiforgeryScript();
app.UseAntiforgery();
app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseRouting();

app.UseNotyf();
app.MapRazorPages();

app.MapGet("/", async () => { return new RazorComponentResult<Home>(); }).RequireRateLimiting("fixed");

app.Run();