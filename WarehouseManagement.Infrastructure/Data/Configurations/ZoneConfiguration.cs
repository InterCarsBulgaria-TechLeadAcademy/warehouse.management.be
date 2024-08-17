using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagement.Infrastructure.Data.Models;

namespace WarehouseManagement.Infrastructure.Data.Configurations;

public class ZoneConfiguration : BaseConfiguration<Zone>, IEntityTypeConfiguration<Zone>
{
    public void Configure(EntityTypeBuilder<Zone> builder)
    {
        base.Configure(builder);

        builder.Property(z => z.Name).HasMaxLength(255);
    }
}
