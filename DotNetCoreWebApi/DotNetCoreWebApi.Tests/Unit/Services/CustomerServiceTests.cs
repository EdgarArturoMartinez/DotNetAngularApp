using DotNetCoreWebApi.Application.Entities;
using DotNetCoreWebApi.Application.Interfaces;
using DotNetCoreWebApi.Application.Services;
using DotNetCoreWebApi.DTOs;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace DotNetCoreWebApi.Tests.Unit.Services;

public class CustomerServiceTests
{
    private readonly Mock<ICustomerRepository> _mockRepository;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly CustomerService _service;

    public CustomerServiceTests()
    {
        _mockRepository = new Mock<ICustomerRepository>();
        _mockConfiguration = new Mock<IConfiguration>();

        // Setup JWT configuration
        var jwtSection = new Mock<IConfigurationSection>();
        jwtSection.Setup(x => x["SecretKey"]).Returns("TestSecretKeyForJWTAuthenticationTesting!@#$%^&*()1234567890");
        jwtSection.Setup(x => x["Issuer"]).Returns("TestIssuer");
        jwtSection.Setup(x => x["Audience"]).Returns("TestAudience");

        _mockConfiguration.Setup(x => x.GetSection("JwtSettings")).Returns(jwtSection.Object);

        _service = new CustomerService(_mockRepository.Object, _mockConfiguration.Object);
    }

    #region Register Tests

    [Fact]
    public async Task RegisterAsync_WithValidData_CreatesCustomerAndReturnsToken()
    {
        // Arrange
        var registerDto = new CustomerRegisterDto
        {
            Email = "test@example.com",
            Password = "Test@123",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "1234567890"
        };

        _mockRepository.Setup(x => x.EmailExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        _mockRepository.Setup(x => x.AddAsync(It.IsAny<Customer>())).ReturnsAsync((Customer c) =>
        {
            c.Id = 1;
            return c;
        });

        // Act
        var result = await _service.RegisterAsync(registerDto);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.Customer.Should().NotBeNull();
        result.Customer.Email.Should().Be("test@example.com");
        result.Customer.FirstName.Should().Be("John");
        result.Customer.LastName.Should().Be("Doe");
        result.Customer.Role.Should().Be(UserRole.Customer);
        result.Customer.IsActive.Should().BeTrue();
        _mockRepository.Verify(x => x.AddAsync(It.IsAny<Customer>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ThrowsInvalidOperationException()
    {
        // Arrange
        var registerDto = new CustomerRegisterDto
        {
            Email = "existing@example.com",
            Password = "Test@123",
            FirstName = "John",
            LastName = "Doe"
        };

        _mockRepository.Setup(x => x.EmailExistsAsync("existing@example.com")).ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RegisterAsync(registerDto));
        _mockRepository.Verify(x => x.AddAsync(It.IsAny<Customer>()), Times.Never);
    }

    #endregion

    #region Login Tests

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsToken()
    {
        // Arrange
        var password = "Test@123";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        var customer = new Customer
        {
            Id = 1,
            Email = "test@example.com",
            PasswordHash = passwordHash,
            FirstName = "John",
            LastName = "Doe",
            Role = UserRole.Customer,
            IsActive = true
        };

        var loginDto = new CustomerLoginDto
        {
            Email = "test@example.com",
            Password = password
        };

        _mockRepository.Setup(x => x.GetByEmailAsync("test@example.com")).ReturnsAsync(customer);
        _mockRepository.Setup(x => x.UpdateLastLoginAsync(1)).Returns(Task.CompletedTask);

        // Act
        var result = await _service.LoginAsync(loginDto);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.Customer.Email.Should().Be("test@example.com");
        _mockRepository.Verify(x => x.UpdateLastLoginAsync(1), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidEmail_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var loginDto = new CustomerLoginDto
        {
            Email = "nonexistent@example.com",
            Password = "Test@123"
        };

        _mockRepository.Setup(x => x.GetByEmailAsync("nonexistent@example.com")).ReturnsAsync((Customer?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.LoginAsync(loginDto));
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword");
        var customer = new Customer
        {
            Id = 1,
            Email = "test@example.com",
            PasswordHash = passwordHash,
            FirstName = "John",
            LastName = "Doe",
            Role = UserRole.Customer,
            IsActive = true
        };

        var loginDto = new CustomerLoginDto
        {
            Email = "test@example.com",
            Password = "WrongPassword"
        };

        _mockRepository.Setup(x => x.GetByEmailAsync("test@example.com")).ReturnsAsync(customer);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.LoginAsync(loginDto));
    }

    [Fact]
    public async Task LoginAsync_WithInactiveAccount_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var password = "Test@123";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        var customer = new Customer
        {
            Id = 1,
            Email = "test@example.com",
            PasswordHash = passwordHash,
            FirstName = "John",
            LastName = "Doe",
            Role = UserRole.Customer,
            IsActive = false
        };

        var loginDto = new CustomerLoginDto
        {
            Email = "test@example.com",
            Password = password
        };

        _mockRepository.Setup(x => x.GetByEmailAsync("test@example.com")).ReturnsAsync(customer);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.LoginAsync(loginDto));
    }

    #endregion

    #region Profile Tests

    [Fact]
    public async Task GetProfileAsync_WithValidId_ReturnsCustomer()
    {
        // Arrange
        var customer = new Customer
        {
            Id = 1,
            Email = "test@example.com",
            PasswordHash = "hash",
            FirstName = "John",
            LastName = "Doe",
            Role = UserRole.Customer,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(customer);

        // Act
        var result = await _service.GetProfileAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Email.Should().Be("test@example.com");
        result.FirstName.Should().Be("John");
    }

    [Fact]
    public async Task GetProfileAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Customer?)null);

        // Act
        var result = await _service.GetProfileAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateProfileAsync_WithValidData_UpdatesCustomer()
    {
        // Arrange
        var customer = new Customer
        {
            Id = 1,
            Email = "test@example.com",
            PasswordHash = "hash",
            FirstName = "John",
            LastName = "Doe",
            Role = UserRole.Customer,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var updateDto = new CustomerUpdateDto
        {
            FirstName = "Jane",
            LastName = "Smith",
            PhoneNumber = "9876543210",
            City = "New York"
        };

        _mockRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(customer);
        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<Customer>())).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateProfileAsync(1, updateDto);

        // Assert
        customer.FirstName.Should().Be("Jane");
        customer.LastName.Should().Be("Smith");
        customer.PhoneNumber.Should().Be("9876543210");
        customer.City.Should().Be("New York");
        customer.UpdatedAt.Should().NotBeNull();
        _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<Customer>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProfileAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var updateDto = new CustomerUpdateDto
        {
            FirstName = "Jane",
            LastName = "Smith"
        };

        _mockRepository.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Customer?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateProfileAsync(999, updateDto));
    }

    #endregion

    #region Admin Tests

    [Fact]
    public async Task GetAllCustomersAsync_ReturnsAllCustomers()
    {
        // Arrange
        var customers = new List<Customer>
        {
            new Customer { Id = 1, Email = "customer1@example.com", PasswordHash = "hash", FirstName = "John", LastName = "Doe", Role = UserRole.Customer, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Customer { Id = 2, Email = "customer2@example.com", PasswordHash = "hash", FirstName = "Jane", LastName = "Smith", Role = UserRole.Customer, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Customer { Id = 3, Email = "admin@example.com", PasswordHash = "hash", FirstName = "Admin", LastName = "User", Role = UserRole.Admin, IsActive = true, CreatedAt = DateTime.UtcNow }
        };

        _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(customers);

        // Act
        var result = await _service.GetAllCustomersAsync();

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task CreateCustomerAsync_WithValidData_CreatesCustomer()
    {
        // Arrange
        var adminDto = new CustomerAdminDto
        {
            Email = "newadmin@example.com",
            FirstName = "Admin",
            LastName = "User",
            Role = UserRole.Admin,
            IsActive = true
        };

        _mockRepository.Setup(x => x.EmailExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        _mockRepository.Setup(x => x.AddAsync(It.IsAny<Customer>())).ReturnsAsync((Customer c) =>
        {
            c.Id = 10;
            return c;
        });

        // Act
        var result = await _service.CreateCustomerAsync(adminDto, "Admin@123");

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be("newadmin@example.com");
        result.Role.Should().Be(UserRole.Admin);
        _mockRepository.Verify(x => x.AddAsync(It.IsAny<Customer>()), Times.Once);
    }

    [Fact]
    public async Task DeleteCustomerAsync_WithValidId_DeletesCustomer()
    {
        // Arrange
        var customer = new Customer
        {
            Id = 1,
            Email = "test@example.com",
            PasswordHash = "hash",
            FirstName = "John",
            LastName = "Doe",
            Role = UserRole.Customer,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(customer);
        _mockRepository.Setup(x => x.DeleteAsync(It.IsAny<Customer>())).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteCustomerAsync(1);

        // Assert
        _mockRepository.Verify(x => x.DeleteAsync(It.IsAny<Customer>()), Times.Once);
    }

    [Fact]
    public async Task DeleteCustomerAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Customer?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteCustomerAsync(999));
    }

    #endregion
}
