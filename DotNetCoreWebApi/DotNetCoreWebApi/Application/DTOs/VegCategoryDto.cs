namespace DotNetCoreWebApi.Application.DTOs;

public class VegCategoryDto
{
    public int IdCategory { get; set; }
    public string CategoryName { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ProductCount { get; set; }
}
