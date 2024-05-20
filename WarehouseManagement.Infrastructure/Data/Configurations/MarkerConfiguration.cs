using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagement.Infrastructure.Data.Models;

namespace WarehouseManagement.Infrastructure.Data.Configurations;

internal class MarkerConfiguration : BaseConfiguration<Marker>
{
    public void Configure(EntityTypeBuilder<Marker> builder)
    {
        base.Configure(builder);

        builder.Property(m => m.Name).HasMaxLength(255);
    }
}
