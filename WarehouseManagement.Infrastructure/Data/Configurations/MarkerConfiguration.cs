using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagement.Infrastructure.Data.Models;
using static WarehouseManagement.Common.ValidationConstants.MarkerConstants;

namespace WarehouseManagement.Infrastructure.Data.Configurations;

internal class MarkerConfiguration : BaseConfiguration<Marker>, IEntityTypeConfiguration<Marker>
{
    public void Configure(EntityTypeBuilder<Marker> builder)
    {
        base.Configure(builder);

        builder.Property(m => m.Name).HasMaxLength(NameMaxLenght);
    }
}
