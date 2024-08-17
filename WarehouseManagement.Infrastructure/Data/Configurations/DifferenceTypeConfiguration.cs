using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagement.Infrastructure.Data.Models;

namespace WarehouseManagement.Infrastructure.Data.Configurations;

public class DifferenceTypeConfiguration : BaseConfiguration<DifferenceType>, IEntityTypeConfiguration<DifferenceType>
{
    public void Configure(EntityTypeBuilder<DifferenceType> builder)
    {
        base.Configure(builder);

        builder.Property(dt => dt.Name).HasMaxLength(250);
    }
}
