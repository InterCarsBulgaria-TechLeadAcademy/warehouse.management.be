using WarehouseManagement.Core.DTOs.Auth;

namespace WarehouseManagement.Core.Contracts;

public interface IAuthService
{
    Task<string> RegisterAsync(RegisterDto registerDto, string creatorId);

    Task<string> LoginAsync(LoginDto loginDto);

    Task LogoutAsync(string userId);
}
