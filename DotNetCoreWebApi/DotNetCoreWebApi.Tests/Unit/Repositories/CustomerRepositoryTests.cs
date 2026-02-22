using DotNetCoreWebApi.Application.Entities;
using DotNetCoreWebApi.Infrastructure.Repositories;
using DotNetCoreWebApi.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace DotNetCoreWebApi.Tests.Unit.Repositories;

public class CustomerRepositoryTests : IDisposable
{
    private readonly CustomerRepository _repository;
    private readonly Application.DBContext.ApplicationDBContext _context;

    public CustomerRepositoryTests()
    {
        _context = TestDbContextFactory.CreateInMemoryContext();
        _repository = new CustomerRepository(_context);
    }

    [Fact]
    public async Task GetByEmailAsync_WithExistingEmail_ReturnsCustomer()
    {
        // Arrange
        var customer = new Customer
        {
            Email = "test@example.com",
            PasswordHash = "hash",
            FirstName = "John",
            LastName = "Doe",
            Role = UserRole.Customer,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await _repository.AddAsync(customer);

        // Act
        var result = await _repository.GetByEmailAsync("test@example.com");

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task GetByEmailAsync_WithNonExistentEmail_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByEmailAsync("nonexistent@example.com");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByEmailAsync_IsCaseInsensitive()
    {
        // Arrange
        var customer = new Customer
        {
            Email = "test@example.com",
            PasswordHash = "hash",
            FirstName = "John",
            LastName = "Doe",
            Role = UserRole.Customer,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await _repository.AddAsync(customer);

        // Act
        var result = await _repository.GetByEmailAsync("TEST@EXAMPLE.COM");

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task EmailExistsAsync_WithExistingEmail_ReturnsTrue()
    {
        // Arrange
        var customer = new Customer
        {
            Email = "test@example.com",
            PasswordHash = "hash",
            FirstName = "John",
            LastName = "Doe",
            Role = UserRole.Customer,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await _repository.AddAsync(customer);

        // Act
        var result = await _repository.EmailExistsAsync("test@example.com");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task EmailExistsAsync_WithNonExistentEmail_ReturnsFalse()
    {
        // Act
        var result = await _repository.EmailExistsAsync("nonexistent@example.com");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetActiveCustomersAsync_ReturnsOnlyActiveCustomers()
    {
        // Arrange
        await _repository.AddAsync(new Customer { Email = "active1@example.com", PasswordHash = "hash", FirstName = "Active", LastName = "One", Role = UserRole.Customer, IsActive = true, CreatedAt = DateTime.UtcNow });
        await _repository.AddAsync(new Customer { Email = "active2@example.com", PasswordHash = "hash", FirstName = "Active", LastName = "Two", Role = UserRole.Customer, IsActive = true, CreatedAt = DateTime.UtcNow });
        await _repository.AddAsync(new Customer { Email = "inactive@example.com", PasswordHash = "hash", FirstName = "Inactive", LastName = "User", Role = UserRole.Customer, IsActive = false, CreatedAt = DateTime.UtcNow });

        // Act
        var result = await _repository.GetActiveCustomersAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(c => c.IsActive);
    }

    [Fact]
    public async Task GetCustomersByRoleAsync_ReturnsCustomersWithSpecifiedRole()
    {
        // Arrange
        await _repository.AddAsync(new Customer { Email = "customer1@example.com", PasswordHash = "hash", FirstName = "Customer", LastName = "One", Role = UserRole.Customer, IsActive = true, CreatedAt = DateTime.UtcNow });
        await _repository.AddAsync(new Customer { Email = "customer2@example.com", PasswordHash = "hash", FirstName = "Customer", LastName = "Two", Role = UserRole.Customer, IsActive = true, CreatedAt = DateTime.UtcNow });
        await _repository.AddAsync(new Customer { Email = "admin@example.com", PasswordHash = "hash", FirstName = "Admin", LastName = "User", Role = UserRole.Admin, IsActive = true, CreatedAt = DateTime.UtcNow });

        // Act
        var result = await _repository.GetCustomersByRoleAsync(UserRole.Customer);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(c => c.Role == UserRole.Customer);
    }

    [Fact]
    public async Task UpdateLastLoginAsync_UpdatesLastLoginTimestamp()
    {
        // Arrange
        var customer = new Customer
        {
            Email = "test@example.com",
            PasswordHash = "hash",
            FirstName = "John",
            LastName = "Doe",
            Role = UserRole.Customer,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        var addedCustomer = await _repository.AddAsync(customer);
        var originalLastLogin = addedCustomer.LastLoginAt;

        // Act
        await Task.Delay(100); // Small delay to ensure timestamp difference
        await _repository.UpdateLastLoginAsync(addedCustomer.Id);

        // Act - Retrieve updated customer
        var updatedCustomer = await _repository.GetByIdAsync(addedCustomer.Id);

        // Assert
        updatedCustomer.Should().NotBeNull();
        updatedCustomer!.LastLoginAt.Should().NotBeNull();
        updatedCustomer.LastLoginAt.Should().BeAfter(originalLastLogin ?? DateTime.MinValue);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}
