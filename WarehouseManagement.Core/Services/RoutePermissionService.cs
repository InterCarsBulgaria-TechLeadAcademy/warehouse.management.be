using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs.RoutePermission;
using WarehouseManagement.Infrastructure.Data.Common;
using WarehouseManagement.Infrastructure.Data.Models;
using static WarehouseManagement.Common.MessageConstants.Keys.RoutePermissionMessageKeys;

namespace WarehouseManagement.Core.Services;

public class RoutePermissionService : IRoutePermissionService
{
    private readonly IRepository repository;

    public RoutePermissionService(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Dictionary<string, ICollection<RoutePermissionDto>>> AllAsync()
    {
        var dictionary = new Dictionary<string, ICollection<RoutePermissionDto>>();

        var routePermissions = await repository
            .AllReadOnly<RoutePermission>()
            .ToListAsync();

        foreach (var rp in routePermissions)
        {
            if (!dictionary.ContainsKey(rp.ControllerName))
            {
                dictionary[rp.ControllerName] = new List<RoutePermissionDto>();
            }

            dictionary[rp.ControllerName].Add(new RoutePermissionDto
            {
                Id = rp.Id.ToString(),
                Name = $"{rp.ControllerName}.{rp.ActionName}"
            });
        }

        return dictionary;
    }

    public async Task<Dictionary<string, ICollection<RoutePermissionDto>>> AllWithDeletedAsync()
    {
        var dictionary = new Dictionary<string, ICollection<RoutePermissionDto>>();

        var routePermissions = await repository
            .AllWithDeleted<RoutePermission>()
            .ToListAsync();

        foreach (var rp in routePermissions)
        {
            if (!dictionary.ContainsKey(rp.ControllerName))
            {
                dictionary[rp.ControllerName] = new List<RoutePermissionDto>();
            }

            dictionary[rp.ControllerName].Add(new RoutePermissionDto
            {
                Id = rp.Id.ToString(),
                Name = $"{rp.ControllerName}.{rp.ActionName}"
            });
        }

        return dictionary;
    }

    public async Task DeleteAsync(string id)
    {
        if (!await ExistsByIdAsync(id))
        {
            throw new KeyNotFoundException(RoutePermissionWithIdNotFound);
        }

        await repository.SoftDeleteById<RoutePermission>(id);
        await repository.SaveChangesAsync();
    }

    public async Task<bool> ExistsByIdAsync(string id)
    {
        return await repository.GetByIdAsync<RoutePermission>(id) != null;
    }

    public async Task<bool> ExistsByRouteAsync(string controller, string action)
    {
        return await repository
            .AllReadOnly<RoutePermission>()
            .AnyAsync(rp => rp.ControllerName == controller
                && rp.ActionName == action);
    }

    public async Task<RoutePermissionDto> GetByIdAsync(string id)
    {
        var permission = await repository.GetByIdAsync<RoutePermission>(id);

        if (permission == null)
        {
            throw new KeyNotFoundException(RoutePermissionWithIdNotFound);
        }

        return new RoutePermissionDto
        {
            Id = permission.Id.ToString(),
            Name = $"{permission.ControllerName}.{permission.ActionName}"
        };
    }
}
