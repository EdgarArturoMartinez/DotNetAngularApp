using DotNetCoreWebApi.Application.Entities;
using DotNetCoreWebApi.Application.Interfaces;
using DotNetCoreWebApi.Application.Services;
using DotNetCoreWebApi.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DotNetCoreWebApi.Controllers;

/// <summary>
/// Controller for ProductImage operations
/// </summary>
[ApiController]
[Route("api/products/{productId}/images")]
public class ProductImagesController : ControllerBase
{
    private readonly IProductImageService _imageService;
    private readonly IFileUploadService _fileUploadService;
    private readonly ILogger<ProductImagesController> _logger;

    public ProductImagesController(IProductImageService imageService, IFileUploadService fileUploadService, ILogger<ProductImagesController> logger)
    {
        _imageService = imageService;
        _fileUploadService = fileUploadService;
        _logger = logger;
    }

    /// <summary>
    /// Get all images for a specific product
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductImageDto>>> GetProductImages(int productId)
    {
        try
        {
            var images = await _imageService.GetImagesByProductIdAsync(productId);
            return Ok(images);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get the main/hero image for a product
    /// </summary>
    [HttpGet("main")]
    public async Task<ActionResult<ProductImageDto>> GetMainImage(int productId)
    {
        try
        {
            var image = await _imageService.GetMainImageAsync(productId);
            if (image == null)
                return NotFound(new { message = $"Main image not found for product {productId}" });

            return Ok(image);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get the mobile optimized image for a product
    /// </summary>
    [HttpGet("mobile")]
    public async Task<ActionResult<ProductImageDto>> GetMobileImage(int productId)
    {
        try
        {
            var image = await _imageService.GetMobileImageAsync(productId);
            if (image == null)
                return NotFound(new { message = $"Mobile image not found for product {productId}" });

            return Ok(image);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get gallery/additional images for a product
    /// </summary>
    [HttpGet("gallery")]
    public async Task<ActionResult<IEnumerable<ProductImageDto>>> GetGalleryImages(int productId)
    {
        try
        {
            var images = await _imageService.GetGalleryImagesAsync(productId);
            return Ok(images);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get a specific image by ID
    /// </summary>
    [HttpGet("{imageId}")]
    public async Task<ActionResult<ProductImageDto>> GetImageById(int productId, int imageId)
    {
        try
        {
            var image = await _imageService.GetImageByIdAsync(imageId);
            if (image == null || image.IdProduct != productId)
                return NotFound(new { message = $"Image {imageId} not found for product {productId}" });

            return Ok(image);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Create a new product image
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ProductImageDto>> CreateImage(int productId, [FromBody] ProductImageCreateUpdateDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var createdImage = await _imageService.CreateImageAsync(productId, createDto);
            return CreatedAtAction(nameof(GetImageById), new { productId, imageId = createdImage.Id }, createdImage);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing product image
    /// </summary>
    [HttpPut("{imageId}")]
    public async Task<IActionResult> UpdateImage(int productId, int imageId, [FromBody] ProductImageCreateUpdateDto updateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updatedImage = await _imageService.UpdateImageAsync(imageId, updateDto);
            return Ok(updatedImage);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a product image
    /// </summary>
    [HttpDelete("{imageId}")]
    public async Task<IActionResult> DeleteImage(int productId, int imageId)
    {
        try
        {
            await _imageService.DeleteImageAsync(imageId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete all images for a product
    /// </summary>
    [HttpDelete]
    public async Task<IActionResult> DeleteAllProductImages(int productId)
    {
        try
        {
            await _imageService.DeleteProductImagesAsync(productId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get image count for a product
    /// </summary>
    [HttpGet("count")]
    public async Task<ActionResult<int>> GetImageCount(int productId)
    {
        try
        {
            var count = await _imageService.GetImageCountAsync(productId);
            return Ok(count);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Validate image dimensions and type before upload
    /// </summary>
    [HttpPost("validate")]
    public async Task<ActionResult> ValidateImage(int productId, [FromBody] ProductImageCreateUpdateDto imageDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var validationResult = await _imageService.ValidateImageAsync(productId, imageDto);
            
            if (!validationResult.IsValid)
                return BadRequest(new { valid = false, message = validationResult.Message });

            return Ok(new { valid = true, message = "Image validation passed" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Upload a product image file
    /// </summary>
    [HttpPost("upload")]
    public async Task<ActionResult<ProductImageDto>> UploadImage(int productId, IFormFile file, [FromForm] int imageType, [FromForm] int displayOrder = 0)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "No file provided" });

        try
        {
            // Save the file
            string relativeImagePath = await _fileUploadService.SaveProductImageAsync(file, productId, imageType);

            // Create ProductImageCreateUpdateDto
            var imageDto = new ProductImageCreateUpdateDto
            {
                ImageUrl = relativeImagePath,
                ImageType = (ProductImageTypeDto)imageType,
                DisplayOrder = displayOrder,
                Width = null,
                Height = null
            };

            // Create the database record
            var createdImage = await _imageService.CreateImageAsync(productId, imageDto);

            _logger.LogInformation($"Image uploaded for product {productId}: {relativeImagePath}");
            return CreatedAtAction(nameof(GetImageById), new { productId, imageId = createdImage.Id }, createdImage);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, $"Error uploading image for product {productId}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error saving image file" });
        }
    }
}
