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
    }
}
