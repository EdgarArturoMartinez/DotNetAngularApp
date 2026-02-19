namespace DotNetCoreWebApi.DTOs;

public class VegProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public int StockQuantity { get; set; }
    public decimal? NetWeight { get; set; }

    public int? IdCategory { get; set; }
    public VegCategoryBasicDto? VegCategory { get; set; }

    public int? IdTypeWeight { get; set; }
    public VegTypeWeightBasicDto? VegTypeWeight { get; set; }
}

public class VegCategoryBasicDto
{
    public int IdCategory { get; set; }
    public string CategoryName { get; set; } = null!;
    public string? Description { get; set; }
}
