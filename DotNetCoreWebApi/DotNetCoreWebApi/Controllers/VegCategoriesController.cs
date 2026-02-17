using DotNetCoreWebApi.Application.DBContext;
using DotNetCoreWebApi.Application.Entities;
using DotNetCoreWebApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VegCategoriesController : ControllerBase
{
    private readonly ApplicationDBContext context;

    public VegCategoriesController(ApplicationDBContext context)
    {
        this.context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VegCategoryDto>>> GetCategories()
    {
        var categories = await context.VegCategories
            .Include(c => c.VegProducts)
            .AsNoTracking()
            .ToListAsync();

        var categoryDtos = categories.Select(c => new VegCategoryDto
        {
            IdCategory = c.IdCategory,
            CategoryName = c.CategoryName,
            Description = c.Description,
            CreatedAt = c.CreatedAt,
            ProductCount = c.VegProducts?.Count ?? 0
        }).ToList();

        return Ok(categoryDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VegCategory>> GetCategoryById(int id)
    {
        var category = await context.VegCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.IdCategory == id);

        if (category == null)
            return NotFound();

        // Return the entity directly for edit operations (without navigation properties)
        return Ok(new 
        {
            idCategory = category.IdCategory,
            categoryName = category.CategoryName,
            description = category.Description,
            createdAt = category.CreatedAt
        });
    }

    [HttpPost]
    public async Task<ActionResult<VegCategory>> CreateCategory(VegCategory category)
    {
        context.VegCategories.Add(category);
        await context.SaveChangesAsync();
        return Ok(category);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, VegCategory category)
    {
        if (id != category.IdCategory)
            return BadRequest();

        context.Entry(category).State = EntityState.Modified;
        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await context.VegCategories.FindAsync(id);
        if (category == null)
            return NotFound();

        context.VegCategories.Remove(category);
        await context.SaveChangesAsync();
        return NoContent();
    }
}
