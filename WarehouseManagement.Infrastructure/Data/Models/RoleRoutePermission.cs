using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseManagement.Infrastructure.Data.Models;

public class RoleRoutePermission
{
    [ForeignKey(nameof(RoutePermission))]
    public Guid RoutePermissionId { get; set; }

    public RoutePermission RoutePermission { get; set; } = null!;

    [ForeignKey(nameof(Role))]
    public Guid RoleId { get; set; }

    public ApplicationRole Role { get; set; } = null!;
}
