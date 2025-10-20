using System.Security.Claims;
using System.Text.Json;
using Htmx;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RouteForce.Application.Common.DTOs;
using RouteForce.Application.Common.Interfaces;
using RouteForce.Core.Enums;
using RouteForce.Web.Configurations;
using RouteForce.Web.Pages.Admin;
using RouteForce.Web.Pages.Authens;
using RouteForce.Web.Sessions;

namespace RouteForce.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization();
        groupBuilder.MapGet("login", LoginPage).AllowAnonymous();
        groupBuilder.MapPost("login", Authenticate)
            .AllowAnonymous()
            .DisableAntiforgery();
        groupBuilder.MapGet("logout", Logout);
        groupBuilder.MapGet("register", RegisterPage).AllowAnonymous();
        groupBuilder.MapGet("register/business", RegisterBusinessForm)
            .AllowAnonymous();
        groupBuilder.MapPost("register/business", RegisterBusiness)
            .AllowAnonymous()
            .DisableAntiforgery();
        groupBuilder.MapGet("register/user", RegisterUserForm)
            .AllowAnonymous();
        groupBuilder.MapPost("register/user", RegisterUser)
            .AllowAnonymous()
            .DisableAntiforgery();
    }
    
    private async Task<RazorComponentResult> LoginPage()
    {
        return new RazorComponentResult<SignIn>();
    }    
    
    private async Task<IResult> Authenticate(
        [FromForm] SignInUserRequest request,
        IApplicationDbContext context,
        HttpResponse response, 
        HttpContext httpContext)
    {
        var user = await context.Users
            .AsNoTracking()
            .Include(u => u.Business)
            .FirstOrDefaultAsync(u => u.Email == request.UserName)
            .ConfigureAwait(false);

        if (user == null || user.Password != request.Password)
        {
            return Results.Redirect("/users/login");
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.UserRole.ToString()),
            new Claim("BusinessId", user.BusinessId.ToString()),
            new Claim("BusinessName", user.Business?.Name ?? string.Empty)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
        };

        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);
        
        httpContext.Response.Htmx(x => x.Redirect("/"));
        return Results.Ok();
    }

    private async Task<IResult> Logout(HttpContext httpContext)
    {
        await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        
        httpContext.Response.Htmx(x => x.Redirect("/"));
        return Results.Ok();
    }

    private async Task<RazorComponentResult> RegisterPage(
        SessionManager sessionManager
        )
    {
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

    private IResult RegisterBusiness(
        [FromForm] CreateBusinessRequest request,
        SessionManager sessionManager)
    {
        var businessJson = JsonSerializer.Serialize(request);
        sessionManager.SetSessionValue("CreateBusinessRequest", businessJson);

        return new RazorComponentResult<_RegisterUserForm>();
    }

    private IResult RegisterUserForm()
    {
        return new RazorComponentResult<_RegisterUserForm>();
    }
        
    private async Task<IResult> RegisterUser(
        [FromForm] CreateUserRequest userRequest,
        SessionManager sessionManager,
        HttpContext httpContext,
        IApplicationDbContext context)
    {
        if (!sessionManager.HasKey("CreateBusinessRequest"))
        {
            return Results.Redirect("/users/register");
        }

        var businessJson = sessionManager.GetSessionValue("CreateBusinessRequest");
        var businessRequest = JsonSerializer.Deserialize<CreateBusinessRequest>(businessJson);

        if (businessRequest == null)
        {
            return Results.Redirect("/users/register");
        }

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
            Notes = businessRequest.Notes,
            IsActive = true,
            CreatedDate = DateTime.UtcNow,
        };

        await context.Businesses.AddAsync(business);
        await context.SaveChangesAsync().ConfigureAwait(false);

        var warehouseCheckpoint = new Core.Models.Checkpoint
        {
            Name = $"{business.Name} - Warehouse",
            Address = business.BusinessAddress,
            ContactPoint = new Core.Models.ContactPoint(
                userRequest.UserName,
                userRequest.Phone,
                userRequest.Email),
            CheckpointType = CheckpointType.Warehouse,
            ManagedByBusinessId = business.Id,
            IsActive = true,
            CreatedDate = DateTime.UtcNow,
            Notes = "Main warehouse checkpoint"
        };

        await context.Checkpoints.AddAsync(warehouseCheckpoint);
        await context.SaveChangesAsync().ConfigureAwait(false);

        var user = new Core.Models.User
        {
            Name = userRequest.UserName,
            Password = userRequest.Password,
            Email = userRequest.Email,
            Phone = userRequest.Phone,
            UserRole = UserRole.Admin,
            BusinessId = business.Id,
            CreatedDate = DateTime.UtcNow,
        };

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync().ConfigureAwait(false);

        httpContext.Response.Htmx(x => x.Redirect("/"));
        return Results.Ok();
    }
}
