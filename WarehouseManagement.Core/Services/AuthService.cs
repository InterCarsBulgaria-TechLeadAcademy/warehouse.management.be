using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WarehouseManagement.Core.Contracts;
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
        var key = Encoding.ASCII.GetBytes("yourSecretKey");

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", userId), new Claim("username", username), new Claim("email", email) }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public Task RegisterAsync()
    {
        throw new NotImplementedException();
    }

    public Task LoginAsync()
    {
        throw new NotImplementedException();
    }
}
