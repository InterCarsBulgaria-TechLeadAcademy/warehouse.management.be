using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagement.Infrastructure.Data.Models;

namespace WarehouseManagement.Infrastructure.Data.Configurations;

internal class DeliveryMarkerConfiguration : IEntityTypeConfiguration<DeliveryMarker>
{
    public void Configure(EntityTypeBuilder<DeliveryMarker> builder)
    {
        builder.HasKey(dm => new { dm.MarkerId, dm.DeliveryId });
        builder
            .HasOne(dm => dm.Delivery)
            .WithMany(d => d.DeliveriesMarkers)
            .HasForeignKey(dm => dm.DeliveryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(dm => dm.Marker)
            .WithMany(m => m.DeliveriesMarkers)
            .HasForeignKey(dm => dm.MarkerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
