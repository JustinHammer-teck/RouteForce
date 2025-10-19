using Htmx.TagHelpers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using RouteForce.Application;
using RouteForce.Infrastructure;
using RouteForce.Web;
using RouteForce.Web.Configurations;
using RouteForce.Web.Pages.Shared;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
builder.Services.AddSecurity(configuration);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(configuration);
builder.Services.AddWebApplication(configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
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

app.UseStaticFiles();
app.UseRouting();
app.MapHtmxAntiforgeryScript();
app.UseCors("htmxcorspolicy");
app.UseSession();
app.UseAntiforgery(); // Order matter
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapEndpoints();

app.MapGet("/error/{http_error_code}", (
        [FromRoute] string http_error_code,
        [FromQuery] string? msg) => 
    new RazorComponentResult<ExceptionPage>(new
    {
        StatusCodeParam = http_error_code, CustomMessage = msg
    }));

app.MapGet("/", () => 
        Results.Redirect("/admin/dashboard")
);

app.Run();