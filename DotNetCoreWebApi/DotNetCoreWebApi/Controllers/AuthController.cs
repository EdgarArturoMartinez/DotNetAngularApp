using DotNetCoreWebApi.Application.Interfaces;
using DotNetCoreWebApi.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNetCoreWebApi.Controllers;

/// <summary>
/// Authentication controller for customer registration and login
/// Public endpoints - no authentication required
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly IEmailService _emailService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        ICustomerService customerService, 
        IEmailService emailService,
        ILogger<AuthController> logger)
    {
        _customerService = customerService;
        _emailService = emailService;
        _logger = logger;
    }

    /// <summary>
    /// Register a new customer account
    /// </summary>
    /// <param name="registerDto">Customer registration details</param>
    /// <returns>Authentication response with JWT token</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] CustomerRegisterDto registerDto)
    {
        try
        {
            var response = await _customerService.RegisterAsync(registerDto);
            _logger.LogInformation("New customer registered: {Email}", registerDto.Email);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Registration failed: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during customer registration");
            return StatusCode(500, new { message = "An error occurred during registration" });
        }
    }

    /// <summary>
    /// Authenticate customer and receive JWT token
    /// </summary>
    /// <param name="loginDto">Customer login credentials</param>
    /// <returns>Authentication response with JWT token</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] CustomerLoginDto loginDto)
    {
        try
        {
            var response = await _customerService.LoginAsync(loginDto);
            _logger.LogInformation("Customer logged in: {Email}", loginDto.Email);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Login failed for {Email}: {Message}", loginDto.Email, ex.Message);
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during customer login");
            return StatusCode(500, new { message = "An error occurred during login" });
        }
    }

    /// <summary>
    /// Get current authenticated user's profile
    /// </summary>
    /// <returns>Customer profile</returns>
    [HttpGet("profile")]
    [Authorize]
    public async Task<ActionResult<CustomerDto>> GetProfile()
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var customer = await _customerService.GetProfileAsync(userId);
            if (customer == null)
            {
                return NotFound(new { message = "Customer not found" });
            }

            return Ok(customer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer profile");
            return StatusCode(500, new { message = "An error occurred retrieving profile" });
        }
    }

    /// <summary>
    /// Update current authenticated user's profile
    /// </summary>
    /// <param name="updateDto">Profile update details</param>
    /// <returns>Success message</returns>
    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] CustomerUpdateDto updateDto)
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            await _customerService.UpdateProfileAsync(userId, updateDto);
            _logger.LogInformation("Customer profile updated: {UserId}", userId);
            return Ok(new { message = "Profile updated successfully" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating customer profile");
            return StatusCode(500, new { message = "An error occurred updating profile" });
        }
    }

    /// <summary>
    /// Change password for authenticated user
    /// </summary>
    /// <param name="changePasswordDto">Current and new password</param>
    /// <returns>Success message</returns>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            await _customerService.ChangePasswordAsync(
                userId, 
                changePasswordDto.CurrentPassword, 
                changePasswordDto.NewPassword
            );
            
            _logger.LogInformation("Password changed for customer: {UserId}", userId);
            return Ok(new { message = "Password changed successfully" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password");
            return StatusCode(500, new { message = "An error occurred changing password" });
        }
    }

    /// <summary>
    /// Check if email is available for registration
    /// </summary>
    /// <param name="email">Email to check</param>
    /// <returns>Availability status</returns>
    [HttpGet("check-email")]
    [AllowAnonymous]
    public async Task<ActionResult<object>> CheckEmail([FromQuery] string email)
    {
        try
        {
            var exists = await _customerService.EmailExistsAsync(email);
            return Ok(new { available = !exists });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking email availability");
            return StatusCode(500, new { message = "An error occurred checking email" });
        }
    }

    /// <summary>
    /// Request a password reset token
    /// Sends a reset token via email if the email exists
    /// </summary>
    /// <param name="forgotPasswordDto">Email address</param>
    /// <returns>Success message (always returns success for security)</returns>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
    {
        try
        {
            var result = await _customerService.GeneratePasswordResetTokenAsync(forgotPasswordDto.Email);
            
            if (result.HasValue)
            {
                // Send password reset email
                await _emailService.SendPasswordResetEmailAsync(
                    result.Value.email, 
                    result.Value.token, 
                    result.Value.customerName);
                
                _logger.LogInformation("Password reset email sent to: {Email}", forgotPasswordDto.Email);
            }
            else
            {
                _logger.LogInformation("Password reset requested for non-existent email: {Email}", forgotPasswordDto.Email);
            }
            
            // Always return success message (security: don't reveal if email exists)
            return Ok(new 
            { 
                message = "If your email is registered, you will receive a password reset link shortly. Please check your inbox."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing password reset request");
            // Don't expose internal errors to user
            return Ok(new 
            { 
                message = "If your email is registered, you will receive a password reset link shortly. Please check your inbox."
            });
        }
    }

    /// <summary>
    /// Reset password using the reset token
    /// </summary>
    /// <param name="resetPasswordDto">Reset token and new password</param>
    /// <returns>Success message</returns>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        try
        {
            await _customerService.ResetPasswordAsync(resetPasswordDto);
            _logger.LogInformation("Password reset successful for: {Email}", resetPasswordDto.Email);
            return Ok(new { message = "Password has been reset successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Password reset failed: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password");
            return StatusCode(500, new { message = "An error occurred resetting password" });
        }
    }
}

/// <summary>
/// DTO for password change request
/// </summary>
public class ChangePasswordDto
{
    public string CurrentPassword { get; set; } = null!;
    
    [System.ComponentModel.DataAnnotations.StringLength(100, MinimumLength = 8)]
    public string NewPassword { get; set; } = null!;
}
