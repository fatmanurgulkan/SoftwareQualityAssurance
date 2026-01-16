using SoftwareQualityAssurance.DTOs;
using SoftwareQualityAssurance.Models;
using SoftwareQualityAssurance.Repositories;

namespace SoftwareQualityAssurance.Services;

public class LocationService : ILocationService
{
    private readonly IRepository<Location> _locationRepository;

    public LocationService(IRepository<Location> locationRepository)
    {
        _locationRepository = locationRepository;
    }

    public async Task<IEnumerable<LocationDto>> GetAllLocationsAsync()
    {
        var locations = await _locationRepository.GetAllAsync();
        return locations.Select(MapToDto);
    }

    public async Task<LocationDto?> GetLocationByIdAsync(int id)
    {
        var location = await _locationRepository.GetByIdAsync(id);
        return location == null ? null : MapToDto(location);
    }

    public async Task<LocationDto> CreateLocationAsync(CreateLocationDto createDto)
    {
        var location = new Location
        {
            CityName = createDto.CityName,
            PlateCode = createDto.PlateCode
        };

        var createdLocation = await _locationRepository.AddAsync(location);
        return MapToDto(createdLocation);
    }

    public async Task<LocationDto?> UpdateLocationAsync(int id, UpdateLocationDto updateDto)
    {
        var location = await _locationRepository.GetByIdAsync(id);
        if (location == null)
            return null;

        location.CityName = updateDto.CityName;
        location.PlateCode = updateDto.PlateCode;

        var updatedLocation = await _locationRepository.UpdateAsync(location);
        return MapToDto(updatedLocation);
    }

    public async Task<bool> DeleteLocationAsync(int id)
    {
        return await _locationRepository.DeleteAsync(id);
    }

    private static LocationDto MapToDto(Location location)
    {
        return new LocationDto
        {
            Id = location.Id,
            CityName = location.CityName,
            PlateCode = location.PlateCode,
            CreatedDate = location.CreatedDate
        };
    }
}

