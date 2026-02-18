using DotNetCoreWebApi.DTOs;

namespace DotNetCoreWebApi.Application.Interfaces;

/// <summary>
/// Service interface for ProductImage business logic and operations
/// </summary>
public interface IProductImageService
{
    /// <summary>
    /// Get all images for a specific product
    /// </summary>
    Task<IEnumerable<ProductImageDto>> GetImagesByProductIdAsync(int productId);

    /// <summary>
    /// Get a specific image by ID
    /// </summary>
    Task<ProductImageDto?> GetImageByIdAsync(int id);

    /// <summary>
    /// Get the main/hero image for a product
    /// </summary>
    Task<ProductImageDto?> GetMainImageAsync(int productId);

    /// <summary>
    /// Get the mobile optimized image for a product
    /// </summary>
    Task<ProductImageDto?> GetMobileImageAsync(int productId);

    /// <summary>
    /// Get gallery images for a product
    /// </summary>
    Task<IEnumerable<ProductImageDto>> GetGalleryImagesAsync(int productId);

    /// <summary>
    /// Create a new product image
    /// </summary>
    Task<ProductImageDto> CreateImageAsync(int productId, ProductImageCreateUpdateDto createDto);

    /// <summary>
    /// Update an existing product image
    /// </summary>
    Task<ProductImageDto> UpdateImageAsync(int id, ProductImageCreateUpdateDto updateDto);

    /// <summary>
    /// Delete a product image
    /// </summary>
    Task DeleteImageAsync(int id);

    /// <summary>
    /// Delete all images for a product
    /// </summary>
    Task DeleteProductImagesAsync(int productId);

    /// <summary>
    /// Get count of images for a product
    /// </summary>
    Task<int> GetImageCountAsync(int productId);

    /// <summary>
    /// Validate image dimensions and type
    /// </summary>
    Task<ValidationResult> ValidateImageAsync(int productId, ProductImageCreateUpdateDto imageDto);
}

/// <summary>
/// Result of image validation
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; }
    public string? Message { get; set; }

    public ValidationResult(bool isValid, string? message = null)
    {
        IsValid = isValid;
        Message = message;
    }
}
