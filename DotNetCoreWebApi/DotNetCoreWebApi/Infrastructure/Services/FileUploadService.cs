using DotNetCoreWebApi.Application.Interfaces;

namespace DotNetCoreWebApi.Infrastructure.Services;

public class FileUploadService : IFileUploadService
{
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly ILogger<FileUploadService> _logger;

    // Allowed image extensions
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    
    // Maximum file sizes (in bytes)
    private readonly Dictionary<int, long> _maxFileSizes = new()
    {
        { 0, 5 * 1024 * 1024 },        // Main: 5MB
        { 1, 4 * 1024 * 1024 },        // Mobile: 4MB
        { 2, 4 * 1024 * 1024 }         // Gallery: 4MB
    };

    public FileUploadService(IWebHostEnvironment hostEnvironment, ILogger<FileUploadService> logger)
    {
        _hostEnvironment = hostEnvironment;
        _logger = logger;
        EnsureImageDirectoryExists();
    }

    public async Task<string> SaveProductImageAsync(IFormFile file, int productId, int imageType)
    {
        // Validate file
        ValidateFile(file, imageType);

        // Generate filename using naming strategy: product_{productId}_{imageType}_{timestamp}_{random}.{ext}
        string filename = GenerateFileName(file, productId, imageType);
        
        // Get the full path
        string imagesPath = Path.Combine(_hostEnvironment.WebRootPath, "images");
        string fullPath = Path.Combine(imagesPath, filename);

        try
        {
            // Delete old file if exists (for update scenarios)
            if (File.Exists(fullPath))
                File.Delete(fullPath);

            // Save the new file
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _logger.LogInformation($"Image uploaded successfully: {filename} for product {productId}");

            // Return relative path for database storage
            return $"images/{filename}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error saving image file: {filename}");
            throw new InvalidOperationException("Failed to save image file", ex);
        }
    }

    public async Task DeleteImageAsync(string relativePath)
    {
        try
        {
            if (string.IsNullOrEmpty(relativePath))
                return;

            string fullPath = Path.Combine(_hostEnvironment.WebRootPath, relativePath);

            // Security: Ensure path is within images directory
            string imagesPath = Path.Combine(_hostEnvironment.WebRootPath, "images");
            if (!Path.GetFullPath(fullPath).StartsWith(Path.GetFullPath(imagesPath)))
            {
                _logger.LogWarning($"Attempted to delete file outside images directory: {fullPath}");
                throw new InvalidOperationException("Invalid file path");
            }

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                _logger.LogInformation($"Image deleted: {relativePath}");
            }

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting image: {relativePath}");
            throw;
        }
    }

    public bool FileExists(string relativePath)
    {
        if (string.IsNullOrEmpty(relativePath))
            return false;

        string fullPath = Path.Combine(_hostEnvironment.WebRootPath, relativePath);
        return File.Exists(fullPath);
    }

    /// <summary>
    /// Generate filename using strategy: product_{productId}_{imageType}_{timestamp}_{random}.{ext}
    /// Example: product_8_main_20260218_a1b2c3d4.jpg
    /// </summary>
    private string GenerateFileName(IFormFile file, int productId, int imageType)
    {
        // Get file extension
        string extension = Path.GetExtension(file.FileName).ToLower();

        // Image type names
        string[] imageTypeNames = { "main", "mobile", "gallery" };
        string imageTypeName = imageTypeNames[imageType];

        // Timestamp (yyyyMMdd format)
        string timestamp = DateTime.UtcNow.ToString("yyyyMMdd");

        // Random suffix (8 characters) to prevent collisions
        string randomSuffix = GenerateRandomString(8);

        return $"product_{productId}_{imageTypeName}_{timestamp}_{randomSuffix}{extension}";
    }

    private string GenerateRandomString(int length)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        var result = new System.Text.StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            result.Append(chars[random.Next(chars.Length)]);
        }

        return result.ToString();
    }

    private void ValidateFile(IFormFile file, int imageType)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty");

        // Check file extension
        string extension = Path.GetExtension(file.FileName).ToLower();
        if (!_allowedExtensions.Contains(extension))
            throw new ArgumentException($"File type {extension} is not allowed. Allowed types: {string.Join(", ", _allowedExtensions)}");

        // Check file size
        if (!_maxFileSizes.ContainsKey(imageType))
            throw new ArgumentException($"Invalid image type: {imageType}");

        long maxSize = _maxFileSizes[imageType];
        if (file.Length > maxSize)
            throw new ArgumentException($"File size exceeds maximum allowed size of {maxSize / (1024 * 1024)}MB");

        // Check MIME type
        if (!IsValidImageMimeType(file.ContentType))
            throw new ArgumentException($"Invalid file MIME type: {file.ContentType}");
    }

    private bool IsValidImageMimeType(string contentType)
    {
        var validMimeTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
        return validMimeTypes.Contains(contentType?.ToLower());
    }

    private void EnsureImageDirectoryExists()
    {
        string imagesPath = Path.Combine(_hostEnvironment.WebRootPath, "images");
        if (!Directory.Exists(imagesPath))
        {
            Directory.CreateDirectory(imagesPath);
            _logger.LogInformation($"Created images directory: {imagesPath}");
        }
    }
}
