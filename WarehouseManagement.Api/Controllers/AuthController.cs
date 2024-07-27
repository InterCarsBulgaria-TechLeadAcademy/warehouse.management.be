using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs.Auth;

namespace WarehouseManagement.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService authService;

    public AuthController(IAuthService authService)
    {
        this.authService = authService;
    }

    [HttpPost("login")]
    [ProducesResponseType(200, Type = typeof(string))]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Login([FromBody] LoginDto logindDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        string jwtToken = await authService.LoginAsync(logindDto);

        return Ok(jwtToken);
    }

    [HttpPost("register")]
    [ProducesResponseType(200, Type = typeof(string))]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        string jwtToken = await authService.RegisterAsync(registerDto);

        return Ok(jwtToken);
    }
}
