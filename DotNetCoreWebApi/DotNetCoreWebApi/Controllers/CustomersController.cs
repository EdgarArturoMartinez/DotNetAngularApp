using DotNetCoreWebApi.Application.Entities;
using DotNetCoreWebApi.Application.Interfaces;
using DotNetCoreWebApi.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNetCoreWebApi.Controllers;

/// <summary>
/// Customers controller for admin operations
/// All endpoints require Admin role authentication
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger)
    {
        _customerService = customerService;
        _logger = logger;
    }

    /// <summary>
    /// Get all customers (Admin only)
    /// </summary>
    /// <returns>List of all customers</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAllCustomers()
    {
        try
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all customers");
            return StatusCode(500, new { message = "An error occurred retrieving customers" });
        }
    }

    /// <summary>
    /// Get customer by ID (Admin only)
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <returns>Customer details</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDto>> GetCustomerById(int id)
    {
        try
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound(new { message = $"Customer with ID {id} not found" });
            }
            return Ok(customer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer {Id}", id);
            return StatusCode(500, new { message = "An error occurred retrieving customer" });
        }
    }

    /// <summary>
    /// Get customers by role (Admin only)
    /// </summary>
    /// <param name="role">User role (Customer or Admin)</param>
    /// <returns>List of customers with specified role</returns>
    [HttpGet("role/{role}")]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomersByRole(UserRole role)
    {
        try
        {
            var customers = await _customerService.GetCustomersByRoleAsync(role);
            return Ok(customers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customers by role {Role}", role);
            return StatusCode(500, new { message = "An error occurred retrieving customers" });
        }
    }

    /// <summary>
    /// Create new customer (Admin only)
    /// </summary>
    /// <param name="request">Customer creation details with password</param>
    /// <returns>Created customer details</returns>
    [HttpPost]
    public async Task<ActionResult<CustomerDto>> CreateCustomer([FromBody] CreateCustomerRequest request)
    {
        try
        {
            var customer = await _customerService.CreateCustomerAsync(request.Customer, request.Password);
            _logger.LogInformation("New customer created by admin: {Email}", request.Customer.Email);
            return CreatedAtAction(nameof(GetCustomerById), new { id = customer.Id }, customer);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Customer creation failed: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer");
            return StatusCode(500, new { message = "An error occurred creating customer" });
        }
    }

    /// <summary>
    /// Update customer (Admin only)
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <param name="adminDto">Updated customer details</param>
    /// <returns>Success message</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CustomerAdminDto adminDto)
    {
        try
        {
            await _customerService.UpdateCustomerAsync(id, adminDto);
            _logger.LogInformation("Customer updated by admin: {Id}", id);
            return Ok(new { message = "Customer updated successfully" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating customer {Id}", id);
            return StatusCode(500, new { message = "An error occurred updating customer" });
        }
    }

    /// <summary>
    /// Delete customer (Admin only)
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        try
        {
            await _customerService.DeleteCustomerAsync(id);
            _logger.LogInformation("Customer deleted by admin: {Id}", id);
            return Ok(new { message = "Customer deleted successfully" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting customer {Id}", id);
            return StatusCode(500, new { message = "An error occurred deleting customer" });
        }
    }
}

/// <summary>
/// Request model for creating customer (includes password)
/// </summary>
public class CreateCustomerRequest
{
    public CustomerAdminDto Customer { get; set; } = null!;
    
    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.StringLength(100, MinimumLength = 8)]
    public string Password { get; set; } = null!;
}
