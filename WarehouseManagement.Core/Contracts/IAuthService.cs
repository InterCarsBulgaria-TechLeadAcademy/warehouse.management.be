namespace WarehouseManagement.Core.Contracts;

public interface IAuthService
{
    string GenerateJwtToken(string userId, string username, string email);

    Task RegisterAsync();

    Task LoginAsync();
}
