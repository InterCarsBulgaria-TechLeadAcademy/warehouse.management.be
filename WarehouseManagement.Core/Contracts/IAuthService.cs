using WarehouseManagement.Core.DTOs.Auth;

namespace WarehouseManagement.Core.Contracts;

public interface IAuthService
{
    Task RegisterAsync(RegisterDto registerDto);

    Task<string> LoginAsync(LoginDto loginDto);

    Task LogoutAsync(string userId);
}
