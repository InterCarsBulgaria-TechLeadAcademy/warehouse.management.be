using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs.Role;
using WarehouseManagement.Core.DTOs.RoutePermission;
using WarehouseManagement.Infrastructure.Data.Common;
using WarehouseManagement.Infrastructure.Data.Models;
using static WarehouseManagement.Common.MessageConstants.Keys.RoleMessageKeys;
using static WarehouseManagement.Common.MessageConstants.Keys.ApplicationUserMessageKeys;

namespace WarehouseManagement.Core.Services;

public class RoleService : IRoleService
{
    private readonly RoleManager<ApplicationRole> roleManager;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IRepository repository;

    public RoleService(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, IRepository repository)
    {
        this.roleManager = roleManager;
        this.userManager = userManager;
        this.repository = repository;
    }

    public async Task<IEnumerable<RoleDetailsDto>> AllAsync()
    {
        return await roleManager.Roles
            .Select(r => new RoleDetailsDto
            {
                Id = r.Id.ToString(),
                Name = r.Name!,
                RolePermissions = r.RoleRoutePermissions
                .Select(rrp => $"{rrp.RoutePermission.ControllerName}.{rrp.RoutePermission.ActionName}")
                .ToList()
            })
            .ToListAsync();
    }

    public async Task AssignRoleToUserAsync(string roleId, string userId)
    {
        var role = await roleManager.FindByIdAsync(id);

        if (role == null)
        {
            throw new ArgumentException(RoleWithThisNameDoesNotExist);
        }

        var user = await repository.GetByIdAsync<ApplicationUser>(Guid.Parse(userId));

        if (user == null)
        {
            throw new KeyNotFoundException(UserWithThisIdNotFound);
        }

        if (await userManager.IsInRoleAsync(user, role.Name))
        {
            throw new InvalidOperationException(RoleCannotBeAssignedTwiceOnTheSameUser);
        }

        await userManager.AddToRoleAsync(user, role.Name!);
    }

    public async Task<bool> CheckRoleAccessAsync(string roleName, string action, string controller)
    {
        var role = await roleManager.FindByNameAsync(roleName);

        if (role == null)
        {
            throw new ArgumentException(RoleWithThisNameDoesNotExist);
        }

        return repository
            .All<RoleRoutePermission>()
            .Any(rrp => rrp.RoleId == role.Id 
                && rrp.RoutePermission.ActionName == action 
                && rrp.RoutePermission.ControllerName == controller);
    }

    public async Task CreateAsync(RoleFormDto model)
    {
        var role = await roleManager.FindByNameAsync(model.Name);

        if (role != null)
        {
            throw new InvalidOperationException(RoleWithThisNameAlreadyExists);
        }

        var permissions = repository
            .All<RoutePermission>()
            .Where(rp => model.PermissionIds.Any(id => id == rp.Id.ToString()));

        if (!permissions.Any())
        {
            throw new InvalidOperationException(RoleCannotBeCreatedWithoutPermissions);
        }

        var result = await roleManager.CreateAsync(new ApplicationRole() { Name = model.Name });

        if (!result.Succeeded)
        {
            throw new Exception("Something went wrong while trying to save the role.");
        }

        var newRole = (await roleManager.FindByNameAsync(model.Name))!;

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

    public async Task DeleteAsync(string id)
    {
        var role = await roleManager.FindByNameAsync(id);

        if (role == null)
        {
            throw new ArgumentException(RoleWithThisNameDoesNotExist);
        }

        await roleManager.DeleteAsync(role);
    }

    public async Task EditAsync(string id, RoleFormDto model)
    {
        var role = await roleManager.FindByIdAsync(id);

        if (role == null)
        {
            throw new ArgumentException(RoleWithThisNameDoesNotExist);
        }

        if (model.Name != role.Name && await roleManager.FindByNameAsync(model.Name) != null)
        {
            throw new ArgumentException(RoleWithThisNameAlreadyExists);
        }

        role.Name = model.Name;
        role.NormalizedName = model.Name.ToUpper();

        var permissions = repository
            .All<RoutePermission>()
            .Where(rp => model.PermissionIds.Contains(rp.Id.ToString()));

        var oldRolePermissions = await repository
            .All<RoleRoutePermission>()
            .Where(rrp => rrp.RoleId == role.Id)
            .ToListAsync();

        repository.DeleteRange(oldRolePermissions);

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

    public async Task<RoleDto> GetByIdAsync(string id)
    {
        var role = await roleManager.Roles
            .Include(r => r.RoleRoutePermissions)
            .ThenInclude(rrp => rrp.RoutePermission)
            .FirstOrDefaultAsync(r => r.Id == Guid.Parse(id));
        
        if (role == null)
        {
            throw new ArgumentException(RoleWithThisNameDoesNotExist);
        }

        return new RoleDto
        {
            Id = role.Id.ToString(),
            Name = role.Name!,
            RoutePermissions = role.RoleRoutePermissions
                .Select(rrp => new RoutePermissionDto
                {
                    Id = rrp.RoutePermission.Id.ToString(),
                    Name = $"{rrp.RoutePermission.ControllerName}.{rrp.RoutePermission.ActionName}"
                })
        };
    }
}
