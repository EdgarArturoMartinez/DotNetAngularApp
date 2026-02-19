namespace DotNetCoreWebApi.Application.Entities;

public class VegProducts
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public decimal? NetWeight { get; set; }

    //Add Foreign Key for VegCategory
    public int? IdCategory { get; set; }

    // Foreign Key for VegTypeWeight
    public int? IdTypeWeight { get; set; }

    // Navigation property for many-to-one relationship with VegCategory
    public virtual VegCategory? VegCategory { get; set; }

    // Navigation property for many-to-one relationship with VegTypeWeight
    public virtual VegTypeWeight? VegTypeWeight { get; set; }

    // Navigation property for one-to-many relationship with ProductImage
    public virtual ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
}
