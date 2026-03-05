namespace DotNetCoreWebApi.Application.Interfaces;

/// <summary>
/// Service for handling file uploads to the wwwroot/images directory
/// </summary>
public interface IFileUploadService
{
    /// <summary>
    /// Save an uploaded image file with automatic naming strategy
    /// </summary>
    /// <param name="file">The uploaded file</param>
    /// <param name="productId">Product ID for naming</param>
    /// <param name="imageType">Image type (0=Main, 1=Mobile, 2=Gallery)</param>
    /// <returns>Relative path to the saved file (e.g., images/product_8_main_20260218_a1b2c3d4.jpg)</returns>
    Task<string> SaveProductImageAsync(IFormFile file, int productId, int imageType);

    /// <summary>
    /// Delete an image file if it exists
    /// </summary>
    Task DeleteImageAsync(string relativePath);

    /// <summary>
    /// Check if a file exists
    /// </summary>
    bool FileExists(string relativePath);
}
