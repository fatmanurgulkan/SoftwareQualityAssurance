namespace SoftwareQualityAssurance.Models;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Navigation properties
    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();
}

