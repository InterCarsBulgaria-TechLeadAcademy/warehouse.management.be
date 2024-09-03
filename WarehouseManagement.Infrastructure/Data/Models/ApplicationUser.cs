using Microsoft.AspNetCore.Identity;

namespace WarehouseManagement.Infrastructure.Data.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    public ApplicationUser()
    {
        this.Id = Guid.NewGuid();
    }

    public Guid? CreatorId { get; set; }

    public ApplicationUser? Creator { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public ICollection<ApplicationUser> SubUsers { get; set; } = new HashSet<ApplicationUser>();

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new HashSet<RefreshToken>();
}
