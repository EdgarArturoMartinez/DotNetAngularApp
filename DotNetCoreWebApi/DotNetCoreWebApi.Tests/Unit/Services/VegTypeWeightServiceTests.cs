using DotNetCoreWebApi.Application.Entities;
using DotNetCoreWebApi.Application.Interfaces;
using DotNetCoreWebApi.Application.Services;
using DotNetCoreWebApi.Application.DTOs;
using DotNetCoreWebApi.Tests.Helpers;
using FluentAssertions;
using Moq;
using Xunit;

namespace DotNetCoreWebApi.Tests.Unit.Services;

/// <summary>
/// Unit tests for VegTypeWeightService
/// </summary>
public class VegTypeWeightServiceTests
{
    private readonly Mock<IVegTypeWeightRepository> _mockRepository;
    private readonly VegTypeWeightService _service;

    public VegTypeWeightServiceTests()
    {
        _mockRepository = new Mock<IVegTypeWeightRepository>();
        _service = new VegTypeWeightService(_mockRepository.Object);
    }

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ReturnsAllTypeWeights()
    {
        // Arrange
        var typeWeights = MockDataGenerator.GenerateVegTypeWeights(5);
        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(typeWeights);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(5);
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoTypeWeights_ReturnsEmptyList()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<VegTypeWeight>());

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_MapsPropertiesToDto()
    {
        // Arrange
        var typeWeight = MockDataGenerator.GenerateVegTypeWeight(1, "Kilogram", "Kg", "Weight in kilograms", true);
        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<VegTypeWeight> { typeWeight });

        // Act
        var result = await _service.GetAllAsync();
        var dto = result.First();

        // Assert
        dto.IdTypeWeight.Should().Be(1);
        dto.Name.Should().Be("Kilogram");
        dto.AbbreviationWeight.Should().Be("Kg");
        dto.Description.Should().Be("Weight in kilograms");
        dto.IsActive.Should().BeTrue();
    }

    #endregion

    #region GetActiveTypesAsync Tests

    [Fact]
    public async Task GetActiveTypesAsync_ReturnsOnlyActiveTypes()
    {
        // Arrange
        var typeWeights = new List<VegTypeWeight>
        {
            MockDataGenerator.GenerateVegTypeWeight(1, "Kilogram", "Kg", isActive: true),
            MockDataGenerator.GenerateVegTypeWeight(2, "Gram", "g", isActive: true)
        };
        _mockRepository.Setup(r => r.GetActiveTypesAsync())
            .ReturnsAsync(typeWeights);

        // Act
        var result = await _service.GetActiveTypesAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(t => t.IdTypeWeight == 1 || t.IdTypeWeight == 2);
    }

    [Fact]
    public async Task GetActiveTypesAsync_ReturnsBasicDtoWithRequiredFieldsOnly()
    {
        // Arrange
        var typeWeight = MockDataGenerator.GenerateVegTypeWeight(1, "Kilogram", "Kg", "Description", true);
        _mockRepository.Setup(r => r.GetActiveTypesAsync())
            .ReturnsAsync(new List<VegTypeWeight> { typeWeight });

        // Act
        var result = await _service.GetActiveTypesAsync();
        var dto = result.First();

        // Assert
        dto.IdTypeWeight.Should().Be(1);
        dto.Name.Should().Be("Kilogram");
        dto.AbbreviationWeight.Should().Be("Kg");
        // BasicDto should not have Description or IsActive
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsTypeWeight()
    {
        // Arrange
        var typeWeight = MockDataGenerator.GenerateVegTypeWeight(1, "Kilogram", "Kg");
        _mockRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(typeWeight);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.IdTypeWeight.Should().Be(1);
        result.Name.Should().Be("Kilogram");
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((VegTypeWeight?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_CreatesTypeWeight()
    {
        // Arrange
        var createDto = new VegTypeWeightCreateUpdateDto
        {
            Name = "Kilogram",
            AbbreviationWeight = "Kg",
            Description = "Weight in kilograms",
            IsActive = true
        };

        var createdTypeWeight = MockDataGenerator.GenerateVegTypeWeight(1, "Kilogram", "Kg", "Weight in kilograms", true);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<VegTypeWeight>()))
            .ReturnsAsync(createdTypeWeight);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Kilogram");
        result.AbbreviationWeight.Should().Be("Kg");
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<VegTypeWeight>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_MapsDtoPropertiesToEntity()
    {
        // Arrange
        var createDto = new VegTypeWeightCreateUpdateDto
        {
            Name = "Gram",
            AbbreviationWeight = "g",
            Description = "Weight in grams",
            IsActive = false
        };

        VegTypeWeight? capturedEntity = null;
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<VegTypeWeight>()))
            .Callback<VegTypeWeight>(e => capturedEntity = e)
            .ReturnsAsync((VegTypeWeight e) => e);

        // Act
        await _service.CreateAsync(createDto);

        // Assert
        capturedEntity.Should().NotBeNull();
        capturedEntity!.Name.Should().Be("Gram");
        capturedEntity.AbbreviationWeight.Should().Be("g");
        capturedEntity.Description.Should().Be("Weight in grams");
        capturedEntity.IsActive.Should().BeFalse();
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_UpdatesExistingTypeWeight()
    {
        // Arrange
        var existingTypeWeight = MockDataGenerator.GenerateVegTypeWeight(1, "Kilogram", "Kg");
        var updateDto = new VegTypeWeightCreateUpdateDto
        {
            Name = "Kilogram Updated",
            AbbreviationWeight = "KG",
            Description = "Updated description",
            IsActive = true
        };

        _mockRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(existingTypeWeight);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<VegTypeWeight>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdateAsync(1, updateDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Kilogram Updated");
        result.AbbreviationWeight.Should().Be("KG");
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<VegTypeWeight>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var updateDto = new VegTypeWeightCreateUpdateDto
        {
            Name = "Test",
            AbbreviationWeight = "T"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((VegTypeWeight?)null);

        // Act
        var act = async () => await _service.UpdateAsync(999, updateDto);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_DeletesExistingTypeWeight()
    {
        // Arrange
        var typeWeight = MockDataGenerator.GenerateVegTypeWeight(1, "Kilogram", "Kg");
        _mockRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(typeWeight);
        _mockRepository.Setup(r => r.DeleteAsync(typeWeight))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(typeWeight), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentId_ThrowsKeyNotFoundException()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((VegTypeWeight?)null);

        // Act
        var act = async () => await _service.DeleteAsync(999);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    #endregion
}
