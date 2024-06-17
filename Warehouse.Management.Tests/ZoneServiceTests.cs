using Microsoft.EntityFrameworkCore;
using Moq;
using WarehouseManagement.Api.Services.Contracts;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs.Zone;
using WarehouseManagement.Core.Services;
using WarehouseManagement.Infrastructure.Data;
using WarehouseManagement.Infrastructure.Data.Common;
using WarehouseManagement.Infrastructure.Data.Models;
using static WarehouseManagement.Common.MessageConstants.Keys.ZoneMessageKeys;

namespace Warehouse.Management.Tests;

public class ZoneServiceTests
{
    private const int InvalidZoneId = 999;

    private WarehouseManagementDbContext dbContext;
    private IZoneService zoneService;
    private Mock<IUserService> mockUserService;

    private Zone zone1;
    private Zone zone2;
    private Zone deletedZone;
    private Zone oldZone1;

    private Marker marker1;
    private Marker marker2;
    private Vendor vendor1;
    private Vendor vendor2;
    private Entry entry1;
    private Entry entry2;

    [SetUp]
    public async Task Setup()
    {
        mockUserService = new Mock<IUserService>();
        mockUserService.Setup(x => x.UserId).Returns("TestUser");
        var options = new DbContextOptionsBuilder<WarehouseManagementDbContext>()
            .UseInMemoryDatabase(
                databaseName: "WarehouseManagementTestDb" + Guid.NewGuid().ToString()
            )
            .Options;

        dbContext = new WarehouseManagementDbContext(options, mockUserService.Object);

        zone1 = new Zone
        {
            Id = 1,
            Name = "Zone1",
            CreatedByUserId = "User1"
        };
        zone2 = new Zone
        {
            Id = 2,
            Name = "Zone2",
            CreatedByUserId = "User2"
        };
        deletedZone = new Zone
        {
            Id = 3,
            Name = "DeletedZone",
            IsDeleted = true,
            CreatedByUserId = "User3",
            DeletedByUserId = "User1"
        };
        oldZone1 = new Zone
        {
            Id = 4,
            Name = "Zone1",
            IsDeleted = true,
            CreatedByUserId = "User2",
            DeletedByUserId = "User2"
        };

        marker1 = new Marker
        {
            Id = 1,
            Name = "Marker1",
            CreatedByUserId = "User1"
        };
        marker2 = new Marker
        {
            Id = 2,
            Name = "Marker2",
            CreatedByUserId = "User2"
        };

        vendor1 = new Vendor
        {
            Id = 1,
            Name = "Vendor1",
            SystemNumber = "V001"
        };
        vendor2 = new Vendor
        {
            Id = 2,
            Name = "Vendor2",
            SystemNumber = "V002"
        };

        entry1 = new Entry
        {
            Id = 1,
            Pallets = 0,
            Packages = 6,
            Pieces = 0,
            Zone = zone1,
            CreatedByUserId = "User1"
        };

        entry2 = new Entry
        {
            Id = 2,
            Pallets = 4,
            Packages = 0,
            Pieces = 0,
            Zone = zone1,
            CreatedByUserId = "User2"
        };

        var zones = new List<Zone> { zone1, zone2, deletedZone, oldZone1 };
        var markers = new List<Marker> { marker1, marker2 };
        var vendors = new List<Vendor> { vendor1, vendor2 };
        var entries = new List<Entry> { entry1, entry2 };

        await dbContext.Zones.AddRangeAsync(zones);

        await dbContext.Markers.AddRangeAsync(markers);
        await dbContext.Vendors.AddRangeAsync(vendors);
        await dbContext.Entries.AddRangeAsync(entries);

        await dbContext.ZonesMarkers.AddRangeAsync(
            new ZoneMarker() { Zone = zone1, Marker = marker1 }
        );
        await dbContext.ZonesMarkers.AddRangeAsync(
            new ZoneMarker() { Zone = zone1, Marker = marker2 }
        );

        await dbContext.VendorsZones.AddRangeAsync(
            new VendorZone() { Zone = zone1, Vendor = vendor1 }
        );
        await dbContext.VendorsZones.AddRangeAsync(
            new VendorZone() { Zone = zone1, Vendor = vendor2 }
        );

        await dbContext.SaveChangesAsync();

        zoneService = new ZoneService(new Repository(dbContext, mockUserService.Object));
    }

    [TearDown]
    public async Task Teardown()
    {
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.DisposeAsync();
    }

    [Test]
    public async Task CreateAsync_SuccessfullyCreatesZone()
    {
        var uniqueName = "UniqueZoneName";
        var zone = new ZoneFormDto() { Name = uniqueName };

        await zoneService.CreateAsync(zone, mockUserService.Object.UserId);

        var addedZone = await dbContext.Zones.FirstAsync(z => z.Name == uniqueName);

        Assert.IsNotNull(addedZone);
        Assert.That(addedZone.Name, Is.EqualTo(uniqueName));
    }

    [Test]
    public void CreateAsync_ThrowsArgumentException_WhenNameExists()
    {
        var existingName = "Zone1";
        var zone = new ZoneFormDto() { Name = existingName };

        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await zoneService.CreateAsync(zone, mockUserService.Object.UserId);
        });
        Assert.That(ex.Message, Is.EqualTo(ZoneWithNameExists));
    }

    [Test]
    public async Task DeleteAsync_SuccessfullyDeletesZone()
    {
        await zoneService.DeleteAsync(2, mockUserService.Object.UserId);

        Assert.IsTrue(zone2.IsDeleted);
        Assert.IsNotNull(await dbContext.Zones.FindAsync(zone2.Id));
    }

    [Test]
    public void DeleteAsync_ThrowsKeyNotFoundException_WhenZoneNotFound()
    {
        var ex = Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await zoneService.DeleteAsync(InvalidZoneId, "TestUser")
        );

        Assert.That(ex.Message, Is.EqualTo(ZoneWithIdNotFound));
    }

    [Test]
    public void DeleteAsync_ThrowsInvalidOperationException_WhenZoneHasMarkers()
    {
        var ex = Assert.ThrowsAsync<InvalidOperationException>(
            async () => await zoneService.DeleteAsync(zone1.Id, "TestUser")
        );

        Assert.That(ex.Message, Is.EqualTo(ZoneHasMarkers));
    }

    [Test]
    public void DeleteAsync_ThrowsInvalidOperationException_WhenZoneHasVendors()
    {
        zone1.ZonesMarkers = new HashSet<ZoneMarker>();

        var ex = Assert.ThrowsAsync<InvalidOperationException>(
            async () => await zoneService.DeleteAsync(zone1.Id, "TestUser")
        );

        Assert.That(ex.Message, Is.EqualTo(ZoneHasVendors));
    }

    [Test]
    public void DeleteAsync_ThrowsInvalidOperationException_WhenZoneHasEntries()
    {
        var ex = Assert.ThrowsAsync<InvalidOperationException>(
            async () => await zoneService.DeleteAsync(zone1.Id, "TestUser")
        );

        Assert.That(ex.Message, Is.EqualTo(ZoneHasMarkers));
    }

    [Test]
    public async Task EditAsync_SuccessfullyUpdatesZone()
    {
        var newName = "UpdatedZone";
        var model = new ZoneFormDto { Name = newName };

        await zoneService.EditAsync(zone1.Id, model, "Modifier");

        var updatedZone = await dbContext.Zones.FindAsync(zone1.Id);

        Assert.That(updatedZone.Name, Is.EqualTo(newName));
        Assert.AreEqual(updatedZone.LastModifiedByUserId, "Modifier");
        Assert.IsNotNull(updatedZone.LastModifiedAt);
    }

    [Test]
    public void EditAsync_ThrowsKeyNotFoundException_WhenZoneNotFound()
    {
        var model = new ZoneFormDto { Name = "NewName" };

        var ex = Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await zoneService.EditAsync(InvalidZoneId, model, "Modifier")
        );

        Assert.That(ex.Message, Is.EqualTo(ZoneWithIdNotFound));
    }

    [Test]
    public void EditAsync_ThrowsKeyNotFoundException_WhenZoneWithNameExists()
    {
        var model = new ZoneFormDto { Name = "Zone1" };

        var ex = Assert.ThrowsAsync<ArgumentException>(
            async () => await zoneService.EditAsync(zone2.Id, model, "Modifier")
        );

        Assert.That(ex.Message, Is.EqualTo(ZoneWithNameExists));
    }

    [Test]
    public async Task ExistsByIdAsync_ReturnsTrue_WhenZoneExists()
    {
        var exists = await zoneService.ExistsByIdAsync(zone1.Id);
        Assert.IsTrue(exists);
    }

    [Test]
    public async Task ExistsByIdAsync_ReturnsFalse_WhenZoneDoesNotExist()
    {
        var exists = await zoneService.ExistsByIdAsync(InvalidZoneId);
        Assert.IsFalse(exists);
    }

    [Test]
    public async Task ExistsByNameAsync_ReturnsTrue_WhenZoneNameExists()
    {
        var exists = await zoneService.ExistsByNameAsync(zone1.Name);
        Assert.IsTrue(exists);
    }

    [Test]
    public async Task ExistsByNameAsync_ReturnsFalse_WhenZoneNameDoesNotExist()
    {
        var exists = await zoneService.ExistsByNameAsync("NonExistingZone");
        Assert.IsFalse(exists);
    }

    [Test]
    public async Task GetAllAsync_ReturnsAllZones()
    {
        var zones = await zoneService.GetAllAsync();

        Assert.That(zones.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task GetAllWithDeletedAsync_ReturnsAllZonesIncludingDeleted()
    {
        var zones = await zoneService.GetAllWithDeletedAsync();

        Assert.That(zones.Count, Is.EqualTo(3));
    }

    [Test]
    public async Task GetByIdAsync_ReturnsCorrectZone()
    {
        var zone = await zoneService.GetByIdAsync(zone1.Id);

        Assert.IsNotNull(zone);
        Assert.That(zone.Name, Is.EqualTo(zone1.Name));
    }

    [Test]
    public void GetByIdAsync_ThrowsKeyNotFoundException_WhenZoneDoesNotExist()
    {
        var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            await zoneService.GetByIdAsync(InvalidZoneId);
        });

        Assert.That(ex.Message, Is.EqualTo(ZoneWithIdNotFound));
    }

    [Test]
    public async Task RestoreAsync_SuccessfullyRestoresZone()
    {
        await zoneService.RestoreAsync(deletedZone.Id);

        Assert.IsFalse(deletedZone.IsDeleted);
    }

    [Test]
    public void RestoreAsync_ThrowsKeyNotFoundException_WhenZoneWithIdNotFound()
    {
        var ex = Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await zoneService.RestoreAsync(InvalidZoneId)
        );
        Assert.That(ex.Message, Is.EqualTo(ZoneWithIdNotFound));
    }

    [Test]
    public void RestoreAsync_ThrowsInvalidOperationException_WhenZoneIsNotDeleted()
    {
        var ex = Assert.ThrowsAsync<InvalidOperationException>(
            async () => await zoneService.RestoreAsync(zone1.Id)
        );
        Assert.That(ex.Message, Is.EqualTo(ZoneNotDeleted));
    }

    [Test]
    public void RestoreAsync_ThrowsInvalidOperationException_WhenZoneWithNameExists()
    {
        var ex = Assert.ThrowsAsync<InvalidOperationException>(
            async () => await zoneService.RestoreAsync(oldZone1.Id)
        );
        Assert.That(ex.Message, Is.EqualTo(ZoneWithNameExists));
    }
}