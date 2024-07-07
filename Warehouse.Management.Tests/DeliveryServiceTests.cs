using Microsoft.EntityFrameworkCore;
using Moq;
using WarehouseManagement.Api.Services.Contracts;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.Services;
using WarehouseManagement.Infrastructure.Data.Common;
using WarehouseManagement.Infrastructure.Data.Models;
using WarehouseManagement.Infrastructure.Data;
using WarehouseManagement.Core.DTOs.Delivery;
using WarehouseManagement.Common.Statuses;

namespace Warehouse.Management.Tests;

public class DeliveryServiceTests
{
    private WarehouseManagementDbContext dbContext;
    private IDeliveryService deliveryService;
    private Mock<IUserService> mockUserService;

    private Delivery delivery;

    private Vendor vendor;

    private Entry waitingEntry;
    private Entry processingEntry;
    private Entry finishedEntry;

    private Zone zone1;
    private Zone zone2;

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

        vendor = new Vendor
        {
            Id = 1,
            Name = "Vendor1",
            SystemNumber = "V12345"
        };

        delivery = new Delivery
        {
            Id = 1001,
            SystemNumber = "D56789",
            ReceptionNumber = "R98765",
            TruckNumber = "TR1234",
            Cmr = "CMR001",
            DeliveryTime = DateTime.Now,
            Pallets = 10,
            Packages = 100,
            Pieces = 1000,
            IsApproved = true,
            Status = DeliveryStatus.Processing,
            StartedProcessing = null,
            FinishedProcessing = null,
            VendorId = vendor.Id,
            Vendor = vendor
        };

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
            CreatedByUserId = "User1"
        };

        waitingEntry = new Entry
        {
            Id = 1,
            Pallets = 12,
            Packages = 0,
            Pieces = 0,
            Zone = zone1,
            StartedProccessing = null,
            FinishedProccessing = null,
            CreatedByUserId = "User1",
            Delivery = delivery
        };

        processingEntry = new Entry
        {
            Id = 2,
            Pallets = 0,
            Packages = 0,
            Pieces = 5,
            Zone = zone1,
            StartedProccessing = DateTime.UtcNow,
            FinishedProccessing = null,
            CreatedByUserId = "User1",
            Delivery = delivery
        };

        finishedEntry = new Entry
        {
            Id = 3,
            Pallets = 0,
            Packages = 4,
            Pieces = 0,
            Zone = zone2,
            StartedProccessing = DateTime.UtcNow.AddDays(-7),
            FinishedProccessing = DateTime.UtcNow,
            CreatedByUserId = "User1",
            Delivery = delivery
        };

        delivery.StartedProcessing = finishedEntry.StartedProccessing; // Or call `ChangeDeliveryStatusIfNeeded` from DeliveryService

        var entries = new List<Entry> { waitingEntry, processingEntry, finishedEntry };
        var zones = new List<Zone> { zone1, zone2 };

        await dbContext.Vendors.AddAsync(vendor);
        await dbContext.Deliveries.AddAsync(delivery);
        await dbContext.Entries.AddRangeAsync(entries);
        await dbContext.Zones.AddRangeAsync(zones);

        await dbContext.SaveChangesAsync();

        deliveryService = new DeliveryService(new Repository(dbContext, mockUserService.Object));
    }

    [TearDown]
    public async Task Teardown()
    {
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.DisposeAsync();
    }

    [Test]
    public async Task GetHistoryAsync_ShouldReturnHistoryOfDelivery()
    {
        var deliveryHistory = await deliveryService.GetHistoryAsync(delivery.Id);

        Assert.IsNotNull(deliveryHistory); // TODO: Change it
    }
}
