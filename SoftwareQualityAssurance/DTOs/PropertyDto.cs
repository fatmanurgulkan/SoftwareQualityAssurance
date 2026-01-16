namespace SoftwareQualityAssurance.DTOs;

public class PropertyDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string BlockNumber { get; set; } = string.Empty;
    public string ParcelNumber { get; set; } = string.Empty;
    public decimal SquareMeters { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int LocationId { get; set; }
    public string LocationCityName { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class CreatePropertyDto
{
    public string Title { get; set; } = string.Empty;
    public string BlockNumber { get; set; } = string.Empty;
    public string ParcelNumber { get; set; } = string.Empty;
    public decimal SquareMeters { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public int LocationId { get; set; }
    public bool IsAvailable { get; set; }
}

public class UpdatePropertyDto
{
    public string Title { get; set; } = string.Empty;
    public string BlockNumber { get; set; } = string.Empty;
    public string ParcelNumber { get; set; } = string.Empty;
    public decimal SquareMeters { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public int LocationId { get; set; }
    public bool IsAvailable { get; set; }
}

