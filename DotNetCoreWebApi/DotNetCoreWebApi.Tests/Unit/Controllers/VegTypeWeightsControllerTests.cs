using DotNetCoreWebApi.Application.Interfaces;
using DotNetCoreWebApi.Controllers;
using DotNetCoreWebApi.DTOs;
using DotNetCoreWebApi.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DotNetCoreWebApi.Tests.Unit.Controllers;

/// <summary>
/// Unit tests for VegTypeWeightsController
/// </summary>
public class VegTypeWeightsControllerTests
{
    private readonly Mock<IVegTypeWeightService> _mockService;
    private readonly Mock<ILogger<VegTypeWeightsController>> _mockLogger;
    private readonly VegTypeWeightsController _controller;

    public VegTypeWeightsControllerTests()
    {
        _mockService = new Mock<IVegTypeWeightService>();
        _mockLogger = new Mock<ILogger<VegTypeWeightsController>>();
        _controller = new VegTypeWeightsController(_mockService.Object, _mockLogger.Object);
    }

    #region GetAllVegTypeWeights Tests

    [Fact]
    public async Task GetAllVegTypeWeights_ReturnsOkWithTypeWeights()
    {
        // Arrange
        var typeWeights = new List<VegTypeWeightDto>
        {
            new() { IdTypeWeight = 1, Name = "Kilogram", AbbreviationWeight = "Kg", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { IdTypeWeight = 2, Name = "Gram", AbbreviationWeight = "g", IsActive = true, CreatedAt = DateTime.UtcNow }
        };
        _mockService.Setup(s => s.GetAllAsync())
            .ReturnsAsync(typeWeights);

        // Act
        var result = await _controller.GetAllVegTypeWeights();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedTypeWeights = okResult.Value.Should().BeAssignableTo<IEnumerable<VegTypeWeightDto>>().Subject;
        returnedTypeWeights.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllVegTypeWeights_ReturnsEmptyList_WhenNoTypeWeights()
    {
        // Arrange
        _mockService.Setup(s => s.GetAllAsync())
            .ReturnsAsync(new List<VegTypeWeightDto>());

        // Act
        var result = await _controller.GetAllVegTypeWeights();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedTypeWeights = okResult.Value.Should().BeAssignableTo<IEnumerable<VegTypeWeightDto>>().Subject;
        returnedTypeWeights.Should().BeEmpty();
    }

    #endregion

    #region GetActiveTypes Tests

    [Fact]
    public async Task GetActiveTypes_ReturnsOkWithActiveTypes()
    {
        // Arrange
        var activeTypes = new List<VegTypeWeightBasicDto>
        {
            new() { IdTypeWeight = 1, Name = "Kilogram", AbbreviationWeight = "Kg" },
            new() { IdTypeWeight = 2, Name = "Gram", AbbreviationWeight = "g" }
        };
        _mockService.Setup(s => s.GetActiveTypesAsync())
            .ReturnsAsync(activeTypes);

        // Act
        var result = await _controller.GetActiveTypes();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedTypes = okResult.Value.Should().BeAssignableTo<IEnumerable<VegTypeWeightBasicDto>>().Subject;
        returnedTypes.Should().HaveCount(2);
    }

    #endregion

    #region GetVegTypeWeight Tests

    [Fact]
    public async Task GetVegTypeWeight_WithValidId_ReturnsOkWithTypeWeight()
    {
        // Arrange
        var typeWeight = new VegTypeWeightDto 
        { 
            IdTypeWeight = 1, 
            Name = "Kilogram", 
            AbbreviationWeight = "Kg", 
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _mockService.Setup(s => s.GetByIdAsync(1))
            .ReturnsAsync(typeWeight);

        // Act
        var result = await _controller.GetVegTypeWeight(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedTypeWeight = okResult.Value.Should().BeAssignableTo<VegTypeWeightDto>().Subject;
        returnedTypeWeight.IdTypeWeight.Should().Be(1);
        returnedTypeWeight.Name.Should().Be("Kilogram");
    }

    [Fact]
    public async Task GetVegTypeWeight_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetByIdAsync(999))
            .ReturnsAsync((VegTypeWeightDto?)null);

        // Act
        var result = await _controller.GetVegTypeWeight(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    #endregion

    #region CreateVegTypeWeight Tests

    [Fact]
    public async Task CreateVegTypeWeight_WithValidDto_ReturnsCreatedAtAction()
    {
        // Arrange
        var createDto = new VegTypeWeightCreateUpdateDto
        {
            Name = "Kilogram",
            AbbreviationWeight = "Kg",
            Description = "Weight in kilograms",
            IsActive = true
        };
        var createdTypeWeight = new VegTypeWeightDto 
        { 
            IdTypeWeight = 1, 
            Name = "Kilogram", 
            AbbreviationWeight = "Kg",
            Description = "Weight in kilograms",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _mockService.Setup(s => s.CreateAsync(createDto))
            .ReturnsAsync(createdTypeWeight);

        // Act
        var result = await _controller.CreateVegTypeWeight(createDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(_controller.GetVegTypeWeight));
        createdResult.RouteValues!["id"].Should().Be(1);
        var returnedTypeWeight = createdResult.Value.Should().BeAssignableTo<VegTypeWeightDto>().Subject;
        returnedTypeWeight.Name.Should().Be("Kilogram");
    }

    #endregion

    #region UpdateVegTypeWeight Tests

    [Fact]
    public async Task UpdateVegTypeWeight_WithValidId_ReturnsOkWithUpdatedTypeWeight()
    {
        // Arrange
        var updateDto = new VegTypeWeightCreateUpdateDto
        {
            Name = "Kilogram Updated",
            AbbreviationWeight = "KG",
            IsActive = true
        };
        var updatedTypeWeight = new VegTypeWeightDto 
        { 
            IdTypeWeight = 1, 
            Name = "Kilogram Updated", 
            AbbreviationWeight = "KG",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _mockService.Setup(s => s.UpdateAsync(1, updateDto))
            .ReturnsAsync(updatedTypeWeight);

        // Act
        var result = await _controller.UpdateVegTypeWeight(1, updateDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedTypeWeight = okResult.Value.Should().BeAssignableTo<VegTypeWeightDto>().Subject;
        returnedTypeWeight.Name.Should().Be("Kilogram Updated");
    }

    [Fact]
    public async Task UpdateVegTypeWeight_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var updateDto = new VegTypeWeightCreateUpdateDto
        {
            Name = "Test",
            AbbreviationWeight = "T"
        };
        _mockService.Setup(s => s.UpdateAsync(999, updateDto))
            .ThrowsAsync(new KeyNotFoundException());

        // Act
        var result = await _controller.UpdateVegTypeWeight(999, updateDto);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    #endregion

    #region DeleteVegTypeWeight Tests

    [Fact]
    public async Task DeleteVegTypeWeight_WithValidId_ReturnsNoContent()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteAsync(1))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteVegTypeWeight(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteVegTypeWeight_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteAsync(999))
            .ThrowsAsync(new KeyNotFoundException());

        // Act
        var result = await _controller.DeleteVegTypeWeight(999);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    #endregion
}
