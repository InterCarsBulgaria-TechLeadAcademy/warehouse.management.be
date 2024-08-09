using WarehouseManagement.Core.DTOs.Role;

namespace WarehouseManagement.Core.Contracts;

public interface IRoleService
{
    Task<bool> CheckRoleAccessAsync(string roleName, string action, string controller);

    Task CreateAsync(RoleFormDto model);

    Task AssignRoleToUserAsync(string roleName, string userId);

    Task EditAsync(string oldName, RoleFormDto model);

    Task DeleteAsync(string roleName);

    Task<IEnumerable<RoleDto>> AllAsync();

    Task<RoleDto> GetByNameAsync(string name);
}
