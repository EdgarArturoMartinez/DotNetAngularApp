using DotNetCoreWebApi.DTOs;

namespace DotNetCoreWebApi.Application.Interfaces;

/// <summary>
/// Service interface for VegCategory business logic
/// </summary>
public interface IVegCategoryService
{
    Task<IEnumerable<VegCategoryDto>> GetAllCategoriesAsync();
    Task<VegCategoryDto?> GetCategoryByIdAsync(int id);
    Task<VegCategoryDto> CreateCategoryAsync(VegCategoryCreateUpdateDto dto);
    Task UpdateCategoryAsync(int id, VegCategoryCreateUpdateDto dto);
    Task DeleteCategoryAsync(int id);
    Task<bool> CategoryExistsAsync(int id);
}
