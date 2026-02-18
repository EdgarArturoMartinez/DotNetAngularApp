namespace DotNetCoreWebApi.DTOs;

/// <summary>
/// DTO for creating or updating a VegCategory
/// </summary>
public class VegCategoryCreateUpdateDto
{
    public string CategoryName { get; set; } = null!;
    public string? Description { get; set; }
}
