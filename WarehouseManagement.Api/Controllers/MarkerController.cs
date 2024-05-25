using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs.Marker;

namespace WarehouseManagement.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MarkerController : ControllerBase
{
    private readonly IMarkerService markerService;

    public MarkerController(IMarkerService markerService)
    {
        this.markerService = markerService;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(200, Type = typeof(MarkerDto))]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetMarker(int id)
    {
        var model = await markerService.GetByIdAsync(id);
        if (model == null)
        {
            return NotFound();
        }

        return Ok(model);
    }

    [HttpGet("all")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<MarkerDto>))]
    public async Task<IActionResult> GetAll()
    {
        var model = await markerService.GetAllAsync();

        return Ok(model);
    }

    [HttpPost("add")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Add([FromBody] MarkerFormDto marker)
    {
        if (await markerService.ExistByNameAsync(marker.Name) == true)
        {
            return BadRequest("A marker with the same name already exists.");
        }

        var userId = User.Id();

        await markerService.AddAsync(marker, userId);

        return Ok("Marker added successfully");
    }

    [HttpPut("edit/{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Edit(int id, [FromBody] MarkerFormDto marker)
    {
        if (await markerService.GetByIdAsync(id) == null)
        {
            return NotFound();
        }

        if (await markerService.ExistByNameAsync(marker.Name))
        {
            return BadRequest("A marker with the same name already exists.");
        }

        var userId = User.Id();

        await markerService.EditAsync(id, marker, userId);

        return Ok("Marker updated successfully");
    }

    [HttpDelete("delete/{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        if (await markerService.GetByIdAsync(id) == null)
        {
            return NotFound();
        }

        var usages = await markerService.GetMarkerUsagesAsync(id);
        if (usages.Any())
        {
            return BadRequest(
                $"Cannot delete this marker because it is used in: {string.Join(", ", usages)}"
            );
        }

        var userId = User.Id();

        await markerService.DeleteAsync(id, userId);

        return Ok(id);
    }

    [HttpPut("restore/{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Restore(int id)
    {
        if (await markerService.IsDeletedByIdAsync(id) == false)
        {
            return BadRequest("The marker is not deleted.");
        }

        var deletedMarkerName = await markerService.GetDeletedMarkerNameByIdAsync(id);
        if (string.IsNullOrEmpty(deletedMarkerName))
        {
            return BadRequest("Marker not found.");
        }

        if (await markerService.ExistByNameAsync(deletedMarkerName))
        {
            return BadRequest($"A marker with the name '{deletedMarkerName}' already exists.");
        }

        var userId = User.Id();

        await markerService.RestoreAsync(id, userId);

        return Ok("Marker restored successfully");
    }

    [HttpGet("deleted")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<MarkerDto>))]
    public async Task<IActionResult> GetDeletedMarkers()
    {
        var model = await markerService.GetDeletedMarkersAsync();
        return Ok(model);
    }
}
