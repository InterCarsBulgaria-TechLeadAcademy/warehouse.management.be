using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs.User;
using static WarehouseManagement.Common.MessageConstants.Keys.ApplicationUserMessageKeys;

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
    [ProducesResponseType(200, Type = typeof(UserAllDto))]
    public async Task<IActionResult> GetAll()
    {
        var users = await userService.GetAllAsync();

        return Ok(users);
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(200, Type = typeof(UserDto))]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetUserInfo()
    {
        var userId = User
            .Claims
            .First(c => c.Type == ClaimTypes.NameIdentifier)
            .Value;

        var userInfo = await userService.GetUserInfoAsync(userId);

        return Ok(userInfo);
    }

    [HttpPatch("delete/{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await userService.DeleteAsync(id);

        return Ok(UserDeletedSuccessfully);
    }
}
