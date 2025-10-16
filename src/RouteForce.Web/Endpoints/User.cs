using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RouteForce.Application.Common.Interfaces;
using RouteForce.Web.Configurations;
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
        return new RazorComponentResult<Home>();
    }    
    
    [ValidateAntiForgeryToken] 
    public async Task<IResult> Authenticate(IApplicationDbContext context)
    {
        return Results.Ok();     
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
