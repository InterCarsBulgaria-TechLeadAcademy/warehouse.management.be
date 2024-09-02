using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs.Auth;
using static WarehouseManagement.Common.MessageConstants.Keys.AuthMassegeKeys;

namespace WarehouseManagement.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService authService;
    private readonly IJwtService jwtService;
    private readonly IRoleService roleService;

    public AuthController(IAuthService authService, IJwtService jwtService, IRoleService roleService)
    {
        this.authService = authService;
        this.jwtService = jwtService;
        this.roleService = roleService;
    }

    [HttpPost("login")]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> SignIn([FromBody] LoginDto logindDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        string jwtToken = await authService.LoginAsync(logindDto);
        string refreshToken = await jwtService.GenerateRefreshToken(User.Id());

        // TODO Fix SameSiteMode to be Strict when UI is deployed
        Response.Cookies.Append("X-Access-Token", jwtToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });
        Response.Cookies.Append("X-Refresh-Token", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });

        return Ok();
    }

    [HttpPost("register")]
    [ProducesResponseType(200, Type = typeof(string))]
    [ProducesResponseType(500)]
    public async Task<IActionResult> SignUp([FromBody] RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var creatorId = User.Id(); // Get the currently logged-in user ID to be representd as the creator of the new user
        var userId = await authService.RegisterAsync(registerDto, creatorId); 
        await roleService.AssignRoleToUserAsync(registerDto.RoleId, userId);

        return Ok(UserRegisteredSuccessfully);
    }

    [HttpPost("refresh")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["X-Refresh-Token"];
        var newAccessToken = await jwtService.GenerateAccessTokenFromRefreshToken(refreshToken);

        Response.Cookies.Append("X-Access-Token", newAccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });
        Response.Cookies.Append("X-Refresh-Token", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });

        return Ok();
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await authService.LogoutAsync(User.Id());

        Response.Cookies.Delete("X-Access-Token");
        Response.Cookies.Delete("X-Refresh-Token");

        return Ok(UserSignedOutSuccessfully);
    }
}
