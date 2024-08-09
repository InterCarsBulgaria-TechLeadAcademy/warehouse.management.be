using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs.Role;
using static WarehouseManagement.Common.MessageConstants.Keys.RoleMessageKeys;

namespace WarehouseManagement.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoleController : ControllerBase
{
    private IRoleService roleService;

    public RoleController(IRoleService roleService)
    {
        this.roleService = roleService;
    }

    [HttpPost("add-to-role")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> AddUserToRole([FromBody] RoleUserAssignDto model)
    {
        await roleService.AssignRoleToUserAsync(model.RoleName, model.UserId);

        return Ok(RoleAssignedToUserSuccessfully);
    }

    [HttpPost("create")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Create([FromBody] RoleFormDto model)
    {
        await roleService.CreateAsync(model);

        return Ok(RoleCreatedSuccessfully);
    }

    [HttpPut("edit/{roleName}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Edit(string roleName, [FromBody] RoleFormDto model)
    {
        await roleService.EditAsync(roleName, model);

        return Ok(RoleEditedSuccessfully);
    }

    [HttpDelete("delete/{roleName}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Delete(string roleName)
    {
        await roleService.DeleteAsync(roleName);

        return Ok(RoleDeletedSuccessfully);
    }

    [HttpGet("all")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<RoleDto>))]
    public async Task<IActionResult> GetAll()
    {
        var models = await roleService.AllAsync();

        return Ok(models);
    }

    [HttpGet("{roleName}")]
    [ProducesResponseType(200, Type = typeof(RoleDto))]
    public async Task<IActionResult> GetByName(string roleName)
    {
        var model = await roleService.GetByNameAsync(roleName);

        return Ok(model);
    }
}
