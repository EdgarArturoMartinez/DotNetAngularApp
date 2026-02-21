using Bogus;
using DotNetCoreWebApi.Application.Entities;

namespace DotNetCoreWebApi.Tests.Helpers;

/// <summary>
/// Generates mock data for testing using Bogus library
/// </summary>
public static class MockDataGenerator
{
    /// <summary>
    /// Generates a list of random VegCategory entities
    /// </summary>
    public static List<VegCategory> GenerateCategories(int count)
    {
        var categoryFaker = new Faker<VegCategory>()
            .RuleFor(c => c.IdCategory, f => f.IndexFaker + 1)
            .RuleFor(c => c.CategoryName, f => f.Commerce.Categories(1)[0])
            .RuleFor(c => c.Description, f => f.Lorem.Sentence());

        return categoryFaker.Generate(count);
    }

    /// <summary>
    /// Generates a single VegCategory with specific values
    /// </summary>
    public static VegCategory GenerateCategory(int id, string name, string? description = null)
    {
        return new VegCategory
        {
            IdCategory = id,
            CategoryName = name,
            Description = description ?? $"Description for {name}"
        };
    }

    /// <summary>
    /// Generates a list of random VegProducts
    /// </summary>
    public static List<VegProducts> GenerateProducts(int count, List<VegCategory>? categories = null)
    {
        var productFaker = new Faker<VegProducts>()
            .RuleFor(p => p.Id, f => f.IndexFaker + 1)
            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
            .RuleFor(p => p.Price, f => f.Random.Decimal(500, 50000))
            .RuleFor(p => p.Description, f => f.Lorem.Sentence())
            .RuleFor(p => p.StockQuantity, f => f.Random.Int(0, 1000))
            .RuleFor(p => p.IdCategory, f => categories != null ? 
                f.PickRandom(categories).IdCategory : 
                f.Random.Int(1, 5));

        return productFaker.Generate(count);
    }

    /// <summary>
    /// Generates a single VegProduct with specific values
    /// </summary>
    public static VegProducts GenerateProduct(
        int id, 
        string name, 
        decimal price, 
        int stockQuantity,
        int? categoryId = null,
        string? description = null)
    {
        return new VegProducts
        {
            Id = id,
            Name = name,
            Price = price,
            StockQuantity = stockQuantity,
            IdCategory = categoryId,
            Description = description ?? $"Description for {name}"
        };
    }

    /// <summary>
    /// Generates a list of random ProductImages
    /// </summary>
    public static List<ProductImage> GenerateProductImages(int count, int productId)
    {
        var imageFaker = new Faker<ProductImage>()
            .RuleFor(i => i.Id, f => f.IndexFaker + 1)
            .RuleFor(i => i.IdProduct, f => productId)
            .RuleFor(i => i.ImageUrl, f => $"images/product_{productId}_{f.Random.AlphaNumeric(8)}.jpg")
            .RuleFor(i => i.ImageType, f => f.Random.Enum<ProductImageType>())
            .RuleFor(i => i.DisplayOrder, f => f.IndexFaker)
            .RuleFor(i => i.UploadedDate, f => f.Date.Recent(30));

        return imageFaker.Generate(count);
    }

    /// <summary>
    /// Generates a list of random VegTypeWeights
    /// </summary>
    public static List<VegTypeWeight> GenerateVegTypeWeights(int count)
    {
        var weightFaker = new Faker<VegTypeWeight>()
            .RuleFor(w => w.Id, f => f.IndexFaker + 1)
            .RuleFor(w => w.Name, f => f.PickRandom(new[] { "Kilogram", "Gram", "Pound", "Ounce" }))
            .RuleFor(w => w.AbbreviationWeight, f => f.PickRandom(new[] { "Kg", "g", "Lb", "Oz" }))
            .RuleFor(w => w.Description, f => f.Lorem.Sentence())
            .RuleFor(w => w.IsActive, f => true);

        return weightFaker.Generate(count);
    }
}
