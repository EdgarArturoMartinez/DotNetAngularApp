using DotNetCoreWebApi.Application.Entities;
using DotNetCoreWebApi.Application.Interfaces;
using DotNetCoreWebApi.Application.Services;
using DotNetCoreWebApi.Application.DTOs;
using DotNetCoreWebApi.Tests.Helpers;
using FluentAssertions;
using Moq;

namespace DotNetCoreWebApi.Tests.Unit.Services;

/// <summary>
/// Unit tests for VegProductService
/// Tests the business logic layer in isolation using mocked dependencies
/// </summary>
public class VegProductServiceTests
{
    private readonly Mock<IVegProductRepository> _mockRepository;
    private readonly VegProductService _service;

    public VegProductServiceTests()
    {
        _mockRepository = new Mock<IVegProductRepository>();
        _service = new VegProductService(_mockRepository.Object);
    }

    #region GetAllProductsAsync Tests

    [Fact]
    public async Task GetAllProductsAsync_ReturnsAllProducts()
    {
        // Arrange
        var products = MockDataGenerator.GenerateProducts(5);
        _mockRepository.Setup(r => r.GetProductsWithCategoryAsync())
            .ReturnsAsync(products);

        // Act
        var result = await _service.GetAllProductsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(5);
        _mockRepository.Verify(r => r.GetProductsWithCategoryAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllProductsAsync_WhenNoProducts_ReturnsEmptyList()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetProductsWithCategoryAsync())
            .ReturnsAsync(new List<VegProducts>());

        // Act
        var result = await _service.GetAllProductsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllProductsAsync_MapsPropertiesToDto()
    {
        // Arrange
        var product = MockDataGenerator.GenerateProduct(1, "Tomato", 5000, 100, 1, "Fresh tomatoes");
        _mockRepository.Setup(r => r.GetProductsWithCategoryAsync())
            .ReturnsAsync(new List<VegProducts> { product });

        // Act
        var result = await _service.GetAllProductsAsync();
        var dto = result.First();

        // Assert
        dto.Id.Should().Be(1);
        dto.Name.Should().Be("Tomato");
        dto.Price.Should().Be(5000);
        dto.StockQuantity.Should().Be(100);
        dto.IdCategory.Should().Be(1);
        dto.Description.Should().Be("Fresh tomatoes");
    }

    #endregion

    #region GetProductByIdAsync Tests

    [Fact]
    public async Task GetProductByIdAsync_WithValidId_ReturnsProduct()
    {
        // Arrange
        var product = MockDataGenerator.GenerateProduct(1, "Carrot", 3000, 50);
        _mockRepository.Setup(r => r.GetProductWithCategoryAsync(1))
            .ReturnsAsync(product);

        // Act
        var result = await _service.GetProductByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Carrot");
        _mockRepository.Verify(r => r.GetProductWithCategoryAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetProductByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetProductWithCategoryAsync(999))
            .ReturnsAsync((VegProducts?)null);

        // Act
        var result = await _service.GetProductByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region CreateProductAsync Tests

    [Fact]
    public async Task CreateProductAsync_WithValidDto_CreatesProduct()
    {
        // Arrange
        var createDto = new VegProductCreateUpdateDto
        {
            Name = "Lettuce",
            Price = 2500,
            StockQuantity = 75,
            IdCategory = 1,
            Description = "Fresh lettuce"
        };

        var createdProduct = new VegProducts
        {
            Id = 1,
            Name = createDto.Name,
            Price = createDto.Price,
            StockQuantity = createDto.StockQuantity,
            IdCategory = createDto.IdCategory,
            Description = createDto.Description
        };

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<VegProducts>()))
            .ReturnsAsync(createdProduct);

        // Act
        var result = await _service.CreateProductAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("Lettuce");
        result.Price.Should().Be(2500);
        _mockRepository.Verify(r => r.AddAsync(It.Is<VegProducts>(
            p => p.Name == createDto.Name && 
                 p.Price == createDto.Price &&
                 p.StockQuantity == createDto.StockQuantity
        )), Times.Once);
    }

    [Fact]
    public async Task CreateProductAsync_MapsAllDtoProperties()
    {
        // Arrange
        var createDto = new VegProductCreateUpdateDto
        {
            Name = "Cucumber",
            Price = 3500,
            StockQuantity = 60,
            IdCategory = 2,
            Description = "Organic cucumber"
        };

        VegProducts? capturedProduct = null;
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<VegProducts>()))
            .Callback<VegProducts>(p => capturedProduct = p)
            .ReturnsAsync((VegProducts p) => p);

        // Act
        await _service.CreateProductAsync(createDto);

        // Assert
        capturedProduct.Should().NotBeNull();
        capturedProduct!.Name.Should().Be(createDto.Name);
        capturedProduct.Price.Should().Be(createDto.Price);
        capturedProduct.StockQuantity.Should().Be(createDto.StockQuantity);
        capturedProduct.IdCategory.Should().Be(createDto.IdCategory);
        capturedProduct.Description.Should().Be(createDto.Description);
    }

    #endregion

    #region UpdateProductAsync Tests

    [Fact]
    public async Task UpdateProductAsync_WithValidId_UpdatesProduct()
    {
        // Arrange
        var existingProduct = MockDataGenerator.GenerateProduct(1, "Old Name", 1000, 10);
        var updateDto = new VegProductCreateUpdateDto
        {
            Name = "New Name",
            Price = 2000,
            StockQuantity = 20,
            IdCategory = 1,
            Description = "Updated description"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(existingProduct);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<VegProducts>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateProductAsync(1, updateDto);

        // Assert
        existingProduct.Name.Should().Be("New Name");
        existingProduct.Price.Should().Be(2000);
        existingProduct.StockQuantity.Should().Be(20);
        _mockRepository.Verify(r => r.UpdateAsync(existingProduct), Times.Once);
    }

    [Fact]
    public async Task UpdateProductAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((VegProducts?)null);

        var updateDto = new VegProductCreateUpdateDto
        {
            Name = "Name",
            Price = 1000,
            StockQuantity = 10
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.UpdateProductAsync(999, updateDto)
        );
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<VegProducts>()), Times.Never);
    }

    #endregion

    #region DeleteProductAsync Tests

    [Fact]
    public async Task DeleteProductAsync_WithValidId_DeletesProduct()
    {
        // Arrange
        var product = MockDataGenerator.GenerateProduct(1, "Product to Delete", 1000, 10);
        _mockRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(product);
        _mockRepository.Setup(r => r.DeleteAsync(product))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteProductAsync(1);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(product), Times.Once);
    }

    [Fact]
    public async Task DeleteProductAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((VegProducts?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.DeleteProductAsync(999)
        );
        _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<VegProducts>()), Times.Never);
    }

    #endregion

    #region GetProductsByCategoryAsync Tests

    [Fact]
    public async Task GetProductsByCategoryAsync_WithValidCategoryId_ReturnsProducts()
    {
        // Arrange
        var products = new List<VegProducts>
        {
            MockDataGenerator.GenerateProduct(1, "Product 1", 1000, 10, 1),
            MockDataGenerator.GenerateProduct(2, "Product 2", 2000, 20, 1)
        };

        _mockRepository.Setup(r => r.GetProductsByCategoryAsync(1))
            .ReturnsAsync(products);

        // Act
        var result = await _service.GetProductsByCategoryAsync(1);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(p => p.IdCategory == 1);
    }

    [Fact]
    public async Task GetProductsByCategoryAsync_WithNoCategoryMatch_ReturnsEmptyList()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetProductsByCategoryAsync(999))
            .ReturnsAsync(new List<VegProducts>());

        // Act
        var result = await _service.GetProductsByCategoryAsync(999);

        // Assert
        result.Should().BeEmpty();
    }

    #endregion
}
