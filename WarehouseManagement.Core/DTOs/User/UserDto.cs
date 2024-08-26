namespace WarehouseManagement.Core.DTOs.User;

public class UserDto
{
    public string Id { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public IEnumerable<string> Roles { get; set; } = new HashSet<string>();

    public IEnumerable<string> RoutePermissionNames { get; set; } = new HashSet<string>();
}
