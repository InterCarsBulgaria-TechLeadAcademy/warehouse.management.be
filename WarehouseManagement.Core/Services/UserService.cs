using Microsoft.AspNetCore.Identity;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs.User;
using WarehouseManagement.Infrastructure.Data.Common;
using WarehouseManagement.Infrastructure.Data.Models;
using static WarehouseManagement.Common.MessageConstants.Keys.ApplicationUserMessageKeys;

namespace WarehouseManagement.Core.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IRepository repository;

    public UserService(UserManager<ApplicationUser> roleManager, IRepository repository)
    {
        this.userManager = roleManager;
        this.repository = repository;
    }

    public async Task<UserDto> GetUserInfo(string userId)
    {
        var user = await repository.GetByIdAsync<ApplicationUser>(Guid.Parse(userId));

        if (user == null)
        {
            throw new KeyNotFoundException(UserWithThisIdNotFound);
        }

        var roles = await userManager.GetRolesAsync(user);

        //var routePermissionIds = repository
        //    .All<RoleRoutePermission>()
        //    .Where(rrp => rrp.RoleId == )


        return new UserDto()
        {
            Id = userId,
            UserName = user.UserName!,
            Email = user.Email!,
            Roles = roles
        };
    }
}
