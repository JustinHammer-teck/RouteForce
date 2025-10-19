using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RouteForce.Application.Common.Interfaces;
using RouteForce.Application.Service;
using RouteForce.Web.Configurations;
using RouteForce.Web.Pages.Authens;
using RouteForce.Web.Pages.Home;

namespace RouteForce.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization();
        groupBuilder.MapGet("login", LoginPage).AllowAnonymous();
        groupBuilder.MapPost("login", Authenticate).AllowAnonymous();
        groupBuilder.MapGet("test", Test);
        groupBuilder.MapGet("whoimi", WhoImI);
    }
    
    [AllowAnonymous]
    public async Task<RazorComponentResult> LoginPage()
    {
        return new RazorComponentResult<SignIn>();
    }    
    
    [ValidateAntiForgeryToken] 
    public async Task<IResult> Authenticate([FromForm] string inputUsername, [FromForm] string inputPassword, [FromServices] IUserServices userServices, HttpContext context)
    {
        var result = await userServices.SignInAsync(inputUsername, inputPassword, context);

        if (!result.Success)
        {
            var errorHtml = $"<div class='text-red-600'>{string.Join("<br/>", result.Errors)}</div>";
            return Results.Content(errorHtml, "text/html");
        }

        // Redirect or return success message
        var successHtml = "<div class='text-green-600'>Login successful! Redirecting...</div><script>window.location='/dashboard'</script>";
        return Results.Content(successHtml, "text/html");
    }
    
    public async Task<IResult> Test()
    {
        return Results.Ok();
    }
    
    public async Task<IResult> WhoImI()
    {
        return Results.Ok();
    }
}
