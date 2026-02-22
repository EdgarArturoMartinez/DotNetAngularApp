using DotNetCoreWebApi.Application.Entities;
using DotNetCoreWebApi.Application.Interfaces;
using DotNetCoreWebApi.Controllers;
using DotNetCoreWebApi.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Xunit;

namespace DotNetCoreWebApi.Tests.Unit.Controllers;

public class AuthControllerTests
{
    private readonly Mock<ICustomerService> _mockService;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Mock<ILogger<AuthController>> _mockLogger;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockService = new Mock<ICustomerService>();
        _mockEmailService = new Mock<IEmailService>();
        _mockLogger = new Mock<ILogger<AuthController>>();
        _controller = new AuthController(_mockService.Object, _mockEmailService.Object, _mockLogger.Object);
    }

    #region Register Tests

    [Fact]
    public async Task Register_WithValidData_ReturnsOkWithToken()
    {
        // Arrange
        var registerDto = new CustomerRegisterDto
        {
            Email = "test@example.com",
            Password = "Test@123",
            FirstName = "John",
            LastName = "Doe"
        };

        var authResponse = new AuthResponseDto
        {
            Token = "test_token",
            Expiration = DateTime.UtcNow.AddHours(24),
            Customer = new CustomerDto
            {
                Id = 1,
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Role = UserRole.Customer,
                IsActive = true,
                EmailConfirmed = false,
                CreatedAt = DateTime.UtcNow
            }
        };

        _mockService.Setup(x => x.RegisterAsync(registerDto)).ReturnsAsync(authResponse);

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedResponse = okResult.Value.Should().BeOfType<AuthResponseDto>().Subject;
        returnedResponse.Token.Should().Be("test_token");
        returnedResponse.Customer.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task Register_WithExistingEmail_ReturnsBadRequest()
    {
        // Arrange
        var registerDto = new CustomerRegisterDto
        {
            Email = "existing@example.com",
            Password = "Test@123",
            FirstName = "John",
            LastName = "Doe"
        };

        _mockService.Setup(x => x.RegisterAsync(registerDto))
            .ThrowsAsync(new InvalidOperationException("Email already registered"));

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion

    #region Login Tests

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var loginDto = new CustomerLoginDto
        {
            Email = "test@example.com",
            Password = "Test@123"
        };

        var authResponse = new AuthResponseDto
        {
            Token = "test_token",
            Expiration = DateTime.UtcNow.AddHours(24),
            Customer = new CustomerDto
            {
                Id = 1,
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Role = UserRole.Customer,
                IsActive = true,
                EmailConfirmed = false,
                CreatedAt = DateTime.UtcNow
            }
        };

        _mockService.Setup(x => x.LoginAsync(loginDto)).ReturnsAsync(authResponse);

        //Act
        var result = await _controller.Login(loginDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedResponse = okResult.Value.Should().BeOfType<AuthResponseDto>().Subject;
        returnedResponse.Token.Should().Be("test_token");
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginDto = new CustomerLoginDto
        {
            Email = "test@example.com",
            Password = "WrongPassword"
        };

        _mockService.Setup(x => x.LoginAsync(loginDto))
            .ThrowsAsync(new UnauthorizedAccessException("Invalid email or password"));

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    #endregion

    #region Profile Tests

    [Fact]
    public async Task GetProfile_WithValidToken_ReturnsProfile()
    {
        // Arrange
        var customerDto = new CustomerDto
        {
            Id = 1,
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            Role = UserRole.Customer,
            IsActive = true,
            EmailConfirmed = false,
            CreatedAt = DateTime.UtcNow
        };

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1")
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        _mockService.Setup(x => x.GetProfileAsync(1)).ReturnsAsync(customerDto);

        // Act
        var result = await _controller.GetProfile();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProfile = okResult.Value.Should().BeOfType<CustomerDto>().Subject;
        returnedProfile.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task UpdateProfile_WithValidData_ReturnsOk()
    {
        // Arrange
        var updateDto = new CustomerUpdateDto
        {
            FirstName = "Jane",
            LastName = "Smith",
            PhoneNumber = "1234567890"
        };

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1")
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        _mockService.Setup(x => x.UpdateProfileAsync(1, updateDto)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateProfile(updateDto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    #endregion

    #region Check Email Tests

    [Fact]
    public async Task CheckEmail_WithAvailableEmail_ReturnsAvailable()
    {
        // Arrange
        _mockService.Setup(x => x.EmailExistsAsync("available@example.com")).ReturnsAsync(false);

        // Act
        var result = await _controller.CheckEmail("available@example.com");

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task CheckEmail_WithTakenEmail_ReturnsNotAvailable()
    {
        // Arrange
        _mockService.Setup(x => x.EmailExistsAsync("taken@example.com")).ReturnsAsync(true);

        // Act
        var result = await _controller.CheckEmail("taken@example.com");

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().NotBeNull();
    }

    #endregion
}
