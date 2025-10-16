using NUnit.Framework;
using Moq;
using RouteForce.Application.Service;
using RouteForce.Core.Model;
using RouteForce.Core.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using static RouteForce.Application.Service.IAuthService;

namespace RouteForce.Application.Test.ServiceTest;

[TestFixture]
public class AuthServiceTests
{
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IPasswordHasher> _passwordHasherMock;
    private Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private Mock<HttpContext> _httpContextMock;
    private Mock<IAuthenticationService> _authenticationServiceMock;
    private AuthService _authService;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _httpContextMock = new Mock<HttpContext>();
        _authenticationServiceMock = new Mock<IAuthenticationService>();
        var serviceProviderMock = new Mock<IServiceProvider>();

        serviceProviderMock
            .Setup(x => x.GetService(typeof(IAuthenticationService)))
            .Returns(_authenticationServiceMock.Object);

        _httpContextMock
            .Setup(x => x.RequestServices)
            .Returns(serviceProviderMock.Object);

        _httpContextAccessorMock
            .Setup(x => x.HttpContext)
            .Returns(_httpContextMock.Object);

        _authenticationServiceMock
            .Setup(x => x.SignInAsync(
                It.IsAny<HttpContext>(),
                It.IsAny<string>(),
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask);

        _authenticationServiceMock
            .Setup(x => x.SignOutAsync(
                It.IsAny<HttpContext>(),
                It.IsAny<string>(),
                It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask);

        _authService = new AuthService(
            _userRepositoryMock.Object,
            _passwordHasherMock.Object,
            _httpContextAccessorMock.Object
        );
    }

    #region SignIn Tests

    [Test]
    public async Task SignIn_WithValidCredentials_ReturnsTrue()
    {
        // Arrange
        var username = "testuser";
        var password = "Password123!";
        var hashedPassword = "hashedPassword123";

        var user = new User
        {
            UserId = 1,
            Username = username,
            Password = hashedPassword,
            Name = "Test User",
            Email = "test@example.com",
            Phone = "1234567890",
            UserType = UserType.AppUser,
            BusinessId = 0,
            CreatedDate = DateTime.UtcNow
        };

        _userRepositoryMock
            .Setup(x => x.FindByUsernameAsync(username))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.VerifyHashedPassword(hashedPassword, password))
            .Returns(true);

        // Act
        var result = await _authService.SignIn(username, password);

        // Assert
        Assert.That(result, Is.True);
        _userRepositoryMock.Verify(x => x.FindByUsernameAsync(username), Times.Once);
    }

    [Test]
    public async Task SignIn_WithInvalidCredentials_ReturnsFalse()
    {
        // Arrange
        var username = "testuser";
        var password = "WrongPassword";
        var hashedPassword = "hashedPassword123";

        var user = new User
        {
            UserId = 1,
            Username = username,
            Password = hashedPassword,
            Name = "Test User",
            Email = "test@example.com",
            Phone = "1234567890",
            UserType = UserType.AppUser,
            BusinessId = 0,
            CreatedDate = DateTime.UtcNow
        };

        _userRepositoryMock
            .Setup(x => x.FindByUsernameAsync(username))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.VerifyHashedPassword(hashedPassword, password))
            .Returns(false);

        // Act
        var result = await _authService.SignIn(username, password);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task SignIn_WithNonExistentUser_ReturnsFalse()
    {
        // Arrange
        _userRepositoryMock
            .Setup(x => x.FindByUsernameAsync("nonexistent"))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _authService.SignIn("nonexistent", "password");

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    [TestCase("", "password")]
    [TestCase("", "password")]
    [TestCase("username", "")]
    [TestCase("username", "")]
    public async Task SignIn_WithEmptyCredentials_ReturnsFalse(string username, string password)
    {
        // Act
        var result = await _authService.SignIn(username, password);

        // Assert
        Assert.That(result, Is.False);
    }

    #endregion

    #region CreateNewUserAsync Tests

    [Test]
    public async Task CreateNewUserAsync_WithValidData_ReturnsTrue()
    {
        // Arrange
        var username = "newuser";
        var password = "Password123!";
        var hashedPassword = "hashedPassword123";

        var userInfo = new UserInfo
        {
            Name = "New User",
            Email = "newuser@example.com",
            Phone = "1234567890"
        };

        _userRepositoryMock
            .Setup(x => x.FindByUsernameAsync(username))
            .ReturnsAsync((User?)null);

        _passwordHasherMock
            .Setup(x => x.HashPassword(password))
            .Returns(hashedPassword);

        _userRepositoryMock
            .Setup(x => x.CreateUserAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _authService.CreateNewUserAsync(
            username, userInfo, password, password, UserType.AppUser, null);

        // Assert
        Assert.That(result, Is.True);
        _userRepositoryMock.Verify(x => x.CreateUserAsync(It.IsAny<User>()), Times.Once);
    }

    [Test]
    public async Task CreateNewUserAsync_WithMismatchedPasswords_ReturnsFalse()
    {
        // Arrange
        var userInfo = new UserInfo
        {
            Name = "New User",
            Email = "newuser@example.com"
        };

        // Act
        var result = await _authService.CreateNewUserAsync(
            "newuser", userInfo, "Password123!", "DifferentPassword", UserType.AppUser, null);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task CreateNewUserAsync_WithShortPassword_ReturnsFalse()
    {
        // Arrange
        var userInfo = new UserInfo
        {
            Name = "New User",
            Email = "newuser@example.com"
        };

        // Act
        var result = await _authService.CreateNewUserAsync(
            "newuser", userInfo, "short", "short", UserType.AppUser, null);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task CreateNewUserAsync_WithExistingUsername_ReturnsFalse()
    {
        // Arrange
        var userInfo = new UserInfo
        {
            Name = "New User",
            Email = "newuser@example.com"
        };

        var existingUser = new User
        {
            UserId = 1,
            Username = "existinguser",
            Password = "hashedPassword",
            Name = "Existing User",
            Email = "existing@example.com",
            Phone = "",
            UserType = UserType.AppUser,
            BusinessId = 0,
            CreatedDate = DateTime.UtcNow
        };

        _userRepositoryMock
            .Setup(x => x.FindByUsernameAsync("existinguser"))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _authService.CreateNewUserAsync(
            "existinguser", userInfo, "Password123!", "Password123!", UserType.AppUser, null);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task CreateNewUserAsync_WithMissingRequiredFields_ReturnsFalse()
    {
        // Arrange
        var userInfo = new UserInfo
        {
            Name = "",
            Email = "test@example.com"
        };

        // Act
        var result = await _authService.CreateNewUserAsync(
            "newuser", userInfo, "Password123!", "Password123!", UserType.AppUser, null);

        // Assert
        Assert.That(result, Is.False);
    }

    #endregion

    #region SignOut Tests

    [Test]
    public async Task SignOut_CallsSignOutAsync()
    {
        // Act
        await _authService.SignOut();

        // Assert
        _authenticationServiceMock.Verify(x => x.SignOutAsync(
            It.IsAny<HttpContext>(),
            It.IsAny<string>(),
            It.IsAny<AuthenticationProperties>()), Times.Once);
    }

    #endregion
}