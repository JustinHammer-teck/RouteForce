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
    app.UseExceptionHandler(exceptionHandlerApp =>
    {
        exceptionHandlerApp.Run(async context =>
        {
            var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
            var exception = exceptionHandlerPathFeature?.Error;
            var errorMessage = exception?.Message ?? "An unexpected error occurred.";

            context.Response.Redirect($"/error/500?msg={Uri.EscapeDataString(errorMessage)}");
            await Task.CompletedTask;
        });
    });
}
else
{
    app.UseExceptionHandler("/error/500");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStatusCodePagesWithReExecute("/error/{0}");

app.UseStaticFiles();
app.UseRouting();
app.UseCors("htmxcorspolicy");
app.UseSession();
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