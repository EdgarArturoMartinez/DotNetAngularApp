using DotNetCoreWebApi.Application.Interfaces;
using DotNetCoreWebApi.Application.Services;
using DotNetCoreWebApi.Controllers;
using DotNetCoreWebApi.Application.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DotNetCoreWebApi.Tests.Unit.Controllers;

/// <summary>
/// Unit tests for ProductImagesController
/// </summary>
public class ProductImagesControllerTests
{
    private readonly Mock<IProductImageService> _mockService;
    private readonly Mock<IFileUploadService> _mockFileService;
    private readonly Mock<ILogger<ProductImagesController>> _mockLogger;
    private readonly ProductImagesController _controller;

    public ProductImagesControllerTests()
    {
        _mockService = new Mock<IProductImageService>();
        _mockFileService = new Mock<IFileUploadService>();
        _mockLogger = new Mock<ILogger<ProductImagesController>>();
        _controller = new ProductImagesController(_mockService.Object, _mockFileService.Object, _mockLogger.Object);
    }

    #region GetProductImages Tests

    [Fact]
    public async Task GetProductImages_ReturnsOkWithImages()
    {
        // Arrange
        var images = new List<ProductImageDto>
        {
            new() { Id = 1, IdProduct = 1, ImageUrl = "images/1.jpg", ImageType = ProductImageTypeDto.Main },
            new() { Id = 2, IdProduct = 1, ImageUrl = "images/2.jpg", ImageType = ProductImageTypeDto.Gallery }
        };
        _mockService.Setup(s => s.GetImagesByProductIdAsync(1))
            .ReturnsAsync(images);

        // Act
        var result = await _controller.GetProductImages(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedImages = okResult.Value.Should().BeAssignableTo<IEnumerable<ProductImageDto>>().Subject;
        returnedImages.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetProductImages_WhenNoImages_ReturnsOkWithEmptyList()
    {
        // Arrange
        _mockService.Setup(s => s.GetImagesByProductIdAsync(1))
            .ReturnsAsync(new List<ProductImageDto>());

        // Act
        var result = await _controller.GetProductImages(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedImages = okResult.Value.Should().BeAssignableTo<IEnumerable<ProductImageDto>>().Subject;
        returnedImages.Should().BeEmpty();
    }

    [Fact]
    public async Task GetProductImages_WhenProductNotFound_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetImagesByProductIdAsync(999))
            .ThrowsAsync(new KeyNotFoundException("Product not found"));

        // Act
        var result = await _controller.GetProductImages(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetProductImages_WhenServiceThrowsException_Returns500()
    {
        // Arrange
        _mockService.Setup(s => s.GetImagesByProductIdAsync(1))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetProductImages(1);

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }

    #endregion

    #region GetMainImage Tests

    [Fact]
    public async Task GetMainImage_ReturnsOkWithMainImage()
    {
        // Arrange
        var mainImage = new ProductImageDto 
        { 
            Id = 1, 
            IdProduct = 1, 
            ImageUrl = "images/main.jpg", 
            ImageType = ProductImageTypeDto.Main 
        };
        _mockService.Setup(s => s.GetMainImageAsync(1))
            .ReturnsAsync(mainImage);

        // Act
        var result = await _controller.GetMainImage(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedImage = okResult.Value.Should().BeAssignableTo<ProductImageDto>().Subject;
        returnedImage.ImageType.Should().Be(ProductImageTypeDto.Main);
    }

    [Fact]
    public async Task GetMainImage_WhenNoMainImage_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetMainImageAsync(1))
            .ReturnsAsync((ProductImageDto?)null);

        // Act
        var result = await _controller.GetMainImage(1);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region GetMobileImage Tests

    [Fact]
    public async Task GetMobileImage_ReturnsOkWithMobileImage()
    {
        // Arrange
        var mobileImage = new ProductImageDto 
        { 
            Id = 1, 
            IdProduct = 1, 
            ImageUrl = "images/mobile.jpg", 
            ImageType = ProductImageTypeDto.Mobile 
        };
        _mockService.Setup(s => s.GetMobileImageAsync(1))
            .ReturnsAsync(mobileImage);

        // Act
        var result = await _controller.GetMobileImage(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedImage = okResult.Value.Should().BeAssignableTo<ProductImageDto>().Subject;
        returnedImage.ImageType.Should().Be(ProductImageTypeDto.Mobile);
    }

    #endregion

    #region GetGalleryImages Tests

    [Fact]
    public async Task GetGalleryImages_ReturnsOkWithGalleryImages()
    {
        // Arrange
        var galleryImages = new List<ProductImageDto>
        {
            new() { Id = 1, IdProduct = 1, ImageUrl = "images/gallery1.jpg", ImageType = ProductImageTypeDto.Gallery },
            new() { Id = 2, IdProduct = 1, ImageUrl = "images/gallery2.jpg", ImageType = ProductImageTypeDto.Gallery }
        };
        _mockService.Setup(s => s.GetGalleryImagesAsync(1))
            .ReturnsAsync(galleryImages);

        // Act
        var result = await _controller.GetGalleryImages(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedImages = okResult.Value.Should().BeAssignableTo<IEnumerable<ProductImageDto>>().Subject;
        returnedImages.Should().HaveCount(2);
        returnedImages.Should().OnlyContain(i => i.ImageType == ProductImageTypeDto.Gallery);
    }

    #endregion

    #region GetImageById Tests

    [Fact]
    public async Task GetImageById_WithValidId_ReturnsOkWithImage()
    {
        // Arrange
        var image = new ProductImageDto 
        { 
            Id = 1, 
            IdProduct = 1, 
            ImageUrl = "images/test.jpg", 
            ImageType = ProductImageTypeDto.Main 
        };
        _mockService.Setup(s => s.GetImageByIdAsync(1))
            .ReturnsAsync(image);

        // Act
        var result = await _controller.GetImageById(1, 1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedImage = okResult.Value.Should().BeAssignableTo<ProductImageDto>().Subject;
        returnedImage.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetImageById_WhenImageNotFound_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetImageByIdAsync(999))
            .ReturnsAsync((ProductImageDto?)null);

        // Act
        var result = await _controller.GetImageById(1, 999);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetImageById_WhenImageBelongsToDifferentProduct_ReturnsNotFound()
    {
        // Arrange
        var image = new ProductImageDto 
        { 
            Id = 1, 
            IdProduct = 2, 
            ImageUrl = "images/test.jpg", 
            ImageType = ProductImageTypeDto.Main 
        };
        _mockService.Setup(s => s.GetImageByIdAsync(1))
            .ReturnsAsync(image);

        // Act
        var result = await _controller.GetImageById(1, 1); // Product 1, but image belongs to product 2

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region CreateImage Tests

    [Fact]
    public async Task CreateImage_WithValidDto_ReturnsCreatedAtAction()
    {
        // Arrange
        var createDto = new ProductImageCreateUpdateDto
        {
            ImageUrl = "images/new.jpg",
            ImageType = ProductImageTypeDto.Main,
            DisplayOrder = 0
        };
        var createdImage = new ProductImageDto 
        { 
            Id = 1, 
            IdProduct = 1, 
            ImageUrl = "images/new.jpg", 
            ImageType = ProductImageTypeDto.Main 
        };
        _mockService.Setup(s => s.CreateImageAsync(1, createDto))
            .ReturnsAsync(createdImage);

        // Act
        var result = await _controller.CreateImage(1, createDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(_controller.GetImageById));
        var returnedImage = createdResult.Value.Should().BeAssignableTo<ProductImageDto>().Subject;
        returnedImage.ImageUrl.Should().Be("images/new.jpg");
    }

    [Fact]
    public async Task CreateImage_WithInvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new ProductImageCreateUpdateDto
        {
            ImageUrl = "images/test.jpg",
            ImageType = ProductImageTypeDto.Main
        };
        _controller.ModelState.AddModelError("ImageUrl", "Required");

        // Act
        var result = await _controller.CreateImage(1, createDto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task CreateImage_WhenProductNotFound_ReturnsNotFound()
    {
        // Arrange
        var createDto = new ProductImageCreateUpdateDto
        {
            ImageUrl = "images/test.jpg",
            ImageType = ProductImageTypeDto.Main
        };
        _mockService.Setup(s => s.CreateImageAsync(999, createDto))
            .ThrowsAsync(new KeyNotFoundException("Product not found"));

        // Act
        var result = await _controller.CreateImage(999, createDto);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task CreateImage_WhenMaxImagesReached_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new ProductImageCreateUpdateDto
        {
            ImageUrl = "images/test.jpg",
            ImageType = ProductImageTypeDto.Gallery
        };
        _mockService.Setup(s => s.CreateImageAsync(1, createDto))
            .ThrowsAsync(new InvalidOperationException("Maximum 10 images per product allowed"));

        // Act
        var result = await _controller.CreateImage(1, createDto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion

    #region UpdateImage Tests

    [Fact]
    public async Task UpdateImage_WithValidDto_ReturnsOkWithUpdatedImage()
    {
        // Arrange
        var updateDto = new ProductImageCreateUpdateDto
        {
            ImageUrl = "images/updated.jpg",
            ImageType = ProductImageTypeDto.Main,
            DisplayOrder = 0
        };
        var updatedImage = new ProductImageDto 
        { 
            Id = 1, 
            IdProduct = 1, 
            ImageUrl = "images/updated.jpg", 
            ImageType = ProductImageTypeDto.Main 
        };
        _mockService.Setup(s => s.UpdateImageAsync(1, updateDto))
            .ReturnsAsync(updatedImage);

        // Act
        var result = await _controller.UpdateImage(1, 1, updateDto);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedImage = okResult.Value.Should().BeAssignableTo<ProductImageDto>().Subject;
        returnedImage.ImageUrl.Should().Be("images/updated.jpg");
    }

    [Fact]
    public async Task UpdateImage_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var updateDto = new ProductImageCreateUpdateDto
        {
            ImageUrl = "images/test.jpg",
            ImageType = ProductImageTypeDto.Main
        };
        _mockService.Setup(s => s.UpdateImageAsync(999, updateDto))
            .ThrowsAsync(new KeyNotFoundException("Image not found"));

        // Act
        var result = await _controller.UpdateImage(1, 999, updateDto);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region DeleteImage Tests

    [Fact]
    public async Task DeleteImage_WithValidId_ReturnsNoContent()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteImageAsync(1))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteImage(1, 1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteImage_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteImageAsync(999))
            .ThrowsAsync(new KeyNotFoundException("Image not found"));

        // Act
        var result = await _controller.DeleteImage(1, 999);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region DeleteAllProductImages Tests

    [Fact]
    public async Task DeleteAllProductImages_WithValidProductId_ReturnsNoContent()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteProductImagesAsync(1))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteAllProductImages(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteAllProductImages_WithInvalidProductId_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteProductImagesAsync(999))
            .ThrowsAsync(new KeyNotFoundException("Product not found"));

        // Act
        var result = await _controller.DeleteAllProductImages(999);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion
}
