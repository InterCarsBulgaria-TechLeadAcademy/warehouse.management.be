namespace WarehouseManagement.Core.Contracts;

public interface IRoleService
{
    Task<bool> CheckRoleAccessAsync(string roleName, string action, string controller);

    Task CreateAsync(string roleName, ICollection<string> rolePermissionsIds);

    Task AssignRoleToUserAsync(string roleName, string userId);

    Task EditPermissionsAsync(string roleName, ICollection<string> newPermissionsIds);

    Task DeleteAsync(string roleName);
}
