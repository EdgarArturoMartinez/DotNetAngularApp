namespace DotNetCoreWebApi.Application.Entities;

public class VegCategory
{
    public int IdCategory { get; set; }
    public required string CategoryName { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    //Navigation property for one-to-many relationship with VegProducts
    public virtual ICollection<VegProducts> VegProducts { get; set; } = new List<VegProducts>();
}
