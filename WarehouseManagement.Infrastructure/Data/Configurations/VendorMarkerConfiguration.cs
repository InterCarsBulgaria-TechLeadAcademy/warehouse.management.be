using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagement.Infrastructure.Data.Models;

namespace WarehouseManagement.Infrastructure.Data.Configurations;

internal class VendorMarkerConfiguration : IEntityTypeConfiguration<VendorMarker>
{
    public void Configure(EntityTypeBuilder<VendorMarker> builder)
    {
        builder.HasKey(vm => new { vm.MarkerId, vm.VendorId });
        builder
            .HasOne(vm => vm.Vendor)
            .WithMany(v => v.VendorsMarkers)
            .HasForeignKey(vm => vm.VendorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(vm => vm.Marker)
            .WithMany(m => m.VendorsMarkers)
            .HasForeignKey(vm => vm.MarkerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
