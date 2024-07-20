using Microsoft.EntityFrameworkCore;
using Moq;
using System.Numerics;
using WarehouseManagement.Api.Services.Contracts;
using WarehouseManagement.Common.Statuses;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.Services;
using WarehouseManagement.Infrastructure.Data;
using WarehouseManagement.Infrastructure.Data.Common;
using WarehouseManagement.Infrastructure.Data.Models;

namespace Warehouse.Management.Tests;

public class DifferenceServiceTests
{
    private WarehouseManagementDbContext dbContext;
    private IDifferenceService differenceService;
    private Mock<IUserService> mockUserService;

    private Difference difference1;
    private Difference difference2;
    private Difference difference3;

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

        var diffType1 = new DifferenceType()
        {
            Id = 1,
            Name = "DiffType1"
        };

        var diffType2 = new DifferenceType()
        {
            Id = 2,
            Name = "DiffType2"
        };

        var diffType3 = new DifferenceType()
        {
            Id = 3,
            Name = "DiffType3"
        };

        var zone1 = new Zone
        {
            Id = 1,
            Name = "Zone1",
            CreatedByUserId = "User1"
        };

        var zone2 = new Zone
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

        var delivery = new Delivery
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
}
