using DotNetCoreWebApi.Application.Entities;
using DotNetCoreWebApi.Infrastructure.Repositories;
using DotNetCoreWebApi.Tests.Helpers;
using FluentAssertions;

namespace DotNetCoreWebApi.Tests.Unit.Repositories;

/// <summary>
/// Unit tests for VegProductRepository
/// Tests data access layer operations using in-memory database
/// </summary>
public class VegProductRepositoryTests : IDisposable
{
    private readonly Application.DBContext.ApplicationDBContext _context;
    private readonly VegProductRepository _repository;

    public VegProductRepositoryTests()
    {
        _context = TestDbContextFactory.CreateInMemoryContext();
        _repository = new VegProductRepository(_context);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllProducts()
    {
        // Arrange
        var products = MockDataGenerator.GenerateProducts(5);
        await _context.VegProducts.AddRangeAsync(products);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsProduct()
    {
        // Arrange
        var product = MockDataGenerator.GenerateProduct(1, "Test Product", 5000, 100);
        await _context.VegProducts.AddAsync(product);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Test Product");
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_AddsProductToDatabase()
    {
        // Arrange
        var product = MockDataGenerator.GenerateProduct(0, "New Product", 3000, 50);

        // Act
        var result = await _repository.AddAsync(product);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        
        var saved = await _context.VegProducts.FindAsync(result.Id);
        saved.Should().NotBeNull();
        saved!.Name.Should().Be("New Product");
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingProduct()
    {
        // Arrange
        var product = MockDataGenerator.GenerateProduct(1, "Original", 1000, 10);
        await _context.VegProducts.AddAsync(product);
        await _context.SaveChangesAsync();

        // Act
        product.Name = "Updated";
        product.Price = 2000;
        await _repository.UpdateAsync(product);

        // Assert
        var updated = await _context.VegProducts.FindAsync(1);
        updated.Should().NotBeNull();
        updated!.Name.Should().Be("Updated");
        updated.Price.Should().Be(2000);
    }

    [Fact]
    public async Task DeleteAsync_RemovesProduct()
    {
        // Arrange
        var product = MockDataGenerator.GenerateProduct(1, "To Delete", 1000, 10);
        await _context.VegProducts.AddAsync(product);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(product);

        // Assert
        var deleted = await _context.VegProducts.FindAsync(1);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task GetProductsWithCategoryAsync_IncludesNavigationProperties()
    {
        // Arrange
        var category = MockDataGenerator.GenerateCategory(1, "Vegetables");
        await _context.VegCategories.AddAsync(category);
        await _context.SaveChangesAsync();

        var product = MockDataGenerator.GenerateProduct(1, "Product", 1000, 10, 1);
        await _context.VegProducts.AddAsync(product);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetProductsWithCategoryAsync();

        // Assert
        var retrieved = result.First();
        retrieved.VegCategory.Should().NotBeNull();
        retrieved.VegCategory!.CategoryName.Should().Be("Vegetables");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
