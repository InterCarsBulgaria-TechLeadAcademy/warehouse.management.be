using WarehouseManagement.Infrastructure.Data.Models;

namespace WarehouseManagement.Core.Contracts;

public interface IJwtService
{
    Task<string> ComposeAccessToken(string userId, string username, string email);

    Task<string> GenerateRefreshToken(string userId);

    Task<string> GenerateAccessTokenFromRefreshToken(string refreshToken);

    Task RevokeOldRefreshTokens(ApplicationUser user);
}
