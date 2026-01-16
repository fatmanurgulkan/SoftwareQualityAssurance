using Microsoft.EntityFrameworkCore;
using SoftwareQualityAssurance.Data;
using SoftwareQualityAssurance.DTOs;
using SoftwareQualityAssurance.Models;
using SoftwareQualityAssurance.Repositories;

namespace SoftwareQualityAssurance.Services;

public class PropertyService : IPropertyService
{
    private readonly IRepository<Property> _propertyRepository;
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<Location> _locationRepository;
    private readonly ApplicationDbContext _context;

    public PropertyService(
        IRepository<Property> propertyRepository,
        IRepository<Category> categoryRepository,
        IRepository<Location> locationRepository,
        ApplicationDbContext context)
    {
        _propertyRepository = propertyRepository;
        _categoryRepository = categoryRepository;
        _locationRepository = locationRepository;
        _context = context;
    }

    public async Task<IEnumerable<PropertyDto>> GetAllPropertiesAsync()
    {
        var properties = await _context.Properties
            .Include(p => p.Category)
            .Include(p => p.Location)
            .ToListAsync();

        return properties.Select(MapToDto);
    }

    public async Task<PropertyDto?> GetPropertyByIdAsync(int id)
    {
        var property = await _context.Properties
            .Include(p => p.Category)
            .Include(p => p.Location)
            .FirstOrDefaultAsync(p => p.Id == id);

        return property == null ? null : MapToDto(property);
    }

    public async Task<PropertyDto> CreatePropertyAsync(CreatePropertyDto createDto)
    {
        // Validate Category exists
        if (!await _categoryRepository.ExistsAsync(createDto.CategoryId))
        {
            throw new InvalidOperationException($"Category with ID {createDto.CategoryId} does not exist.");
        }

        // Validate Location exists
        if (!await _locationRepository.ExistsAsync(createDto.LocationId))
        {
            throw new InvalidOperationException($"Location with ID {createDto.LocationId} does not exist.");
        }

        var property = new Property
        {
            Title = createDto.Title,
            BlockNumber = createDto.BlockNumber,
            ParcelNumber = createDto.ParcelNumber,
            SquareMeters = createDto.SquareMeters,
            Price = createDto.Price,
            CategoryId = createDto.CategoryId,
            LocationId = createDto.LocationId,
            IsAvailable = createDto.IsAvailable
        };

        var createdProperty = await _propertyRepository.AddAsync(property);
        
        // Reload with includes
        var propertyWithIncludes = await _context.Properties
            .Include(p => p.Category)
            .Include(p => p.Location)
            .FirstOrDefaultAsync(p => p.Id == createdProperty.Id);

        return MapToDto(propertyWithIncludes!);
    }

    public async Task<PropertyDto?> UpdatePropertyAsync(int id, UpdatePropertyDto updateDto)
    {
        var property = await _propertyRepository.GetByIdAsync(id);
        if (property == null)
            return null;

        // Validate Category exists
        if (!await _categoryRepository.ExistsAsync(updateDto.CategoryId))
        {
            throw new InvalidOperationException($"Category with ID {updateDto.CategoryId} does not exist.");
        }

        // Validate Location exists
        if (!await _locationRepository.ExistsAsync(updateDto.LocationId))
        {
            throw new InvalidOperationException($"Location with ID {updateDto.LocationId} does not exist.");
        }

        property.Title = updateDto.Title;
        property.BlockNumber = updateDto.BlockNumber;
        property.ParcelNumber = updateDto.ParcelNumber;
        property.SquareMeters = updateDto.SquareMeters;
        property.Price = updateDto.Price;
        property.CategoryId = updateDto.CategoryId;
        property.LocationId = updateDto.LocationId;
        property.IsAvailable = updateDto.IsAvailable;

        await _propertyRepository.UpdateAsync(property);

        // Reload with includes
        var propertyWithIncludes = await _context.Properties
            .Include(p => p.Category)
            .Include(p => p.Location)
            .FirstOrDefaultAsync(p => p.Id == id);

        return MapToDto(propertyWithIncludes!);
    }

    public async Task<bool> DeletePropertyAsync(int id)
    {
        return await _propertyRepository.DeleteAsync(id);
    }

    private static PropertyDto MapToDto(Property property)
    {
        return new PropertyDto
        {
            Id = property.Id,
            Title = property.Title,
            BlockNumber = property.BlockNumber,
            ParcelNumber = property.ParcelNumber,
            SquareMeters = property.SquareMeters,
            Price = property.Price,
            CategoryId = property.CategoryId,
            CategoryName = property.Category?.Name ?? string.Empty,
            LocationId = property.LocationId,
            LocationCityName = property.Location?.CityName ?? string.Empty,
            IsAvailable = property.IsAvailable,
            CreatedDate = property.CreatedDate
        };
    }
}

