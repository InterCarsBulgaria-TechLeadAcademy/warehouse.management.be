namespace WarehouseManagement.Core.DTOs.Role;

public class RoleDetailsDto
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public List<string> RolePermissions { get; set; } = new List<string>();
}