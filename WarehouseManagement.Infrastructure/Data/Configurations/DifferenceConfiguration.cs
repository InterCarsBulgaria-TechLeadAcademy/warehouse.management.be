using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagement.Infrastructure.Data.Models;
using static WarehouseManagement.Common.ValidationConstants.DifferenceConstants;

namespace WarehouseManagement.Infrastructure.Data.Configurations;

public class DifferenceConfiguration : BaseConfiguration<Difference>, IEntityTypeConfiguration<Difference>
{
    public void Configure(EntityTypeBuilder<Difference> builder)
    {
        base.Configure(builder);

        builder.Property(d => d.ReceptionNumber).HasMaxLength(ReceptionNumberMaxLength);
        builder.Property(d => d.InternalNumber).HasMaxLength(InternalNumberMaxLength);
        builder.Property(d => d.ActiveNumber).HasMaxLength(ActiveNumberMaxLength);
        builder.Property(d => d.Comment).HasMaxLength(CommentMaxLength);
        builder.Property(d => d.AdminComment).HasMaxLength(AdminCommentMaxLength);
        builder.Property(d => d.Count).HasMaxLength(MaxCount);

        builder
            .HasOne(d => d.Type)
            .WithMany(dt => dt.Differences)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(d => d.Delivery)
            .WithMany(d => d.Differences)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(d => d.Zone)
            .WithMany(z => z.Differences)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
