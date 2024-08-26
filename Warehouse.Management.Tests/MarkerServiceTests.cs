using Microsoft.EntityFrameworkCore;
using Moq;
using WarehouseManagement.Api.Services.Contracts;
using WarehouseManagement.Common.MessageConstants.Keys;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs;
using WarehouseManagement.Core.DTOs.Marker;
using WarehouseManagement.Core.Services;
using WarehouseManagement.Infrastructure.Data;
using WarehouseManagement.Infrastructure.Data.Common;
using WarehouseManagement.Infrastructure.Data.Models;

namespace WarehouseManagement.Tests;

[TestFixture]
public class MarkerServiceTests
{
    private WarehouseManagementDbContext dbContext;
    private IRepository repository;
    private IMarkerService markerService;
    private Mock<WarehouseManagement.Api.Services.Contracts.IUserService> mockUserService;

    private Marker marker1;
    private Marker marker2;
    private Marker marker3;
    private Marker marker5;
    private Zone zone1;
    private Zone zone2;
    private Vendor vendor1;
    private Vendor vendor2;

    [SetUp]
    public async Task Setup()
    {
        mockUserService = new Mock<WarehouseManagement.Api.Services.Contracts.IUserService>();
        mockUserService.Setup(x => x.UserId).Returns("TestUser");
        var options = new DbContextOptionsBuilder<WarehouseManagementDbContext>()
            .UseInMemoryDatabase(
                databaseName: "WarehouseManagementTestDb" + Guid.NewGuid().ToString()
            )
            .Options;

        dbContext = new WarehouseManagementDbContext(options, mockUserService.Object);

        // Markers
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
        marker3 = new Marker
        {
            Id = 3,
            Name = "Marker3",
            CreatedByUserId = "User1"
        };
        marker5 = new Marker
        {
            Id = 5,
            Name = "Marker5",
            IsDeleted = true
        };

        // Zones
        zone1 = new Zone
        {
            Id = 1,
            Name = "Zone1",
            IsFinal = false
        };
        zone2 = new Zone
        {
            Id = 2,
            Name = "Zone2",
            IsFinal = true
        };

        // Vendors
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

        // Adding test data
        await dbContext.Markers.AddRangeAsync(
            new List<Marker> { marker1, marker2, marker3, marker5 }
        );
        await dbContext.Zones.AddRangeAsync(new List<Zone> { zone1, zone2 });
        await dbContext.Vendors.AddRangeAsync(new List<Vendor> { vendor1, vendor2 });

        // Adding mapping entities
        await dbContext.ZonesMarkers.AddAsync(
            new ZoneMarker { ZoneId = zone1.Id, MarkerId = marker1.Id }
        );
        await dbContext.VendorsMarkers.AddAsync(
            new VendorMarker { VendorId = vendor1.Id, MarkerId = marker1.Id }
        );

        await dbContext.SaveChangesAsync();

        repository = new Repository(dbContext, mockUserService.Object);
        markerService = new MarkerService(repository);
    }

    [TearDown]
    public async Task Teardown()
    {
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.DisposeAsync();
    }

    [Test]
    public async Task DeleteAsync_ThrowsKeyNotFoundException_WhenMarkerNotFound()
    {
        // Non-existent marker.
        int nonExistentMarkerId = 999;
        Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await markerService.DeleteAsync(nonExistentMarkerId)
        );
    }

    [Test]
    public async Task DeleteAsync_ThrowsInvalidOperationException_WhenMarkerHasUsages()
    {
        int usedMarkerId = 1; // Marker ID that already has usages.

        Assert.ThrowsAsync<InvalidOperationException>(
            async () => await markerService.DeleteAsync(usedMarkerId)
        );
    }

    [Test]
    public async Task DeleteAsync_Successful_WhenNoUsages()
    {
        int freeMarkerId = 2; //Marker ID without usages.
        await markerService.DeleteAsync(freeMarkerId);

        var deletedMarker = await dbContext.Markers.FindAsync(freeMarkerId);
        Assert.IsTrue(deletedMarker.IsDeleted);
    }

    [Test]
    public async Task AddAsync_ThrowsArgumentException_WhenNameExists()
    {
        //Name that already exists.
        var existingName = "Marker1";
        var newMarker = new MarkerFormDto { Name = existingName };

        var ex = Assert.ThrowsAsync<ArgumentException>(
            async () => await markerService.AddAsync(newMarker, "TestUser")
        );
        Assert.That(ex.Message, Is.EqualTo($"MarkerWithNameExist {newMarker.Name}"));
    }

    [Test]
    public async Task AddAsync_SuccessfullyAddsMarker_WhenNameDoesNotExist()
    {
        // New name that does not exist in the database.
        var uniqueName = "NewUniqueMarkerName";
        var newMarker = new MarkerFormDto { Name = uniqueName };

        int newMarkerId = await markerService.AddAsync(newMarker, "TestUser");

        var addedMarker = await dbContext.Markers.FindAsync(newMarkerId);
        Assert.IsNotNull(addedMarker);
        Assert.AreEqual(uniqueName, addedMarker.Name);
    }

    [Test]
    public async Task EditAsync_ThrowsKeyNotFoundException_WhenMarkerNotFound()
    {
        int nonExistentMarkerId = 999; //Non - existent marker ID.
        var model = new MarkerFormDto { Name = "NewName" };

        var ex = Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await markerService.EditAsync(nonExistentMarkerId, model, "User123")
        );
        Assert.That(
            ex.Message,
            Is.EqualTo($"{MarkerMessageKeys.MarkerWithIdNotFound} {nonExistentMarkerId}")
        );
    }

    [Test]
    public async Task EditAsync_ThrowsArgumentException_WhenNameExists()
    {
        int existingMarkerId = 1; //Marker ID that already exists.
        var model = new MarkerFormDto { Name = "Marker2" }; //Name that already exists.

        var ex = Assert.ThrowsAsync<ArgumentException>(
            async () => await markerService.EditAsync(existingMarkerId, model, "User123")
        );
        Assert.That(
            ex.Message,
            Is.EqualTo($"{MarkerMessageKeys.MarkerWithNameExist} {model.Name}")
        );
    }

    [Test]
    public async Task EditAsync_SuccessfullyUpdatesMarker_WhenValid()
    {
        int markerIdToUpdate = 1;
        var model = new MarkerFormDto { Name = "UpdatedName" };
        var originalMarker = await dbContext.Markers.FindAsync(markerIdToUpdate); //Debug

        await markerService.EditAsync(markerIdToUpdate, model, "User123");

        var updatedMarker = await dbContext.Markers.FindAsync(markerIdToUpdate);
        Assert.AreEqual("UpdatedName", updatedMarker.Name);
        Assert.AreEqual("User123", updatedMarker.LastModifiedByUserId);
        Assert.IsNotNull(updatedMarker.LastModifiedAt);
    }

    [Test]
    public async Task ExistByNameAsync_ReturnsTrue_WhenMarkerExistsAndIsNotDeleted()
    {
        var markerName = "Marker1"; // Existing marker that is not deleted.
        bool exists = await markerService.ExistByNameAsync(markerName);
        Assert.IsTrue(exists);
    }

    [Test]
    public async Task ExistByNameAsync_ReturnsFalse_WhenMarkerExistsButIsDeleted()
    {
        // Marker that is deleted.
        var deletedMarker = new Marker { Name = "DeletedMarker", IsDeleted = true };
        await dbContext.Markers.AddAsync(deletedMarker);
        await dbContext.SaveChangesAsync();

        bool exists = await markerService.ExistByNameAsync("DeletedMarker");
        Assert.IsFalse(exists);
    }

    [Test]
    public async Task ExistByNameAsync_ReturnsFalse_WhenMarkerDoesNotExist()
    {
        //Non-existing marker.
        var nonExistingName = "NonExistingMarker";
        bool exists = await markerService.ExistByNameAsync(nonExistingName);
        Assert.IsFalse(exists);
    }

    [Test]
    public async Task GetAllAsync_ReturnsFilteredMarkers_WhenSearchQueryIsProvided()
    {
        var paginationParams = new PaginationParameters { SearchQuery = "Marker1" };

        var results = await markerService.GetAllAsync(paginationParams);

        // Search query to check the filtered results contains on "Marker1".
        Assert.IsTrue(results.All(m => m.Name.Contains("Marker1")));
    }

    [Test]
    public async Task GetAllAsync_AppliesPagination_WhenParametersSpecified()
    {
        var paginationParams = new PaginationParameters
        {
            SearchQuery = "",
            PageNumber = 1,
            PageSize = 2
        };

        var results = await markerService.GetAllAsync(paginationParams);

        // Check if the number of returned results does not exceed the set PageSize.
        Assert.IsTrue(results.Count() <= 2);
    }

    [Test]
    public async Task GetByIdAsync_ReturnsMarkerDto_WhenMarkerExists()
    {
        int existingMarkerId = 1; //Existing Marker
        var result = await markerService.GetByIdAsync(existingMarkerId);

        Assert.IsNotNull(result);
        Assert.AreEqual("Marker1", result.Name);
    }

    [Test]
    public async Task GetByIdAsync_ThrowsKeyNotFoundException_WhenMarkerDoesNotExist()
    {
        int nonExistentMarkerId = 999; // Non-existing Id
        var ex = Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await markerService.GetByIdAsync(nonExistentMarkerId)
        );
        Assert.That(
            ex.Message,
            Is.EqualTo($"{MarkerMessageKeys.MarkerWithIdNotFound} {nonExistentMarkerId}")
        );
    }

    [Test]
    public async Task RestoreAsync_ThrowsKeyNotFoundException_WhenMarkerDoesNotExist()
    {
        int nonExistentMarkerId = 999; //Non-existing id
        Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await markerService.RestoreAsync(nonExistentMarkerId, "TestUser")
        );
    }

    [Test]
    public async Task RestoreAsync_ThrowsInvalidOperationException_WhenMarkerIsNotDeleted()
    {
        int existingMarkerId = 1; //Non-deleted marker
        Assert.ThrowsAsync<InvalidOperationException>(
            async () => await markerService.RestoreAsync(existingMarkerId, "TestUser")
        );
    }

    [Test]
    public async Task RestoreAsync_ThrowsArgumentException_WhenActiveMarkerWithSameNameExists()
    {
        var deletedMarker = new Marker //Deleted marker , but there is an active marker with the same name
        {
            Id = 4,
            Name = "Marker1",
            IsDeleted = true
        };

        await dbContext.Markers.AddAsync(deletedMarker);
        await dbContext.SaveChangesAsync();

        Assert.ThrowsAsync<ArgumentException>(
            async () => await markerService.RestoreAsync(deletedMarker.Id, "TestUser")
        );
    }

    [Test]
    public async Task RestoreAsync_SuccessfullyRestoresDeletedMarker()
    {
        var deletedMarkerId = 5; //Deleted marker and there isnt an active marker with the same name

        await markerService.RestoreAsync(deletedMarkerId, "TestUser");

        var restoredMarker = await dbContext.Markers.FindAsync(deletedMarkerId);
        Assert.IsFalse(restoredMarker.IsDeleted);
        Assert.AreEqual("TestUser", restoredMarker.LastModifiedByUserId);
    }

    [Test]
    public async Task GetDeletedMarkersAsync_ReturnsOnlyDeletedMarkers()
    {
        var deletedMarkers = await markerService.GetDeletedMarkersAsync();

        var expectedNames = new List<string> { "Marker5" };
        Assert.That(deletedMarkers, Is.EquivalentTo(expectedNames));
        Assert.That(deletedMarkers.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task GetDeletedMarkersAsync_DoesNotReturnActiveMarkers()
    {
        var deletedMarkers = await markerService.GetDeletedMarkersAsync();

        // Check if the existing marker are excluded
        Assert.IsFalse(deletedMarkers.Contains("Marker1"));
        Assert.IsFalse(deletedMarkers.Contains("Marker2"));
        Assert.IsFalse(deletedMarkers.Contains("Marker3"));
    }

    [Test]
    public async Task GetMarkerUsagesAsync_ReturnsAllUsages_WhenMarkerIsUsedEverywhere()
    {
        int markerId = 1; // This marker is used in zones and vendors, but not in deliveries.
        var usages = await markerService.GetMarkerUsagesAsync(markerId);

        Assert.IsFalse(usages.ContainsKey("Deliveries"));
        Assert.IsTrue(usages.ContainsKey("Zones"));
        Assert.IsTrue(usages.ContainsKey("Vendors"));
        Assert.That(usages["Zones"], Is.Not.Empty);
        Assert.That(usages["Vendors"], Is.Not.Empty);
    }

    [Test]
    public async Task GetMarkerUsagesAsync_ReturnsEmptyDictionary_WhenNoUsagesExist()
    {
        int markerId = 2; //This marker is not used anywhere.
        var usages = await markerService.GetMarkerUsagesAsync(markerId);

        Assert.IsEmpty(usages);
    }

    [Test]
    public async Task GetDeletedMarkerNameByIdAsync_ReturnsName_WhenMarkerIsDeleted()
    {
        int deletedMarkerId = 5; // ID of a deleted marker to check if the correct name is returned.
        var name = await markerService.GetDeletedMarkerNameByIdAsync(deletedMarkerId);

        Assert.AreEqual("Marker5", name);
    }

    [Test]
    public async Task GetDeletedMarkerNameByIdAsync_ReturnsNull_WhenMarkerIsNotDeleted()
    {
        int activeMarkerId = 1; // Non-deleted marker
        var name = await markerService.GetDeletedMarkerNameByIdAsync(activeMarkerId);

        Assert.IsNull(name); // ID of an active marker to check if null is returned since the marker is not deleted.
    }

    [Test]
    public async Task GetDeletedMarkerNameByIdAsync_ThrowsKeyNotFoundException_WhenMarkerDoesNotExist()
    {
        int nonExistentMarkerId = 999; // Non-existing id
        Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await markerService.GetDeletedMarkerNameByIdAsync(nonExistentMarkerId)
        );
    }
}
