namespace DotNetCoreWebApi.Application.Entities;

public class ProductImage
{
    public int Id { get; set; }

    public int IdProduct { get; set; }

    public required string ImageUrl { get; set; } // Relative path or full URL (e.g., "/images/products/carrot-main.jpg")

    public ProductImageType ImageType { get; set; } // Main, Mobile, or Gallery

    public int DisplayOrder { get; set; } // For sorting in galleries (0 = main, 1 = mobile, 2+ = gallery order)

    public int? Width { get; set; } // Original upload width in pixels

    public int? Height { get; set; } // Original upload height in pixels

    public DateTime UploadedDate { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    // Navigation property for many-to-one relationship with VegProducts
    public virtual VegProducts? Product { get; set; }
}

/// <summary>
/// Defines image type categories for responsive image selection
/// </summary>
public enum ProductImageType
{
    /// <summary>
    /// Main/Hero image - 1000x800px (5:4 ratio)
    /// Used for product detail hero, featured sections
    /// </summary>
    Main = 0,

    /// <summary>
    /// Mobile optimized image - 600x600px (1:1 square)
    /// Used for shopping carts, product grids, thumbnails
    /// </summary>
    Mobile = 1,

    /// <summary>
    /// Gallery/Additional images - 900x675px (4:3 ratio)
    /// Used for carousel, product detail gallery, alternate views
    /// </summary>
    Gallery = 2
}
