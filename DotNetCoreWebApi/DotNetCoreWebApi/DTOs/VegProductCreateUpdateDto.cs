namespace DotNetCoreWebApi.DTOs;

/// <summary>
/// DTO for creating or updating a VegProduct
/// </summary>
public class VegProductCreateUpdateDto
{
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public int StockQuantity { get; set; }
    public int? IdCategory { get; set; }
}
