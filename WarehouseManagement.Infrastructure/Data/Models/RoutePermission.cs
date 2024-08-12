namespace WarehouseManagement.Infrastructure.Data.Models;

public class RoutePermission : BaseClass
{
    public RoutePermission()
    {
        this.Id = Guid.NewGuid();
    }

    public new Guid Id { get; set; }

    public string ActionName { get; set; } = string.Empty;

    public string ControllerName { get; set; } = string.Empty;

    public ICollection<RoleRoutePermission> RoleRoutePermissions { get; set; } = new HashSet<RoleRoutePermission>();
}
