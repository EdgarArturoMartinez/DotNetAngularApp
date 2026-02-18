using DotNetCoreWebApi.Application.Interfaces;
using DotNetCoreWebApi.DTOs;
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

    public VegProductsController(IVegProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Get all products with their categories
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VegProductDto>>> GetAllProducts()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }

    /// <summary>
    /// Get a specific product by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<VegProductDto>> GetProductById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);

        if (product == null)
            return NotFound(new { message = $"Product with ID {id} not found" });

        return Ok(product);
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

        var created = await _productService.CreateProductAsync(productDto);
        return Ok(created);
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
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
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
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get products by category ID
    /// </summary>
    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<VegProductDto>>> GetProductsByCategory(int categoryId)
    {
        var products = await _productService.GetProductsByCategoryAsync(categoryId);
        return Ok(products);
    }
}
