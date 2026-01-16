using Microsoft.AspNetCore.Mvc;
using SoftwareQualityAssurance.DTOs;
using SoftwareQualityAssurance.Services;

namespace SoftwareQualityAssurance.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertiesController : ControllerBase
{
    private readonly IPropertyService _propertyService;

    public PropertiesController(IPropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PropertyDto>>> GetAll()
    {
        var properties = await _propertyService.GetAllPropertiesAsync();
        return Ok(properties);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PropertyDto>> GetById(int id)
    {
        var property = await _propertyService.GetPropertyByIdAsync(id);
        if (property == null)
            return NotFound();

        return Ok(property);
    }

    [HttpPost]
    public async Task<ActionResult<PropertyDto>> Create([FromBody] CreatePropertyDto createDto)
    {
        try
        {
            var property = await _propertyService.CreatePropertyAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = property.Id }, property);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PropertyDto>> Update(int id, [FromBody] UpdatePropertyDto updateDto)
    {
        try
        {
            var property = await _propertyService.UpdatePropertyAsync(id, updateDto);
            if (property == null)
                return NotFound();

            return Ok(property);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _propertyService.DeletePropertyAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}

