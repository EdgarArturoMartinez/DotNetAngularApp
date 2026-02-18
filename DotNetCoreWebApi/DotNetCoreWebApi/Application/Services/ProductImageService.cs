using DotNetCoreWebApi.Application.Entities;
using DotNetCoreWebApi.Application.Interfaces;
using DotNetCoreWebApi.DTOs;
using DotNetCoreWebApi.Infrastructure.Repositories;

namespace DotNetCoreWebApi.Application.Services;

/// <summary>
/// Service implementation for ProductImage business logic
/// </summary>
public class ProductImageService : IProductImageService
{
    private readonly IProductImageRepository _imageRepository;
    private readonly IRepository<VegProducts> _productRepository;

    public ProductImageService(IProductImageRepository imageRepository, IRepository<VegProducts> productRepository)
    {
        _imageRepository = imageRepository;
        _productRepository = productRepository;
    }

    /// <summary>
    /// Get all images for a specific product
    /// </summary>
    public async Task<IEnumerable<ProductImageDto>> GetImagesByProductIdAsync(int productId)
    {
        // Verify product exists
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {productId} not found");

        var images = await _imageRepository.GetImagesByProductIdAsync(productId);
        
        return images.Select(img => MapToDto(img));
    }

    /// <summary>
    /// Get a specific image by ID
    /// </summary>
    public async Task<ProductImageDto?> GetImageByIdAsync(int id)
    {
        var image = await _imageRepository.GetByIdAsync(id);
        return image == null ? null : MapToDto(image);
    }

    /// <summary>
    /// Get the main/hero image for a product
    /// </summary>
    public async Task<ProductImageDto?> GetMainImageAsync(int productId)
    {
        var image = await _imageRepository.GetMainImageByProductIdAsync(productId);
        return image == null ? null : MapToDto(image);
    }

    /// <summary>
    /// Get the mobile optimized image for a product
    /// </summary>
    public async Task<ProductImageDto?> GetMobileImageAsync(int productId)
    {
        var image = await _imageRepository.GetMobileImageByProductIdAsync(productId);
        return image == null ? null : MapToDto(image);
    }

    /// <summary>
    /// Get gallery images for a product
    /// </summary>
    public async Task<IEnumerable<ProductImageDto>> GetGalleryImagesAsync(int productId)
    {
        var images = await _imageRepository.GetGalleryImagesByProductIdAsync(productId);
        return images.Select(img => MapToDto(img));
    }

    /// <summary>
    /// Create a new product image
    /// </summary>
    public async Task<ProductImageDto> CreateImageAsync(int productId, ProductImageCreateUpdateDto createDto)
    {
        // Verify product exists
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {productId} not found");

        // Validate image
        var validation = await ValidateImageAsync(productId, createDto);
        if (!validation.IsValid)
            throw new InvalidOperationException(validation.Message);

        // Check max images per product
        var imageCount = await _imageRepository.GetImageCountByProductIdAsync(productId);
        if (imageCount >= 10)
            throw new InvalidOperationException("Maximum 10 images per product allowed");

        var image = new ProductImage
        {
            IdProduct = productId,
            ImageUrl = createDto.ImageUrl,
            ImageType = (ProductImageType)(int)createDto.ImageType,
            DisplayOrder = createDto.DisplayOrder,
            Width = createDto.Width,
            Height = createDto.Height,
            UploadedDate = DateTime.UtcNow,
            IsActive = true
        };

        var createdImage = await _imageRepository.AddAsync(image);
        return MapToDto(createdImage);
    }

    /// <summary>
    /// Update an existing product image
    /// </summary>
    public async Task<ProductImageDto> UpdateImageAsync(int id, ProductImageCreateUpdateDto updateDto)
    {
        var image = await _imageRepository.GetByIdAsync(id);
        if (image == null)
            throw new KeyNotFoundException($"Image with ID {id} not found");

        // Validate image
        var validation = await ValidateImageAsync(image.IdProduct, updateDto);
        if (!validation.IsValid)
            throw new InvalidOperationException(validation.Message);

        image.ImageUrl = updateDto.ImageUrl;
        image.ImageType = (ProductImageType)(int)updateDto.ImageType;
        image.DisplayOrder = updateDto.DisplayOrder;
        image.Width = updateDto.Width;
        image.Height = updateDto.Height;

        await _imageRepository.UpdateAsync(image);
        return MapToDto(image);
    }

    /// <summary>
    /// Delete a product image
    /// </summary>
    public async Task DeleteImageAsync(int id)
    {
        var image = await _imageRepository.GetByIdAsync(id);
        if (image == null)
            throw new KeyNotFoundException($"Image with ID {id} not found");

        await _imageRepository.DeleteAsync(image);
    }

    /// <summary>
    /// Delete all images for a product
    /// </summary>
    public async Task DeleteProductImagesAsync(int productId)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {productId} not found");

        await _imageRepository.DeleteProductImagesAsync(productId);
    }

    /// <summary>
    /// Get count of images for a product
    /// </summary>
    public async Task<int> GetImageCountAsync(int productId)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {productId} not found");

        return await _imageRepository.GetImageCountByProductIdAsync(productId);
    }

    /// <summary>
    /// Validate image dimensions and type
    /// </summary>
    public Task<ValidationResult> ValidateImageAsync(int productId, ProductImageCreateUpdateDto imageDto)
    {
        // Main Image validation: 1000x800px (5:4 ratio ± 10%)
        if (imageDto.ImageType == ProductImageTypeDto.Main)
        {
            if (imageDto.Width.HasValue && imageDto.Height.HasValue)
            {
                var aspectRatio = (double)imageDto.Width.Value / imageDto.Height.Value;
                var expectedRatio = 5.0 / 4.0; // 1.25
                var tolerance = expectedRatio * 0.1; // 10% tolerance

                if (Math.Abs(aspectRatio - expectedRatio) > tolerance)
                {
                    return Task.FromResult(new ValidationResult(false, 
                        $"Main image must have 5:4 aspect ratio (1.25±0.125). Your image is {aspectRatio:F2}"));
                }

                if (imageDto.Width < 800 || imageDto.Width > 1200)
                {
                    return Task.FromResult(new ValidationResult(false, 
                        "Main image width should be between 800-1200px (recommended: 1000px)"));
                }
            }
        }

        // Mobile Image validation: 600x600px (1:1 square ± 5%)
        else if (imageDto.ImageType == ProductImageTypeDto.Mobile)
        {
            if (imageDto.Width.HasValue && imageDto.Height.HasValue)
            {
                var aspectRatio = (double)imageDto.Width.Value / imageDto.Height.Value;
                var expectedRatio = 1.0; // 1:1
                var tolerance = expectedRatio * 0.05; // 5% tolerance

                if (Math.Abs(aspectRatio - expectedRatio) > tolerance)
                {
                    return Task.FromResult(new ValidationResult(false,
                        $"Mobile image must have 1:1 square aspect ratio. Your image is {aspectRatio:F2}"));
                }

                if (imageDto.Width < 500 || imageDto.Width > 700)
                {
                    return Task.FromResult(new ValidationResult(false,
                        "Mobile image should be between 500-700px square (recommended: 600x600px)"));
                }
            }
        }

        // Gallery Image validation: 900x675px (4:3 ratio ± 10%)
        else if (imageDto.ImageType == ProductImageTypeDto.Gallery)
        {
            if (imageDto.Width.HasValue && imageDto.Height.HasValue)
            {
                var aspectRatio = (double)imageDto.Width.Value / imageDto.Height.Value;
                var expectedRatio = 4.0 / 3.0; // 1.33
                var tolerance = expectedRatio * 0.1; // 10% tolerance

                if (Math.Abs(aspectRatio - expectedRatio) > tolerance)
                {
                    return Task.FromResult(new ValidationResult(false,
                        $"Gallery image must have 4:3 aspect ratio (1.33±0.13). Your image is {aspectRatio:F2}"));
                }

                if (imageDto.Width < 700 || imageDto.Width > 1000)
                {
                    return Task.FromResult(new ValidationResult(false,
                        "Gallery image width should be between 700-1000px (recommended: 900px)"));
                }
            }
        }

        return Task.FromResult(new ValidationResult(true));
    }

    /// <summary>
    /// Map ProductImage entity to DTO
    /// </summary>
    private ProductImageDto MapToDto(ProductImage image)
    {
        return new ProductImageDto
        {
            Id = image.Id,
            IdProduct = image.IdProduct,
            ImageUrl = image.ImageUrl,
            ImageType = (ProductImageTypeDto)(int)image.ImageType,
            DisplayOrder = image.DisplayOrder,
            Width = image.Width,
            Height = image.Height,
            UploadedDate = image.UploadedDate,
            IsActive = image.IsActive
        };
    }
}
