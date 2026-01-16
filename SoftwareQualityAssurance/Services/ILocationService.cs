using SoftwareQualityAssurance.DTOs;

namespace SoftwareQualityAssurance.Services;

public interface ILocationService
{
    Task<IEnumerable<LocationDto>> GetAllLocationsAsync();
    Task<LocationDto?> GetLocationByIdAsync(int id);
    Task<LocationDto> CreateLocationAsync(CreateLocationDto createDto);
    Task<LocationDto?> UpdateLocationAsync(int id, UpdateLocationDto updateDto);
    Task<bool> DeleteLocationAsync(int id);
}

