using Microsoft.EntityFrameworkCore;
using Moq;
using WarehouseManagement.Api.Services.Contracts;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs.Entry;
using WarehouseManagement.Core.Services;
using WarehouseManagement.Infrastructure.Data;
using WarehouseManagement.Infrastructure.Data.Common;
using WarehouseManagement.Infrastructure.Data.Models;
using WarehouseManagement.Common.Statuses;
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

        waitingEntry = new Entry
        {
            Id = 1,
            Pallets = 0,
            Packages = 6,
            Pieces = 0,
            Zone = zone1,
            StartedProccessing = null,
            FinishedProccessing = null,
            CreatedByUserId = "User1"
        };

        processingEntry = new Entry
        {
            Id = 2,
            Pallets = 4,
            Packages = 0,
            Pieces = 0,
            Zone = zone1,
            StartedProccessing = DateTime.UtcNow,
            FinishedProccessing = null,
            CreatedByUserId = "User1"
        };

        finishedEntry = new Entry
        {
            Id = 3,
            Pallets = 0,
            Packages = 0,
            Pieces = 8,
            Zone = zone2,
            StartedProccessing = DateTime.UtcNow.AddDays(-7),
            FinishedProccessing = DateTime.UtcNow,
            CreatedByUserId = "User1"
        };

        deletedEntry = new Entry
        {
            Id = 4,
            Pallets = 0,
            Packages = 0,
            Pieces = 5,
            Zone = zone1,
            StartedProccessing = null,
            FinishedProccessing = null,
            CreatedByUserId = "User1",
            IsDeleted = true
        };

        var entries = new List<Entry> { waitingEntry, processingEntry, finishedEntry, deletedEntry };
        var zones = new List<Zone> { zone1, zone2 };

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
        await entryService.CreateAsync(new EntryFormDto
        {
            Pallets = 0,
            Packages = 0,
            Pieces = 15
        }, mockUserService.Object.UserId);

        var addedEntry = await dbContext.Entries.FirstOrDefaultAsync(e => e.Pieces == 15);

        Assert.IsNotNull(addedEntry);
    }

    // Test should be refactored after merge of PR #30
    [Test]
    public void CreateAsync_ThrowsArgumentException_WhenEntryHasMoreThanOneTypeSet()
    {
        var entryDto = new EntryFormDto
        {
            Pallets = 0,
            Packages = 6,
            Pieces = 11
        };

        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await entryService.CreateAsync(entryDto, mockUserService.Object.UserId);
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
        var entries = await entryService.GetAllAsync();

        Assert.That(entries.Count(), Is.EqualTo(3));
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnAllEntries_WithTheProvidedStatus()
    {
        var entries = await entryService.GetAllAsync([EntryStatuses.Waiting]);

        Assert.That(entries.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task GetAllByZoneAsync_ShouldReturnAllEntries_ForTheGivenZone()
    {
        var entries = await entryService.GetAllByZoneAsync(1);

        Assert.That(entries.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetAllByZoneAsync_ShouldReturnAllEntries_WithTheProvidedStatus_ForTheGivenZone()
    {
        var entries = await entryService.GetAllByZoneAsync(1, [EntryStatuses.Waiting]);

        Assert.That(entries.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task GetAllWithDeletedAsync_ShouldReturnAllEntries_IncludingTheDeleted()
    {
        var entries = await entryService.GetAllWithDeletedAsync();

        Assert.That(entries.Count(), Is.EqualTo(4));
    }

    [Test]
    public async Task GetAllWithDeletedAsync_ShouldReturnAllEntries_IncludingTheDeleted_ForTheGivenZone_WheZoneIdIsProvided()
    {
        var entries = await entryService.GetAllWithDeletedAsync(1);

        Assert.That(entries.Count(), Is.EqualTo(3));
    }

    [Test]
    public async Task GetAllWithDeletedAsync_ShouldReturnAllEntries_IncludingTheDeleted_ForTheGivenZone_WithTheProvidedStatus()
    {
        var entries = await entryService.GetAllWithDeletedAsync(1, [EntryStatuses.Waiting]);

        Assert.That(entries.Count(), Is.EqualTo(2));
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

        Assert.That(ex.Message, Is.EqualTo(EntryWithIdNotFound));
    }

    [Test]
    public async Task MoveEntryToZoneWithId_ShouldSuccessfullyMoveEntry()
    {
        await entryService.MoveEntryToZoneWithId(waitingEntry.Id, zone2.Id);

        Assert.That(waitingEntry.ZoneId, Is.EqualTo(zone2.Id));
    }

    [Test]
    public void MoveEntryToZoneWithId_ThrowsKeyNotFoundException_WithInvalidEntryId()
    {
        var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            await entryService.MoveEntryToZoneWithId(InvalidEntryId, zone2.Id);
        });

        Assert.That(ex.Message, Is.EqualTo(EntryWithIdNotFound));
    }

    [Test]
    public void MoveEntryToZoneWithId_ThrowsKeyNotFoundException_WithInvalidZoneId()
    {
        var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            await entryService.MoveEntryToZoneWithId(processingEntry.Id, InvalidZoneId);
        });

        Assert.That(ex.Message, Is.EqualTo(ZoneWithIdNotFound));
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
}
