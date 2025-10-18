using System.Text.Json;
using Htmx.Net.Toast.Abstractions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RouteForce.Application.Common.DTOs;
using RouteForce.Application.Common.Interfaces;
using RouteForce.Core.Enums;
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
    
    private async Task<RazorComponentResult> RegisterPage(
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

    private IResult RegisterBusinessForm(SessionManager sessionManager)
    {
        if (!sessionManager.HasKey("CreateBusinessRequest"))
        {
            return new RazorComponentResult<_RegisterBusinessForm>(new { Session = sessionManager });
        }

        var businessJson = sessionManager.GetSessionValue("CreateBusinessRequest");
        var savedRequest = JsonSerializer.Deserialize<CreateBusinessRequest>(businessJson);

        sessionManager.SetSessionValue("CreateBusinessRequest", businessJson);

        return new RazorComponentResult<_RegisterBusinessForm>(
            new {
                Session = sessionManager,
                SavedRequest = savedRequest
            });
    }

    [ValidateAntiForgeryToken]
    public async Task<IResult> RegisterBusiness(
        [FromForm] CreateBusinessRequest request,
        SessionManager sessionManager)
    {
        var businessJson = JsonSerializer.Serialize(request);
        sessionManager.SetSessionValue("CreateBusinessRequest", businessJson);

        return new RazorComponentResult<_RegisterUserForm>();
    }

    public async Task<IResult> RegisterUserForm()
    {
        return new RazorComponentResult<_RegisterUserForm>();
    }
        
    [ValidateAntiForgeryToken]
    public async Task<IResult> RegisterUser(
        [FromForm] CreateUserRequest userRequest,
        SessionManager sessionManager,
        IApplicationDbContext context,
        INotyfService notyf)
    {
        if (!sessionManager.HasKey("CreateBusinessRequest"))
        {
            notyf.Error("Business information not found. Please start from the beginning.", 3);
            return Results.Redirect("/users/register");
        }

        var businessJson = sessionManager.GetSessionValue("CreateBusinessRequest");
        var businessRequest = JsonSerializer.Deserialize<CreateBusinessRequest>(businessJson);

        if (businessRequest == null)
        {
            notyf.Error("Invalid business data. Please try again.", 3);
            return Results.Redirect("/users/register");
        }

        var user = new Core.Models.User
        {
            Name = userRequest.UserName,
            Password = userRequest.Password,
            Email = userRequest.Email,
            Phone = userRequest.Phone,
            UserRole = UserRole.Admin,
            CreatedDate = DateTime.UtcNow,
        };

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync().ConfigureAwait(false);

        var business = new Core.Models.Business
        {
            Name = businessRequest.Name,
            BusinessAddress = new Core.Models.Address(
                businessRequest.AddressLine,
                businessRequest.City,
                businessRequest.State,
                businessRequest.PostalCode,
                businessRequest.Country,
                businessRequest.Latitude,
                businessRequest.Longitude),
            Notes = businessRequest.Notes ?? string.Empty,
            IsActive = true,
            CreatedDate = DateTime.UtcNow,
        };

        await context.Businesses.AddAsync(business);
        await context.SaveChangesAsync().ConfigureAwait(false);

        user.BusinessId = business.Id;
        await context.SaveChangesAsync().ConfigureAwait(false);

        notyf.Success("Account created successfully!", 3);
        return Results.Redirect("/users/login");
    }
}
