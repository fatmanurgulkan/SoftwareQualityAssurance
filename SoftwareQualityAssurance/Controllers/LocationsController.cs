using Microsoft.AspNetCore.Mvc;
using SoftwareQualityAssurance.DTOs;
using SoftwareQualityAssurance.Services;

namespace SoftwareQualityAssurance.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly ILocationService _locationService;

    public LocationsController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LocationDto>>> GetAll()
    {
        var locations = await _locationService.GetAllLocationsAsync();
        return Ok(locations);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LocationDto>> GetById(int id)
    {
        var location = await _locationService.GetLocationByIdAsync(id);
        if (location == null)
            return NotFound();

        return Ok(location);
    }

    [HttpPost]
    public async Task<ActionResult<LocationDto>> Create([FromBody] CreateLocationDto createDto)
    {
        var location = await _locationService.CreateLocationAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = location.Id }, location);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<LocationDto>> Update(int id, [FromBody] UpdateLocationDto updateDto)
    {
        var location = await _locationService.UpdateLocationAsync(id, updateDto);
        if (location == null)
            return NotFound();

        return Ok(location);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _locationService.DeleteLocationAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}

