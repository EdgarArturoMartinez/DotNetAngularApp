using DotNetCoreWebApi.Application.Interfaces;
using DotNetCoreWebApi.Controllers;
using DotNetCoreWebApi.Application.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace DotNetCoreWebApi.Tests.Unit.Controllers;

/// <summary>
/// Unit tests for VegCategoriesController
/// </summary>
public class VegCategoriesControllerTests
{
    private readonly Mock<IVegCategoryService> _mockService;
    private readonly Mock<ILogger<VegCategoriesController>> _mockLogger;
    private readonly VegCategoriesController _controller;

    public VegCategoriesControllerTests()
    {
        _mockService = new Mock<IVegCategoryService>();
        _mockLogger = new Mock<ILogger<VegCategoriesController>>();
        _controller = new VegCategoriesController(_mockService.Object, _mockLogger.Object);
    }

    #region GetCategories Tests

    [Fact]
    public async Task GetCategories_ReturnsOkResultWithCategories()
    {
        // Arrange
        var categories = new List<VegCategoryDto>
        {
            new VegCategoryDto { IdCategory = 1, CategoryName = "Vegetables" },
            new VegCategoryDto { IdCategory = 2, CategoryName = "Fruits" }
        };
        _mockService.Setup(s => s.GetAllCategoriesAsync())
            .ReturnsAsync(categories);

        // Act
        var result = await _controller.GetCategories();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCategories = okResult.Value.Should().BeAssignableTo<IEnumerable<VegCategoryDto>>().Subject;
        returnedCategories.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetCategories_WhenNoCategories_ReturnsOkWithEmptyList()
    {
        // Arrange
        _mockService.Setup(s => s.GetAllCategoriesAsync())
            .ReturnsAsync(new List<VegCategoryDto>());

        // Act
        var result = await _controller.GetCategories();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCategories = okResult.Value.Should().BeAssignableTo<IEnumerable<VegCategoryDto>>().Subject;
        returnedCategories.Should().BeEmpty();
    }

    #endregion

    #region GetCategoryById Tests

    [Fact]
    public async Task GetCategoryById_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var category = new VegCategoryDto { IdCategory = 1, CategoryName = "Vegetables", Description = "Fresh vegetables" };
        _mockService.Setup(s => s.GetCategoryByIdAsync(1))
            .ReturnsAsync(category);

        // Act
        var result = await _controller.GetCategoryById(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCategory = okResult.Value.Should().BeOfType<VegCategoryDto>().Subject;
        returnedCategory.IdCategory.Should().Be(1);
        returnedCategory.CategoryName.Should().Be("Vegetables");
    }

    [Fact]
    public async Task GetCategoryById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetCategoryByIdAsync(999))
            .ReturnsAsync((VegCategoryDto?)null);

        // Act
        var result = await _controller.GetCategoryById(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region CreateCategory Tests

    [Fact]
    public async Task CreateCategory_WithValidDto_ReturnsOkResult()
    {
        // Arrange
        var createDto = new VegCategoryCreateUpdateDto
        {
            CategoryName = "Fruits",
            Description = "Fresh fruits"
        };

        var createdCategory = new VegCategoryDto
        {
            IdCategory = 1,
            CategoryName = createDto.CategoryName,
            Description = createDto.Description
        };

        _mockService.Setup(s => s.CreateCategoryAsync(createDto))
            .ReturnsAsync(createdCategory);

        // Act
        var result = await _controller.CreateCategory(createDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCategory = okResult.Value.Should().BeOfType<VegCategoryDto>().Subject;
        returnedCategory.CategoryName.Should().Be("Fruits");
    }

    #endregion

    #region UpdateCategory Tests

    [Fact]
    public async Task UpdateCategory_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var updateDto = new VegCategoryCreateUpdateDto
        {
            CategoryName = "Updated Category",
            Description = "Updated description"
        };

        _mockService.Setup(s => s.UpdateCategoryAsync(1, updateDto))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateCategory(1, updateDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mockService.Verify(s => s.UpdateCategoryAsync(1, updateDto), Times.Once);
    }

    [Fact]
    public async Task UpdateCategory_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var updateDto = new VegCategoryCreateUpdateDto
        {
            CategoryName = "Category",
            Description = "Description"
        };

        _mockService.Setup(s => s.UpdateCategoryAsync(999, updateDto))
            .ThrowsAsync(new KeyNotFoundException());

        // Act
        var result = await _controller.UpdateCategory(999, updateDto);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region DeleteCategory Tests

    [Fact]
    public async Task DeleteCategory_WithValidId_ReturnsNoContent()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteCategoryAsync(1))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteCategory(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mockService.Verify(s => s.DeleteCategoryAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteCategory_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteCategoryAsync(999))
            .ThrowsAsync(new KeyNotFoundException());

        // Act
        var result = await _controller.DeleteCategory(999);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion
}
