using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WarehouseManagement.Core.Contracts;

namespace WarehouseManagement.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService userService;

    public UserController(IUserService userService)
    {
        this.userService = userService;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var users = await userService.GetAllAsync();

        return Ok(users);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetUserInfo()
    {
        var userId = User
            .Claims
            .First(c => c.Type == ClaimTypes.NameIdentifier)
            .Value;

        var userInfo = await userService.GetUserInfoAsync(userId);

        return Ok(userInfo);
    }
}
