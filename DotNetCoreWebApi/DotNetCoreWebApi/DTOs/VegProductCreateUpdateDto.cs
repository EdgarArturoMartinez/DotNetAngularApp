using System.ComponentModel.DataAnnotations;

namespace DotNetCoreWebApi.DTOs;

/// <summary>
/// DTO for creating or updating a VegProduct
/// </summary>
public class VegProductCreateUpdateDto
{
    public string Name { get; set; } = null!;
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public int StockQuantity { get; set; }
    public int? IdCategory { get; set; }
    [Range(0, double.MaxValue)]
    public decimal? NetWeight { get; set; }
    public int? IdTypeWeight { get; set; }
}
