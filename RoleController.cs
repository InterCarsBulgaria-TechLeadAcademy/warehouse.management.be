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
        await roleService.AssignRoleByIdToUserAsync(model.Id, model.UserId);

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

    [HttpPut("edit/{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Edit(string id, [FromBody] RoleFormDto model)
    {
        await roleService.EditAsync(id, model);

        return Ok(RoleEditedSuccessfully);
    }

    [HttpDelete("delete/{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Delete(string id)
    {
        await roleService.DeleteAsync(id);

        return Ok(RoleDeletedSuccessfully);
    }

    [HttpGet("all")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<RoleDetailsDto>))]
    public async Task<IActionResult> GetAll()
    {
        var models = await roleService.AllAsync();

        return Ok(models);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(200, Type = typeof(RoleDto))]
    public async Task<IActionResult> GetById(string id)
    {
        var model = await roleService.GetByIdAsync(id);

        return Ok(model);
    }
}
