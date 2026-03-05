using DotNetCoreWebApi.Application.Entities;
using DotNetCoreWebApi.Application.Interfaces;
using DotNetCoreWebApi.Application.Services;
using DotNetCoreWebApi.Application.DTOs;
using DotNetCoreWebApi.Infrastructure.Repositories;
using DotNetCoreWebApi.Tests.Helpers;
using FluentAssertions;
using Moq;
using Xunit;

namespace DotNetCoreWebApi.Tests.Unit.Services;

/// <summary>
/// Unit tests for ProductImageService
/// </summary>
public class ProductImageServiceTests
{
    private readonly Mock<IProductImageRepository> _mockImageRepository;
    private readonly Mock<IRepository<VegProducts>> _mockProductRepository;
    private readonly Mock<IFileUploadService> _mockFileService;
    private readonly ProductImageService _service;

    public ProductImageServiceTests()
    {
        _mockImageRepository = new Mock<IProductImageRepository>();
        _mockProductRepository = new Mock<IRepository<VegProducts>>();
        _mockFileService = new Mock<IFileUploadService>();
        _service = new ProductImageService(
            _mockImageRepository.Object,
            _mockProductRepository.Object,
            _mockFileService.Object);
    }

    #region GetImagesByProductIdAsync Tests

    [Fact]
    public async Task GetImagesByProductIdAsync_ReturnsAllImagesForProduct()
    {
        // Arrange
        var images = MockDataGenerator.GenerateProductImages(3, 1);
        _mockImageRepository.Setup(r => r.GetImagesByProductIdAsync(1))
            .ReturnsAsync(images);

        // Act
        var result = await _service.GetImagesByProductIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        _mockImageRepository.Verify(r => r.GetImagesByProductIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetImagesByProductIdAsync_WhenNoImages_ReturnsEmptyList()
    {
        // Arrange
        _mockImageRepository.Setup(r => r.GetImagesByProductIdAsync(1))
            .ReturnsAsync(new List<ProductImage>());

        // Act
        var result = await _service.GetImagesByProductIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetImagesByProductIdAsync_MapsPropertiesToDto()
    {
        // Arrange
        var image = MockDataGenerator.GenerateProductImage(1, 1, "images/test.jpg", ProductImageType.Main, 0);
        _mockImageRepository.Setup(r => r.GetImagesByProductIdAsync(1))
            .ReturnsAsync(new List<ProductImage> { image });

        // Act
        var result = await _service.GetImagesByProductIdAsync(1);
        var dto = result.First();

        // Assert
        dto.Id.Should().Be(1);
        dto.IdProduct.Should().Be(1);
        dto.ImageUrl.Should().Be("images/test.jpg");
        dto.ImageType.Should().Be((ProductImageTypeDto)ProductImageType.Main);
        dto.DisplayOrder.Should().Be(0);
    }

    #endregion

    #region GetImageByIdAsync Tests

    [Fact]
    public async Task GetImageByIdAsync_WithValidId_ReturnsImage()
    {
        // Arrange
        var image = MockDataGenerator.GenerateProductImage(1, 1, "images/test.jpg");
        _mockImageRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(image);

        // Act
        var result = await _service.GetImageByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        _mockImageRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetImageByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        _mockImageRepository.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((ProductImage?)null);

        // Act
        var result = await _service.GetImageByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetMainImageAsync Tests

    [Fact]
    public async Task GetMainImageAsync_ReturnsMainImage()
    {
        // Arrange
        var image = MockDataGenerator.GenerateProductImage(1, 1, "images/main.jpg", ProductImageType.Main);
        _mockImageRepository.Setup(r => r.GetMainImageByProductIdAsync(1))
            .ReturnsAsync(image);

        // Act
        var result = await _service.GetMainImageAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.ImageType.Should().Be((ProductImageTypeDto)ProductImageType.Main);
        _mockImageRepository.Verify(r => r.GetMainImageByProductIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetMainImageAsync_WhenNoMainImage_ReturnsNull()
    {
        // Arrange
        _mockImageRepository.Setup(r => r.GetMainImageByProductIdAsync(1))
            .ReturnsAsync((ProductImage?)null);

        // Act
        var result = await _service.GetMainImageAsync(1);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetMobileImageAsync Tests

    [Fact]
    public async Task GetMobileImageAsync_ReturnsMobileImage()
    {
        // Arrange
        var image = MockDataGenerator.GenerateProductImage(1, 1, "images/mobile.jpg", ProductImageType.Mobile);
        _mockImageRepository.Setup(r => r.GetMobileImageByProductIdAsync(1))
            .ReturnsAsync(image);

        // Act
        var result = await _service.GetMobileImageAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.ImageType.Should().Be((ProductImageTypeDto)ProductImageType.Mobile);
    }

    #endregion

    #region GetGalleryImagesAsync Tests

    [Fact]
    public async Task GetGalleryImagesAsync_ReturnsGalleryImages()
    {
        // Arrange
        var images = new List<ProductImage>
        {
            MockDataGenerator.GenerateProductImage(1, 1, "images/gallery1.jpg", ProductImageType.Gallery, 2),
            MockDataGenerator.GenerateProductImage(2, 1, "images/gallery2.jpg", ProductImageType.Gallery, 3)
        };
        _mockImageRepository.Setup(r => r.GetGalleryImagesByProductIdAsync(1))
            .ReturnsAsync(images);

        // Act
        var result = await _service.GetGalleryImagesAsync(1);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(i => i.ImageType == (ProductImageTypeDto)ProductImageType.Gallery);
    }

    #endregion

    #region CreateImageAsync Tests

    [Fact]
    public async Task CreateImageAsync_CreatesImage()
    {
        // Arrange
        var product = MockDataGenerator.GenerateProduct(1, "Test Product", 1000, 10);
        var createDto = new ProductImageCreateUpdateDto
        {
            ImageUrl = "images/new.jpg",
            ImageType = ProductImageTypeDto.Main,
            DisplayOrder = 0,
            Width = 1000,
            Height = 800
        };
        var createdImage = MockDataGenerator.GenerateProductImage(1, 1, "images/new.jpg");

        _mockProductRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(product);
        _mockImageRepository.Setup(r => r.GetImageCountByProductIdAsync(1))
            .ReturnsAsync(0);
        _mockImageRepository.Setup(r => r.GetMainImageByProductIdAsync(1))
            .ReturnsAsync((ProductImage?)null);
        _mockImageRepository.Setup(r => r.AddAsync(It.IsAny<ProductImage>()))
            .ReturnsAsync(createdImage);

        // Act
        var result = await _service.CreateImageAsync(1, createDto);

        // Assert
        result.Should().NotBeNull();
        result.ImageUrl.Should().Be("images/new.jpg");
        _mockImageRepository.Verify(r => r.AddAsync(It.IsAny<ProductImage>()), Times.Once);
    }

    [Fact]
    public async Task CreateImageAsync_WithNonExistentProduct_ThrowsKeyNotFoundException()
    {
        // Arrange
        var createDto = new ProductImageCreateUpdateDto
        {
            ImageUrl = "images/test.jpg",
            ImageType = ProductImageTypeDto.Main
        };
        _mockProductRepository.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((VegProducts?)null);

        // Act
        var act = async () => await _service.CreateImageAsync(999, createDto);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*Product with ID 999 not found*");
    }

    [Fact]
    public async Task CreateImageAsync_WhenMaxImagesReached_ThrowsInvalidOperationException()
    {
        // Arrange
        var product = MockDataGenerator.GenerateProduct(1, "Test Product", 1000, 10);
        var createDto = new ProductImageCreateUpdateDto
        {
            ImageUrl = "images/test.jpg",
            ImageType = ProductImageTypeDto.Gallery
        };

        _mockProductRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(product);
        _mockImageRepository.Setup(r => r.GetImageCountByProductIdAsync(1))
            .ReturnsAsync(10);

        // Act
        var act = async () => await _service.CreateImageAsync(1, createDto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Maximum 10 images per product allowed*");
    }

    #endregion

    #region UpdateImageAsync Tests

    [Fact]
    public async Task UpdateImageAsync_UpdatesExistingImage()
    {
        // Arrange
        var existingImage = MockDataGenerator.GenerateProductImage(1, 1, "images/old.jpg");
        var updateDto = new ProductImageCreateUpdateDto
        {
            ImageUrl = "images/updated.jpg",
            ImageType = ProductImageTypeDto.Main,
            DisplayOrder = 0,
            Width = 1200,
            Height = 900
        };

        _mockImageRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(existingImage);
        _mockImageRepository.Setup(r => r.GetMainImageByProductIdAsync(1))
            .ReturnsAsync((ProductImage?)null);
        _mockImageRepository.Setup(r => r.UpdateAsync(It.IsAny<ProductImage>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdateImageAsync(1, updateDto);

        // Assert
        result.Should().NotBeNull();
        result.ImageUrl.Should().Be("images/updated.jpg");
        _mockImageRepository.Verify(r => r.UpdateAsync(It.IsAny<ProductImage>()), Times.Once);
    }

    [Fact]
    public async Task UpdateImageAsync_WithNonExistentId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var updateDto = new ProductImageCreateUpdateDto
        {
            ImageUrl = "images/test.jpg",
            ImageType = ProductImageTypeDto.Main
        };
        _mockImageRepository.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((ProductImage?)null);

        // Act
        var act = async () => await _service.UpdateImageAsync(999, updateDto);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    #endregion

    #region DeleteImageAsync Tests

    [Fact]
    public async Task DeleteImageAsync_DeletesExistingImage()
    {
        // Arrange
        var image = MockDataGenerator.GenerateProductImage(1, 1, "images/test.jpg");
        _mockImageRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(image);
        _mockImageRepository.Setup(r => r.DeleteAsync(image))
            .Returns(Task.CompletedTask);
        _mockFileService.Setup(f => f.DeleteImageAsync("images/test.jpg"))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteImageAsync(1);

        // Assert
        _mockImageRepository.Verify(r => r.DeleteAsync(image), Times.Once);
        _mockFileService.Verify(f => f.DeleteImageAsync("images/test.jpg"), Times.Once);
    }

    [Fact]
    public async Task DeleteImageAsync_WithNonExistentId_ThrowsKeyNotFoundException()
    {
        // Arrange
        _mockImageRepository.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((ProductImage?)null);

        // Act
        var act = async () => await _service.DeleteImageAsync(999);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    #endregion

    #region DeleteProductImagesAsync Tests

    [Fact]
    public async Task DeleteProductImagesAsync_DeletesAllImagesForProduct()
    {
        // Arrange
        var product = MockDataGenerator.GenerateProduct(1, "Test Product", 1000, 10);
        var images = MockDataGenerator.GenerateProductImages(3, 1);
        
        _mockProductRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(product);
        _mockImageRepository.Setup(r => r.GetImagesByProductIdAsync(1))
            .ReturnsAsync(images);
        _mockImageRepository.Setup(r => r.DeleteProductImagesAsync(1))
            .Returns(Task.CompletedTask);
        _mockFileService.Setup(f => f.DeleteImageAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteProductImagesAsync(1);

        // Assert
        _mockImageRepository.Verify(r => r.DeleteProductImagesAsync(1), Times.Once);
        _mockFileService.Verify(f => f.DeleteImageAsync(It.IsAny<string>()), Times.Exactly(3));
    }

    [Fact]
    public async Task DeleteProductImagesAsync_WithNonExistentProduct_ThrowsKeyNotFoundException()
    {
        // Arrange
        _mockProductRepository.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((VegProducts?)null);

        // Act
        var act = async () => await _service.DeleteProductImagesAsync(999);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    #endregion

    #region GetImageCountAsync Tests

    [Fact]
    public async Task GetImageCountAsync_ReturnsCorrectCount()
    {
        // Arrange
        var product = MockDataGenerator.GenerateProduct(1, "Test Product", 1000, 10);
        _mockProductRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(product);
        _mockImageRepository.Setup(r => r.GetImageCountByProductIdAsync(1))
            .ReturnsAsync(5);

        // Act
        var result = await _service.GetImageCountAsync(1);

        // Assert
        result.Should().Be(5);
    }

    [Fact]
    public async Task GetImageCountAsync_WithNonExistentProduct_ThrowsKeyNotFoundException()
    {
        // Arrange
        _mockProductRepository.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((VegProducts?)null);

        // Act
        var act = async () => await _service.GetImageCountAsync(999);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    #endregion
}
