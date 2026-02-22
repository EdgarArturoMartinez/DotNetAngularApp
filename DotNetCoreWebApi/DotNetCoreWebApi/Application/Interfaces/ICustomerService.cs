using DotNetCoreWebApi.Application.Entities;
using DotNetCoreWebApi.DTOs;

namespace DotNetCoreWebApi.Application.Interfaces;

/// <summary>
/// Service interface for Customer/Authentication operations
/// Handles user registration, authentication, and profile management
/// </summary>
public interface ICustomerService
{
    // Authentication methods
    Task<AuthResponseDto> RegisterAsync(CustomerRegisterDto registerDto);
    Task<AuthResponseDto> LoginAsync(CustomerLoginDto loginDto);
    Task<CustomerDto?> GetProfileAsync(int customerId);
    Task UpdateProfileAsync(int customerId, CustomerUpdateDto updateDto);
    Task ChangePasswordAsync(int customerId, string currentPassword, string newPassword);

    // Admin methods - Full CRUD
    Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
    Task<CustomerDto?> GetCustomerByIdAsync(int id);
    Task<CustomerDto> CreateCustomerAsync(CustomerAdminDto adminDto, string password);
    Task UpdateCustomerAsync(int id, CustomerAdminDto adminDto);
    Task DeleteCustomerAsync(int id);
    Task<IEnumerable<CustomerDto>> GetCustomersByRoleAsync(UserRole role);

    // Utility methods
    Task<bool> EmailExistsAsync(string email);

    // Password reset methods
    Task<(string token, string customerName, string email)?> GeneratePasswordResetTokenAsync(string email);
    Task ResetPasswordAsync(ResetPasswordDto resetDto);
}
