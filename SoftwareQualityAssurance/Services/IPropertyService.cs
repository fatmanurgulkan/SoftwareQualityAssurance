using SoftwareQualityAssurance.DTOs;

namespace SoftwareQualityAssurance.Services;

public interface IPropertyService
{
    Task<IEnumerable<PropertyDto>> GetAllPropertiesAsync();
    Task<PropertyDto?> GetPropertyByIdAsync(int id);
    Task<PropertyDto> CreatePropertyAsync(CreatePropertyDto createDto);
    Task<PropertyDto?> UpdatePropertyAsync(int id, UpdatePropertyDto updateDto);
    Task<bool> DeletePropertyAsync(int id);
}

