using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DotNetCoreWebApi.Application.Entities;
using DotNetCoreWebApi.Application.Interfaces;
using DotNetCoreWebApi.Application.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DotNetCoreWebApi.Application.Services;

/// <summary>
/// Service implementation for Customer/Authentication operations
/// Handles password hashing, JWT token generation, and user management
/// </summary>
public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IConfiguration _configuration;

    public CustomerService(ICustomerRepository customerRepository, IConfiguration configuration)
    {
        _customerRepository = customerRepository;
        _configuration = configuration;
    }

    #region Authentication Methods

    /// <summary>
    /// Register a new customer
    /// </summary>
    public async Task<AuthResponseDto> RegisterAsync(CustomerRegisterDto registerDto)
    {
        // Check if email already exists
        if (await _customerRepository.EmailExistsAsync(registerDto.Email))
        {
            throw new InvalidOperationException("Email already registered");
        }

        // Create customer entity
        var customer = new Customer
        {
            Email = registerDto.Email.ToLower().Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
            FirstName = registerDto.FirstName.Trim(),
            LastName = registerDto.LastName.Trim(),
            PhoneNumber = registerDto.PhoneNumber?.Trim(),
            StreetAddress = registerDto.StreetAddress?.Trim(),
            City = registerDto.City?.Trim(),
            StateProvince = registerDto.StateProvince?.Trim(),
            PostalCode = registerDto.PostalCode?.Trim(),
            Country = registerDto.Country?.Trim(),
            Role = UserRole.Customer, // Default role
            IsActive = true,
            EmailConfirmed = false,
            CreatedAt = DateTime.UtcNow
        };

        var createdCustomer = await _customerRepository.AddAsync(customer);

        // Generate JWT token
        var token = GenerateJwtToken(createdCustomer);

        return new AuthResponseDto
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddHours(24),
            Customer = MapToDto(createdCustomer)
        };
    }

    /// <summary>
    /// Authenticate customer and return JWT token
    /// </summary>
    public async Task<AuthResponseDto> LoginAsync(CustomerLoginDto loginDto)
    {
        var customer = await _customerRepository.GetByEmailAsync(loginDto.Email);

        if (customer == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        if (!customer.IsActive)
        {
            throw new UnauthorizedAccessException("Account is deactivated");
        }

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, customer.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Update last login timestamp
        await _customerRepository.UpdateLastLoginAsync(customer.Id);

        // Generate JWT token
        var token = GenerateJwtToken(customer);

        return new AuthResponseDto
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddHours(24),
            Customer = MapToDto(customer)
        };
    }

    /// <summary>
    /// Get customer profile
    /// </summary>
    public async Task<CustomerDto?> GetProfileAsync(int customerId)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);
        return customer == null ? null : MapToDto(customer);
    }

    /// <summary>
    /// Update customer profile
    /// </summary>
    public async Task UpdateProfileAsync(int customerId, CustomerUpdateDto updateDto)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);

        if (customer == null)
        {
            throw new KeyNotFoundException($"Customer with ID {customerId} not found");
        }

        customer.FirstName = updateDto.FirstName.Trim();
        customer.LastName = updateDto.LastName.Trim();
        customer.PhoneNumber = updateDto.PhoneNumber?.Trim();
        customer.StreetAddress = updateDto.StreetAddress?.Trim();
        customer.City = updateDto.City?.Trim();
        customer.StateProvince = updateDto.StateProvince?.Trim();
        customer.PostalCode = updateDto.PostalCode?.Trim();
        customer.Country = updateDto.Country?.Trim();
        customer.UpdatedAt = DateTime.UtcNow;

        await _customerRepository.UpdateAsync(customer);
    }

    /// <summary>
    /// Change customer password
    /// </summary>
    public async Task ChangePasswordAsync(int customerId, string currentPassword, string newPassword)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);

        if (customer == null)
        {
            throw new KeyNotFoundException($"Customer with ID {customerId} not found");
        }

        // Verify current password
        if (!BCrypt.Net.BCrypt.Verify(currentPassword, customer.PasswordHash))
        {
            throw new UnauthorizedAccessException("Current password is incorrect");
        }

        // Hash and update new password
        customer.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        customer.UpdatedAt = DateTime.UtcNow;

        await _customerRepository.UpdateAsync(customer);
    }

    #endregion

    #region Admin Methods

    /// <summary>
    /// Get all customers (Admin only)
    /// </summary>
    public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
    {
        var customers = await _customerRepository.GetAllAsync();
        return customers.Select(MapToDto);
    }

    /// <summary>
    /// Get customer by ID (Admin only)
    /// </summary>
    public async Task<CustomerDto?> GetCustomerByIdAsync(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        return customer == null ? null : MapToDto(customer);
    }

    /// <summary>
    /// Create customer (Admin only)
    /// </summary>
    public async Task<CustomerDto> CreateCustomerAsync(CustomerAdminDto adminDto, string password)
    {
        // Check if email already exists
        if (await _customerRepository.EmailExistsAsync(adminDto.Email))
        {
            throw new InvalidOperationException("Email already registered");
        }

        var customer = new Customer
        {
            Email = adminDto.Email.ToLower().Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            FirstName = adminDto.FirstName.Trim(),
            LastName = adminDto.LastName.Trim(),
            PhoneNumber = adminDto.PhoneNumber?.Trim(),
            Role = adminDto.Role,
            IsActive = adminDto.IsActive,
            StreetAddress = adminDto.StreetAddress?.Trim(),
            City = adminDto.City?.Trim(),
            StateProvince = adminDto.StateProvince?.Trim(),
            PostalCode = adminDto.PostalCode?.Trim(),
            Country = adminDto.Country?.Trim(),
            EmailConfirmed = false,
            CreatedAt = DateTime.UtcNow
        };

        var createdCustomer = await _customerRepository.AddAsync(customer);
        return MapToDto(createdCustomer);
    }

    /// <summary>
    /// Update customer (Admin only)
    /// </summary>
    public async Task UpdateCustomerAsync(int id, CustomerAdminDto adminDto)
    {
        var customer = await _customerRepository.GetByIdAsync(id);

        if (customer == null)
        {
            throw new KeyNotFoundException($"Customer with ID {id} not found");
        }

        // Check if email is being changed and if it already exists
        if (customer.Email.ToLower() != adminDto.Email.ToLower())
        {
            if (await _customerRepository.EmailExistsAsync(adminDto.Email))
            {
                throw new InvalidOperationException("Email already registered");
            }
            customer.Email = adminDto.Email.ToLower().Trim();
        }

        customer.FirstName = adminDto.FirstName.Trim();
        customer.LastName = adminDto.LastName.Trim();
        customer.PhoneNumber = adminDto.PhoneNumber?.Trim();
        customer.Role = adminDto.Role;
        customer.IsActive = adminDto.IsActive;
        customer.StreetAddress = adminDto.StreetAddress?.Trim();
        customer.City = adminDto.City?.Trim();
        customer.StateProvince = adminDto.StateProvince?.Trim();
        customer.PostalCode = adminDto.PostalCode?.Trim();
        customer.Country = adminDto.Country?.Trim();
        customer.UpdatedAt = DateTime.UtcNow;

        await _customerRepository.UpdateAsync(customer);
    }

    /// <summary>
    /// Delete customer (Admin only)
    /// </summary>
    public async Task DeleteCustomerAsync(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);

        if (customer == null)
        {
            throw new KeyNotFoundException($"Customer with ID {id} not found");
        }

        await _customerRepository.DeleteAsync(customer);
    }

    /// <summary>
    /// Get customers by role (Admin only)
    /// </summary>
    public async Task<IEnumerable<CustomerDto>> GetCustomersByRoleAsync(UserRole role)
    {
        var customers = await _customerRepository.GetCustomersByRoleAsync(role);
        return customers.Select(MapToDto);
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Check if email exists
    /// </summary>
    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _customerRepository.EmailExistsAsync(email);
    }

    #endregion

    #region Password Reset Methods

    /// <summary>
    /// Generate a password reset token for a customer
    /// Token is valid for 1 hour
    /// </summary>
    public async Task<(string token, string customerName, string email)?> GeneratePasswordResetTokenAsync(string email)
    {
        var customer = await _customerRepository.GetByEmailAsync(email);
        if (customer == null)
        {
            // Return null even if email doesn't exist (security best practice)
            // Don't reveal if email exists in system
            return null;
        }

        // Generate secure random token
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var expiry = DateTime.UtcNow.AddHours(1);

        await _customerRepository.SaveResetTokenAsync(customer.Id, token, expiry);

        var customerName = $"{customer.FirstName} {customer.LastName}";
        return (token, customerName, customer.Email);
    }

    /// <summary>
    /// Reset customer password using reset token
    /// </summary>
    public async Task ResetPasswordAsync(ResetPasswordDto resetDto)
    {
        var customer = await _customerRepository.GetByResetTokenAsync(resetDto.Token);
        
        if (customer == null)
        {
            throw new InvalidOperationException("Invalid or expired reset token");
        }

        // Verify the email matches
        if (!customer.Email.Equals(resetDto.Email, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Invalid reset request");
        }

        // Hash the new password
        customer.PasswordHash = BCrypt.Net.BCrypt.HashPassword(resetDto.NewPassword);
        customer.UpdatedAt = DateTime.UtcNow;

        // Clear the reset token
        await _customerRepository.ClearResetTokenAsync(customer.Id);

        // Update the customer
        await _customerRepository.UpdateAsync(customer);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Generate JWT token for authenticated customer
    /// </summary>
    private string GenerateJwtToken(Customer customer)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
        var issuer = jwtSettings["Issuer"] ?? "VeggieWorldAPI";
        var audience = jwtSettings["Audience"] ?? "VeggieWorldClient";

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
            new Claim(ClaimTypes.Email, customer.Email),
            new Claim(ClaimTypes.Name, $"{customer.FirstName} {customer.LastName}"),
            new Claim(ClaimTypes.Role, customer.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Map Customer entity to CustomerDto
    /// </summary>
    private CustomerDto MapToDto(Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            Email = customer.Email,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            PhoneNumber = customer.PhoneNumber,
            Role = customer.Role,
            StreetAddress = customer.StreetAddress,
            City = customer.City,
            StateProvince = customer.StateProvince,
            PostalCode = customer.PostalCode,
            Country = customer.Country,
            IsActive = customer.IsActive,
            EmailConfirmed = customer.EmailConfirmed,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt,
            LastLoginAt = customer.LastLoginAt
        };
    }

    #endregion
}
