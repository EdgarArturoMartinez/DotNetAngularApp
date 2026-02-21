using DotNetCoreWebApi.Application.DBContext;
using DotNetCoreWebApi.Application.Entities;
using DotNetCoreWebApi.Application.Interfaces;
using DotNetCoreWebApi.Application.Services;
using DotNetCoreWebApi.DTOs;
using DotNetCoreWebApi.Infrastructure.Repositories;
using DotNetCoreWebApi.Tests.Helpers;
using FluentAssertions;

namespace DotNetCoreWebApi.Tests.Integration;

/// <summary>
/// Integration tests that verify the full application stack
/// Tests the interaction between services, repositories, and database
/// </summary>
public class ProductCrudIntegrationTests : IDisposable
{
    private readonly ApplicationDBContext _context;
    private readonly IVegProductRepository _productRepository;
    private readonly IVegCategoryRepository _categoryRepository;
    private readonly IVegProductService _productService;
    private readonly IVegCategoryService _categoryService;

    public ProductCrudIntegrationTests()
    {
        // Create in-memory database for testing
        _context = TestDbContextFactory.CreateInMemoryContext();
        
        // Initialize repositories
        _productRepository = new VegProductRepository(_context);
        _categoryRepository = new VegCategoryRepository(_context);
        
        // Initialize services
        _productService = new VegProductService(_productRepository);
        _categoryService = new VegCategoryService(_categoryRepository);
    }

    [Fact]
    public async Task FullProductCrudWorkflow_WorksCorrectly()
    {
        // ========================================
        // ARRANGE: Create a category first
        // ========================================
        var categoryDto = new VegCategoryCreateUpdateDto
        {
            CategoryName = "Vegetables",
            Description = "Fresh vegetables"
        };
        var createdCategory = await _categoryService.CreateCategoryAsync(categoryDto);

        // ========================================
        // ACT & ASSERT: CREATE Product
        // ========================================
        var createDto = new VegProductCreateUpdateDto
        {
            Name = "Tomato",
            Price = 5000,
            StockQuantity = 100,
            IdCategory = createdCategory.IdCategory,
            Description = "Fresh tomatoes"
        };

        var createdProduct = await _productService.CreateProductAsync(createDto);
        
        createdProduct.Should().NotBeNull();
        createdProduct.Id.Should().BeGreaterThan(0);
        createdProduct.Name.Should().Be("Tomato");
        createdProduct.Price.Should().Be(5000);
        createdProduct.StockQuantity.Should().Be(100);

        // ======================================== 
        // ACT & ASSERT: READ Product by ID
        // ========================================
        var retrievedProduct = await _productService.GetProductByIdAsync(createdProduct.Id);
        
        retrievedProduct.Should().NotBeNull();
        retrievedProduct!.Id.Should().Be(createdProduct.Id);
        retrievedProduct.Name.Should().Be("Tomato");

        // ========================================
        // ACT & ASSERT: READ All Products
        // ========================================
        var allProducts = await _productService.GetAllProductsAsync();
        
        allProducts.Should().NotBeEmpty();
        allProducts.Should().Contain(p => p.Id == createdProduct.Id);

        // ========================================
        // ACT & ASSERT: UPDATE Product
        // ========================================
        var updateDto = new VegProductCreateUpdateDto
        {
            Name = "Cherry Tomato",
            Price = 7000,
            StockQuantity = 150,
            IdCategory = createdCategory.IdCategory,
            Description = "Fresh cherry tomatoes"
        };

        await _productService.UpdateProductAsync(createdProduct.Id, updateDto);

        var updatedProduct = await _productService.GetProductByIdAsync(createdProduct.Id);
        updatedProduct.Should().NotBeNull();
        updatedProduct!.Name.Should().Be("Cherry Tomato");
        updatedProduct. Price.Should().Be(7000);
        updatedProduct.StockQuantity.Should().Be(150);

        // ========================================
        // ACT & ASSERT: GET Products by Category
        // ========================================
        var categoryProducts = await _productService.GetProductsByCategoryAsync(createdCategory.IdCategory);
        
        categoryProducts.Should().NotBeEmpty();
        categoryProducts.Should().Contain(p => p.Id == createdProduct.Id);

        // ========================================
        // ACT & ASSERT: DELETE Product
        // ========================================
        await _productService.DeleteProductAsync(createdProduct.Id);

        var deletedProduct = await _productService.GetProductByIdAsync(createdProduct.Id);
        deletedProduct.Should().BeNull();
    }

    [Fact]
    public async Task CreateMultipleProducts_AllArePersisted()
    {
        // Arrange
        var category = await _categoryService.CreateCategoryAsync(new VegCategoryCreateUpdateDto
        {
            CategoryName = "Fruits",
            Description = "Fresh fruits"
        });

        var productDtos = new[]
        {
            new VegProductCreateUpdateDto { Name = "Apple", Price = 4000, StockQuantity = 50, IdCategory = category.IdCategory },
            new VegProductCreateUpdateDto { Name = "Banana", Price = 3000, StockQuantity = 75, IdCategory = category.IdCategory },
            new VegProductCreateUpdateDto { Name = "Orange", Price = 3500, StockQuantity = 60, IdCategory = category.IdCategory }
        };

        // Act
        var createdProducts = new List<VegProductDto>();
        foreach (var dto in productDtos)
        {
            var product = await _productService.CreateProductAsync(dto);
            createdProducts.Add(product);
        }

        // Assert
        var allProducts = await _productService.GetAllProductsAsync();
        allProducts.Should().HaveCount(3);
        allProducts.Select(p => p.Name).Should().Contain(new[] { "Apple", "Banana", "Orange" });
    }

    [Fact]
    public async Task UpdateNonExistentProduct_ThrowsKeyNotFoundException()
    {
        // Arrange
        var updateDto = new VegProductCreateUpdateDto
        {
            Name = "Ghost Product",
            Price = 1000,
            StockQuantity = 10
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _productService.UpdateProductAsync(9999, updateDto)
        );
    }

    [Fact]
    public async Task DeleteNonExistentProduct_ThrowsKeyNotFoundException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _productService.DeleteProductAsync(9999)
        );
    }

    [Fact]
    public async Task CategoryCrudWorkflow_WorksCorrectly()
    {
        // CREATE
        var createDto = new VegCategoryCreateUpdateDto
        {
            CategoryName = "Organic",
            Description = "Organic products"
        };
        var created = await _categoryService.CreateCategoryAsync(createDto);
        created.Should().NotBeNull();
        created.CategoryName.Should().Be("Organic");

        // READ
        var retrieved = await _categoryService.GetCategoryByIdAsync(created.IdCategory);
        retrieved.Should().NotBeNull();
        retrieved!.CategoryName.Should().Be("Organic");

        // UPDATE
        var updateDto = new VegCategoryCreateUpdateDto
        {
            CategoryName = "Organic Premium",
            Description = "Premium organic products"
        };
        await _categoryService.UpdateCategoryAsync(created.IdCategory, updateDto);
        
        var updated = await _categoryService.GetCategoryByIdAsync(created.IdCategory);
        updated!.CategoryName.Should().Be("Organic Premium");

        // DELETE
        await _categoryService.DeleteCategoryAsync(created.IdCategory);
        var deleted = await _categoryService.GetCategoryByIdAsync(created.IdCategory);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task ProductsFilteredByCategoryId_ReturnsCorrectProducts()
    {
        // Arrange
        var category1 = await _categoryService.CreateCategoryAsync(new VegCategoryCreateUpdateDto
        {
            CategoryName = "Category 1"
        });
        
        var category2 = await _categoryService.CreateCategoryAsync(new VegCategoryCreateUpdateDto
        {
            CategoryName = "Category 2"
        });

        await _productService.CreateProductAsync(new VegProductCreateUpdateDto
        {
            Name = "Product A",
            Price = 1000,
            StockQuantity = 10,
            IdCategory = category1.IdCategory
        });

        await _productService.CreateProductAsync(new VegProductCreateUpdateDto
        {
            Name = "Product B",
            Price = 2000,
            StockQuantity = 20,
            IdCategory = category1.IdCategory
        });

        await _productService.CreateProductAsync(new VegProductCreateUpdateDto
        {
            Name = "Product C",
            Price = 3000,
            StockQuantity = 30,
            IdCategory = category2.IdCategory
        });

        // Act
        var category1Products = await _productService.GetProductsByCategoryAsync(category1.IdCategory);

        // Assert
        category1Products.Should().HaveCount(2);
        category1Products.Should().OnlyContain(p => p.IdCategory == category1.IdCategory);
        category1Products.Select(p => p.Name).Should().Contain(new[] { "Product A", "Product B" });
    }

    [Fact]
    public async Task CreateProductWithoutCategory_WorksCorrectly()
    {
        // Arrange
        var createDto = new VegProductCreateUpdateDto
        {
            Name = "Uncategorized Product",
            Price = 1500,
            StockQuantity = 25,
            IdCategory = null,
            Description = "Product without category"
        };

        // Act
        var created = await _productService.CreateProductAsync(createDto);

        // Assert
        created.Should().NotBeNull();
        created.IdCategory.Should().BeNull();
        created.Name.Should().Be("Uncategorized Product");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
