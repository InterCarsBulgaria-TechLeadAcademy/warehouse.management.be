using WarehouseManagement.Core.DTOs.Auth;

namespace WarehouseManagement.Core.Contracts;

public interface IAuthService
{
    string GenerateJwtToken(string userId, string username, string email);

    Task<string> RegisterAsync(RegisterDto registerDto);

    Task<string> LoginAsync(LoginDto loginDto);
}
