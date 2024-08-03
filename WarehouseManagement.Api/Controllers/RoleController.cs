using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Core.Contracts;

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

    public async Task<IActionResult> Create()
    {
        throw new NotImplementedException();
    }
}
