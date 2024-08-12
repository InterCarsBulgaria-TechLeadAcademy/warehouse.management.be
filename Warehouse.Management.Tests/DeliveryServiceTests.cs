using Microsoft.EntityFrameworkCore;
using Moq;
using WarehouseManagement.Api.Services.Contracts;
using WarehouseManagement.Common.Enums;
using WarehouseManagement.Common.MessageConstants.Keys;
using WarehouseManagement.Common.Statuses;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs.Delivery;
using WarehouseManagement.Core.Services;
using WarehouseManagement.Infrastructure.Data;
using WarehouseManagement.Infrastructure.Data.Common;
using WarehouseManagement.Infrastructure.Data.Models;

namespace Warehouse.Management.Tests;

public class DeliveryServiceTests
{
    private WarehouseManagementDbContext dbContext;
    private IDeliveryService deliveryService;
    private IEntryService entryService;
    private Mock<IUserService> mockUserService;

    private Delivery delivery;
    private Delivery deliveryApproved;
    private Delivery deliveryToBeApproved;

    private Vendor vendor;

    private Entry waitingEntry;
    private Entry processingEntry;
    private Entry finishedEntry;
    private Entry finishedEntry1;

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
            IsApproved = false,
            Status = DeliveryStatus.Processing,
            StartedProcessing = null,
            FinishedProcessing = null,
            VendorId = vendor.Id,
            Vendor = vendor
        };

        deliveryApproved = new Delivery
        {
            Id = 1002,
            SystemNumber = "D567891",
            ReceptionNumber = "R987651",
            TruckNumber = "TR12341",
            Cmr = "CMR0011",
            DeliveryTime = DateTime.Now,
            Pallets = 10,
            Packages = 100,
            Pieces = 1000,
            IsApproved = true,
            Status = DeliveryStatus.Finished,
            StartedProcessing = null,
            FinishedProcessing = null,
            VendorId = vendor.Id,
            Vendor = vendor
        };

        deliveryToBeApproved = new Delivery
        {
            Id = 1003,
            SystemNumber = "D567892",
            ReceptionNumber = "R987652",
            TruckNumber = "TR12342",
            Cmr = "CMR0012",
            DeliveryTime = DateTime.Now,
            Pallets = 10,
            Packages = 100,
            Pieces = 1000,
            IsApproved = false,
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
            StartedProcessing = null,
            FinishedProcessing = null,
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
            StartedProcessing = DateTime.UtcNow,
            FinishedProcessing = null,
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
            StartedProcessing = DateTime.UtcNow.AddDays(-7),
            FinishedProcessing = DateTime.UtcNow,
            CreatedByUserId = "User1",
            Delivery = delivery
        };

        finishedEntry1 = new Entry
        {
            Id = 4,
            Pallets = 0,
            Packages = 4,
            Pieces = 0,
            Zone = zone2,
            StartedProcessing = DateTime.UtcNow.AddDays(-7),
            FinishedProcessing = DateTime.UtcNow,
            CreatedByUserId = "User1",
            Delivery = deliveryToBeApproved
        };

        // Getting the earliest StartedProcessing
        delivery.StartedProcessing = finishedEntry.StartedProcessing; // Or call `ChangeDeliveryStatusIfNeeded` from DeliveryService

        var entries = new List<Entry>
        {
            waitingEntry,
            processingEntry,
            finishedEntry,
            finishedEntry1
        };
        var zones = new List<Zone> { zone1, zone2 };

        await dbContext.Vendors.AddAsync(vendor);
        await dbContext.Deliveries.AddAsync(delivery);
        await dbContext.Deliveries.AddAsync(deliveryApproved);
        await dbContext.Deliveries.AddAsync(deliveryToBeApproved);
        await dbContext.Zones.AddRangeAsync(zones);
        await dbContext.Entries.AddRangeAsync(entries);

        await dbContext.SaveChangesAsync();

        deliveryService = new DeliveryService(new Repository(dbContext, mockUserService.Object));
        entryService = new EntryService(new Repository(dbContext, mockUserService.Object));
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
        await entryService.FinishProcessingAsync(processingEntry.Id);

        await entryService.StartProcessingAsync(waitingEntry.Id);
        await entryService.FinishProcessingAsync(waitingEntry.Id);

        var history = await deliveryService.GetHistoryAsync(delivery.Id);

        Assert.IsNotNull(history);
        Assert.That(history.Id, Is.EqualTo(delivery.Id));
    }

    [Test]
    public async Task GetHistoryAsync_ShouldLogProcessingEntryFinishedProcessingChange()
    {
        await entryService.FinishProcessingAsync(processingEntry.Id);

        var history = await deliveryService.GetHistoryAsync(delivery.Id);

        var processingEntryFinishedProcessingChange = history.Changes.First(c =>
            c.EntityId == processingEntry.Id
        );

        Assert.That(processingEntryFinishedProcessingChange.From, Is.EqualTo(null));
        Assert.That(
            processingEntryFinishedProcessingChange.To,
            Is.EqualTo(processingEntry.FinishedProcessing.ToString())
        );
        Assert.That(
            processingEntryFinishedProcessingChange.Type,
            Is.EqualTo(DeliveryHistoryChangeType.Entry)
        );
    }

    [Test]
    public async Task GetHistoryAsync_ShouldLogWaitingEntryStartedProcessingChange()
    {
        await entryService.StartProcessingAsync(waitingEntry.Id);

        var history = await deliveryService.GetHistoryAsync(delivery.Id);

        var waitingEntryStartedProcessingChange = history.Changes.First(c =>
            c.EntityId == waitingEntry.Id
        );

        Assert.That(waitingEntryStartedProcessingChange.From, Is.EqualTo(null));
        Assert.That(
            waitingEntryStartedProcessingChange.To,
            Is.EqualTo(waitingEntry.StartedProcessing.ToString())
        );
        Assert.That(
            waitingEntryStartedProcessingChange.LogType,
            Is.EqualTo(LogType.EntryStatusChange)
        );
        Assert.That(
            waitingEntryStartedProcessingChange.Type,
            Is.EqualTo(DeliveryHistoryChangeType.Entry)
        );
    }

    [Test]
    public async Task GetHistoryAsync_ShouldLogWaitingEntryFinishedProcessingChange()
    {
        await entryService.StartProcessingAsync(waitingEntry.Id);
        await entryService.FinishProcessingAsync(waitingEntry.Id);

        var history = await deliveryService.GetHistoryAsync(delivery.Id);

        var waitingEntryFinishedProcessingChange = history.Changes.First(c =>
            c.EntityId == waitingEntry.Id && c.PropertyName == "FinishedProcessing"
        );

        Assert.That(waitingEntryFinishedProcessingChange.From, Is.EqualTo(null));
        Assert.That(
            waitingEntryFinishedProcessingChange.To,
            Is.EqualTo(waitingEntry.FinishedProcessing.ToString())
        );
        Assert.That(
            waitingEntryFinishedProcessingChange.LogType,
            Is.EqualTo(LogType.EntryStatusChange)
        );
        Assert.That(
            waitingEntryFinishedProcessingChange.Type,
            Is.EqualTo(DeliveryHistoryChangeType.Entry)
        );
    }

    [Test]
    public async Task GetHistoryAsync_ShouldLogDeliveryStatusChange()
    {
        await entryService.FinishProcessingAsync(processingEntry.Id);

        await entryService.StartProcessingAsync(waitingEntry.Id);
        await entryService.FinishProcessingAsync(waitingEntry.Id);

        await deliveryService.ChangeDeliveryStatusIfNeeded(delivery.Id);

        var history = await deliveryService.GetHistoryAsync(delivery.Id);

        var deliveryStatusChange = history.Changes.First(c =>
            c.EntityId == delivery.Id && c.PropertyName == "Status"
        );

        Assert.That(deliveryStatusChange.From, Is.EqualTo(DeliveryStatus.Processing.ToString()));
        Assert.That(deliveryStatusChange.To, Is.EqualTo(DeliveryStatus.Finished.ToString()));
        Assert.That(deliveryStatusChange.LogType, Is.EqualTo(LogType.DeliveryStatusChange));
        Assert.That(deliveryStatusChange.Type, Is.EqualTo(DeliveryHistoryChangeType.Delivery));
    }

    [Test]
    public async Task GetHistoryAsync_ShouldLogDeliveryFinishedProcessingChange()
    {
        await entryService.FinishProcessingAsync(processingEntry.Id);

        await entryService.StartProcessingAsync(waitingEntry.Id);
        await entryService.FinishProcessingAsync(waitingEntry.Id);

        await deliveryService.ChangeDeliveryStatusIfNeeded(delivery.Id);

        var history = await deliveryService.GetHistoryAsync(delivery.Id);

        var deliveryFinishedProcessingChange = history.Changes.First(c =>
            c.EntityId == delivery.Id && c.PropertyName == "FinishedProcessing"
        );

        Assert.That(deliveryFinishedProcessingChange.From, Is.EqualTo(null));
        Assert.That(
            deliveryFinishedProcessingChange.To,
            Is.EqualTo(delivery.FinishedProcessing.ToString())
        );
        Assert.That(
            deliveryFinishedProcessingChange.Type,
            Is.EqualTo(DeliveryHistoryChangeType.Delivery)
        );
    }

    [Test]
    public async Task GetHistoryAsync_ShouldLogDeliveryStartedProcessingChange()
    {
        var newDelivery = new Delivery
        {
            Id = 999,
            SystemNumber = "D46789",
            ReceptionNumber = "R98735",
            TruckNumber = "TB1234",
            Cmr = "CMR101",
            DeliveryTime = DateTime.Now,
            Pallets = 12,
            Packages = 155,
            Pieces = 1020,
            IsApproved = false,
            StartedProcessing = null,
            FinishedProcessing = null,
            VendorId = vendor.Id,
            Vendor = vendor
        };

        var newEntry = new Entry
        {
            Id = 55,
            Pallets = 22,
            Packages = 0,
            Pieces = 0,
            Zone = zone1,
            StartedProcessing = null,
            FinishedProcessing = null,
            CreatedByUserId = "User1",
            Delivery = newDelivery
        };

        await dbContext.Deliveries.AddAsync(newDelivery);
        await dbContext.Entries.AddAsync(newEntry);
        await dbContext.SaveChangesAsync();

        await entryService.StartProcessingAsync(newEntry.Id);

        await deliveryService.ChangeDeliveryStatusIfNeeded(newDelivery.Id);

        var history = await deliveryService.GetHistoryAsync(newDelivery.Id);

        var deliveryStatusChange = history.Changes.First(c =>
            c.EntityId == newDelivery.Id && c.PropertyName == "Status"
        );

        Assert.That(deliveryStatusChange.From, Is.EqualTo(DeliveryStatus.Waiting.ToString()));
        Assert.That(deliveryStatusChange.To, Is.EqualTo(DeliveryStatus.Processing.ToString()));
        Assert.That(deliveryStatusChange.Type, Is.EqualTo(DeliveryHistoryChangeType.Delivery));
        Assert.That(deliveryStatusChange.LogType, Is.EqualTo(LogType.DeliveryStatusChange));
    }

    [Test]
    public async Task GetHistoryAsync_ShouldReturnCorrectDateOfChange()
    {
        await entryService.FinishProcessingAsync(processingEntry.Id);

        var entityChangeDateTime = (
            await dbContext.EntityChanges.FirstAsync(change =>
                int.Parse(change.EntityId) == processingEntry.Id
            )
        ).ChangedAt;

        var history = await deliveryService.GetHistoryAsync(delivery.Id);
        var processingEntryFinishedDateTime = history
            .Changes.First(change => change.EntityId == processingEntry.Id)
            .ChangeDate;

        Assert.That(processingEntryFinishedDateTime, Is.EqualTo(entityChangeDateTime));
    }

    [Test]
    public void ApproveAsync_ShouldThrow_KeyNotFoundException_IfDeliveryDoesNotExist()
    {
        var nonExistingId = -1;

        var ex = Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await deliveryService.ApproveAsync(nonExistingId)
        );

        Assert.That(
            ex.Message,
            Is.EqualTo($"{DeliveryMessageKeys.DeliveryWithIdNotFound} {nonExistingId}")
        );
    }

    [Test]
    public void ApproveAsync_ShouldThrow_ArgumentException_IfDeliveryHasNotFinishedEnties()
    {
        var existingId = 1001;

        var ex = Assert.ThrowsAsync<ArgumentException>(
            async () => await deliveryService.ApproveAsync(existingId)
        );

        Assert.That(ex.Message, Is.EqualTo(DeliveryMessageKeys.DeliveryHasNotFinishedEntries));
    }

    [Test]
    public void ApproveAsync_ShouldThrow_InvalidOperationException_IfDeliveryIsAlreadyApproved()
    {
        var approvedDeliveryId = 1002;

        var ex = Assert.ThrowsAsync<InvalidOperationException>(
            async () => await deliveryService.ApproveAsync(approvedDeliveryId)
        );

        Assert.That(ex.Message, Is.EqualTo(DeliveryMessageKeys.DeliveryIsAlreadyApproved));
    }

    [Test]
    public async Task ApproveAsync_ShouldChangeIsApprovedToTrue()
    {
        var deliveryTobeApprovedId = 1003;

        await deliveryService.ApproveAsync(deliveryTobeApprovedId);

        var result = deliveryToBeApproved.IsApproved;

        Assert.That(result, Is.True);
    }
}
