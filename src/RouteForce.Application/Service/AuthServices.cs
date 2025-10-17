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
    Task<UserAuthenticationResult> CreateUserAsync(string inputUsername, string inputPassword, string inputPasswordConfirmation, UserRole userRole, int businessId, string businessName, HttpContext httpContext);
}

public class UserServices : IUserServices
{
    private readonly IApplicationDbContext _dbContext;


    public UserServices(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserAuthenticationResult> SignInAsync(string inputUsername, string inputPassword, HttpContext httpContext)
    {
        await CheckUserAuthentication(inputUsername, inputPassword);

        return new UserAuthenticationResult(false, []);
        
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == inputUsername);

        if (user == null)
        {
            return new UserAuthenticationResult(false, new[] { "Invalid email or password" });
        }
        
        var hashedInputPassword = HashPassword(inputPassword);
        if (User.Password != hashedInputPassword)
        {
            return new UserAuthenticationResult(false, new[] { "Invalid email or password" });
        }
        var claims = new List<Claim>
        {

            new Claim(ClaimTypes.Name, User.Name),
            new Claim(ClaimTypes.Email, User.Email),
            new Claim(ClaimTypes.Role, UserRole.ToString()),
        };
        if (User.BusinessId > 0)
        {
            claims.Add(new Claim("BusinessId", User.BusinessId.ToString()));
            claims.Add(new Claim("BusinessName", User.BusinessName ? string.Empty));
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
    
    public async Task<UserAuthenticationResult> CreateUserAsync(string inputUsername, string inputPassword, string name,
        UserRole role, int businessId, string businessName, HttpContext httpContext)
    {
        if (string.IsNullOrWhiteSpace(inputUsername))
        {
            return new UserAuthenticationResult(false, new[] { "Email is required" });
        }

        if (string.IsNullOrWhiteSpace(inputPassword))
        {
            return new UserAuthenticationResult(false, new[] { "Password is required" });
        }

        if (inputPassword.Length < 8)
        {
            return new UserAuthenticationResult(false, new[] { "Password must be at least 8 characters" });
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return new UserAuthenticationResult(false, new[] { "Name is required" });
        }
        
        //Check if email already exists
        var existingUser = await _dbContext.Users.AnyAsync(u => u.Email == inputUsername);

        if (existingUser)
        {
            return new UserAuthenticationResult(false, new[] { "Email already exists" });
        }
        
        

        var newUser = new User
        {
            UserId = Guid.NewGuid().ToString(), // Generate unique ID
            UserName = name,
            Email = inputUsername,
            Password = HashPassword(inputPassword),
            //Phone = phone ?? string.Empty,
            UserRole = role,
            //BusinessId = businessId ? 0,
            BusinessName = businessName,
            CreatedDate = DateTime.UtcNow
        };
        _dbContext.Users.Add(newUser);
        await _dbContext.SaveChangesAsync(CancellationToken.None);
        await SignInNewUser(newUser, httpContext);
        return new UserAuthenticationResult(true, Array.Empty<string>());
    }
    public async Task SignOutAsync(HttpContext httpContext)
    {
        // Sign out and delete the authentication cookie
        await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
    private async Task SignInNewUser(User user, HttpContext httpContext)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.UserRole.ToString()),
            new Claim("FullName", user.UserName)
        };

        if (user.BusinessId > 0)
        {
            claims.Add(new Claim("BusinessId", user.BusinessId.ToString()));
            claims.Add(new Claim("BusinessName", user.BusinessName));
        }

        var claimsIdentity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7),
            AllowRefresh = true
        };

        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
    }
    
    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
    
    private async Task<UserAuthenticationResult> CheckUserAuthentication(string inputUsername, string inputPassword)
    {
        _dbContext.Users.Where(x => x.Email == inputUsername && x.Password == inputPassword);
        return new UserAuthenticationResult(false, []);
    }
    
}



        
    

