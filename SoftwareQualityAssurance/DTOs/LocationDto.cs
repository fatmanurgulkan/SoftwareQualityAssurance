namespace SoftwareQualityAssurance.DTOs;

public class LocationDto
{
    public int Id { get; set; }
    public string CityName { get; set; } = string.Empty;
    public string PlateCode { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}

public class CreateLocationDto
{
    public string CityName { get; set; } = string.Empty;
    public string PlateCode { get; set; } = string.Empty;
}

public class UpdateLocationDto
{
    public string CityName { get; set; } = string.Empty;
    public string PlateCode { get; set; } = string.Empty;
}

