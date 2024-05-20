using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagement.Infrastructure.Data.Models;

namespace WarehouseManagement.Infrastructure.Data.Configurations;

internal class VendorZoneConfiguration : IEntityTypeConfiguration<VendorZone>
{
    public void Configure(EntityTypeBuilder<VendorZone> builder)
    {
        builder.HasKey(vz => new { vz.ZoneId, vz.VendorId });
        builder
            .HasOne(vz => vz.Vendor)
            .WithMany(v => v.VendorsZones)
            .HasForeignKey(vz => vz.VendorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(vz => vz.Zone)
            .WithMany(z => z.VendorsZones)
            .HasForeignKey(vz => vz.ZoneId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
