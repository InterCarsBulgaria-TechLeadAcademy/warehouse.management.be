using WarehouseManagement.Core.DTOs.Role;

namespace WarehouseManagement.Core.Contracts;

public interface IRoleService
{
    Task<bool> CheckRoleAccessAsync(string roleName, string action, string controller);

    Task CreateAsync(RoleFormDto model);

    Task AssignRoleByIdToUserAsync(string roleId, string userId);

    Task AssignRoleByNameToUserAsync(string roleName, string userId);

    Task EditAsync(string id, RoleFormDto model);

    Task DeleteAsync(string id);

    Task<IEnumerable<RoleDetailsDto>> AllAsync();

    Task<RoleDto> GetByIdAsync(string id);

    Task<RoleDto> GetByNameAsync(string roleName);
}
