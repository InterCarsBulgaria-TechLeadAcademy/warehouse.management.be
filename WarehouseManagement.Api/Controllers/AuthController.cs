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

    public AuthController(IAuthService authService, IJwtService jwtService)
    {
        this.authService = authService;
        this.jwtService = jwtService;
    }

    [HttpPost("login")]
    [ProducesResponseType(200, Type = typeof(TokenResponse))]
    [ProducesResponseType(500)]
    public async Task<IActionResult> SignIn([FromBody] LoginDto logindDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        string jwtToken = await authService.LoginAsync(logindDto);
        string refreshToken = await jwtService.GenerateRefreshToken(User.Id());

        return Ok(new TokenResponse() { AccessToken = jwtToken, RefreshToken = refreshToken });
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

        await authService.RegisterAsync(registerDto);

        return Ok(UserRegisteredSuccessfully);
    }

    [HttpPost("refresh")]
    [ProducesResponseType(200, Type = typeof(TokenResponse))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Refresh([FromBody] string refreshToken)
    {
        var newAccessToken = await jwtService.GenerateAccessTokenFromRefreshToken(refreshToken);

        var response = new TokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = refreshToken
        };

        return Ok(response);
    }

    [HttpPost("/logout")]
    public async Task<IActionResult> Logout()
    {
        await authService.LogoutAsync(User.Id());

        return Ok(UserSignedOutSuccessfully);
    }
}
