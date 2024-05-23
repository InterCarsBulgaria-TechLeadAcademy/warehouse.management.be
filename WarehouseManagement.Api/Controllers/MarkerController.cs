using System.Security.Claims;
using Microsoft.AspNetCore.Http;
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

    [HttpGet]
    [Route("{id}")]
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

    [HttpGet]
    [Route("all")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<MarkerDto>))]
    public async Task<IActionResult> GetAll()
    {
        var model = await markerService.GetAllAsync();

        return Ok(model);
    }

    [HttpPost]
    [Route("add")]
    public async Task<IActionResult> Add([FromBody] MarkerFormDto marker)
    {
        if (await markerService.ExistByNameAsync(marker.Name) == true)
        {
            ModelState.AddModelError("Name", "A marker with the same name already exists.");
            return BadRequest(ModelState); //TODO: Ask how to return the error
        }

        var userId = "userId";

        await markerService.AddAsync(marker, userId);

        return Ok("");
    }
}
