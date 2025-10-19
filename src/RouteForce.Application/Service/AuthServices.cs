using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using RouteForce.Application.Common;
using RouteForce.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RouteForce.Core.Enums;
using RouteForce.Core.Models;

namespace RouteForce.Application.Service;

public interface IUserServices
{
    Task<UserAuthenticationResult> SignInAsync(string inputUsername, string inputPassword, HttpContext httpContext);
    Task SignOutAsync(HttpContext httpContext);
}

public class UserServices : IUserServices
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserServices(IApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<UserAuthenticationResult> SignInAsync(string inputUsername, string inputPassword, HttpContext httpContext)
    {
        var authResult = await CheckUserAuthentication(inputUsername, inputPassword);
    
        if (!authResult.Success)
        {
            return authResult;
        }
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == inputUsername);

        if (user == null)
        {
            return new UserAuthenticationResult(false, new[] { "Invalid email or password" });
        }
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId),  
            new Claim(ClaimTypes.Name, user.UserName),          
            new Claim(ClaimTypes.Email, user.Email),            
            new Claim(ClaimTypes.Role, user.UserRole.ToString()), 
                          
        };
        if (user.BusinessId > 0)
        {
            claims.Add(new Claim("BusinessId", user.BusinessId.ToString()));
            claims.Add(new Claim("BusinessName", user.BusinessName ?? string.Empty));

        }
        
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            // Whether the authentication session is persisted across multiple requests
            IsPersistent = true,
            
            // The time at which the authentication ticket expires
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7),
            
            // Whether the authentication ticket can be refreshed
            AllowRefresh = true,
            
            // The time at which the authentication ticket was issued
            IssuedUtc = DateTimeOffset.UtcNow,
            
            // Store additional data if needed
            Items =
            {
                { "LoginTime", DateTimeOffset.UtcNow.ToString() }
            }
        };
        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

        return new UserAuthenticationResult(true, Array.Empty<string>());
        
    }
    
    public async Task SignOutAsync(HttpContext httpContext)
    {
        // Sign out and delete the authentication cookie
        await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        
        
    }
    
    private async Task<UserAuthenticationResult> CheckUserAuthentication(string inputUsername, string inputPassword)
    {
        if (string.IsNullOrWhiteSpace(inputUsername) || string.IsNullOrWhiteSpace(inputPassword))
        {
            return new UserAuthenticationResult(false, new[] { "Username and password are required" });
        }

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == inputUsername);

        if (user == null)
        {
            return new UserAuthenticationResult(false, new[] { "Invalid email or password" });
        }
        
        return new UserAuthenticationResult(true, Array.Empty<string>());
    }
    
    
}
public record UserAuthenticationResult(bool Success, IEnumerable<string> Errors);




        
    

