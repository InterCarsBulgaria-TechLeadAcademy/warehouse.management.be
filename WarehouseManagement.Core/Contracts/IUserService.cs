using WarehouseManagement.Core.DTOs.User;

namespace WarehouseManagement.Core.Contracts
{
    public interface IUserService
    {
        Task<UserDto> GetUserInfo(string userId);
    }
}
