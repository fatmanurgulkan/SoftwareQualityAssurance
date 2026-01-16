namespace SoftwareQualityAssurance.Models;

public class Location : BaseEntity
{
    public string CityName { get; set; } = string.Empty;
    public string PlateCode { get; set; } = string.Empty;

    // Navigation properties
    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();
}

