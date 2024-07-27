using Microsoft.AspNetCore.Identity;

namespace WarehouseManagement.Infrastructure.Data.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    public ApplicationUser()
    {
        this.Id = Guid.NewGuid();
    }
}
