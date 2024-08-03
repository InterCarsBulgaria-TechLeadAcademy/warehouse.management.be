using Microsoft.AspNetCore.Identity;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Infrastructure.Data.Common;
using WarehouseManagement.Infrastructure.Data.Models;
using static WarehouseManagement.Common.MessageConstants.Keys.RoleMessageKeys;

namespace WarehouseManagement.Core.Services;

public class RoleService : IRoleService
{
    private readonly RoleManager<ApplicationRole> roleManager;
    private readonly IRepository repository;

    public RoleService(RoleManager<ApplicationRole> roleManager, IRepository repository)
    {
        this.roleManager = roleManager;
        this.repository = repository;
    }

    public Task AssignRoleToUserAsync(string roleName, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> CheckRoleAccessAsync(string roleName, string action, string controller)
    {
        var role = await roleManager.FindByNameAsync(roleName);

        if (role == null)
        {
            throw new ArgumentException(RoleWithThisNameDoesNotExist);
        }

        return role.RoleRoutePermissions
            .Any(rp => rp.RoutePermission.ActionName == action && rp.RoutePermission.ControllerName == controller);
    }

    public async Task CreateAsync(string roleName, ICollection<string> rolePermissionsIds)
    {
        var role = await roleManager.FindByNameAsync(roleName);

        if (role != null)
        {
            throw new InvalidOperationException(RoleWithThisNameExists);
        }

        var result = await roleManager.CreateAsync(new ApplicationRole() { Name = roleName });

        if (!result.Succeeded)
        {
            throw new Exception("Something went wrong while trying to save the role.");
        }

        var newRole = (await roleManager.FindByNameAsync(roleName))!;

        var permissions = repository
            .All<RoutePermission>()
            .Where(rp => rolePermissionsIds.Contains(rp.Id.ToString()));

        foreach (var permission in permissions)
        {
            await repository.AddAsync(new RoleRoutePermission()
            {
                Role = newRole,
                RoutePermission = permission
            });
        }

        await repository.SaveChangesAsync();
    }

    public async Task DeleteAsync(string roleName)
    {
        var role = await roleManager.FindByNameAsync(roleName);

        if (role == null)
        {
            throw new InvalidOperationException(RoleWithThisNameDoesNotExist);
        }

        await roleManager.DeleteAsync(role);
    }

    public async Task EditPermissionsAsync(string roleName, ICollection<string> newPermissionsIds)
    {
        var role = await roleManager.FindByNameAsync(roleName);

        if (role == null)
        {
            throw new InvalidOperationException(RoleWithThisNameDoesNotExist);
        }

        var permissions = repository
            .All<RoutePermission>()
            .Where(rp => newPermissionsIds.Contains(rp.Id.ToString()));

        foreach (var permission in permissions)
        {
            await repository.AddAsync(new RoleRoutePermission()
            {
                Role = role,
                RoutePermission = permission
            });
        }

        await repository.SaveChangesWithLogAsync();
    }
}
