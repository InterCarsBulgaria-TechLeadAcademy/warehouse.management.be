using Microsoft.EntityFrameworkCore;
using Moq;
using WarehouseManagement.Api.Services.Contracts;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.Services;
using WarehouseManagement.Infrastructure.Data;
using WarehouseManagement.Infrastructure.Data.Common;
using WarehouseManagement.Infrastructure.Data.Models;

namespace Warehouse.Management.Tests;

public class VendorServiceTests
{
    private WarehouseManagementDbContext dbContext;
    private IRepository repository;
    private IVendorService vendorService;
    private Mock<IUserService> mockUserService;

    private Vendor vendor1;
    private Vendor vendor2;
    private Vendor vendor3;
    private Vendor vendorWithMarkers;
    private Vendor vendorWithZones;
    private Vendor vendorWithZonesAndMarkes;
    private Vendor deletedVendor1;
    private Vendor deletedVendor2;
    private Marker marker1;
    private Marker marker2;
    private Marker marker3;
    private Zone zone1;
    private Zone zone2;
    private Zone zone3;

    [SetUp]
    public async Task Setup()
    {
        vendor1 = new Vendor()
        {
            Id = 1,
            Name = "Vendor1",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            CreatedByUserId = "user1"
        };
        vendor2 = new Vendor()
        {
            Id = 2,
            Name = "Vendor2",
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            CreatedByUserId = "user2"
        };
        vendor3 = new Vendor()
        {
            Id = 3,
            Name = "Vendor3",
            CreatedAt = DateTime.UtcNow.AddDays(-3),
            CreatedByUserId = "user3"
        };
        vendorWithMarkers = new Vendor()
        {
            Id = 4,
            Name = "vendorWithMarkers",
            CreatedAt = DateTime.UtcNow.AddDays(-3),
            CreatedByUserId = "user3"
        };
        vendorWithZones = new Vendor()
        {
            Id = 5,
            Name = "vendorWithZones",
            CreatedAt = DateTime.UtcNow.AddDays(-3),
            CreatedByUserId = "user3"
        };
        vendorWithZonesAndMarkes = new Vendor()
        {
            Id = 6,
            Name = "vendorWithZonesAndMarkes",
            CreatedAt = DateTime.UtcNow.AddDays(-3),
            CreatedByUserId = "user3"
        };
        deletedVendor1 = new Vendor()
        {
            Id = 7,
            Name = "deletedVendor1",
            CreatedAt = DateTime.UtcNow.AddDays(-3),
            CreatedByUserId = "user3"
        };
        deletedVendor2 = new Vendor()
        {
            Id = 8,
            Name = "deletedVendor2",
            CreatedAt = DateTime.UtcNow.AddDays(-3),
            CreatedByUserId = "user3"
        };

        marker1 = new Marker()
        {
            Id = 1,
            Name = "marker1",
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = "markerMaker1",
        };
        marker2 = new Marker()
        {
            Id = 2,
            Name = "marker2",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            CreatedByUserId = "markerMaker2",
        };
        marker3 = new Marker()
        {
            Id = 3,
            Name = "marker3",
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            CreatedByUserId = "markerMaker3",
        };

        zone1 = new Zone()
        {
            Id = 1,
            Name = "Zone1",
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = "ZoneMaker1",
        };
        zone2 = new Zone()
        {
            Id = 2,
            Name = "Zone2",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            CreatedByUserId = "ZoneMaker2",
        };
        zone3 = new Zone()
        {
            Id = 3,
            Name = "Zone3",
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            CreatedByUserId = "ZoneMaker3",
        };

        var vendorZone1 = new VendorZone() { ZoneId = zone1.Id, VendorId = vendorWithZones.Id, };

        var vendorZone2 = new VendorZone() { ZoneId = zone2.Id, VendorId = vendorWithZones.Id, };

        var vendorZone3 = new VendorZone()
        {
            ZoneId = zone3.Id,
            VendorId = vendorWithZonesAndMarkes.Id,
        };

        var vendorMarker1 = new VendorMarker()
        {
            MarkerId = marker1.Id,
            VendorId = vendorWithMarkers.Id,
        };

        var vendorMarker2 = new VendorMarker()
        {
            MarkerId = marker2.Id,
            VendorId = vendorWithMarkers.Id,
        };

        var vendorMarker3 = new VendorMarker()
        {
            MarkerId = marker3.Id,
            VendorId = vendorWithZonesAndMarkes.Id
        };

        mockUserService = new Mock<IUserService>();
        mockUserService.Setup(x => x.UserId).Returns("TestUser");
        var options = new DbContextOptionsBuilder<WarehouseManagementDbContext>()
            .UseInMemoryDatabase(
                databaseName: "WarehouseManagementTestDb" + Guid.NewGuid().ToString()
            )
            .Options;

        dbContext = new WarehouseManagementDbContext(options, mockUserService.Object);

        dbContext.AddRange(
            vendor1,
            vendor2,
            vendor3,
            vendorWithMarkers,
            vendorWithZones,
            vendorWithZonesAndMarkes,
            deletedVendor1,
            deletedVendor2
        );
        dbContext.AddRange(marker1, marker2, marker3);
        dbContext.AddRange(zone1, zone2, zone3);
        dbContext.AddRange(vendorMarker1, vendorMarker2, vendorMarker3);
        dbContext.AddRange(vendorZone1, vendorMarker2, vendorMarker3);

        await dbContext.SaveChangesAsync();

        repository = new Repository(dbContext, mockUserService.Object);
        vendorService = new VendorService(repository);

        var vendors = await dbContext.Vendors.ToListAsync();
        var markers = await dbContext.Markers.ToListAsync();
        var zones = await dbContext.Zones.ToListAsync();
    }

    [Test]
    public void Test()
    {
        var a = 1;
        var b = -1;

        Assert.That(a, Is.EqualTo(1));
    }

    [TearDown]
    public async Task Teardown()
    {
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.DisposeAsync();
    }
}
