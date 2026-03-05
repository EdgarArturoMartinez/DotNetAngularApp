using System.ComponentModel.DataAnnotations;

namespace DotNetCoreWebApi.Application.DTOs;

/// <summary>
/// DTO for retrieving product image information
/// </summary>
public class ProductImageDto
{
    public int Id { get; set; }

    public int IdProduct { get; set; }

    public required string ImageUrl { get; set; }

    public ProductImageTypeDto ImageType { get; set; }

    public int DisplayOrder { get; set; }

    public int? Width { get; set; }

    public int? Height { get; set; }

    public DateTime UploadedDate { get; set; }

    public bool IsActive { get; set; }
}

/// <summary>
/// DTO for creating/updating product images
/// </summary>
public class ProductImageCreateUpdateDto
{
    [Required(ErrorMessage = "Image URL is required")]
    [StringLength(255, ErrorMessage = "Image URL cannot exceed 255 characters")]
    public required string ImageUrl { get; set; }

    [Required(ErrorMessage = "Image type is required")]
    public ProductImageTypeDto ImageType { get; set; }

    public int DisplayOrder { get; set; } = 0;

    [Range(0, 10000, ErrorMessage = "Width must be between 0 and 10000")]
    public int? Width { get; set; }

    [Range(0, 10000, ErrorMessage = "Height must be between 0 and 10000")]
    public int? Height { get; set; }
}

/// <summary>
/// Enum for product image types used in DTOs
/// Main = 0, Mobile = 1, Gallery = 2
/// </summary>
public enum ProductImageTypeDto
{
    Main = 0,
    Mobile = 1,
    Gallery = 2
}
