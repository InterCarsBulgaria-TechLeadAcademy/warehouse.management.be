using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs.Auth;
using WarehouseManagement.Infrastructure.Data.Common;
using WarehouseManagement.Infrastructure.Data.Models;
using static WarehouseManagement.Common.MessageConstants.Keys.RoleMessageKeys;

public class AuthService : IAuthService
{
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly RoleManager<ApplicationRole> roleManager;

    private readonly IRepository repository;
    private readonly IJwtService jwtService;

    public AuthService(
        IRepository repository, 
        SignInManager<ApplicationUser> signInManager, 
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IJwtService jwtService)
    {
        this.signInManager = signInManager;
        this.userManager = userManager;
        this.roleManager = roleManager;

        this.repository = repository;
        this.jwtService = jwtService;
    }

    public async Task<string> RegisterAsync(RegisterDto registerDto)
    {
        var roleExists = await roleManager.RoleExistsAsync(registerDto.RoleName);

        if (!roleExists)
        {
            throw new ArgumentException(RoleWithThisNameDoesNotExist);
        }

        var user = new ApplicationUser
        {
            UserName = registerDto.Username,
            Email = registerDto.Email,
        };

        var result = await userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        return user.Id.ToString();
    }

    public async Task<string> LoginAsync(LoginDto loginDto)
    {
        var user = await repository
            .All<ApplicationUser>()
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.UserName == loginDto.Username);

        if (user == null)
        {
            throw new ArgumentException("Email or password is incorrect.");
        }

        var result = await signInManager.PasswordSignInAsync(user.UserName!, loginDto.Password, false, false);

        if (!result.Succeeded)
        {
            throw new Exception("Invalid login attempt.");
        }

        await jwtService.RevokeOldRefreshTokens(user);
        return await jwtService.ComposeAccessToken(user!.Id.ToString(), user.UserName!, user.Email!);
    }

    public async Task LogoutAsync(string userId)
    {
        var refreshToken = await repository
            .All<RefreshToken>()
            .FirstAsync(t => t.UserId.ToString() == userId);

        refreshToken.IsRevoked = true;

        await signInManager.SignOutAsync();
    }
}