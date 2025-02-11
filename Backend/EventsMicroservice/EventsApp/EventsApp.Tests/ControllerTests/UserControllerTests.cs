using Domain.DTO;
using Domain.Identity;
using EventsApp.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;

public class UserControllerTests
{
    private readonly Mock<UserManager<EventsAppUser>> _mockUserManager;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly UserController _controller;
    private readonly Mock<IConfigurationSection> _mockJwtSettings;
    private string _validToken;

    public UserControllerTests()
    {
        _mockUserManager = new Mock<UserManager<EventsAppUser>>(
            Mock.Of<IUserStore<EventsAppUser>>(),
            null, null, null, null, null, null, null, null
        );
        _mockConfiguration = new Mock<IConfiguration>();
        _mockJwtSettings = new Mock<IConfigurationSection>();

        SetupConfiguration();

        _controller = new UserController(_mockUserManager.Object, _mockConfiguration.Object)
        {
            ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        var user = new EventsAppUser
        {
            Id = "123",
            UserName = "testuser",
            Email = "test@test.com"
        };
        _validToken = GenerateValidToken(user).GetAwaiter().GetResult();
    }

    private async Task<string> GenerateValidToken(EventsAppUser user)
    {
        // Mock roles retrieval
        _mockUserManager.Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "User" });

        // Generate token using the controller's actual method
        return await _controller.GenerateJwtToken(user);
    }

    private void SetupConfiguration()
    {
        _mockConfiguration
            .Setup(c => c["JwtSettings:Key"])
            .Returns("64-bit-keys-are-invalid-use-32-chars-here");

        _mockConfiguration
            .Setup(c => c[It.Is<string>(s => s == "JwtSettings:Issuer")])
            .Returns("test-issuer");

        _mockConfiguration
            .Setup(c => c[It.Is<string>(s => s == "JwtSettings:Audience")])
            .Returns("test-audience");
    }

    [Fact]
    public void GetAllUsers_ShouldReturnUsersList()
    {
        var users = new List<EventsAppUser> { new EventsAppUser { Id = "1", UserName = "user1" } }.AsQueryable();
        _mockUserManager.Setup(m => m.Users).Returns(users);

        var result = _controller.GetAllUsers();
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUsers = Assert.IsType<List<EventsAppUser>>(okResult.Value);
        Assert.Single(returnedUsers);
    }

    [Fact]
    public async Task GetUser_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        _mockUserManager.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
                        .ReturnsAsync((EventsAppUser)null);

        // Act
        var result = await _controller.GetUser("invalid-id");

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetUser_ShouldReturnOk_WhenUserExists()
    {
        // Arrange
        var user = new EventsAppUser { Id = "valid-id", UserName = "testuser" };
        _mockUserManager.Setup(m => m.FindByIdAsync("valid-id"))
                        .ReturnsAsync(user);

        // Act
        var result = await _controller.GetUser("valid-id");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(user, okResult.Value);
    }

    [Fact]
    public async Task CreateUser_ShouldReturnCreated_WhenUserIsCreatedSuccessfully()
    {
        // Arrange
        var model = new CreateUserDto
        {
            UserName = "newuser",
            Email = "newuser@example.com",
            Password = "Password123"
        };
        var expectedUserId = "new-id";


        _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<EventsAppUser>(), It.IsAny<string>()))
            .Callback<EventsAppUser, string>((user, password) =>
            {
                user.Id = expectedUserId;
            })
            .ReturnsAsync(IdentityResult.Success);

        _mockUserManager.Setup(m => m.AddToRoleAsync(It.IsAny<EventsAppUser>(), "User"))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _controller.CreateUser(model);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal("GetUser", createdAtActionResult.ActionName);
        Assert.Equal(expectedUserId, createdAtActionResult.RouteValues["id"]);

        var createdUser = Assert.IsType<EventsAppUser>(createdAtActionResult.Value);
        Assert.Equal(model.UserName, createdUser.UserName);
        Assert.Equal(model.Email, createdUser.Email);
        Assert.Equal(expectedUserId, createdUser.Id);
    }



    [Fact]
    public async Task CreateUser_ShouldReturnBadRequest_WhenUserCreationFails()
    {
        // Arrange
        var model = new CreateUserDto { UserName = "newuser", Email = "newuser@example.com", Password = "Password123" };

        _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<EventsAppUser>(), It.IsAny<string>()))
                        .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "User creation failed" }));

        // Act
        var result = await _controller.CreateUser(model);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errors = Assert.IsAssignableFrom<IEnumerable<IdentityError>>(badRequestResult.Value);
        Assert.Contains(errors, e => e.Description == "User creation failed");
    }

    [Fact]
    public async Task CreateUser_ShouldReturnBadRequest_WhenRoleAssignmentFails()
    {
        // Arrange
        var model = new CreateUserDto { UserName = "newuser", Email = "newuser@example.com", Password = "Password123" };
        var user = new EventsAppUser { Id = "new-id", UserName = "newuser", Email = "newuser@example.com" };

        _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<EventsAppUser>(), It.IsAny<string>()))
                        .ReturnsAsync(IdentityResult.Success);

        _mockUserManager.Setup(m => m.AddToRoleAsync(It.IsAny<EventsAppUser>(), "User"))
                        .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Role assignment failed" }));

        // Act
        var result = await _controller.CreateUser(model);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errors = Assert.IsAssignableFrom<IEnumerable<IdentityError>>(badRequestResult.Value);
        Assert.Contains(errors, e => e.Description == "Role assignment failed");
    }

    [Fact]
    public async Task CreateAdmin_AdminUser_CreatesAdminSuccessfully()
    {
        // Arrange
        var model = new CreateUserDto { UserName = "admin", Email = "admin@test.com", Password = "P@ssw0rd" };
        var user = new EventsAppUser { UserName = model.UserName, Email = model.Email };

        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<EventsAppUser>(), model.Password))
            .ReturnsAsync(IdentityResult.Success);
        _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<EventsAppUser>(), "Admin"))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _controller.CreateAdmin(model);

        // Assert
        var createdAtResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(UserController.GetUser), createdAtResult.ActionName);
        _mockUserManager.Verify(x => x.CreateAsync(It.IsAny<EventsAppUser>(), model.Password), Times.Once);
        _mockUserManager.Verify(x => x.AddToRoleAsync(It.IsAny<EventsAppUser>(), "Admin"), Times.Once);
    }

    [Fact]
    public async Task CreateAdmin_ShouldReturnBadRequest_WhenUserCreationFails()
    {
        // Arrange
        var model = new CreateUserDto { UserName = "adminuser", Email = "admin@example.com", Password = "Password123" };

        _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<EventsAppUser>(), It.IsAny<string>()))
                        .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "User creation failed" }));

        // Act
        var result = await _controller.CreateAdmin(model);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errors = Assert.IsAssignableFrom<IEnumerable<IdentityError>>(badRequestResult.Value);
        Assert.Contains(errors, e => e.Description == "User creation failed");
    }

    [Fact]
    public async Task CreateAdmin_ShouldReturnBadRequest_WhenRoleAssignmentFails()
    {
        // Arrange
        var model = new CreateUserDto { UserName = "adminuser", Email = "admin@example.com", Password = "Password123" };
        var user = new EventsAppUser { Id = "new-id", UserName = "adminuser", Email = "admin@example.com" };

        _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<EventsAppUser>(), It.IsAny<string>()))
                        .ReturnsAsync(IdentityResult.Success);

        _mockUserManager.Setup(m => m.AddToRoleAsync(It.IsAny<EventsAppUser>(), "Admin"))
                        .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Role assignment failed" }));

        // Act
        var result = await _controller.CreateAdmin(model);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errors = Assert.IsAssignableFrom<IEnumerable<IdentityError>>(badRequestResult.Value);
        Assert.Contains(errors, e => e.Description == "Role assignment failed");
    }


    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkWithCookie()
    {
        // Arrange
        var model = new LoginDto { Email = "test@test.com", Password = "P@ssw0rd" };
        var user = new EventsAppUser { Email = model.Email, UserName = "testuser" };

        _mockUserManager.Setup(x => x.FindByEmailAsync(model.Email))
            .ReturnsAsync(user);

        _mockUserManager.Setup(x => x.CheckPasswordAsync(user, model.Password))
            .ReturnsAsync(true);

        _mockUserManager.Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "User" });

        // Act
        var result = await _controller.Login(model);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Login_InvalidUser_ReturnsUnauthorized()
    {
        // Arrange
        var model = new LoginDto { Email = "wrong@test.com", Password = "P@ssw0rd" };

        _mockUserManager.Setup(x => x.FindByEmailAsync(model.Email))
            .ReturnsAsync((EventsAppUser)null);

        // Act
        var result = await _controller.Login(model);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);

        var message = unauthorizedResult.Value?.GetType().GetProperty("message")?.GetValue(unauthorizedResult.Value);
        Assert.Equal("Invalid username or password", message);

        Assert.DoesNotContain("Set-Cookie", _controller.Response.Headers);
    }

    [Fact]
    public async Task Login_InvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var model = new LoginDto { Email = "test@test.com", Password = "WrongPassword" };
        var user = new EventsAppUser { Email = model.Email, UserName = "testuser" };

        _mockUserManager.Setup(x => x.FindByEmailAsync(model.Email))
            .ReturnsAsync(user);
        _mockUserManager.Setup(x => x.CheckPasswordAsync(user, It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Login(model);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);

        var message = unauthorizedResult.Value?.GetType().GetProperty("message")?.GetValue(unauthorizedResult.Value);
        Assert.Equal("Invalid username or password", message);

        Assert.DoesNotContain("Set-Cookie", _controller.Response.Headers);
    }

    [Fact]
    public void Validate_ValidToken_ReturnsUsername()
    {
        // Arrange
        _controller.ControllerContext.HttpContext.Request.Cookies =
            new MockCookieCollection("jwt", _validToken);

        // Act
        var result = _controller.Validation();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("testuser", okResult.Value);
    }

    [Fact]
    public void Validate_InvalidToken_ReturnsUnauthorized()
    {
        // Arrange
        _controller.ControllerContext.HttpContext.Request.Cookies =
            new MockCookieCollection("jwt", "invalid-token");

        // Act
        var result = _controller.Validation();

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public void Validate_MissingToken_ReturnsUnauthorized()
    {
        // Arrange - No cookies set
        _controller.ControllerContext.HttpContext.Request.Cookies =
            new MockCookieCollection("jwt", "");

        // Act
        var result = _controller.Validation();

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task IsAdmin_AdminUser_ReturnsTrue()
    {
        // Arrange
        var user = new EventsAppUser
        {
            Id = "123",
            UserName = "adminuser",
            Email = "admin@test.com"
        };

        _mockUserManager.Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "Admin" });

        var validToken = await _controller.GenerateJwtToken(user);

        _controller.ControllerContext.HttpContext.Request.Cookies =
            new MockCookieCollection("jwt", validToken);

        // Act
        var result = _controller.isAdmin();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)okResult.Value);
    }

    [Fact]
    public async Task IsAdmin_NonAdminUser_ReturnsFalse()
    {
        // Arrange
        var user = new EventsAppUser
        {
            Id = "456",
            UserName = "regularuser",
            Email = "user@test.com"
        };

        _mockUserManager.Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "User" });

        var validToken = await _controller.GenerateJwtToken(user);

        _controller.ControllerContext.HttpContext.Request.Cookies =
            new MockCookieCollection("jwt", validToken);

        // Act
        var result = _controller.isAdmin();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.False((bool)okResult.Value);
    }

    [Fact]
    public void IsAdmin_NoToken_ReturnsUnauthorized()
    {
        // Arrange
        _controller.ControllerContext.HttpContext.Request.Cookies =
            new MockCookieCollection("jwt", "");

        // Act
        var result = _controller.isAdmin();

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Not authenticated", unauthorizedResult.Value);
    }


    [Fact]
    public void Logout_ClearsCookie()
    {
        // Act
        var result = _controller.Logout();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Logged out successfully", okResult.Value?.GetType().GetProperty("message")?.GetValue(okResult.Value));

        var cookieHeader = _controller.Response.Headers["Set-Cookie"].ToString();

        Assert.Contains("jwt=;", cookieHeader);
        Assert.Contains("expires=", cookieHeader, StringComparison.OrdinalIgnoreCase);

        var expiresMatch = Regex.Match(cookieHeader, @"expires=([^;]+)", RegexOptions.IgnoreCase);
        Assert.True(expiresMatch.Success);

        var parsedSuccessfully = DateTime.TryParseExact(
            expiresMatch.Groups[1].Value,
            "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal,
            out var expiresDate
        );

        Assert.True(parsedSuccessfully, "Invalid date format");
        Assert.True(expiresDate < DateTime.UtcNow.AddSeconds(-1), "Expiration not in past");
    }


    private class MockCookieCollection : IRequestCookieCollection
    {
        private readonly Dictionary<string, string> _cookies = new Dictionary<string, string>();

        public MockCookieCollection(string key, string value)
        {
            _cookies[key] = value;
        }

        public string this[string key] => _cookies[key];

        public int Count => _cookies.Count;

        public ICollection<string> Keys => _cookies.Keys;

        public bool ContainsKey(string key) => _cookies.ContainsKey(key);

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _cookies.GetEnumerator();

        public bool TryGetValue(string key, out string value) => _cookies.TryGetValue(key, out value);

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }

}
