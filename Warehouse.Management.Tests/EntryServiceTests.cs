using Microsoft.EntityFrameworkCore;
using Moq;
using WarehouseManagement.Api.Services.Contracts;
using WarehouseManagement.Common.Statuses;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs;
using WarehouseManagement.Core.DTOs.Entry;
using WarehouseManagement.Core.Services;
using WarehouseManagement.Infrastructure.Data;
using WarehouseManagement.Infrastructure.Data.Common;
using WarehouseManagement.Infrastructure.Data.Models;
using static WarehouseManagement.Common.MessageConstants.Keys.EntryMessageKey;
using static WarehouseManagement.Common.MessageConstants.Keys.ZoneMessageKeys;

namespace Warehouse.Management.Tests;

public class EntryServiceTests
{
    private const int InvalidEntryId = -1;
    private const int InvalidZoneId = -1;

    private WarehouseManagementDbContext dbContext;
    private IEntryService entryService;
    private Mock<IUserService> mockUserService;

    private Entry waitingEntry;
    private Entry processingEntry;
    private Entry finishedEntry;
    private Entry deletedEntry;

    private Zone zone1;
    private Zone zone2;

    private Delivery delivery;

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
            CreatedByUserId = "User1"
        };

        var vendor = new Vendor
        {
            Id = 1,
            Name = "Vendor1",
            SystemNumber = "V12345"
        };

        delivery = new Delivery
        {
            Id = 1,
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

        waitingEntry = new Entry
        {
            Id = 1,
            Pallets = 0,
            Packages = 6,
            Pieces = 0,
            Zone = zone1,
            StartedProcessing = null,
            FinishedProcessing = null,
            CreatedByUserId = "User1",
            Delivery = delivery,
            DeliveryId = delivery.Id
        };

        processingEntry = new Entry
        {
            Id = 2,
            Pallets = 4,
            Packages = 0,
            Pieces = 0,
            Zone = zone1,
            StartedProcessing = DateTime.UtcNow,
            FinishedProcessing = null,
            CreatedByUserId = "User1",
            Delivery = delivery,
            DeliveryId = delivery.Id
        };

        finishedEntry = new Entry
        {
            Id = 3,
            Pallets = 0,
            Packages = 0,
            Pieces = 8,
            Zone = zone2,
            StartedProcessing = DateTime.UtcNow.AddDays(-7),
            FinishedProcessing = DateTime.UtcNow,
            CreatedByUserId = "User1",
            Delivery = delivery,
            DeliveryId = delivery.Id
        };

        deletedEntry = new Entry
        {
            Id = 4,
            Pallets = 0,
            Packages = 0,
            Pieces = 5,
            Zone = zone1,
            StartedProcessing = null,
            FinishedProcessing = null,
            CreatedByUserId = "User1",
            IsDeleted = true,
            Delivery = delivery,
            DeliveryId = delivery.Id
        };

        var entries = new List<Entry> { waitingEntry, processingEntry, finishedEntry, deletedEntry };
        var zones = new List<Zone> { zone1, zone2 };

        await dbContext.Vendors.AddAsync(vendor);
        await dbContext.Deliveries.AddAsync(delivery);

        await dbContext.Entries.AddRangeAsync(entries);
        await dbContext.Zones.AddRangeAsync(zones);

        await dbContext.SaveChangesAsync();

        entryService = new EntryService(new Repository(dbContext, mockUserService.Object));
    }

    [TearDown]
    public async Task Teardown()
    {
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.DisposeAsync();
    }

    [Test]
    public async Task CreateAsync_SuccessfullyCreatesEntry()
    {
        var entries = new List<EntryFormDto>()
        {
            new EntryFormDto()
            {
                Pallets = 5,
                Packages = 0,
                Pieces = 0,
                DeliveryId = 1,
                ZoneId = 1
            },
            new EntryFormDto()
            {
                Pallets = 0,
                Packages = 0,
                Pieces = 15,
                DeliveryId = 1,
                ZoneId = 1
            },
            new EntryFormDto()
            {
                Pallets = 0,
                Packages = 4,
                Pieces = 0,
                DeliveryId = 1,
                ZoneId = 1
            }
        };

        await entryService.CreateAsync(entries, mockUserService.Object.UserId);

        Assert.That(dbContext.Entries.Count(), Is.EqualTo(6));
    }

    // Test should be refactored after merge of PR #30
    [Test]
    public void CreateAsync_ThrowsArgumentException_WhenEntryHasMoreThanOneTypeSet()
    {
        var entries = new List<EntryFormDto>()
        {
            new EntryFormDto()
            {
                Pallets = 5,
                Packages = 5,
                Pieces = 0,
                DeliveryId = 1,
                ZoneId = 1
            },
            new EntryFormDto()
            {
                Pallets = 0,
                Packages = 0,
                Pieces = 15,
                DeliveryId = 1,
                ZoneId = 1
            }
        };

        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await entryService.CreateAsync(entries, mockUserService.Object.UserId);
        });

        Assert.That(ex.Message, Is.EqualTo(EntryCanHaveOnlyOneTypeSet));
    }

    [Test]
    public async Task DeleteAsync_SuccessfulyDeletesEntry()
    {
        await entryService.DeleteAsync(2, mockUserService.Object.UserId);

        Assert.IsTrue(processingEntry.IsDeleted);
        Assert.IsNotNull(await dbContext.Entries.FindAsync(processingEntry.Id));
    }

    [Test]
    public void DeleteAsync_ThrowsKeyNotFoundException_WhenEntryNotFound()
    {
        var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            await entryService.DeleteAsync(InvalidEntryId, mockUserService.Object.UserId);
        });

        Assert.That(ex.Message, Is.EqualTo(EntryWithIdNotFound));
    }

    [Test]
    public async Task EditAsync_SuccessfulyEditsEntry()
    {
        var model = new EntryFormDto
        {
            Pallets = 0,
            Packages = 0,
            Pieces = 15
        };

        await entryService.EditAsync(finishedEntry.Id, model, mockUserService.Object.UserId);

        Assert.That(finishedEntry.Pieces, Is.EqualTo(15));
    }

    [Test]
    public void EditAsync_ThrowsKeyNotFoundException_WhenEntryNotFound()
    {
        var model = new EntryFormDto
        {
            Pallets = 0,
            Packages = 0,
            Pieces = 15
        };

        var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            await entryService.EditAsync(InvalidEntryId, model, mockUserService.Object.UserId);
        });

        Assert.That(ex.Message, Is.EqualTo(EntryWithIdNotFound));
    }

    [Test]
    public async Task ExistsByIdAsync_ShouldReturnTrue_IfEntryExists()
    {
        var exists = await entryService.ExistsByIdAsync(processingEntry.Id);
        Assert.IsTrue(exists);
    }

    [Test]
    public async Task ExistsByIdAsync_ShouldReturnFalse_IfEntryDoesNotExist()
    {
        var exists = await entryService.ExistsByIdAsync(InvalidEntryId);

        Assert.IsFalse(exists);
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnAllEntries()
    {
        var entries = await entryService.GetAllAsync(new PaginationParameters());

        Assert.That(entries.Results.Count(), Is.EqualTo(3));
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnAllEntries_WithTheProvidedStatus()
    {
        var entries = await entryService.GetAllAsync(new PaginationParameters(), [EntryStatuses.Waiting]);

        Assert.That(entries.Results.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task GetAllByZoneAsync_ShouldReturnAllEntries_ForTheGivenZone()
    {
        var entries = await entryService.GetAllByZoneAsync(new PaginationParameters(), 1);

        Assert.That(entries.Results.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetAllByZoneAsync_ShouldReturnAllEntries_WithTheProvidedStatus_ForTheGivenZone()
    {
        var entries = await entryService.GetAllByZoneAsync(new PaginationParameters(), 1, [EntryStatuses.Waiting]);

        Assert.That(entries.Results.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task GetAllWithDeletedAsync_ShouldReturnAllEntries_IncludingTheDeleted()
    {
        var entries = await entryService.GetAllWithDeletedAsync(new PaginationParameters());

        Assert.That(entries.Results.Count(), Is.EqualTo(4));
    }

    [Test]
    public async Task GetAllWithDeletedAsync_ShouldReturnAllEntries_IncludingTheDeleted_ForTheGivenZone_WheZoneIdIsProvided()
    {
        var entries = await entryService.GetAllWithDeletedAsync(new PaginationParameters(), 1);

        Assert.That(entries.Results.Count(), Is.EqualTo(3));
    }

    [Test]
    public async Task GetAllWithDeletedAsync_ShouldReturnAllEntries_IncludingTheDeleted_ForTheGivenZone_WithTheProvidedStatus()
    {
        var entries = await entryService.GetAllWithDeletedAsync(new PaginationParameters(), 1, [EntryStatuses.Waiting]);

        Assert.That(entries.Results.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetByIdAsync_SuccessfullyReturnsEntity()
    {
        var entry = await entryService.GetByIdAsync(1);

        Assert.That(entry.Id, Is.EqualTo(1));
    }

    [Test]
    public void GetById_ThrowsKeyNotFoundException_WithInvalidId()
    {
        var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            await entryService.GetByIdAsync(InvalidEntryId);
        });

        Assert.That(ex.Message, Is.EqualTo($"{EntryWithIdNotFound} {InvalidEntryId}"));
    }

    [Test]
    public async Task RestoreAsync_SuccessfullyUndeletesEntry()
    {
        await entryService.RestoreAsync(deletedEntry.Id);

        Assert.False(deletedEntry.IsDeleted);
    }

    [Test]
    public void RestoreAsync_ThrowsKeyNotFoundException_WithInvalidId()
    {
        var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            await entryService.RestoreAsync(InvalidEntryId);
        });

        Assert.That(ex.Message, Is.EqualTo(EntryWithIdNotFound));
    }

    [Test]
    public void RestoreAsync_ThrowsInvalidOperationException_WhenEntryIsNotDeleted()
    {
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await entryService.RestoreAsync(waitingEntry.Id);
        });
        
        Assert.That(ex.Message, Is.EqualTo(EntryNotDeleted));
    }

    [Test]
    public async Task StartProcessingAsync_ShouldSetStartProcessingProperty_ToCurrentDate()
    {
        await entryService.StartProcessingAsync(waitingEntry.Id);

        var entityChange = await dbContext.EntityChanges
            .FirstAsync(change => int.Parse(change.EntityId) == waitingEntry.Id);

        Assert.That(entityChange.OldValue, Is.EqualTo(null));
        Assert.NotNull(waitingEntry.StartedProcessing);
        Assert.IsNull(waitingEntry.FinishedProcessing);
    }

    [Test]
    public void StartProcessingAsync_ThrowsKeyNotFoundException_WhenInvalidIdIsPassed()
    {
        var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            await entryService.StartProcessingAsync(InvalidEntryId);
        });

        Assert.That(ex.Message, Is.EqualTo(EntryWithIdNotFound));
    }

    [Test]
    public void StartProcessingAsync_ThrowsInvalidOperationException_WhenStartProcessingIsNotNull()
    {
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await entryService.StartProcessingAsync(processingEntry.Id);
        });

        Assert.That(ex.Message, Is.EqualTo($"{EntryHasAlreadyStartedProcessing} {processingEntry.Id}"));
    }

    [Test]
    public void StartProcessingAsync_ThrowsInvalidOperationException_WhenEntryHasAlreadyFinishedProcessing()
    {
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await entryService.StartProcessingAsync(finishedEntry.Id);
        });

        Assert.That(ex.Message, Is.EqualTo($"{EntryHasAlreadyFinishedProcessing} {finishedEntry.Id}"));
    }

    [Test]
    public async Task FinishProcessingAsync_ShouldSetFinishProcessingProperty_ToCurrentDate()
    {
        await entryService.FinishProcessingAsync(processingEntry.Id);

        var entityChange = await dbContext.EntityChanges
            .FirstAsync(change => int.Parse(change.EntityId) == processingEntry.Id);

        Assert.That(entityChange.OldValue, Is.EqualTo(null));
        Assert.NotNull(processingEntry.FinishedProcessing);
    }

    public void FinishProcessingAsync_ThrowsKeyNotFoundException_WhenInvalidIdIsPassed()
    {
        var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            await entryService.FinishProcessingAsync(InvalidEntryId);
        });

        Assert.That(ex.Message, Is.EqualTo(EntryWithIdNotFound));
    }

    [Test]
    public void FinishProcessingAsync_ThrowsInvalidOperationException_WhenFinishProcessingIsNotNull()
    {
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await entryService.FinishProcessingAsync(finishedEntry.Id);
        });

        Assert.That(ex.Message, Is.EqualTo($"{EntryHasAlreadyFinishedProcessing} {finishedEntry.Id}"));
    }

    [Test]
    public void FinishProcessingAsync_ThrowsInvalidOperationException_WhenEntryHasNotStartedProcessing()
    {
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await entryService.FinishProcessingAsync(waitingEntry.Id);
        });

        Assert.That(ex.Message, Is.EqualTo($"{EntryHasNotStartedProcessing} {waitingEntry.Id}"));
    }

    [Test]
    public async Task MoveAsync_ShouldMoveEntryToNewZoneSuccessfully()
    {
        await entryService.MoveAsync(processingEntry.Id, zone2.Id, mockUserService.Object.UserId);

        Assert.That(processingEntry.ZoneId, Is.EqualTo(zone2.Id));
        Assert.That(processingEntry.StartedProcessing, Is.Null);
    }

    [Test]
    public void MoveAsync_ThrowsKeyNotFoundException_WhenEntryWithIdNotFound()
    {
        var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            await entryService.MoveAsync(InvalidEntryId, zone2.Id, mockUserService.Object.UserId);
        });

        Assert.That(ex.Message, Is.EqualTo(EntryWithIdNotFound));
    }

    [Test]
    public void MoveAsync_ThrowsInvalidOperationException_WhenEntryHasFinishedProcessing()
    {
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await entryService.MoveAsync(finishedEntry.Id, zone2.Id, mockUserService.Object.UserId);
        });

        Assert.That(ex.Message, Is.EqualTo(EntryHasFinishedProcessingAndCannotBeMoved));
    }

    [Test]
    public void MoveAsync_ThrowsKeyNotFoundException_WhenZoneWithIdNotFound()
    {
        var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            await entryService.MoveAsync(waitingEntry.Id, InvalidZoneId, mockUserService.Object.UserId);
        });

        Assert.That(ex.Message, Is.EqualTo(ZoneWithIdNotFound));
    }

    [Test]
    public void MoveAsync_ThrowsInvalidOperationException_WhenNewZoneIdMatchesOldOne()
    {
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await entryService.MoveAsync(waitingEntry.Id, zone1.Id, mockUserService.Object.UserId);
        });

        Assert.That(ex.Message, Is.EqualTo(EntryCannotBeMovedToSameZone));
    }

    [Test]
    public async Task SplitAsync_ShouldSuccessfullySplitEntry_IntoNewZone()
    {
        const int ExpectedRemainingPallets = 1;
        const int CountToSplit = 3;

        await entryService.SplitAsync(processingEntry.Id, new EntrySplitDto() { Count = CountToSplit, NewZoneId = zone2.Id}, mockUserService.Object.UserId);

        Assert.NotNull(processingEntry.StartedProcessing);
        Assert.That(processingEntry.Pallets, Is.EqualTo(ExpectedRemainingPallets));
        Assert.IsTrue(await dbContext.Entries.AnyAsync(e => e.ZoneId == zone2.Id && e.Pallets == CountToSplit));
    }

    [Test]
    public async Task SplitAsync_ShouldSuccessfullySplitEntry_IntoSameZone()
    {
        const int ExpectedRemainingPallets = 1;
        const int CountToSplit = 3;

        await entryService.SplitAsync(processingEntry.Id, new EntrySplitDto() { Count = CountToSplit, NewZoneId = zone1.Id }, mockUserService.Object.UserId);

        Assert.NotNull(processingEntry.StartedProcessing);
        Assert.That(processingEntry.Pallets, Is.EqualTo(ExpectedRemainingPallets));
        Assert.IsTrue(await dbContext.Entries.AnyAsync(e => e.ZoneId == zone1.Id && e.Pallets == CountToSplit));
    }

    [Test]
    public void SplitAsync_ThrowsKeyNotFoundException_WhenEntryWithIdNotFound()
    {
        var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            await entryService.SplitAsync(InvalidEntryId, new EntrySplitDto() { Count = 1, NewZoneId = zone2.Id }, mockUserService.Object.UserId);
        });

        Assert.That(ex.Message, Is.EqualTo(EntryWithIdNotFound));
    }

    [Test]
    public void SplitAsync_ThrowsInvalidOperationException_WhenEntryHasFinishedProcessing()
    {
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await entryService.SplitAsync(finishedEntry.Id, new EntrySplitDto() { Count = 1, NewZoneId = zone1.Id }, mockUserService.Object.UserId);
        });

        Assert.That(ex.Message, Is.EqualTo(EntryHasFinishedProcessingAndCannotBeSplit));
    }

    [Test]
    public void SplitAsync_ThrowsKeyNotFoundException_WhenZoneWithIdNotFound()
    {
        var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            await entryService.SplitAsync(waitingEntry.Id, new EntrySplitDto () { Count = 1, NewZoneId = InvalidZoneId }, mockUserService.Object.UserId);
        });

        Assert.That(ex.Message, Is.EqualTo(ZoneWithIdNotFound));
    }

    [Test]
    public void SplitAsync_ThrowsInvalidOperationException_WhenCountIsNegative()
    {
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await entryService.SplitAsync(processingEntry.Id, new EntrySplitDto() { Count = -1, NewZoneId = zone2.Id }, mockUserService.Object.UserId);
        });

        Assert.That(ex.Message, Is.EqualTo(InsufficientAmountToSplit));
    }

    [Test]
    public void SplitAsync_ThrowsInvalidOperationException_WhenCountIsZero()
    {
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await entryService.SplitAsync(processingEntry.Id, new EntrySplitDto() { Count = 0, NewZoneId = zone2.Id }, mockUserService.Object.UserId);
        });

        Assert.That(ex.Message, Is.EqualTo(InsufficientAmountToSplit));
    }

    [Test]
    public void SplitAsync_ThrowsInvalidOperationException_WhenCountIsEqualToTheCountOfItemsTheEntryContains()
    {
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await entryService.SplitAsync(processingEntry.Id, new EntrySplitDto() { Count = 4, NewZoneId = zone2.Id }, mockUserService.Object.UserId);
        });

        Assert.That(ex.Message, Is.EqualTo(InsufficientAmountToSplit));
    }

    [Test]
    public void SplitAsync_ThrowsInvalidOperationException_WhenCountIsMoreThanTheCountOfItemsTheEntryContains()
    {
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await entryService.SplitAsync(processingEntry.Id, new EntrySplitDto() { Count = 5, NewZoneId = zone2.Id }, mockUserService.Object.UserId);
        });

        Assert.That(ex.Message, Is.EqualTo(InsufficientAmountToSplit));
    }
}
