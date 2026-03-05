using DotNetCoreWebApi.Application.Entities;
using DotNetCoreWebApi.Application.Interfaces;
using DotNetCoreWebApi.Application.Services;
using DotNetCoreWebApi.Application.DTOs;
using DotNetCoreWebApi.Tests.Helpers;
using FluentAssertions;
using Moq;

namespace DotNetCoreWebApi.Tests.Unit.Services;

/// <summary>
/// Unit tests for VegCategoryService
/// </summary>
public class VegCategoryServiceTests
{
    private readonly Mock<IVegCategoryRepository> _mockRepository;
    private readonly VegCategoryService _service;

    public VegCategoryServiceTests()
    {
        _mockRepository = new Mock<IVegCategoryRepository>();
        _service = new VegCategoryService(_mockRepository.Object);
    }

    #region GetAllCategoriesAsync Tests

    [Fact]
    public async Task GetAllCategoriesAsync_ReturnsAllCategories()
    {
        // Arrange
        var categories = MockDataGenerator.GenerateCategories(3);
        _mockRepository.Setup(r => r.GetCategoriesWithProductsAsync())
            .ReturnsAsync(categories);

        // Act
        var result = await _service.GetAllCategoriesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        _mockRepository.Verify(r => r.GetCategoriesWithProductsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllCategoriesAsync_WhenNoCategories_ReturnsEmptyList()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetCategoriesWithProductsAsync())
            .ReturnsAsync(new List<VegCategory>());

        // Act
        var result = await _service.GetAllCategoriesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    #endregion

    #region GetCategoryByIdAsync Tests

    [Fact]
    public async Task GetCategoryByIdAsync_WithValidId_ReturnsCategory()
    {
        // Arrange
        var category = MockDataGenerator.GenerateCategory(1, "Vegetables", "Fresh vegetables");
        _mockRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(category);

        // Act
        var result = await _service.GetCategoryByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.IdCategory.Should().Be(1);
        result.CategoryName.Should().Be("Vegetables");
        result.Description.Should().Be("Fresh vegetables");
    }

    [Fact]
    public async Task GetCategoryByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((VegCategory?)null);

        // Act
        var result = await _service.GetCategoryByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region CreateCategoryAsync Tests

    [Fact]
    public async Task CreateCategoryAsync_WithValidDto_CreatesCategory()
    {
        // Arrange
        var createDto = new VegCategoryCreateUpdateDto
        {
            CategoryName = "Fruits",
            Description = "Fresh fruits"
        };

        var createdCategory = new VegCategory
        {
            IdCategory = 1,
            CategoryName = createDto.CategoryName,
            Description = createDto.Description
        };

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<VegCategory>()))
            .ReturnsAsync(createdCategory);

        // Act
        var result = await _service.CreateCategoryAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.IdCategory.Should().Be(1);
        result.CategoryName.Should().Be("Fruits");
        result.Description.Should().Be("Fresh fruits");
        _mockRepository.Verify(r => r.AddAsync(It.Is<VegCategory>(
            c => c.CategoryName == createDto.CategoryName &&
                 c.Description == createDto.Description
        )), Times.Once);
    }

    #endregion

    #region UpdateCategoryAsync Tests

    [Fact]
    public async Task UpdateCategoryAsync_WithValidId_UpdatesCategory()
    {
        // Arrange
        var existingCategory = MockDataGenerator.GenerateCategory(1, "Old Name", "Old Description");
        var updateDto = new VegCategoryCreateUpdateDto
        {
            CategoryName = "New Name",
            Description = "New Description"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(existingCategory);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<VegCategory>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateCategoryAsync(1, updateDto);

        // Assert
        existingCategory.CategoryName.Should().Be("New Name");
        existingCategory.Description.Should().Be("New Description");
        _mockRepository.Verify(r => r.UpdateAsync(existingCategory), Times.Once);
    }

    [Fact]
    public async Task UpdateCategoryAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((VegCategory?)null);

        var updateDto = new VegCategoryCreateUpdateDto
        {
            CategoryName = "Name",
            Description = "Description"
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.UpdateCategoryAsync(999, updateDto)
        );
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<VegCategory>()), Times.Never);
    }

    #endregion

    #region DeleteCategoryAsync Tests

    [Fact]
    public async Task DeleteCategoryAsync_WithValidId_DeletesCategory()
    {
        // Arrange
        var category = MockDataGenerator.GenerateCategory(1, "Category to Delete");
        _mockRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(category);
        _mockRepository.Setup(r => r.DeleteAsync(category))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteCategoryAsync(1);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(category), Times.Once);
    }

    [Fact]
    public async Task DeleteCategoryAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((VegCategory?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.DeleteCategoryAsync(999)
        );
        _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<VegCategory>()), Times.Never);
    }

    #endregion
}
