using DotNetCoreWebApi.Application.DBContext;
using DotNetCoreWebApi.Application.Entities;
using DotNetCoreWebApi.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreWebApi.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Customer entity
/// </summary>
public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(ApplicationDBContext context) : base(context)
    {
    }

    /// <summary>
    /// Get customer by email address (case-insensitive)
    /// </summary>
    public async Task<Customer?> GetByEmailAsync(string email)
    {
        return await _context.Set<Customer>()
            .FirstOrDefaultAsync(c => c.Email.ToLower() == email.ToLower());
    }

    /// <summary>
    /// Check if email already exists (case-insensitive)
    /// </summary>
    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Set<Customer>()
            .AnyAsync(c => c.Email.ToLower() == email.ToLower());
    }

    /// <summary>
    /// Get all active customers
    /// </summary>
    public async Task<IEnumerable<Customer>> GetActiveCustomersAsync()
    {
        return await _context.Set<Customer>()
            .Where(c => c.IsActive)
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .ToListAsync();
    }

    /// <summary>
    /// Get customers by role
    /// </summary>
    public async Task<IEnumerable<Customer>> GetCustomersByRoleAsync(UserRole role)
    {
        return await _context.Set<Customer>()
            .Where(c => c.Role == role)
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .ToListAsync();
    }

    /// <summary>
    /// Update last login timestamp
    /// </summary>
    public async Task UpdateLastLoginAsync(int customerId)
    {
        var customer = await _context.Set<Customer>().FindAsync(customerId);
        if (customer != null)
        {
            customer.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Save password reset token for a customer
    /// </summary>
    public async Task SaveResetTokenAsync(int customerId, string token, DateTime expiry)
    {
        var customer = await _context.Set<Customer>().FindAsync(customerId);
        if (customer != null)
        {
            customer.ResetToken = token;
            customer.ResetTokenExpiry = expiry;
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Get customer by reset token (if valid and not expired)
    /// </summary>
    public async Task<Customer?> GetByResetTokenAsync(string token)
    {
        return await _context.Set<Customer>()
            .FirstOrDefaultAsync(c => 
                c.ResetToken == token && 
                c.ResetTokenExpiry != null && 
                c.ResetTokenExpiry > DateTime.UtcNow);
    }

    /// <summary>
    /// Clear reset token after successful password reset
    /// </summary>
    public async Task ClearResetTokenAsync(int customerId)
    {
        var customer = await _context.Set<Customer>().FindAsync(customerId);
        if (customer != null)
        {
            customer.ResetToken = null;
            customer.ResetTokenExpiry = null;
            await _context.SaveChangesAsync();
        }
    }
}
