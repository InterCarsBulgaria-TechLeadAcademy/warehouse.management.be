using Microsoft.EntityFrameworkCore;
using Moq;
using WarehouseManagement.Api.Services.Contracts;
using WarehouseManagement.Common.MessageConstants.Keys;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs;
using WarehouseManagement.Core.DTOs.Vendor;
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
            SystemNumber = "Sm1",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            CreatedByUserId = "user1"
        };
        vendor2 = new Vendor()
        {
            Id = 2,
            Name = "Vendor2",
            SystemNumber = "Sm2",
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            CreatedByUserId = "user2"
        };
        vendor3 = new Vendor()
        {
            Id = 3,
            Name = "Vendor3",
            SystemNumber = "Sm3",
            CreatedAt = DateTime.UtcNow.AddDays(-3),
            CreatedByUserId = "user3"
        };
        vendorWithMarkers = new Vendor()
        {
            Id = 4,
            Name = "vendorWithMarkers",
            SystemNumber = "Sm4",
            CreatedAt = DateTime.UtcNow.AddDays(-3),
            CreatedByUserId = "user3"
        };
        vendorWithZones = new Vendor()
        {
            Id = 5,
            Name = "vendorWithZones",
            SystemNumber = "Sm5",
            CreatedAt = DateTime.UtcNow.AddDays(-3),
            CreatedByUserId = "user3"
        };
        vendorWithZonesAndMarkes = new Vendor()
        {
            Id = 6,
            Name = "vendorWithZonesAndMarkes",
            SystemNumber = "Sm6",
            CreatedAt = DateTime.UtcNow.AddDays(-3),
            CreatedByUserId = "user3"
        };
        deletedVendor1 = new Vendor()
        {
            Id = 7,
            Name = "deletedVendor1",
            SystemNumber = "Sm7",
            CreatedAt = DateTime.UtcNow.AddDays(-3),
            CreatedByUserId = "user3",
            IsDeleted = true,
            DeletedAt = DateTime.UtcNow,
            DeletedByUserId = "adminUserId"
        };
        deletedVendor2 = new Vendor()
        {
            Id = 8,
            Name = "deletedVendor2",
            SystemNumber = "Sm8",
            CreatedAt = DateTime.UtcNow.AddDays(-3),
            CreatedByUserId = "user3",
            IsDeleted = true,
            DeletedAt = DateTime.UtcNow,
            DeletedByUserId = "adminUserId"
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
    public async Task GetByIdAsync_ShouldReturnDtoModelIfIdExist()
    {
        var existingId = vendor1.Id;
        var result = await vendorService.GetByIdAsync(existingId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(existingId));
        Assert.That(result.Name, Is.EqualTo(vendor1.Name));
    }

    [Test]
    public void GetByIdAsync_ShouldThrowKeyNotFoundExceptionIfVendorDoesNotExist()
    {
        var nonExistingId = -1;

        var ex = Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await vendorService.GetByIdAsync(nonExistingId)
        );

        Assert.That(
            ex.Message,
            Is.EqualTo($"{VendorMessageKeys.VendorWithIdNotFound} {nonExistingId}")
        );
    }

    [Test]
    public async Task GetAllAsync_ReturnsDtoCollection()
    {
        var paginationParamas = new PaginationParameters()
        {
            PageNumber = 1,
            PageSize = int.MaxValue,
            SearchQuery = null
        };

        var expectedCount = 6;
        var result = await vendorService.GetAllAsync(paginationParamas);

        Assert.That(result.Count, Is.EqualTo(expectedCount));
    }

    [Test]
    public async Task GetAllAsync_ReturnsFilteredDtoCollection_WhenPaginationIsApllied()
    {
        var paginationParamas1 = new PaginationParameters()
        {
            PageNumber = 1,
            PageSize = 2,
            SearchQuery = null
        };
        var paginationParamas2 = new PaginationParameters()
        {
            PageNumber = 1,
            PageSize = 3,
            SearchQuery = null
        };

        var result1 = await vendorService.GetAllAsync(paginationParamas1);
        var result2 = await vendorService.GetAllAsync(paginationParamas2);

        Assert.That(result1.Count, Is.LessThanOrEqualTo(paginationParamas1.PageSize));
        Assert.That(result2.Count, Is.LessThanOrEqualTo(paginationParamas2.PageSize));
    }

    [Test]
    public async Task GetAllAsync_ReturnsFilteredDtoCollection_WhenSearchByVendorNameIsApplied()
    {
        var searchedVendorSystemNumber = "Sm1";
        var searchTermCaseInsensitivy = "sM1";
        var searchTermCaseSensitivy = "Sm1";

        var result1 = await vendorService.GetAllAsync(
            new PaginationParameters()
            {
                PageNumber = 1,
                PageSize = 2,
                SearchQuery = searchTermCaseInsensitivy
            }
        );

        var result2 = await vendorService.GetAllAsync(
            new PaginationParameters()
            {
                PageNumber = 1,
                PageSize = 2,
                SearchQuery = searchTermCaseSensitivy
            }
        );

        Assert.IsTrue(result1.All(m => m.SystemNumber.Contains(searchedVendorSystemNumber)));
        Assert.IsTrue(result2.All(m => m.SystemNumber.Contains(searchedVendorSystemNumber)));
    }

    [Test]
    public async Task GetAllAsync_ReturnsFilteredDtoCollection_WhenSearchByVendorSystemNumberIsApplied()
    {
        var searchedVendorName = "Vendor1";
        var searchTermCaseInsensitivy = "veNdOr1";
        var searchTermCaseSensitivy = "Vendor1";

        var result1 = await vendorService.GetAllAsync(
            new PaginationParameters()
            {
                PageNumber = 1,
                PageSize = 2,
                SearchQuery = searchTermCaseInsensitivy
            }
        );

        var result2 = await vendorService.GetAllAsync(
            new PaginationParameters()
            {
                PageNumber = 1,
                PageSize = 2,
                SearchQuery = searchTermCaseSensitivy
            }
        );

        Assert.IsTrue(result1.All(m => m.Name.Contains(searchedVendorName)));
        Assert.IsTrue(result2.All(m => m.Name.Contains(searchedVendorName)));
    }

    [Test]
    public async Task AddAsync_ShouldAddNewVendorToDb()
    {
        var newVendor = new Vendor()
        {
            Name = "newVendor",
            SystemNumber = "newVendorSystemNumber",
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = "someUserId",
        };

        var vendorCountBeforeAdding = await dbContext.Vendors.CountAsync();
        await dbContext.AddAsync(newVendor);
        await dbContext.SaveChangesAsync();
        var result = await dbContext.Vendors.CountAsync();

        Assert.That(result, Is.GreaterThan(vendorCountBeforeAdding));
        Assert.That(result, Is.EqualTo(vendorCountBeforeAdding + 1));
    }

    [Test]
    public void AddAsync_ShouldThrowArgumentException_IfVendorWithNameAlreadyExist()
    {
        var newVendor = new VendorFormDto()
        {
            Name = vendor1.Name,
            SystemNumber = "newVendorSystemNumber"
        };

        var ex = Assert.ThrowsAsync<ArgumentException>(
            async () => await vendorService.AddAsync(newVendor, "userId")
        );

        Assert.That(
            ex.Message,
            Is.EqualTo($"{VendorMessageKeys.VendorWithNameExist} {newVendor.Name}")
        );
    }

    [Test]
    public void AddAsync_ShouldThrowArgumentException_IfVendorWithSystemNumberAlreadyExist()
    {
        var newVendor = new VendorFormDto()
        {
            Name = "someTestName",
            SystemNumber = vendor1.SystemNumber
        };

        var ex = Assert.ThrowsAsync<ArgumentException>(
            async () => await vendorService.AddAsync(newVendor, "userId")
        );

        Assert.That(
            ex.Message,
            Is.EqualTo($"{VendorMessageKeys.VendorWithSystemNumberExist} {newVendor.SystemNumber}")
        );
    }

    [TearDown]
    public async Task Teardown()
    {
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.DisposeAsync();
    }
}
