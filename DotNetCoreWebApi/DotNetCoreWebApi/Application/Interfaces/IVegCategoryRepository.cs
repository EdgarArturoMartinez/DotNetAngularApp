using DotNetCoreWebApi.Application.Entities;

namespace DotNetCoreWebApi.Application.Interfaces;

/// <summary>
/// Repository interface for VegCategory entity
/// </summary>
public interface IVegCategoryRepository : IRepository<VegCategory>
{
    Task<IEnumerable<VegCategory>> GetCategoriesWithProductsAsync();
    Task<VegCategory?> GetCategoryWithProductsAsync(int id);
}
