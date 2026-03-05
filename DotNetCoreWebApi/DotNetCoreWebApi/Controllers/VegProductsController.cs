using DotNetCoreWebApi.Application.Interfaces;
using DotNetCoreWebApi.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DotNetCoreWebApi.Controllers;

/// <summary>
/// Controller for VegProduct operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class VegProductsController : ControllerBase
{
    private readonly IVegProductService _productService;
    private readonly ILogger<VegProductsController> _logger;

    public VegProductsController(IVegProductService productService, ILogger<VegProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    /// <summary>
    /// Get all products with their categories
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VegProductDto>>> GetAllProducts()
    {
        try
        {
            var products = await _productService.GetAllProductsAsync();
            _logger.LogDebug("Retrieved {Count} products", products.Count());
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all products");
            return StatusCode(500, new { message = "An error occurred retrieving products" });
        }
    }

    /// <summary>
    /// Get a specific product by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<VegProductDto>> GetProductById(int id)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                _logger.LogWarning("Product not found: {ProductId}", id);
                return NotFound(new { message = $"Product with ID {id} not found" });
            }

            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product {ProductId}", id);
            return StatusCode(500, new { message = "An error occurred retrieving the product" });
        }
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<VegProductDto>> CreateProduct([FromBody] VegProductCreateUpdateDto productDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (string.IsNullOrWhiteSpace(productDto.Name))
            return BadRequest(new { message = "Product name is required" });

        try
        {
            var created = await _productService.CreateProductAsync(productDto);
            _logger.LogInformation("Product created: {ProductName} (ID: {ProductId})", created.Name, created.Id);
            return Ok(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product {ProductName}", productDto.Name);
            return StatusCode(500, new { message = "An error occurred creating the product" });
        }
    }

    /// <summary>
    /// Update an existing product
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] VegProductCreateUpdateDto productDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (string.IsNullOrWhiteSpace(productDto.Name))
            return BadRequest(new { message = "Product name is required" });

        try
        {
            await _productService.UpdateProductAsync(id, productDto);
            _logger.LogInformation("Product updated: {ProductId}", id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Update failed - product not found: {ProductId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product {ProductId}", id);
            return StatusCode(500, new { message = "An error occurred updating the product" });
        }
    }

    /// <summary>
    /// Delete a product
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        try
        {
            await _productService.DeleteProductAsync(id);
            _logger.LogInformation("Product deleted: {ProductId}", id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Delete failed - product not found: {ProductId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product {ProductId}", id);
            return StatusCode(500, new { message = "An error occurred deleting the product" });
        }
    }

    /// <summary>
    /// Get products by category ID
    /// </summary>
    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<VegProductDto>>> GetProductsByCategory(int categoryId)
    {
        try
        {
            var products = await _productService.GetProductsByCategoryAsync(categoryId);
            _logger.LogDebug("Retrieved {Count} products for category {CategoryId}", products.Count(), categoryId);
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products for category {CategoryId}", categoryId);
            return StatusCode(500, new { message = "An error occurred retrieving products" });
        }
    }
}
