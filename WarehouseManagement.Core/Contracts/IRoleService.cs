using WarehouseManagement.Core.DTOs.Role;

namespace WarehouseManagement.Core.Contracts;

public interface IRoleService
{
    Task<bool> CheckRoleAccessAsync(string roleName, string action, string controller);

    Task CreateAsync(RoleFormDto model);

    Task AssignRoleToUserAsync(string id, string userId);

    Task EditAsync(string id, RoleFormDto model);

    Task DeleteAsync(string id);

    Task<IEnumerable<RoleDetailsDto>> AllAsync();

    Task<RoleDto> GetByIdAsync(string id);
}
