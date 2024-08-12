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
    [ProducesResponseType(200, Type = typeof(Dictionary<string, ICollection<RoutePermissionDto>>))]
    public async Task<IActionResult> GetAll()
    {
        var permissionsByControllerName = await routePermissionService.AllAsync();

        return Ok(permissionsByControllerName);
    }

    [HttpGet("all-with-deleted")]
    [ProducesResponseType(200, Type = typeof(Dictionary<string, ICollection<RoutePermissionDto>>))]
    public async Task<IActionResult> GetAllWithDeleted()
    {
        var permissionsByControllerName = await routePermissionService.AllWithDeletedAsync();

        return Ok(permissionsByControllerName);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(200, Type = typeof(RoutePermissionDto))]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(string id)
    {
        var permission = await routePermissionService.GetByIdAsync(id);

        return Ok(permission);
    }

    [HttpDelete("delete/{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(string id)
    {
        await routePermissionService.DeleteAsync(id);

        return Ok(RoutePermissionDeletedSuccessfully);
    }
}
