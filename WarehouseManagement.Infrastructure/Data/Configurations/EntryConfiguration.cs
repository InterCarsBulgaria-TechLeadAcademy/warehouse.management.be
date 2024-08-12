using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagement.Infrastructure.Data.Models;

namespace WarehouseManagement.Infrastructure.Data.Configurations
{
    public class EntryConfiguration : BaseConfiguration<Entry>, IEntityTypeConfiguration<Entry>
    {
        public void Configure(EntityTypeBuilder<Entry> builder)
        {
            base.Configure(builder);

            builder
                .HasOne(e => e.Zone)
                .WithMany(z => z.Entries)
                .HasForeignKey(e => e.ZoneId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(e => e.Delivery)
                .WithMany(d => d.Entries)
                .HasForeignKey(e => e.DeliveryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
