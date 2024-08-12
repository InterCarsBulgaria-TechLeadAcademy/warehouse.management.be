using WarehouseManagement.Core.DTOs.RoutePermission;

namespace WarehouseManagement.Core.Contracts;

public interface IRoutePermissionService
{
    Task<Dictionary<string, ICollection<RoutePermissionDto>>> AllAsync();

    Task<Dictionary<string, ICollection<RoutePermissionDto>>> AllWithDeletedAsync();

    Task<RoutePermissionDto> GetByIdAsync(string id);

    Task DeleteAsync(string id);

    Task<bool> ExistsByIdAsync(string id);

    Task<bool> ExistsByRouteAsync(string controller, string action);
}
