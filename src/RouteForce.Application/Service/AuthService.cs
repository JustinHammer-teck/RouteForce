using RouteForce.Core.Enums;
using RouteForce.Core.Model;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace RouteForce.Application.Service;

public class AuthService 
{
    public async Task<bool> SignInAsync(HttpContext httpContext, string username, string password)
    {
        var user = _userRepository.FindByUserName(username);
        if (user == null) return false;

        if (!_passwordHasher.VerifyHashedPassword(user.Password, password))
            return false;

        // Create claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.UserType.ToString())
        };

        // Create identity and principal
        var identity = new ClaimsIdentity(claims, "MyCookieAuth");
        var principal = new ClaimsPrincipal(identity);

        // Sign in
        await httpContext.SignInAsync("MyCookieAuth", principal);

        return true;
    }

    public bool CreateNewUser(string username, User userInfo, string password, string confirmPassword, UserType userType, int? businessId = null)
    {
        if (_userRepository.FindByUserName(username) != null) return false;
        if (password != confirmPassword || string.IsNullOrWhiteSpace(password)) return false;

        var hashedPassword = _passwordHasher.HashPassword(password);

        var newUser = new User
        {
            Username = username,
            Password = hashedPassword,
            UserType = userType,
            BusinessId = userType == UserType.Business ? businessId : null,
            Email = userInfo.Email,
            Phone = userInfo.Phone
        };

        _userRepository.CreateUser(newUser);
        return true;
    }
}