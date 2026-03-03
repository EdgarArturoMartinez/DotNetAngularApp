using DotNetCoreWebApi.Application.Interfaces;
using DotNetCoreWebApi.Controllers;
using DotNetCoreWebApi.DTOs;
using DotNetCoreWebApi.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace DotNetCoreWebApi.Tests.Unit.Controllers;

/// <summary>
/// Unit tests for VegProductsController
/// Tests HTTP layer responses and status codes
/// </summary>
public class VegProductsControllerTests
{
    private readonly Mock<IVegProductService> _mockService;
    private readonly Mock<ILogger<VegProductsController>> _mockLogger;
    private readonly VegProductsController _controller;

    public VegProductsControllerTests()
    {
        _mockService = new Mock<IVegProductService>();
        _mockLogger = new Mock<ILogger<VegProductsController>>();
        _controller = new VegProductsController(_mockService.Object, _mockLogger.Object);
    }

    #region GetAllProducts Tests

    [Fact]
    public async Task GetAllProducts_ReturnsOkResultWithProducts()
    {
        // Arrange
        var products = new List<VegProductDto>
        {
            new VegProductDto { Id = 1, Name = "Tomato", Price = 5000, StockQuantity = 100 },
            new VegProductDto { Id = 2, Name = "Carrot", Price = 3000, StockQuantity = 50 }
        };
        _mockService.Setup(s => s.GetAllProductsAsync())
            .ReturnsAsync(products);

        // Act
        var result = await _controller.GetAllProducts();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProducts = okResult.Value.Should().BeAssignableTo<IEnumerable<VegProductDto>>().Subject;
        returnedProducts.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllProducts_WhenNoProducts_ReturnsOkWithEmptyList()
    {
        // Arrange
        _mockService.Setup(s => s.GetAllProductsAsync())
            .ReturnsAsync(new List<VegProductDto>());

        // Act
        var result = await _controller.GetAllProducts();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProducts = okResult.Value.Should().BeAssignableTo<IEnumerable<VegProductDto>>().Subject;
        returnedProducts.Should().BeEmpty();
    }

    #endregion

    #region GetProductById Tests

    [Fact]
    public async Task GetProductById_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var product = new VegProductDto { Id = 1, Name = "Tomato", Price = 5000, StockQuantity = 100 };
        _mockService.Setup(s => s.GetProductByIdAsync(1))
            .ReturnsAsync(product);

        // Act
        var result = await _controller.GetProductById(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProduct = okResult.Value.Should().BeOfType<VegProductDto>().Subject;
        returnedProduct.Id.Should().Be(1);
        returnedProduct.Name.Should().Be("Tomato");
    }

    [Fact]
    public async Task GetProductById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetProductByIdAsync(999))
            .ReturnsAsync((VegProductDto?)null);

        // Act
        var result = await _controller.GetProductById(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region CreateProduct Tests

    [Fact]
    public async Task CreateProduct_WithValidDto_ReturnsOkResult()
    {
        // Arrange
        var createDto = new VegProductCreateUpdateDto
        {
            Name = "Lettuce",
            Price = 2500,
            StockQuantity = 75,
            Description = "Fresh lettuce"
        };

        var createdProduct = new VegProductDto
        {
            Id = 1,
            Name = createDto.Name,
            Price = createDto.Price,
            StockQuantity = createDto.StockQuantity,
            Description = createDto.Description
        };

        _mockService.Setup(s => s.CreateProductAsync(createDto))
            .ReturnsAsync(createdProduct);

        // Act
        var result = await _controller.CreateProduct(createDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProduct = okResult.Value.Should().BeOfType<VegProductDto>().Subject;
        returnedProduct.Name.Should().Be("Lettuce");
    }

    #endregion

    #region UpdateProduct Tests

    [Fact]
    public async Task UpdateProduct_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var updateDto = new VegProductCreateUpdateDto
        {
            Name = "Updated Product",
            Price = 2000,
            StockQuantity = 20
        };

        _mockService.Setup(s => s.UpdateProductAsync(1, updateDto))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateProduct(1, updateDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mockService.Verify(s => s.UpdateProductAsync(1, updateDto), Times.Once);
    }

    [Fact]
    public async Task UpdateProduct_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var updateDto = new VegProductCreateUpdateDto
        {
            Name = "Product",
            Price = 1000,
            StockQuantity = 10
        };

        _mockService.Setup(s => s.UpdateProductAsync(999, updateDto))
            .ThrowsAsync(new KeyNotFoundException());

        // Act
        var result = await _controller.UpdateProduct(999, updateDto);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region DeleteProduct Tests

    [Fact]
    public async Task DeleteProduct_WithValidId_ReturnsNoContent()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteProductAsync(1))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteProduct(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mockService.Verify(s => s.DeleteProductAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteProduct_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteProductAsync(999))
            .ThrowsAsync(new KeyNotFoundException());

        // Act
        var result = await _controller.DeleteProduct(999);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region GetProductsByCategory Tests

    [Fact]
    public async Task GetProductsByCategory_WithValidCategoryId_ReturnsOkResult()
    {
        // Arrange
        var products = new List<VegProductDto>
        {
            new VegProductDto { Id = 1, Name = "Product 1", IdCategory = 1, Price = 1000, StockQuantity = 10 },
            new VegProductDto { Id = 2, Name = "Product 2", IdCategory = 1, Price = 2000, StockQuantity = 20 }
        };

        _mockService.Setup(s => s.GetProductsByCategoryAsync(1))
            .ReturnsAsync(products);

        // Act
        var result = await _controller.GetProductsByCategory(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProducts = okResult.Value.Should().BeAssignableTo<IEnumerable<VegProductDto>>().Subject;
        returnedProducts.Should().HaveCount(2);
        returnedProducts.Should().OnlyContain(p => p.IdCategory == 1);
    }

    #endregion
}
