using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WarehouseManagement.Infrastructure.Data.Models;

namespace WarehouseManagement.Infrastructure.Data
{
    public class WarehouseManagementDbContext : IdentityDbContext
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public WarehouseManagementDbContext(
            DbContextOptions<WarehouseManagementDbContext> options,
            IHttpContextAccessor httpContextAccessor
        )
            : base(options)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public DbSet<Delivery> Deliveries { get; set; } = null!;
        public DbSet<Entry> Entries { get; set; } = null!;
        public DbSet<Marker> Markers { get; set; } = null!;
        public DbSet<Vendor> Vendors { get; set; } = null!;
        public DbSet<Zone> Zones { get; set; } = null!;
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
            var userId =
                httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? "guest";
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
                var entry in ChangeTracker
                    .Entries()
                    .Where(e =>
                        e.State == EntityState.Added
                        || e.State == EntityState.Modified
                        || e.State == EntityState.Deleted
                    )
            )
            {
                var entityName = entry.Entity.GetType().Name;
                var entityId = GetEntityId(entry);

                foreach (var property in entry.Properties)
                {
                    if (entry.State == EntityState.Added)
                    {
                        changes.Add(
                            new EntityChange
                            {
                                EntityName = entityName,
                                EntityId = entityId,
                                PropertyName = property.Metadata.Name,
                                OldValue = null,
                                NewValue = property.CurrentValue?.ToString(),
                                ChangedAt = now,
                                ChangedByUserId = userId,
                                Action = entry.State.ToString()
                            }
                        );
                    }
                    else if (entry.State == EntityState.Modified && property.IsModified)
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
                                ChangedByUserId = userId,
                                Action = entry.State.ToString()
                            }
                        );
                    }
                    else if (entry.State == EntityState.Deleted)
                    {
                        changes.Add(
                            new EntityChange
                            {
                                EntityName = entityName,
                                EntityId = entityId,
                                PropertyName = property.Metadata.Name,
                                OldValue = property.OriginalValue?.ToString(),
                                NewValue = null,
                                ChangedAt = now,
                                ChangedByUserId = userId,
                                Action = entry.State.ToString()
                            }
                        );
                    }
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

            if (primaryKey != null) //TODO: Check why the id is magic numbers (like -14329842)
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
