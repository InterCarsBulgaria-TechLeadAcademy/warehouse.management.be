using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs;
using WarehouseManagement.Core.DTOs.Marker;
using static WarehouseManagement.Common.MessageConstants.Keys.MarkerMessageKeys;

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

        return Ok(model);
    }

    [HttpGet("all")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<MarkerDto>))]
    public async Task<IActionResult> GetAll()
    {
        var model = await markerService.GetAllAsync();

        return Ok(model);
    }

    [HttpGet("all-with-params")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<MarkerDto>))]
    public async Task<IActionResult> GetAllWithParams([FromQuery] PaginationParameters paginationParams)
    {
        var model = await markerService.GetAllAsync(paginationParams);

        return Ok(model);
    }

    [HttpPost("add")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Add([FromBody] MarkerFormDto marker)
    {
        var markerId = await markerService.AddAsync(marker, User.Id());

        return Ok(markerId);
    }

    [HttpPut("edit/{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Edit(int id, [FromBody] MarkerFormDto marker)
    {
        await markerService.EditAsync(id, marker, User.Id());

        return Ok(MarkerEditedSuccessfully);
    }

    [HttpDelete("delete/{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        await markerService.DeleteAsync(id);

        return Ok(MarkerDeletedSuccessfully);
    }

    [HttpPut("restore/{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Restore(int id)
    {
        await markerService.RestoreAsync(id, User.Id());

        return Ok(MarkerRestored);
    }

    [HttpGet("deleted")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<MarkerDto>))]
    public async Task<IActionResult> GetDeletedMarkers()
    {
        var model = await markerService.GetDeletedMarkersAsync();
        return Ok(model);
    }
}
