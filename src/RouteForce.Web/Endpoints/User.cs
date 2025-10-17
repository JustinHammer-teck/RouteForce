using Htmx.Net.Toast.Abstractions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RouteForce.Application.Common.DTOs;
using RouteForce.Application.Common.Interfaces;
using RouteForce.Web.Configurations;
using RouteForce.Web.Pages.Admin;
using RouteForce.Web.Pages.Home;
using RouteForce.Web.Sessions;

namespace RouteForce.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization();
        groupBuilder.MapGet("login", LoginPage).AllowAnonymous();
        groupBuilder.MapPost("login", Authenticate).AllowAnonymous();
        groupBuilder.MapGet("register", RegisterPage).AllowAnonymous();
        groupBuilder.MapGet("register/business", RegisterBusinessForm).AllowAnonymous();
        groupBuilder.MapPost("register/business", RegisterBusiness).AllowAnonymous();
        groupBuilder.MapGet("register/user", RegisterUserForm).AllowAnonymous();
        groupBuilder.MapPost("register/user", RegisterUser).AllowAnonymous();
        groupBuilder.MapPost("register", Register).AllowAnonymous();
    }
    
    public async Task<RazorComponentResult> LoginPage()
    {
        return new RazorComponentResult<Home>();
    }    
    
    
    [ValidateAntiForgeryToken] 
    public async Task<IResult> Authenticate(IApplicationDbContext context)
    {
        return Results.Ok();     
    }
    
    public async Task<RazorComponentResult> RegisterPage(
        SessionManager sessionManager, 
        INotyfService notyf)
    {
        notyf.Success("Successful Create User & Business", 1);  
        return new RazorComponentResult<CreateAccount>(
            new
            {
                Session = sessionManager, 
                FormStep = 1
            }
        );
    }

    public async Task<IResult> RegisterBusinessForm()
    {
        return new RazorComponentResult<_RegisterBusinessForm>();
    }

    [ValidateAntiForgeryToken] 
    public async Task<IResult> RegisterBusiness([FromForm] CreateBusinessRequest request)
    {
        return new RazorComponentResult<_RegisterBusinessForm>();
    }

    public async Task<IResult> RegisterUserForm()
    {
        return new RazorComponentResult<_RegisterUserForm>();
    }
        
    [ValidateAntiForgeryToken] 
    public async Task<IResult> RegisterUser()
    {
        return Results.Created();
    }
    
    public async Task<IResult> Register(
        SessionManager sessionManager,
        INotyfService notyf)
    {
        notyf.Success("Successful Create User & Business", 1);  
        return Results.Created();
    }
}
