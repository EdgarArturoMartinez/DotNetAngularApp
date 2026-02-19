using DotNetCoreWebApi.Application.Entities;
using DotNetCoreWebApi.Application.Interfaces;
using DotNetCoreWebApi.DTOs;

namespace DotNetCoreWebApi.Application.Services;

/// <summary>
/// Service implementation for VegProduct business logic
/// </summary>
public class VegProductService : IVegProductService
{
    private readonly IVegProductRepository _productRepository;

    public VegProductService(IVegProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<VegProductDto>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetProductsWithCategoryAsync();
        
        return products.Select(p => new VegProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Description = p.Description,
            StockQuantity = p.StockQuantity,
            NetWeight = p.NetWeight,
            IdCategory = p.IdCategory,
            IdTypeWeight = p.IdTypeWeight,
            VegCategory = p.VegCategory == null ? null : new VegCategoryBasicDto
            {
                IdCategory = p.VegCategory.IdCategory,
                CategoryName = p.VegCategory.CategoryName,
                Description = p.VegCategory.Description
            },
            VegTypeWeight = p.VegTypeWeight == null ? null : new VegTypeWeightBasicDto
            {
                IdTypeWeight = p.VegTypeWeight.Id,
                Name = p.VegTypeWeight.Name,
                AbbreviationWeight = p.VegTypeWeight.AbbreviationWeight
            }
        });
    }

    public async Task<VegProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _productRepository.GetProductWithCategoryAsync(id);
        
        if (product == null)
            return null;

        return new VegProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            StockQuantity = product.StockQuantity,
            NetWeight = product.NetWeight,
            IdCategory = product.IdCategory,
            IdTypeWeight = product.IdTypeWeight,
            VegCategory = product.VegCategory == null ? null : new VegCategoryBasicDto
            {
                IdCategory = product.VegCategory.IdCategory,
                CategoryName = product.VegCategory.CategoryName,
                Description = product.VegCategory.Description
            },
            VegTypeWeight = product.VegTypeWeight == null ? null : new VegTypeWeightBasicDto
            {
                IdTypeWeight = product.VegTypeWeight.Id,
                Name = product.VegTypeWeight.Name,
                AbbreviationWeight = product.VegTypeWeight.AbbreviationWeight
            }
        };
    }

    public async Task<VegProductDto> CreateProductAsync(VegProductCreateUpdateDto dto)
    {
        var product = new VegProducts
        {
            Name = dto.Name,
            Price = dto.Price,
            Description = dto.Description,
            StockQuantity = dto.StockQuantity,
            NetWeight = dto.NetWeight,
            IdCategory = dto.IdCategory,
            IdTypeWeight = dto.IdTypeWeight
        };

        var created = await _productRepository.AddAsync(product);

        return new VegProductDto
        {
            Id = created.Id,
            Name = created.Name,
            Price = created.Price,
            Description = created.Description,
            StockQuantity = created.StockQuantity,
            NetWeight = created.NetWeight,
            IdCategory = created.IdCategory,
            IdTypeWeight = created.IdTypeWeight
        };
    }

    public async Task UpdateProductAsync(int id, VegProductCreateUpdateDto dto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {id} not found");

        product.Name = dto.Name;
        product.Price = dto.Price;
        product.Description = dto.Description;
        product.StockQuantity = dto.StockQuantity;
        product.NetWeight = dto.NetWeight;
        product.IdCategory = dto.IdCategory;
        product.IdTypeWeight = dto.IdTypeWeight;

        await _productRepository.UpdateAsync(product);
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {id} not found");

        await _productRepository.DeleteAsync(product);
    }

    public async Task<bool> ProductExistsAsync(int id)
    {
        return await _productRepository.ExistsAsync(id);
    }

    public async Task<IEnumerable<VegProductDto>> GetProductsByCategoryAsync(int categoryId)
    {
        var products = await _productRepository.GetProductsByCategoryAsync(categoryId);
        
        return products.Select(p => new VegProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Description = p.Description,
            StockQuantity = p.StockQuantity,
            NetWeight = p.NetWeight,
            IdCategory = p.IdCategory,
            IdTypeWeight = p.IdTypeWeight,
            VegCategory = p.VegCategory == null ? null : new VegCategoryBasicDto
            {
                IdCategory = p.VegCategory.IdCategory,
                CategoryName = p.VegCategory.CategoryName,
                Description = p.VegCategory.Description
            },
            VegTypeWeight = p.VegTypeWeight == null ? null : new VegTypeWeightBasicDto
            {
                IdTypeWeight = p.VegTypeWeight.Id,
                Name = p.VegTypeWeight.Name,
                AbbreviationWeight = p.VegTypeWeight.AbbreviationWeight
            }
        });
    }
}
