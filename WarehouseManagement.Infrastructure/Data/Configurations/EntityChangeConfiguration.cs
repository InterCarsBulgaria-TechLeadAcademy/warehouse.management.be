using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagement.Infrastructure.Data.Models;

namespace WarehouseManagement.Infrastructure.Data.Configurations;

public class EntityChangeConfiguration : IEntityTypeConfiguration<EntityChange>
{
    public void Configure(EntityTypeBuilder<EntityChange> builder)
    {
        builder.Property(ec => ec.EntityName).HasMaxLength(255);
        builder.Property(ec => ec.EntityId).HasMaxLength(255);
        builder.Property(ec => ec.PropertyName).HasMaxLength(255);
        builder.Property(ec => ec.Action).HasMaxLength(50);
        builder.Property(ec => ec.ChangedByUserId).HasMaxLength(255);
    }
}
