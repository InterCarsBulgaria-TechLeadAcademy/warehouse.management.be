using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs.Auth;
using WarehouseManagement.Infrastructure.Data.Common;
using WarehouseManagement.Infrastructure.Data.Models;

namespace WarehouseManagement.Core.Services;

public class AuthService : IAuthService
{
    private readonly IRepository repository;

    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly UserManager<ApplicationUser> userManager;

    public AuthService(
        IRepository repository, 
        SignInManager<ApplicationUser> signInManager, 
        UserManager<ApplicationUser> userManager)
    {
        this.repository = repository;
        this.signInManager = signInManager;
        this.userManager = userManager;
    }

    public string GenerateJwtToken(string userId, string username, string email)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("u2vXj3t8P9pL4cQ7rS6sW1e0bG5zK7aD2nS3yU8oJ9lF6dA9gH");

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", userId), new Claim("username", username), new Claim("email", email) }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<string> RegisterAsync(RegisterDto registerDto)
    {
        var user = new ApplicationUser
        {
            UserName = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = HashPassword(registerDto.Password)
        };

        var result = await userManager.CreateAsync(user);

        if (!result.Succeeded)
        {
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        return GenerateJwtToken(user.Id.ToString(), user.UserName, user.Email);
    }

    public async Task<string> LoginAsync(LoginDto loginDto)
    {
        var result = await signInManager.PasswordSignInAsync(loginDto.Username, loginDto.Password, false, lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            throw new Exception("Invalid login attempt.");
        }

        var user = await userManager.FindByNameAsync(loginDto.Username);

        return GenerateJwtToken(user!.Id.ToString(), user.UserName!, user.Email!);
    }

    private string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(128 / 8);

        var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password,
            salt,
            KeyDerivationPrf.HMACSHA256,
            100000,
            256 / 8));

        return hashed;
    }
}
