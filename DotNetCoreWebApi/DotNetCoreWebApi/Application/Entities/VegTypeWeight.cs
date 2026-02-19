using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetCoreWebApi.Application.Entities;

/// <summary>
/// Entity for weight/measure types (Grams, Ounces, Liters, etc.)
/// </summary>
public class VegTypeWeight
{
    [Column("IdTypeWeight")]
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string AbbreviationWeight { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation property for one-to-many relationship with VegProducts
    public virtual ICollection<VegProducts> VegProducts { get; set; } = new List<VegProducts>();
}
