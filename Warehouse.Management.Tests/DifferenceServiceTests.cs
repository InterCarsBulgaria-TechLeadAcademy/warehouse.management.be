using Microsoft.EntityFrameworkCore;
using Moq;
using WarehouseManagement.Api.Services.Contracts;
using WarehouseManagement.Common.Statuses;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs;
using WarehouseManagement.Core.DTOs.Difference;
using WarehouseManagement.Core.Services;
using WarehouseManagement.Infrastructure.Data;
using WarehouseManagement.Infrastructure.Data.Common;
using WarehouseManagement.Infrastructure.Data.Models;
using static WarehouseManagement.Common.MessageConstants.Keys.DifferenceMessageKeys;

namespace Warehouse.Management.Tests;

public class DifferenceServiceTests
{
    private const int InvalidDifferenceId = -1;

    private WarehouseManagementDbContext dbContext;
    private IDifferenceService differenceService;
    private Mock<IUserService> mockUserService;

    private Difference difference1;
    private Difference difference2;
    private Difference difference3;

    private DifferenceType diffType1;
    private DifferenceType diffType2;
    private DifferenceType diffType3;

    private Delivery delivery;

    private Zone zone1;
    private Zone zone2;

    [SetUp]
    public async Task SetUp()
    {
        mockUserService = new Mock<IUserService>();
        mockUserService.Setup(x => x.UserId).Returns("TestUser");

        var options = new DbContextOptionsBuilder<WarehouseManagementDbContext>()
            .UseInMemoryDatabase(
                databaseName: "WarehouseManagementTestDb" + Guid.NewGuid().ToString()
            )
            .Options;

        dbContext = new WarehouseManagementDbContext(options, mockUserService.Object);

        diffType1 = new DifferenceType()
        {
            Id = 1,
            Name = "DiffType1"
        };

        diffType2 = new DifferenceType()
        {
            Id = 2,
            Name = "DiffType2"
        };

        diffType3 = new DifferenceType()
        {
            Id = 3,
            Name = "DiffType3"
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

        var vendor = new Vendor
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
            Vendor = vendor
        };

        difference1 = new Difference
        {
            ReceptionNumber = "123456",
            InternalNumber = "ABC789",
            ActiveNumber = "XYZ123",
            Comment = "Sample comment",
            AdminComment = "Admin comment",
            Count = 10,
            Status = DifferenceStatus.Waiting,
            Type = diffType1,
            Delivery = delivery,
            Zone = zone1
        };

        difference2 = new Difference
        {
            ReceptionNumber = "133456",
            InternalNumber = "ATC789",
            ActiveNumber = "XYZ923",
            Comment = "Sample comment2",
            AdminComment = "Admin comment2",
            Count = -10,
            Status = DifferenceStatus.Processing,
            Type = diffType2,
            Delivery = delivery,
            Zone = zone1
        };

        difference3 = new Difference
        {
            ReceptionNumber = "127456",
            InternalNumber = "RRC789",
            ActiveNumber = "AYZ123",
            Comment = "Sample comment3",
            AdminComment = "Admin comment3",
            Count = 7,
            Status = DifferenceStatus.Finished,
            Type = diffType3,
            Delivery = delivery,
            Zone = zone2
        };

        var diffTypes = new List<DifferenceType>() { diffType1, diffType2, diffType3 };
        var zones = new List<Zone>() { zone1, zone2, };
        var differences = new List<Difference>() { difference1, difference2, difference3 };

        await dbContext.DifferenceTypes.AddRangeAsync(diffTypes);
        await dbContext.Zones.AddRangeAsync(zones);
        await dbContext.Vendors.AddAsync(vendor);
        await dbContext.Deliveries.AddAsync(delivery);
        await dbContext.Differences.AddRangeAsync(differences);

        await dbContext.SaveChangesAsync();

        differenceService = new DifferenceService(new Repository(dbContext, mockUserService.Object));
    }

    [TearDown]
    public async Task Teardown()
    {
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.DisposeAsync();
    }

    [Test]
    public async Task CreateAsync_ShouldSuccessfullyAddDifference()
    {
        const int ExpectedTotalDifferencesCount = 4;

        var model = new DifferenceFormDto()
        {
            ReceptionNumber = "133476",
            InternalNumber = "A7C789",
            ActiveNumber = "XYZ973",
            Comment = "Samplesss",
            Count = -18,
            DifferenceTypeId = diffType1.Id,
            DeliveryId = delivery.Id,
            ZoneId = zone1.Id
        };

        await differenceService.CreateAsync(model, mockUserService.Object.UserId);

        Assert.That(dbContext.Differences.Count(), Is.EqualTo(ExpectedTotalDifferencesCount));
    }

    [Test]
    public async Task DeleteAsync_ShouldDeleteDifferenceSuccessfully()
    {
        const int ExpectedTotalDifferencesCount = 2;

        await differenceService.DeleteAsync(difference1.Id);

        Assert.That(dbContext.Differences.Count(), Is.EqualTo(ExpectedTotalDifferencesCount));
    }

    [Test]
    public void DeleteAsync_ShouldThrowKeyNotFoundException_WhenInvalidIdIsProvided()
    {
        var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            await differenceService.DeleteAsync(InvalidDifferenceId);
        });

        Assert.That(ex.Message, Is.EqualTo(DifferenceWithIdNotFound));
    }

    [Test]
    public async Task EditAsync_ShouldSuccessfullyUpdateDifference()
    {
        const string UpdatedComment = "Updated comment";

        var model = new DifferenceFormDto()
        {
            ReceptionNumber = difference1.ReceptionNumber,
            InternalNumber = difference1.InternalNumber,
            ActiveNumber = difference1.ActiveNumber,
            Comment = UpdatedComment,
            Count = difference1.Count,
            DifferenceTypeId = difference1.TypeId,
            DeliveryId = difference1.DeliveryId,
            ZoneId = difference1.ZoneId
        };

        await differenceService.EditAsync(difference1.Id, model, mockUserService.Object.UserId);

        var updatedDifference = await dbContext.Differences.FindAsync(difference1.Id);

        Assert.That(updatedDifference.Comment, Is.EqualTo(UpdatedComment));
        Assert.That(updatedDifference.LastModifiedByUserId, Is.EqualTo(mockUserService.Object.UserId));
    }

    [Test]
    public void EditAsync_ShouldThrowKeyNotFoundException_WhenInvalidIdIsProvided()
    {
        var model = new DifferenceFormDto()
        {
            ReceptionNumber = "NewReception",
            InternalNumber = "NewInternal",
            ActiveNumber = "NewActive",
            Comment = "NewComment",
            Count = 5,
            DifferenceTypeId = diffType1.Id,
            DeliveryId = delivery.Id,
            ZoneId = zone1.Id
        };

        var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            await differenceService.EditAsync(InvalidDifferenceId, model, mockUserService.Object.UserId);
        });

        Assert.That(ex.Message, Is.EqualTo(DifferenceWithIdNotFound));
    }

    [Test]
    public async Task ExistsByIdAsync_ShouldReturnTrue_WhenDifferenceExists()
    {
        var exists = await differenceService.ExistsByIdAsync(difference1.Id);

        Assert.IsTrue(exists);
    }

    [Test]
    public async Task ExistsByIdAsync_ShouldReturnFalse_WhenDifferenceDoesNotExist()
    {
        var exists = await differenceService.ExistsByIdAsync(InvalidDifferenceId);

        Assert.IsFalse(exists);
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnAllDifferences()
    {
        var paginationParams = new PaginationParameters { PageNumber = 1, PageSize = 10 };

        var model = await differenceService.GetAllAsync(paginationParams);

        Assert.That(model.Results.Count(), Is.EqualTo(3));
    }

    [Test]
    public async Task GetAllWithDeletedAsync_ShouldReturnAllDifferencesIncludingDeleted()
    {
        const int ExpectedTotalDifferencesCount = 3;

        await differenceService.DeleteAsync(difference1.Id);

        var paginationParams = new PaginationParameters { PageNumber = 1, PageSize = 10 };

        var result = await differenceService.GetAllWithDeletedAsync(paginationParams);

        Assert.That(result.Results.Count(), Is.EqualTo(ExpectedTotalDifferencesCount));
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnCorrectDifference()
    {
        var result = await differenceService.GetByIdAsync(difference1.Id);

        Assert.That(result.Id, Is.EqualTo(difference1.Id));
    }

    [Test]
    public void GetByIdAsync_ShouldThrowKeyNotFoundException_WhenInvalidIdIsProvided()
    {
        var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            await differenceService.GetByIdAsync(InvalidDifferenceId);
        });

        Assert.That(ex.Message, Is.EqualTo(DifferenceWithIdNotFound));
    }

    [Test]
    public async Task RestoreAsync_ShouldRestoreDeletedDifference()
    {
        await differenceService.DeleteAsync(difference1.Id);

        await differenceService.RestoreAsync(difference1.Id);

        var restoredDifference = await dbContext.Differences.FindAsync(difference1.Id);

        Assert.That(restoredDifference.IsDeleted, Is.False);
    }

    [Test]
    public void RestoreAsync_ShouldThrowKeyNotFoundException_WhenInvalidIdIsProvided()
    {
        var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            await differenceService.RestoreAsync(InvalidDifferenceId);
        });

        Assert.That(ex.Message, Is.EqualTo(DifferenceWithIdNotFound));
    }

    [Test]
    public void RestoreAsync_ShouldThrowInvalidOperationException_WhenDifferenceIsNotDeleted()
    {
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await differenceService.RestoreAsync(difference1.Id);
        });

        Assert.That(ex.Message, Is.EqualTo(DifferenceNotDeleted));
    }

    [Test]
    public async Task StartProcessing_ShouldChangeStatusToProcessing()
    {
        await differenceService.StartProcessing(difference1.Id);

        var difference = await dbContext.Differences.FindAsync(difference1.Id);

        Assert.That(difference.Status, Is.EqualTo(DifferenceStatus.Processing));
    }

    [Test]
    public void StartProcessing_ShouldThrowKeyNotFoundException_WhenInvalidIdIsProvided()
    {
        var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            await differenceService.StartProcessing(InvalidDifferenceId);
        });

        Assert.That(ex.Message, Is.EqualTo(DifferenceWithIdNotFound));
    }

    [Test]
    public void StartProcessing_ShouldThrowInvalidOperationException_WhenStatusIsNotWaiting()
    {
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await differenceService.StartProcessing(difference2.Id);
        });

        Assert.That(ex.Message, Is.EqualTo(DifferenceCannotProceedToProcessing));
    }

    [Test]
    public async Task FinishProcessing_ShouldChangeStatusToFinishedAndAddAdminComment()
    {
        const string AdminComment = "Finished processing";

        await differenceService.StartProcessing(difference1.Id);

        await differenceService.FinishProcessing(new DifferenceAdminCommentDto() { DifferenceId = difference1.Id, AdminComment = AdminComment });
        
        var difference = await dbContext.Differences.FindAsync(difference1.Id);

        Assert.That(difference.Status, Is.EqualTo(DifferenceStatus.Finished));
        Assert.That(difference.AdminComment, Is.EqualTo(AdminComment));
    }

    [Test]
    public void FinishProcessing_ShouldThrowKeyNotFoundException_WhenInvalidIdIsProvided()
    {
        const string AdminComment = "Finished processing";

        var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            await differenceService.FinishProcessing(new DifferenceAdminCommentDto() { DifferenceId = InvalidDifferenceId, AdminComment = AdminComment });
        });

        Assert.That(ex.Message, Is.EqualTo(DifferenceWithIdNotFound));
    }

    [Test]
    public void FinishProcessing_ShouldThrowInvalidOperationException_WhenStatusIsNotProcessing()
    {
        const string AdminComment = "Finished processing";

        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await differenceService.FinishProcessing(new DifferenceAdminCommentDto() { DifferenceId = difference1.Id, AdminComment = AdminComment });
        });

        Assert.That(ex.Message, Is.EqualTo(DifferenceCannotBeFinished));
    }

    [Test]
    public async Task NoDifferences_ShouldChangeStatusToNoDifferencesAndAddAdminComment()
    {
        const string AdminComment = "No differences found";

        await differenceService.StartProcessing(difference1.Id);

        await differenceService.NoDifferences(new DifferenceAdminCommentDto() { DifferenceId = difference1.Id, AdminComment = AdminComment });

        var difference = await dbContext.Differences.FindAsync(difference1.Id);

        Assert.That(difference.Status, Is.EqualTo(DifferenceStatus.NoDifferences));
        Assert.That(difference.AdminComment, Is.EqualTo(AdminComment));
    }

    [Test]
    public void NoDifferences_ShouldThrowKeyNotFoundException_WhenInvalidIdIsProvided()
    {
        const string AdminComment = "No differences found";

        var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            await differenceService.NoDifferences(new DifferenceAdminCommentDto() { DifferenceId = InvalidDifferenceId, AdminComment = AdminComment });
        });

        Assert.That(ex.Message, Is.EqualTo(DifferenceWithIdNotFound));
    }

    [Test]
    public void NoDifferences_ShouldThrowInvalidOperationException_WhenStatusIsNotProcessing()
    {
        const string AdminComment = "No differences found";

        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await differenceService.NoDifferences(new DifferenceAdminCommentDto() { DifferenceId = difference1.Id, AdminComment = AdminComment });
        });

        Assert.That(ex.Message, Is.EqualTo(DifferenceCannotBeFinished));
    }
}
