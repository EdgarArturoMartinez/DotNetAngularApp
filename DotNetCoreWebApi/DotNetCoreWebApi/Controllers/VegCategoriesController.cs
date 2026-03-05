using DotNetCoreWebApi.Application.Interfaces;
using DotNetCoreWebApi.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DotNetCoreWebApi.Controllers;

/// <summary>
/// Controller for VegCategory operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class VegCategoriesController : ControllerBase
{
    private readonly IVegCategoryService _categoryService;
    private readonly ILogger<VegCategoriesController> _logger;

    public VegCategoriesController(IVegCategoryService categoryService, ILogger<VegCategoriesController> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

    /// <summary>
    /// Get all categories with product count
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VegCategoryDto>>> GetCategories()
    {
        try
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            _logger.LogDebug("Retrieved {Count} categories", categories.Count());
            return Ok(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all categories");
            return StatusCode(500, new { message = "An error occurred retrieving categories" });
        }
    }

    /// <summary>
    /// Get a specific category by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<VegCategoryDto>> GetCategoryById(int id)
    {
        try
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);

            if (category == null)
            {
                _logger.LogWarning("Category not found: {CategoryId}", id);
                return NotFound(new { message = $"Category with ID {id} not found" });
            }

            return Ok(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving category {CategoryId}", id);
            return StatusCode(500, new { message = "An error occurred retrieving the category" });
        }
    }

    /// <summary>
    /// Create a new category
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<VegCategoryDto>> CreateCategory([FromBody] VegCategoryCreateUpdateDto categoryDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var created = await _categoryService.CreateCategoryAsync(categoryDto);
            _logger.LogInformation("Category created: {CategoryName} (ID: {CategoryId})", created.CategoryName, created.IdCategory);
            return Ok(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category {CategoryName}", categoryDto.CategoryName);
            return StatusCode(500, new { message = "An error occurred creating the category" });
        }
    }

    /// <summary>
    /// Update an existing category
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] VegCategoryCreateUpdateDto categoryDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _categoryService.UpdateCategoryAsync(id, categoryDto);
            _logger.LogInformation("Category updated: {CategoryId}", id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Update failed - category not found: {CategoryId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category {CategoryId}", id);
            return StatusCode(500, new { message = "An error occurred updating the category" });
        }
    }

    /// <summary>
    /// Delete a category
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        try
        {
            await _categoryService.DeleteCategoryAsync(id);
            _logger.LogInformation("Category deleted: {CategoryId}", id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Delete failed - category not found: {CategoryId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category {CategoryId}", id);
            return StatusCode(500, new { message = "An error occurred deleting the category" });
        }
    }
}
