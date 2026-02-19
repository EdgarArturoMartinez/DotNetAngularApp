namespace DotNetCoreWebApi.DTOs;

/// <summary>
/// DTO for returning TypeWeight data
/// </summary>
public class VegTypeWeightDto
{
    public int IdTypeWeight { get; set; }
    public string Name { get; set; } = null!;
    public string AbbreviationWeight { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO for creating or updating a TypeWeight
/// </summary>
public class VegTypeWeightCreateUpdateDto
{
    public required string Name { get; set; }
    public required string AbbreviationWeight { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Basic DTO for TypeWeight references in other entities
/// </summary>
public class VegTypeWeightBasicDto
{
    public int IdTypeWeight { get; set; }
    public string Name { get; set; } = null!;
    public string AbbreviationWeight { get; set; } = null!;
}
