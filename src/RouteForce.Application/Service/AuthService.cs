
using RouteForce.Core.Model;
using RouteForce.Core.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace RouteForce.Application.Service
{
    public interface IAuthService
    {
        Task<bool> SignIn(string user, string password);

        Task<bool> CreateNewUserAsync(string user, UserInfo userInfo, string password, string confirmPassword,
            UserType userType, int? businessId);

        Task SignOut();

        public class UserInfo
        {
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string? Phone { get; set; }
        }

        public class AuthService : IAuthService
        {
            private readonly IUserRepository _userRepository;
            private readonly IPasswordHasher _passwordHasher;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public AuthService(
                IUserRepository userRepository,
                IPasswordHasher passwordHasher,
                IHttpContextAccessor httpContextAccessor)
            {
                _userRepository = userRepository;
                _passwordHasher = passwordHasher;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<bool> SignIn(string user, string password)
            {
                if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password))
                    return false;

                var foundUser = await _userRepository.FindByUsernameAsync(user);
                if (foundUser == null)
                    return false;

                if (!_passwordHasher.VerifyHashedPassword(foundUser.Password, password))
                    return false;

                await SignInUser(foundUser);
                return true;
            }

            public async Task<bool> CreateNewUserAsync(
                string user,
                UserInfo userInfo,
                string password,
                string confirmPassword,
                UserType userType,
                int? businessId)
            {
                // Validate passwords match
                if (password != confirmPassword)
                    return false;

                // Validate password strength
                if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                    return false;

                // Validate required fields
                if (string.IsNullOrWhiteSpace(user) ||
                    string.IsNullOrWhiteSpace(userInfo?.Name) ||
                    string.IsNullOrWhiteSpace(userInfo?.Email))
                    return false;

                // Check if username exists
                var existingUser = await _userRepository.FindByUsernameAsync(user);
                if (existingUser != null)
                    return false;

                // Create new user
                var newUser = new User
                {
                    Username = user,
                    Password = _passwordHasher.HashPassword(password),
                    UserType = userType,
                    BusinessId = userType == UserType.Business && businessId.HasValue ? businessId.Value : 0,
                    Name = userInfo.Name,
                    Email = userInfo.Email,
                    Phone = userInfo.Phone ?? string.Empty,
                    CreatedDate = DateTime.UtcNow
                };

                try
                {
                    await _userRepository.CreateUserAsync(newUser);
                    await SignInUser(newUser);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            public async Task SignOut()
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null)
                {
                    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                }
            }

            private async Task SignInUser(User user)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.UserType.ToString()),
                    new Claim("Name", user.Name),
                    new Claim("UserType", user.UserType.ToString())
                };

                if (user.UserType == UserType.Business && user.BusinessId > 0)
                {
                    claims.Add(new Claim("BusinessId", user.BusinessId.ToString()));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7),
                    AllowRefresh = true
                };

                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null)
                {
                    await httpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);
                }
            }
        }
    }
}