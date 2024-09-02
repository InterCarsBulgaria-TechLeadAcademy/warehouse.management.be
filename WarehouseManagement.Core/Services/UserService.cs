using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Common.Utilities;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs.User;
using WarehouseManagement.Infrastructure.Data.Common;
using WarehouseManagement.Infrastructure.Data.Models;
using static WarehouseManagement.Common.MessageConstants.Keys.ApplicationUserMessageKeys;

namespace WarehouseManagement.Core.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly RoleManager<ApplicationRole> roleManager;
    private readonly IRepository repository;

    public UserService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IRepository repository)
    {
        this.userManager = userManager;
        this.roleManager = roleManager;
        this.repository = repository;
    }

    public async Task<IEnumerable<UserAllDto>> GetAllAsync()
    {
        var users = await repository
            .AllReadOnly<ApplicationUser>()
            .Include(u => u.Creator)
            .ToListAsync();

        var userDtos = new List<UserAllDto>();

        foreach (var user in users)
        {
            userDtos.Add(new UserAllDto()
            {
                Id = user.Id.ToString(),
                UserName = user.UserName!,
                Email = user.Email!,
                CreatedAt = UtcNowDateTimeStringFormatted.GetUtcNow(user.CreatedAt),
                CreatedBy = user.Creator != null ? user.Creator.UserName! : null,
                Role = await GetFirstRoleForUserAsync(user)
            });
        }

        return userDtos;
    }

    public async Task<UserDto> GetUserInfoAsync(string userId)
    {
        var user = await repository.GetByIdAsync<ApplicationUser>(Guid.Parse(userId));

        if (user == null)
        {
            throw new KeyNotFoundException(UserWithThisIdNotFound);
        }

        var roleNames = await userManager.GetRolesAsync(user);
        var routePermissionNames = new HashSet<string>();

        foreach (var roleName in roleNames)
        {
            var currentRole = await roleManager.FindByNameAsync(roleName);

            var permissionNamesForCurrentRole = await repository
                .All<RoleRoutePermission>()
                .Where(rrp => rrp.RoleId == currentRole!.Id)
                .Select(rrp => $"{rrp.RoutePermission.ControllerName}.{rrp.RoutePermission.ActionName}")
                .ToListAsync();

            foreach (var permissionName in permissionNamesForCurrentRole)
            {
                routePermissionNames.Add(permissionName);
            }
        }

        return new UserDto()
        {
            Id = userId,
            UserName = user.UserName!,
            Email = user.Email!,
            Roles = roleNames,
            Role = roleNames.FirstOrDefault() ?? string.Empty,
            RoutePermissionNames = routePermissionNames
        };
    }

    private async Task<string?> GetFirstRoleForUserAsync(ApplicationUser user)
    {
        if (user == null)
        {
            throw new ArgumentException("No user provided.");
        }

        var roles = await userManager.GetRolesAsync(user);

        return roles.FirstOrDefault();
    }
}
