using DotNetCoreWebApi.Application.Entities;

namespace DotNetCoreWebApi.Application.Interfaces;

/// <summary>
/// Repository interface for ProductImage entity operations
/// </summary>
public interface IProductImageRepository : IRepository<ProductImage>
{
    /// <summary>
    /// Get all images for a specific product with product details
    /// </summary>
    Task<IEnumerable<ProductImage>> GetImagesByProductIdAsync(int productId);

    /// <summary>
    /// Get main image for a product
    /// </summary>
    Task<ProductImage?> GetMainImageByProductIdAsync(int productId);

    /// <summary>
    /// Get mobile optimized image for a product
    /// </summary>
    Task<ProductImage?> GetMobileImageByProductIdAsync(int productId);

    /// <summary>
    /// Get gallery images for a product (excluding main and mobile)
    /// </summary>
    Task<IEnumerable<ProductImage>> GetGalleryImagesByProductIdAsync(int productId);

    /// <summary>
    /// Delete all images for a product
    /// </summary>
    Task DeleteProductImagesAsync(int productId);

    /// <summary>
    /// Get image count for a product
    /// </summary>
    Task<int> GetImageCountByProductIdAsync(int productId);
}
