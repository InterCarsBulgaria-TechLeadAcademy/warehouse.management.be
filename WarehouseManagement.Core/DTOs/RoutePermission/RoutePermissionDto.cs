namespace WarehouseManagement.Core.DTOs.RoutePermission;

public class RoutePermissionDto
{
    public string Id { get; set; } = string.Empty;

    // ControllerName + ActionName
    public string Name { get; set; } = string.Empty;
}
