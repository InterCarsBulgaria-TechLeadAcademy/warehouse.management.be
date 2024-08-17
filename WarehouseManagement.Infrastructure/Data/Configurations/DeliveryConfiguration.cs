using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagement.Common.Statuses;
using WarehouseManagement.Infrastructure.Data.Models;

namespace WarehouseManagement.Infrastructure.Data.Configurations;

public class DeliveryConfiguration : BaseConfiguration<Delivery>, IEntityTypeConfiguration<Delivery>
{
    public void Configure(EntityTypeBuilder<Delivery> builder)
    {
        base.Configure(builder);

        builder.Property(d => d.Cmr).HasMaxLength(255); //TODO: ask about length
        builder.Property(d => d.SystemNumber).HasMaxLength(255);
        builder.Property(d => d.ReceptionNumber).HasMaxLength(255);
        builder.Property(d => d.TruckNumber).HasMaxLength(255);
        builder.Property(d => d.Status).HasDefaultValue(DeliveryStatus.Waiting);
        builder
            .HasOne(d => d.Vendor)
            .WithMany(v => v.Deliveries)
            .HasForeignKey(d => d.VendorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
