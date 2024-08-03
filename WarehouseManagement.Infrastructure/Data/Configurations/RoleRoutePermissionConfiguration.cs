using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagement.Infrastructure.Data.Models;

namespace WarehouseManagement.Infrastructure.Data.Configurations
{
    public class RoleRoutePermissionConfiguration : IEntityTypeConfiguration<RoleRoutePermission>
    {
        public void Configure(EntityTypeBuilder<RoleRoutePermission> builder)
        {
            builder.HasKey(rp => new { rp.RoleId, rp.RoutePermissionId });

            builder
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RoleRoutePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(rp => rp.RoutePermission)
                .WithMany(r => r.RoleRoutePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
