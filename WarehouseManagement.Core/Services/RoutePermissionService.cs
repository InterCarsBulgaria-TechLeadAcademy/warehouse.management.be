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

    public async Task AddAsync(RoutePermissionFormDto model)
    {
        if (await ExistsByNameAsync(model.Name))
        {
            throw new ArgumentException(RoutePermissionWithThisNameAlreadyExists);
        }

        if (await ExistsByRouteAsync(model.ControllerName, model.ActionName))
        {
            throw new ArgumentException(RoutePermissionWithThisRouteAlreadyExists);
        }

        await repository.AddAsync(new RoutePermission
        {
            Name = model.Name,
            ActionName = model.ActionName,
            ControllerName = model.ControllerName
        });
        await repository.SaveChangesAsync();
    }

    public async Task<IEnumerable<RoutePermissionDto>> AllAsync()
    {
        return await repository.AllReadOnly<RoutePermission>()
            .Select(rp => new RoutePermissionDto
            {
                Id = rp.Id.ToString(),
                Name = rp.Name,
                ActionName = rp.ActionName,
                ControllerName = rp.ControllerName
            }).ToListAsync();
    }

    public async Task<IEnumerable<RoutePermissionDto>> AllWithDeletedAsync()
    {
        return await repository.AllWithDeletedReadOnly<RoutePermission>()
            .Select(rp => new RoutePermissionDto
            {
                Id = rp.Id.ToString(),
                Name = rp.Name,
                ActionName = rp.ActionName,
                ControllerName = rp.ControllerName
            }).ToListAsync();
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

    public async Task EditAsync(string id, RoutePermissionFormDto model)
    {
        if (!await ExistsByIdAsync(id))
        {
            throw new KeyNotFoundException(RoutePermissionWithIdNotFound);
        }

        if (await ExistsByNameAsync(model.Name))
        {
            throw new ArgumentException(RoutePermissionWithThisNameAlreadyExists);
        }

        if (await ExistsByRouteAsync(model.ControllerName, model.ActionName))
        {
            throw new ArgumentException(RoutePermissionWithThisRouteAlreadyExists);
        }

        var permission = await repository.GetByIdAsync<RoutePermission>(id);

        permission!.Name = model.Name;
        permission.ActionName = model.ActionName;
        permission.ControllerName = model.ControllerName;

        await repository.SaveChangesWithLogAsync();
    }

    public async Task<bool> ExistsByIdAsync(string id)
    {
        return await repository.GetByIdAsync<RoutePermission>(id) != null;
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await repository
            .AllReadOnly<RoutePermission>()
            .AnyAsync(rp => rp.Name == name);
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
            Name = permission.Name,
            ActionName = permission.ActionName,
            ControllerName = permission.ControllerName
        };
    }
}
