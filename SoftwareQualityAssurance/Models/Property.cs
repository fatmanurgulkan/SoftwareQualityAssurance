namespace SoftwareQualityAssurance.Models;

public class Property : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string BlockNumber { get; set; } = string.Empty;
    public string ParcelNumber { get; set; } = string.Empty;
    public decimal SquareMeters { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public int LocationId { get; set; }
    public bool IsAvailable { get; set; }

    // Navigation properties
    public virtual Category Category { get; set; } = null!;
    public virtual Location Location { get; set; } = null!;
}

