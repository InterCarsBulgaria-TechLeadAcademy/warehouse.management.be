namespace WarehouseManagement.Core.DTOs.Role;

public class RoleFormDto
{
    public string Name { get; set; } = string.Empty;

    public IEnumerable<string> PermissionIds { get; set; } = new HashSet<string>();
}
