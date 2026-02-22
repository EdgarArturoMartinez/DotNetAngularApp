using DotNetCoreWebApi.Application.Entities;

namespace DotNetCoreWebApi.Application.Interfaces;

/// <summary>
/// Repository interface for Customer entity
/// Extends the generic repository with customer-specific operations
/// </summary>
public interface ICustomerRepository : IRepository<Customer>
{
    /// <summary>
    /// Get customer by email address
    /// </summary>
    Task<Customer?> GetByEmailAsync(string email);

    /// <summary>
    /// Check if email already exists
    /// </summary>
    Task<bool> EmailExistsAsync(string email);

    /// <summary>
    /// Get all active customers
    /// </summary>
    Task<IEnumerable<Customer>> GetActiveCustomersAsync();

    /// <summary>
    /// Get customers by role
    /// </summary>
    Task<IEnumerable<Customer>> GetCustomersByRoleAsync(UserRole role);

    /// <summary>
    /// Update last login timestamp
    /// </summary>
    Task UpdateLastLoginAsync(int customerId);

    /// <summary>
    /// Save password reset token for a customer
    /// </summary>
    Task SaveResetTokenAsync(int customerId, string token, DateTime expiry);

    /// <summary>
    /// Get customer by reset token (if valid and not expired)
    /// </summary>
    Task<Customer?> GetByResetTokenAsync(string token);

    /// <summary>
    /// Clear reset token after successful password reset
    /// </summary>
    Task ClearResetTokenAsync(int customerId);
}
