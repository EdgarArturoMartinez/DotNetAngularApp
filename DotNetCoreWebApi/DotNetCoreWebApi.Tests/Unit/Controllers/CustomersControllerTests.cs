using DotNetCoreWebApi.Application.Entities;
using DotNetCoreWebApi.Application.Interfaces;
using DotNetCoreWebApi.Controllers;
using DotNetCoreWebApi.Application.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DotNetCoreWebApi.Tests.Unit.Controllers;

public class CustomersControllerTests
{
    private readonly Mock<ICustomerService> _mockService;
    private readonly Mock<ILogger<CustomersController>> _mockLogger;
    private readonly CustomersController _controller;

    public CustomersControllerTests()
    {
        _mockService = new Mock<ICustomerService>();
        _mockLogger = new Mock<ILogger<CustomersController>>();
        _controller = new CustomersController(_mockService.Object, _mockLogger.Object);
    }

    #region GetAllCustomers Tests

    [Fact]
    public async Task GetAllCustomers_ReturnsAllCustomers()
    {
        // Arrange
        var customers = new List<CustomerDto>
        {
            new CustomerDto { Id = 1, Email = "customer1@example.com", FirstName = "John", LastName = "Doe", Role = UserRole.Customer, IsActive = true, EmailConfirmed = false, CreatedAt = DateTime.UtcNow },
            new CustomerDto { Id = 2, Email = "customer2@example.com", FirstName = "Jane", LastName = "Smith", Role = UserRole.Customer, IsActive = true, EmailConfirmed = false, CreatedAt = DateTime.UtcNow }
        };

        _mockService.Setup(x => x.GetAllCustomersAsync()).ReturnsAsync(customers);

        // Act
        var result = await _controller.GetAllCustomers();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCustomers = okResult.Value.Should().BeAssignableTo<IEnumerable<CustomerDto>>().Subject;
        returnedCustomers.Should().HaveCount(2);
    }

    #endregion

    #region GetCustomerById Tests

    [Fact]
    public async Task GetCustomerById_WithValidId_ReturnsCustomer()
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

        _mockService.Setup(x => x.GetCustomerByIdAsync(1)).ReturnsAsync(customerDto);

        // Act
        var result = await _controller.GetCustomerById(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCustomer = okResult.Value.Should().BeOfType<CustomerDto>().Subject;
        returnedCustomer.Id.Should().Be(1);
        returnedCustomer.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task GetCustomerById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(x => x.GetCustomerByIdAsync(999)).ReturnsAsync((CustomerDto?)null);

        // Act
        var result = await _controller.GetCustomerById(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region GetCustomersByRole Tests

    [Fact]
    public async Task GetCustomersByRole_ReturnsCustomersWithRole()
    {
        // Arrange
        var admins = new List<CustomerDto>
        {
            new CustomerDto { Id = 1, Email = "admin1@example.com", FirstName = "Admin", LastName = "One", Role = UserRole.Admin, IsActive = true, EmailConfirmed = false, CreatedAt = DateTime.UtcNow },
            new CustomerDto { Id = 2, Email = "admin2@example.com", FirstName = "Admin", LastName = "Two", Role = UserRole.Admin, IsActive = true, EmailConfirmed = false, CreatedAt = DateTime.UtcNow }
        };

        _mockService.Setup(x => x.GetCustomersByRoleAsync(UserRole.Admin)).ReturnsAsync(admins);

        // Act
        var result = await _controller.GetCustomersByRole(UserRole.Admin);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCustomers = okResult.Value.Should().BeAssignableTo<IEnumerable<CustomerDto>>().Subject;
        returnedCustomers.Should().HaveCount(2);
        returnedCustomers.Should().OnlyContain(c => c.Role == UserRole.Admin);
    }

    #endregion

    #region CreateCustomer Tests

    [Fact]
    public async Task CreateCustomer_WithValidData_ReturnsCreated()
    {
        // Arrange
        var request = new CreateCustomerRequest
        {
            Customer = new CustomerAdminDto
            {
                Email = "newcustomer@example.com",
                FirstName = "New",
                LastName = "Customer",
                Role = UserRole.Customer,
                IsActive = true
            },
            Password = "Test@123"
        };

        var createdCustomer = new CustomerDto
        {
            Id = 10,
            Email = "newcustomer@example.com",
            FirstName = "New",
            LastName = "Customer",
            Role = UserRole.Customer,
            IsActive = true,
            EmailConfirmed = false,
            CreatedAt = DateTime.UtcNow
        };

        _mockService.Setup(x => x.CreateCustomerAsync(request.Customer, request.Password))
            .ReturnsAsync(createdCustomer);

        // Act
        var result = await _controller.CreateCustomer(request);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var returnedCustomer = createdResult.Value.Should().BeOfType<CustomerDto>().Subject;
        returnedCustomer.Id.Should().Be(10);
        returnedCustomer.Email.Should().Be("newcustomer@example.com");
    }

    [Fact]
    public async Task CreateCustomer_WithExistingEmail_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateCustomerRequest
        {
            Customer = new CustomerAdminDto
            {
                Email = "existing@example.com",
                FirstName = "Existing",
                LastName = "User",
                Role = UserRole.Customer,
                IsActive = true
            },
            Password = "Test@123"
        };

        _mockService.Setup(x => x.CreateCustomerAsync(request.Customer, request.Password))
            .ThrowsAsync(new InvalidOperationException("Email already registered"));

        // Act
        var result = await _controller.CreateCustomer(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion

    #region UpdateCustomer Tests

    [Fact]
    public async Task UpdateCustomer_WithValidData_ReturnsOk()
    {
        // Arrange
        var adminDto = new CustomerAdminDto
        {
            Email = "updated@example.com",
            FirstName = "Updated",
            LastName = "User",
            Role = UserRole.Customer,
            IsActive = true
        };

        _mockService.Setup(x => x.UpdateCustomerAsync(1, adminDto)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateCustomer(1, adminDto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task UpdateCustomer_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var adminDto = new CustomerAdminDto
        {
            Email = "updated@example.com",
            FirstName = "Updated",
            LastName = "User",
            Role = UserRole.Customer,
            IsActive = true
        };

        _mockService.Setup(x => x.UpdateCustomerAsync(999, adminDto))
            .ThrowsAsync(new KeyNotFoundException("Customer with ID 999 not found"));

        // Act
        var result = await _controller.UpdateCustomer(999, adminDto);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region DeleteCustomer Tests

    [Fact]
    public async Task DeleteCustomer_WithValidId_ReturnsOk()
    {
        // Arrange
        _mockService.Setup(x => x.DeleteCustomerAsync(1)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteCustomer(1);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task DeleteCustomer_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(x => x.DeleteCustomerAsync(999))
            .ThrowsAsync(new KeyNotFoundException("Customer with ID 999 not found"));

        // Act
        var result = await _controller.DeleteCustomer(999);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion
}
