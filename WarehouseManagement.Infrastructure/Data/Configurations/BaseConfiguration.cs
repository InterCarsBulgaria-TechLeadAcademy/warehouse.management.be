using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagement.Infrastructure.Data.Models;

namespace WarehouseManagement.Infrastructure.Data.Configurations;

public class BaseConfiguration<T> : IEntityTypeConfiguration<T>
    where T : BaseClass
{
    public void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasQueryFilter(f => !f.IsDeleted);
        builder.Property(p => p.CreatedByUserId).HasMaxLength(255);
        builder.Property(p => p.LastModifiedByUserId).HasMaxLength(255);
        builder.Property(p => p.DeletedByUserId).HasMaxLength(255);
        builder.Property(p => p.CreatedAt).HasDefaultValue(DateTime.UtcNow);
    }
}
