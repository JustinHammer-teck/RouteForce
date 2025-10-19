using Htmx.Net.Toast.Extensions;
using Htmx.TagHelpers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using RouteForce.Application;
using RouteForce.Infrastructure;
using RouteForce.Web;
using RouteForce.Web.Configurations;
using RouteForce.Web.Pages.Home;
using RouteForce.Web.Pages.Shared;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;


var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

var cookiePolicyOptions = new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
};

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSecurity(configuration);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(configuration);
builder.Services.AddWebApplication(configuration);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseHsts();
}
else
{
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

app.UseHttpsRedirection();
app.UseStatusCodePagesWithReExecute("/error/{0}");
app.UseCors("htmxcorspolicy");
app.MapHtmxAntiforgeryScript();
app.UseNotyf();
app.UseAntiforgery();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapEndpoints();
app.MapDefaultControllerRoute();
app.UseCookiePolicy(cookiePolicyOptions);


app.MapGet("/error/{http_error_code}", (
        [FromRoute] string http_error_code,
        [FromQuery] string? msg) => 
    new RazorComponentResult<ExceptionPage>(new
    {
        StatusCodeParam = http_error_code, CustomMessage = msg
    }));

app.MapGet("/", () => new RazorComponentResult<Home>());

app.Run();