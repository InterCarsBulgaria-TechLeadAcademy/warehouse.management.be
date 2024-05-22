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
}
