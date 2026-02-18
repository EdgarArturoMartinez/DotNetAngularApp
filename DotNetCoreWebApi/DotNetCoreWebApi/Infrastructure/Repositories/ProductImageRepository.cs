using DotNetCoreWebApi.Application.DBContext;
using DotNetCoreWebApi.Application.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreWebApi.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for ProductImage entity operations
/// </summary>
public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
{
    public ProductImageRepository(ApplicationDBContext context) : base(context)
    {
    }

    /// <summary>
    /// Get all images for a specific product with product details
    /// </summary>
    public async Task<IEnumerable<ProductImage>> GetImagesByProductIdAsync(int productId)
    {
        return await _dbSet
            .Where(pi => pi.IdProduct == productId && pi.IsActive)
            .OrderBy(pi => pi.DisplayOrder)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Get main image for a product
    /// </summary>
    public async Task<ProductImage?> GetMainImageByProductIdAsync(int productId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(pi => pi.IdProduct == productId 
                && pi.ImageType == ProductImageType.Main 
                && pi.IsActive);
    }

    /// <summary>
    /// Get mobile optimized image for a product
    /// </summary>
    public async Task<ProductImage?> GetMobileImageByProductIdAsync(int productId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(pi => pi.IdProduct == productId 
                && pi.ImageType == ProductImageType.Mobile 
                && pi.IsActive);
    }

    /// <summary>
    /// Get gallery images for a product (excluding main and mobile)
    /// </summary>
    public async Task<IEnumerable<ProductImage>> GetGalleryImagesByProductIdAsync(int productId)
    {
        return await _dbSet
            .Where(pi => pi.IdProduct == productId 
                && pi.ImageType == ProductImageType.Gallery 
                && pi.IsActive)
            .OrderBy(pi => pi.DisplayOrder)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Delete all images for a product
    /// </summary>
    public async Task DeleteProductImagesAsync(int productId)
    {
        var images = await _dbSet
            .Where(pi => pi.IdProduct == productId)
            .ToListAsync();

        if (images.Any())
        {
            _dbSet.RemoveRange(images);
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Get image count for a product
    /// </summary>
    public async Task<int> GetImageCountByProductIdAsync(int productId)
    {
        return await _dbSet
            .CountAsync(pi => pi.IdProduct == productId && pi.IsActive);
    }
}
