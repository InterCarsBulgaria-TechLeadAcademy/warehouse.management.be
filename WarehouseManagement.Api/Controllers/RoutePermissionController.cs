using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs.RoutePermission;
using static WarehouseManagement.Common.MessageConstants.Keys.RoutePermissionMessageKeys;

namespace WarehouseManagement.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoutePermissionController : ControllerBase
{
    private readonly IRoutePermissionService routePermissionService;

    public RoutePermissionController(IRoutePermissionService routePermissionService)
    {
        this.routePermissionService = routePermissionService;
    }

    [HttpGet("all")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<RoutePermissionDto>))]
    public async Task<IActionResult> GetAll()
    {
        var permissions = await routePermissionService.AllAsync();

        return Ok(permissions);
    }

    [HttpGet("all-with-deleted")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<RoutePermissionDto>))]
    public async Task<IActionResult> GetAllWithDeleted()
    {
        var permissions = await routePermissionService.AllWithDeletedAsync();

        return Ok(permissions);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(200, Type = typeof(RoutePermissionDto))]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(string id)
    {
        var permission = await routePermissionService.GetByIdAsync(id);

        return Ok(permission);
    }

    [HttpPost("add")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Add([FromBody] RoutePermissionFormDto model)
    {
        await routePermissionService.AddAsync(model);

        return Ok(RoutePermissionAddedSuccessfully);
    }

    [HttpPut("edit/{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Edit(string id, [FromBody] RoutePermissionFormDto model)
    {
        await routePermissionService.EditAsync(id, model);

        return Ok(RoutePermissionAddedSuccessfully);
    }

    [HttpDelete("delete/{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(string id)
    {
        await routePermissionService.DeleteAsync(id);

        return Ok(RoutePermissionAddedSuccessfully);
    }
}
