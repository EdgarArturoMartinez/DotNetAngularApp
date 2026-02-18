using DotNetCoreWebApi.Application.Entities;

namespace DotNetCoreWebApi.Application.Interfaces;

/// <summary>
/// Repository interface for VegProduct entity
/// </summary>
public interface IVegProductRepository : IRepository<VegProducts>
{
    Task<IEnumerable<VegProducts>> GetProductsWithCategoryAsync();
    Task<VegProducts?> GetProductWithCategoryAsync(int id);
    Task<IEnumerable<VegProducts>> GetProductsByCategoryAsync(int categoryId);
}
