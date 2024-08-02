using WarehouseManagement.Core.DTOs.Auth;

namespace WarehouseManagement.Core.Contracts;

public interface IAuthService
{
    string GenerateJwtToken(string userId, string username, string email);

    Task<string> GenerateRefreshToken(string userId);

    Task<string> GenerateAccessTokenFromRefreshToken(string refreshToken);

    Task RegisterAsync(RegisterDto registerDto);

    Task<string> LoginAsync(LoginDto loginDto);

    Task LogoutAsync(string userId);
}
