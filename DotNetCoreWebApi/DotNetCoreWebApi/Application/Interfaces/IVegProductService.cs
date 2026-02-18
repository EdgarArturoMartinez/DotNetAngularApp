using DotNetCoreWebApi.DTOs;

namespace DotNetCoreWebApi.Application.Interfaces;

/// <summary>
/// Service interface for VegProduct business logic
/// </summary>
public interface IVegProductService
{
    Task<IEnumerable<VegProductDto>> GetAllProductsAsync();
    Task<VegProductDto?> GetProductByIdAsync(int id);
    Task<VegProductDto> CreateProductAsync(VegProductCreateUpdateDto dto);
    Task UpdateProductAsync(int id, VegProductCreateUpdateDto dto);
    Task DeleteProductAsync(int id);
    Task<bool> ProductExistsAsync(int id);
    Task<IEnumerable<VegProductDto>> GetProductsByCategoryAsync(int categoryId);
}
