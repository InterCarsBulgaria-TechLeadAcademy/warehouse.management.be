using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagement.Infrastructure.Data.Models;

namespace WarehouseManagement.Infrastructure.Data.Configurations;

internal class VendorConfiguration : BaseConfiguration<Vendor>
{
    public void Configure(EntityTypeBuilder<Vendor> builder)
    {
        base.Configure(builder);

        builder.Property(v => v.Name).HasMaxLength(255);
        builder.Property(v => v.SystemNumber).HasMaxLength(255);
    }
}
