using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WarehouseManagement.Api.Services.Contracts;
using WarehouseManagement.Infrastructure.Data.Models;

namespace WarehouseManagement.Infrastructure.Data
{
    public class WarehouseManagementDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        private readonly IUserService userService;

        public WarehouseManagementDbContext(
            DbContextOptions<WarehouseManagementDbContext> options,
            IUserService userService
        )
            : base(options)
        {
            this.userService = userService;
        }

        public DbSet<Delivery> Deliveries { get; set; } = null!;
        public DbSet<Entry> Entries { get; set; } = null!;
        public DbSet<Marker> Markers { get; set; } = null!;
        public DbSet<Vendor> Vendors { get; set; } = null!;
        public DbSet<Zone> Zones { get; set; } = null!;
        public DbSet<DifferenceType> DifferenceTypes { get; set; } = null!;
        public DbSet<Difference> Differences { get; set; } = null!;
        public DbSet<DeliveryMarker> DeliveriesMarkers { get; set; } = null!;
        public DbSet<VendorMarker> VendorsMarkers { get; set; } = null!;
        public DbSet<VendorZone> VendorsZones { get; set; } = null!;
        public DbSet<ZoneMarker> ZonesMarkers { get; set; } = null!;
        public DbSet<EntityChange> EntityChanges { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }

        public async Task<int> SaveChangesWithLogAsync(
            CancellationToken cancellationToken = default
        )
        {
            var userId = userService.UserId;
            var changes = OnBeforeSaveChanges(userId);
            var result = await base.SaveChangesAsync(cancellationToken);
            await OnAfterSaveChangesAsync(changes);
            return result;
        }

        private List<EntityChange> OnBeforeSaveChanges(string userId)
        {
            ChangeTracker.DetectChanges();
            var changes = new List<EntityChange>();

            var now = DateTime.UtcNow;

            foreach (
                var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Modified)
            )
            {
                var entityName = entry.Entity.GetType().Name;
                var entityId = GetEntityId(entry);

                if (string.IsNullOrEmpty(entityId))
                {
                    continue;
                }

                foreach (var property in entry.Properties.Where(p => p.IsModified))
                {
                    changes.Add(
                        new EntityChange
                        {
                            EntityName = entityName,
                            EntityId = entityId,
                            PropertyName = property.Metadata.Name,
                            OldValue = property.OriginalValue?.ToString(),
                            NewValue = property.CurrentValue?.ToString(),
                            ChangedAt = now,
                            ChangedByUserId = userId
                        }
                    );
                }
            }

            return changes;
        }

        private async Task OnAfterSaveChangesAsync(List<EntityChange> changes)
        {
            if (changes.Any())
            {
                await EntityChanges.AddRangeAsync(changes);
                await base.SaveChangesAsync();
            }
        }

        private string GetEntityId(EntityEntry entry)
        {
            var primaryKey = entry.Metadata.FindPrimaryKey();

            if (primaryKey != null)
            {
                var keyValues = primaryKey
                    .Properties.Select(p => entry.CurrentValues[p]?.ToString())
                    .ToArray();
                return string.Join("-", keyValues);
            }
            return string.Empty;
        }
    }
}
