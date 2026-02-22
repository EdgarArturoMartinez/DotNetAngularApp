using DotNetCoreWebApi.Application.Entities;
using DotNetCoreWebApi.DTOs;
using DotNetCoreWebApi.Tests.Helpers;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace DotNetCoreWebApi.Tests.Integration;

/// <summary>
/// Integration tests for customer authentication and management workflows
/// Tests the complete flow from registration to login to profile management
/// </summary>
public class CustomerAuthIntegrationTests : IDisposable
{
    private readonly Application.DBContext.ApplicationDBContext _context;
    private readonly Infrastructure.Repositories.CustomerRepository _repository;
    private readonly Application.Services.CustomerService _service;

    public CustomerAuthIntegrationTests()
    {
        _context = TestDbContextFactory.CreateInMemoryContext();
        _repository = new Infrastructure.Repositories.CustomerRepository(_context);
        
        // Setup configuration for JWT using mock
        var jwtSection = new Moq.Mock<Microsoft.Extensions.Configuration.IConfigurationSection>();
        jwtSection.Setup(x => x["SecretKey"]).Returns("TestSecretKeyForJWTAuthenticationTesting!@#$%^&*()1234567890");
        jwtSection.Setup(x => x["Issuer"]).Returns("TestIssuer");
        jwtSection.Setup(x => x["Audience"]).Returns("TestAudience");

        var configuration = new Moq.Mock<Microsoft.Extensions.Configuration.IConfiguration>();
        configuration.Setup(x => x.GetSection("JwtSettings")).Returns(jwtSection.Object);

        _service = new Application.Services.CustomerService(_repository, configuration.Object);
    }

    [Fact]
    public async Task CompleteAuthenticationFlow_RegisterLoginAndUpdateProfile_Success()
    {
        // Step 1: Register a new customer
        var registerDto = new CustomerRegisterDto
        {
            Email = "integration@example.com",
            Password = "Test@123",
            FirstName = "Integration",
            LastName = "Test",
            PhoneNumber = "1234567890",
            City = "Test City"
        };

        var registerResponse = await _service.RegisterAsync(registerDto);

        // Assert registration
        registerResponse.Should().NotBeNull();
        registerResponse.Token.Should().NotBeNullOrEmpty();
        registerResponse.Customer.Email.Should().Be("integration@example.com");
        registerResponse.Customer.Role.Should().Be(UserRole.Customer);
        registerResponse.Customer.IsActive.Should().BeTrue();

        // Step 2: Login with registered credentials
        var loginDto = new CustomerLoginDto
        {
            Email = "integration@example.com",
            Password = "Test@123"
        };

        var loginResponse = await _service.LoginAsync(loginDto);

        // Assert login
        loginResponse.Should().NotBeNull();
        loginResponse.Token.Should().NotBeNullOrEmpty();
        loginResponse.Customer.Email.Should().Be("integration@example.com");

        // Step 3: Get profile
        var customerId = registerResponse.Customer.Id;
        var profile = await _service.GetProfileAsync(customerId);

        // Assert profile
        profile.Should().NotBeNull();
        profile!.Email.Should().Be("integration@example.com");
        profile.FirstName.Should().Be("Integration");

        // Step 4: Update profile
        var updateDto = new CustomerUpdateDto
        {
            FirstName = "Updated",
            LastName = "User",
            PhoneNumber = "9876543210",
            City = "Updated City",
            Country = "USA"
        };

        await _service.UpdateProfileAsync(customerId, updateDto);

        // Assert update
        var updatedProfile = await _service.GetProfileAsync(customerId);
        updatedProfile.Should().NotBeNull();
        updatedProfile!.FirstName.Should().Be("Updated");
        updatedProfile.LastName.Should().Be("User");
        updatedProfile.City.Should().Be("Updated City");
        updatedProfile.Country.Should().Be("USA");
        updatedProfile.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task RegisterAndLogin_WithInvalidPassword_Fails()
    {
        // Step 1: Register
        var registerDto = new CustomerRegisterDto
        {
            Email = "testfail@example.com",
            Password = "Correct@123",
            FirstName = "Test",
            LastName = "User"
        };

        await _service.RegisterAsync(registerDto);

        // Step 2: Attempt login with wrong password
        var loginDto = new CustomerLoginDto
        {
            Email = "testfail@example.com",
            Password = "Wrong@123"
        };

        // Assert login fails
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.LoginAsync(loginDto));
    }

    [Fact]
    public async Task AdminOperations_CreateUpdateDelete_Success()
    {
        // Step 1: Create customer as admin
        var adminDto = new CustomerAdminDto
        {
            Email = "admin-created@example.com",
            FirstName = "Admin",
            LastName = "Created",
            PhoneNumber = "1111111111",
            Role = UserRole.Customer,
            IsActive = true
        };

        var createdCustomer = await _service.CreateCustomerAsync(adminDto, "Admin@123");

        // Assert creation
        createdCustomer.Should().NotBeNull();
        createdCustomer.Email.Should().Be("admin-created@example.com");
        createdCustomer.Role.Should().Be(UserRole.Customer);

        // Step 2: Update customer
        var updateAdminDto = new CustomerAdminDto
        {
            Email = "admin-created@example.com",
            FirstName = "Updated Admin",
            LastName = "Created",
            PhoneNumber = "2222222222",
            Role = UserRole.Admin, // Promote to admin
            IsActive = true
        };

        await _service.UpdateCustomerAsync(createdCustomer.Id, updateAdminDto);

        // Assert update
        var updatedCustomer = await _service.GetCustomerByIdAsync(createdCustomer.Id);
        updatedCustomer.Should().NotBeNull();
        updatedCustomer!.FirstName.Should().Be("Updated Admin");
        updatedCustomer.Role.Should().Be(UserRole.Admin);

        // Step 3: Delete customer
        await _service.DeleteCustomerAsync(createdCustomer.Id);

        // Assert deletion
        var deletedCustomer = await _service.GetCustomerByIdAsync(createdCustomer.Id);
        deletedCustomer.Should().BeNull();
    }

    [Fact]
    public async Task GetCustomersByRole_FiltersCorrectly()
    {
        // Arrange: Create customers with different roles
        await _service.RegisterAsync(new CustomerRegisterDto
        {
            Email = "customer1@example.com",
            Password = "Test@123",
            FirstName = "Customer",
            LastName = "One"
        });

        await _service.RegisterAsync(new CustomerRegisterDto
        {
            Email = "customer2@example.com",
            Password = "Test@123",
            FirstName = "Customer",
            LastName = "Two"
        });

        await _service.CreateCustomerAsync(new CustomerAdminDto
        {
            Email = "admin1@example.com",
            FirstName = "Admin",
            LastName = "One",
            Role = UserRole.Admin,
            IsActive = true
        }, "Admin@123");

        // Act
        var customers = await _service.GetCustomersByRoleAsync(UserRole.Customer);
        var admins = await _service.GetCustomersByRoleAsync(UserRole.Admin);

        // Assert
        customers.Should().HaveCount(2);
        customers.Should().OnlyContain(c => c.Role == UserRole.Customer);
        
        admins.Should().HaveCount(1);
        admins.Should().OnlyContain(c => c.Role == UserRole.Admin);
    }

    [Fact]
    public async Task DuplicateEmailRegistration_ThrowsException()
    {
        // Step 1: Register first customer
        var registerDto1 = new CustomerRegisterDto
        {
            Email = "duplicate@example.com",
            Password = "Test@123",
            FirstName = "First",
            LastName = "User"
        };

        await _service.RegisterAsync(registerDto1);

        // Step 2: Attempt to register with same email
        var registerDto2 = new CustomerRegisterDto
        {
            Email = "duplicate@example.com",
            Password = "Different@123",
            FirstName = "Second",
            LastName = "User"
        };

        // Assert duplicate registration fails
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RegisterAsync(registerDto2));
    }

    [Fact]
    public async Task PasswordChange_UpdatesPasswordCorrectly()
    {
        // Step 1: Register customer
        var registerDto = new CustomerRegisterDto
        {
            Email = "pwchange@example.com",
            Password = "Original@123",
            FirstName = "Password",
            LastName = "Change"
        };

        var registerResponse = await _service.RegisterAsync(registerDto);
        var customerId = registerResponse.Customer.Id;

        // Step 2: Change password
        await _service.ChangePasswordAsync(customerId, "Original@123", "NewPassword@123");

        // Step 3: Try to login with old password (should fail)
        var loginWithOldPassword = new CustomerLoginDto
        {
            Email = "pwchange@example.com",
            Password = "Original@123"
        };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.LoginAsync(loginWithOldPassword));

        // Step 4: Login with new password (should succeed)
        var loginWithNewPassword = new CustomerLoginDto
        {
            Email = "pwchange@example.com",
            Password = "NewPassword@123"
        };

        var loginResponse = await _service.LoginAsync(loginWithNewPassword);
        loginResponse.Should().NotBeNull();
        loginResponse.Token.Should().NotBeNullOrEmpty();
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}
