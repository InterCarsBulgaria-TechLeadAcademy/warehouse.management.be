using Microsoft.AspNetCore.Identity;

namespace WarehouseManagement.Infrastructure.Data.Models;

public class ApplicationRole : IdentityRole<Guid>
{
    public ApplicationRole()
        : base()
    {
        this.Id = Guid.NewGuid();
    }

    public ApplicationRole(string name)
        : base(name)
    {
        this.Id = Guid.NewGuid();
    }

    public ICollection<RoleRoutePermission> RoleRoutePermissions { get; set; } = new HashSet<RoleRoutePermission>();
}
