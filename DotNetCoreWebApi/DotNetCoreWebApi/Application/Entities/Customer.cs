using System.ComponentModel.DataAnnotations;

namespace DotNetCoreWebApi.Application.Entities;

/// <summary>
/// Customer entity representing both customers and admin users
/// Implements user authentication and authorization for the e-commerce platform
/// </summary>
public class Customer
{
    public int Id { get; set; }
    
    [Required]
    [EmailAddress]
    [StringLength(255)]
    public required string Email { get; set; }
    
    [Required]
    public required string PasswordHash { get; set; }
    
    [Required]
    [StringLength(100)]
    public required string FirstName { get; set; }
    
    [Required]
    [StringLength(100)]
    public required string LastName { get; set; }
    
    [Phone]
    [StringLength(20)]
    public string? PhoneNumber { get; set; }
    
    // Role-based authorization
    public UserRole Role { get; set; } = UserRole.Customer;
    
    // Address information for shipping
    [StringLength(255)]
    public string? StreetAddress { get; set; }
    
    [StringLength(100)]
    public string? City { get; set; }
    
    [StringLength(100)]
    public string? StateProvince { get; set; }
    
    [StringLength(20)]
    public string? PostalCode { get; set; }
    
    [StringLength(100)]
    public string? Country { get; set; }
    
    // Account status
    public bool IsActive { get; set; } = true;
    public bool EmailConfirmed { get; set; } = false;
    
    // Password reset
    [StringLength(500)]
    public string? ResetToken { get; set; }
    public DateTime? ResetTokenExpiry { get; set; }
    
    // Audit fields
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    
    // Navigation properties (for future order management)
    // public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}

/// <summary>
/// User role enumeration for authorization
/// Admin: Full CRUD access to all entities
/// Customer: Read-only access, can manage own orders and profile
/// </summary>
public enum UserRole
{
    Customer = 0,
    Admin = 1
}
