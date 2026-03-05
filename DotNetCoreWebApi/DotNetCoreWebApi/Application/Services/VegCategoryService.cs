using DotNetCoreWebApi.Application.Entities;
using DotNetCoreWebApi.Application.Interfaces;
using DotNetCoreWebApi.Application.DTOs;

namespace DotNetCoreWebApi.Application.Services;

/// <summary>
/// Service implementation for VegCategory business logic
/// </summary>
public class VegCategoryService : IVegCategoryService
{
    private readonly IVegCategoryRepository _categoryRepository;

    public VegCategoryService(IVegCategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<VegCategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.GetCategoriesWithProductsAsync();
        
        return categories.Select(c => new VegCategoryDto
        {
            IdCategory = c.IdCategory,
            CategoryName = c.CategoryName,
            Description = c.Description,
            CreatedAt = c.CreatedAt,
            ProductCount = c.VegProducts?.Count ?? 0
        });
    }

    public async Task<VegCategoryDto?> GetCategoryByIdAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        
        if (category == null)
            return null;

        return new VegCategoryDto
        {
            IdCategory = category.IdCategory,
            CategoryName = category.CategoryName,
            Description = category.Description,
            CreatedAt = category.CreatedAt,
            ProductCount = 0 // Don't load products for single get
        };
    }

    public async Task<VegCategoryDto> CreateCategoryAsync(VegCategoryCreateUpdateDto dto)
    {
        var category = new VegCategory
        {
            CategoryName = dto.CategoryName,
            Description = dto.Description,
            CreatedAt = DateTime.Now
        };

        var created = await _categoryRepository.AddAsync(category);

        return new VegCategoryDto
        {
            IdCategory = created.IdCategory,
            CategoryName = created.CategoryName,
            Description = created.Description,
            CreatedAt = created.CreatedAt,
            ProductCount = 0
        };
    }

    public async Task UpdateCategoryAsync(int id, VegCategoryCreateUpdateDto dto)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        
        if (category == null)
            throw new KeyNotFoundException($"Category with ID {id} not found");

        category.CategoryName = dto.CategoryName;
        category.Description = dto.Description;

        await _categoryRepository.UpdateAsync(category);
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        
        if (category == null)
            throw new KeyNotFoundException($"Category with ID {id} not found");

        await _categoryRepository.DeleteAsync(category);
    }

    public async Task<bool> CategoryExistsAsync(int id)
    {
        return await _categoryRepository.ExistsAsync(id);
    }
}
