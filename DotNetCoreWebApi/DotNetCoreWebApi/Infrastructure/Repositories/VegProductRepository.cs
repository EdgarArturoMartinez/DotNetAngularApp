using DotNetCoreWebApi.Application.DBContext;
using DotNetCoreWebApi.Application.Entities;
using DotNetCoreWebApi.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreWebApi.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for VegProduct entity
/// </summary>
public class VegProductRepository : Repository<VegProducts>, IVegProductRepository
{
    public VegProductRepository(ApplicationDBContext context) : base(context)
    {
    }

    public async Task<IEnumerable<VegProducts>> GetProductsWithCategoryAsync()
    {
        return await _dbSet
            .Include(p => p.VegCategory)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<VegProducts?> GetProductWithCategoryAsync(int id)
    {
        return await _dbSet
            .Include(p => p.VegCategory)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<VegProducts>> GetProductsByCategoryAsync(int categoryId)
    {
        return await _dbSet
            .Include(p => p.VegCategory)
            .Where(p => p.IdCategory == categoryId)
            .AsNoTracking()
            .ToListAsync();
    }
}
