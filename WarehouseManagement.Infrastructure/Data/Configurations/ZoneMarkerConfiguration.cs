using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagement.Infrastructure.Data.Models;

namespace WarehouseManagement.Infrastructure.Data.Configurations;

internal class ZoneMarkerConfiguration : IEntityTypeConfiguration<ZoneMarker>
{
    public void Configure(EntityTypeBuilder<ZoneMarker> builder)
    {
        builder.HasKey(zm => new { zm.ZoneId, zm.MarkerId });
        builder
            .HasOne(zm => zm.Marker)
            .WithMany(m => m.ZonesMarkers)
            .HasForeignKey(zm => zm.MarkerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(zm => zm.Zone)
            .WithMany(z => z.ZonesMarkers)
            .HasForeignKey(zm => zm.ZoneId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
