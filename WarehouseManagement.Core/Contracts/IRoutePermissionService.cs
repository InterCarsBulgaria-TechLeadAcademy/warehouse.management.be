using WarehouseManagement.Core.DTOs.RoutePermission;

namespace WarehouseManagement.Core.Contracts;

public interface IRoutePermissionService
{
    Task<IEnumerable<RoutePermissionDto>> AllAsync();

    Task<IEnumerable<RoutePermissionDto>> AllWithDeletedAsync();

    Task<RoutePermissionDto> GetByIdAsync(string id);

    Task AddAsync(RoutePermissionFormDto model);

    Task EditAsync(string id, RoutePermissionFormDto model);

    Task DeleteAsync(string id);

    Task<bool> ExistsByIdAsync(string id);

    Task<bool> ExistsByNameAsync(string name);

    Task<bool> ExistsByRouteAsync(string controller, string action);
}
