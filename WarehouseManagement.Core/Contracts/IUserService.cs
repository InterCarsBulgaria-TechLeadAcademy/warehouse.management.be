﻿using WarehouseManagement.Core.DTOs.User;

namespace WarehouseManagement.Core.Contracts
{
    public interface IUserService
    {
        Task<UserDto> GetUserInfoAsync(string userId);

        Task<IEnumerable<UserAllDto>> GetAllAsync();

        Task DeleteAsync(string userId);
    }
}
