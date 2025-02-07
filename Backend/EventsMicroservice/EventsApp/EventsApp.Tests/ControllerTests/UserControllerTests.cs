 using Moq;
using Xunit;
using EventsApp.Controllers;
using Microsoft.AspNetCore.Identity;
using Domain.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Domain.DTO;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Dynamic;

public class UserControllerTests
{
    private readonly Mock<UserManager<EventsAppUser>> _mockUserManager;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _mockUserManager = new Mock<UserManager<EventsAppUser>>(
            Mock.Of<IUserStore<EventsAppUser>>(),
            null, null, null, null, null, null, null, null
        );
        _mockConfiguration = new Mock<IConfiguration>();
        _controller = new UserController(_mockUserManager.Object, _mockConfiguration.Object);
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
        var model = new CreateUserDto { UserName = "newuser", Email = "newuser@example.com", Password = "Password123" };
        var user = new EventsAppUser { Id = "new-id", UserName = "newuser", Email = "newuser@example.com" };

        // Mock UserManager methods
        _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<EventsAppUser>(), It.IsAny<string>()))
                        .ReturnsAsync(IdentityResult.Success);
        _mockUserManager.Setup(m => m.AddToRoleAsync(It.IsAny<EventsAppUser>(), "User"))
                        .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _controller.CreateUser(model);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal("GetUser", createdAtActionResult.ActionName);
        Assert.Equal("new-id", createdAtActionResult.RouteValues["id"]);
        Assert.Equal(user, createdAtActionResult.Value);
    }

    [Fact]
    public async Task CreateUser_ShouldReturnBadRequest_WhenUserCreationFails()
    {
        // Arrange
        var model = new CreateUserDto { UserName = "newuser", Email = "newuser@example.com", Password = "Password123" };

        // Mock UserManager CreateAsync to return failure
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

        // Mock UserManager CreateAsync to succeed
        _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<EventsAppUser>(), It.IsAny<string>()))
                        .ReturnsAsync(IdentityResult.Success);

        // Mock AddToRoleAsync to fail
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
    public async Task CreateAdmin_ShouldReturnCreated_WhenAdminIsCreatedSuccessfully()
    {
        // Arrange
        var model = new CreateUserDto { UserName = "adminuser", Email = "admin@example.com", Password = "Password123" };
        var user = new EventsAppUser { Id = "new-id", UserName = "adminuser", Email = "admin@example.com" };

        // Mock UserManager CreateAsync to return success
        _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<EventsAppUser>(), It.IsAny<string>()))
                        .ReturnsAsync(IdentityResult.Success);

        // Mock AddToRoleAsync to return success
        _mockUserManager.Setup(m => m.AddToRoleAsync(It.IsAny<EventsAppUser>(), "Admin"))
                        .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _controller.CreateAdmin(model);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal("GetUser", createdAtActionResult.ActionName);
        Assert.Equal("new-id", createdAtActionResult.RouteValues["id"]);
        Assert.Equal(user, createdAtActionResult.Value);
    }

    [Fact]
    public async Task CreateAdmin_ShouldReturnBadRequest_WhenUserCreationFails()
    {
        // Arrange
        var model = new CreateUserDto { UserName = "adminuser", Email = "admin@example.com", Password = "Password123" };

        // Mock UserManager CreateAsync to return failure
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

        // Mock UserManager CreateAsync to succeed
        _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<EventsAppUser>(), It.IsAny<string>()))
                        .ReturnsAsync(IdentityResult.Success);

        // Mock AddToRoleAsync to fail
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
    public async Task CreateAdmin_ShouldReturnForbidden_WhenUserIsNotAuthorized()
    {
        // Arrange
        var model = new CreateUserDto { UserName = "adminuser", Email = "admin@example.com", Password = "Password123" };

        // Mock HttpContext and User
        var mockHttpContext = new Mock<HttpContext>();
        var mockUser = new Mock<ClaimsPrincipal>();

        // Set up the user not having the "Admin" role
        mockUser.Setup(u => u.IsInRole("Admin")).Returns(false);

        mockHttpContext.Setup(c => c.User).Returns(mockUser.Object);

        // Mock the Controller's HttpContext
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = mockHttpContext.Object
        };

        // Act
        var result = await _controller.CreateAdmin(model);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }
    /*[Fact]
    public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
    {
        // Arrange
        var model = new LoginDto { Email = "user@example.com", Password = "Password123" };
        var user = new EventsAppUser { Email = "user@example.com", UserName = "username" };
        var mockJwtToken = "mock-jwt-token";

        // Mock FindByEmailAsync to return the user
        _mockUserManager.Setup(m => m.FindByEmailAsync(model.Email))
                        .ReturnsAsync(user);

        // Mock CheckPasswordAsync to return true
        _mockUserManager.Setup(m => m.CheckPasswordAsync(user, model.Password))
                        .ReturnsAsync(true);

        // Mock GenerateJwtToken to return a mocked token
        _mockJwtService.Setup(s => s.GenerateJwtToken(user))
                       .ReturnsAsync(mockJwtToken);

        // Act
        var result = await _controller.Login(model);

        // Assert for Ok result and message
        var okResult = Assert.IsType<OkObjectResult>(result);
        dynamic returnValue = okResult.Value;
        Assert.Equal("Logged in successfully", returnValue.message);

        // Assert that the cookie is set correctly (mocked here for simplicity)
        Assert.True(_mockHttpContext.Response.Cookies.ContainsKey("jwt"));
    }


    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
    {
        // Arrange
        var model = new LoginDto { Email = "user@example.com", Password = "WrongPassword" };
        var user = new EventsAppUser { Email = "user@example.com", UserName = "username" };

        // Mock FindByEmailAsync to return the user
        _mockUserManager.Setup(m => m.FindByEmailAsync(model.Email))
                        .ReturnsAsync(user);

        // Mock CheckPasswordAsync to return false
        _mockUserManager.Setup(m => m.CheckPasswordAsync(user, model.Password))
                        .ReturnsAsync(false);

        // Act
        var result = await _controller.Login(model);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        var returnValue = Assert.IsType<ExpandoObject>(unauthorizedResult.Value);
        Assert.Contains("message", returnValue);
        Assert.Equal("Invalid username or password", returnValue.message);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenUserNotFound()
    {
        // Arrange
        var model = new LoginDto { Email = "nonexistentuser@example.com", Password = "Password123" };

        // Mock FindByEmailAsync to return null (user not found)
        _mockUserManager.Setup(m => m.FindByEmailAsync(model.Email))
                        .ReturnsAsync((EventsAppUser)null);

        // Act
        var result = await _controller.Login(model);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        var returnValue = Assert.IsType<ExpandoObject>(unauthorizedResult.Value);
        Assert.Contains("message", returnValue);
        Assert.Equal("Invalid username or password", returnValue.message);
    }*/
}
