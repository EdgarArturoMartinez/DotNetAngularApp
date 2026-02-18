using DotNetCoreWebApi.Application.DBContext;
using DotNetCoreWebApi.Application.Entities;
using DotNetCoreWebApi.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreWebApi.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for VegCategory entity
/// </summary>
public class VegCategoryRepository : Repository<VegCategory>, IVegCategoryRepository
{
    public VegCategoryRepository(ApplicationDBContext context) : base(context)
    {
    }

    public async Task<IEnumerable<VegCategory>> GetCategoriesWithProductsAsync()
    {
        return await _dbSet
            .Include(c => c.VegProducts)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<VegCategory?> GetCategoryWithProductsAsync(int id)
    {
        return await _dbSet
            .Include(c => c.VegProducts)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.IdCategory == id);
    }
}
