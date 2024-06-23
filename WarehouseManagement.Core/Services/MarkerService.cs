using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs;
using WarehouseManagement.Core.DTOs.Marker;
using WarehouseManagement.Core.Extensions;
using WarehouseManagement.Infrastructure.Data.Common;
using WarehouseManagement.Infrastructure.Data.Models;
using static WarehouseManagement.Common.MessageConstants.Keys.MarkerMessageKeys;

namespace WarehouseManagement.Core.Services;

public class MarkerService : IMarkerService
{
    private readonly IRepository repository;

    public MarkerService(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task<int> AddAsync(MarkerFormDto model, string userId)
    {
        if (await ExistByNameAsync(model.Name))
        {
            throw new ArgumentException($"{MarkerWithNameExist} {model.Name}");
        }

        var marker = new Marker
        {
            Name = model.Name,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = userId,
        };

        await repository.AddAsync(marker);
        await repository.SaveChangesAsync();

        return marker.Id;
    }

    public async Task DeleteAsync(int id)
    {
        var marker = await repository.GetByIdAsync<Marker>(id);

        if (marker == null)
        {
            throw new KeyNotFoundException($"{MarkerWithIdNotFound} {id}");
        }

        var usages = await GetMarkerUsagesAsync(id);
        if (usages.Any())
        {
            var usageMessage =
                $"{MarkerHasUsages} "
                + string.Join(" ", usages.Select(u => $"{u.Key}: {string.Join(",", u.Value)}"));
            throw new InvalidOperationException(usageMessage);
        }

        repository.SoftDelete(marker);
        await repository.SaveChangesAsync();
    }

    public async Task EditAsync(int id, MarkerFormDto model, string userId)
    {
        var marker = await repository.GetByIdAsync<Marker>(id);

        if (marker == null)
        {
            throw new KeyNotFoundException($"{MarkerWithIdNotFound} {id}");
        }

        if (await ExistByNameAsync(model.Name))
        {
            throw new ArgumentException($"{MarkerWithNameExist} {model.Name}");
        }

        marker.Name = model.Name;
        marker.LastModifiedAt = DateTime.UtcNow;
        marker.LastModifiedByUserId = userId;

        await repository.SaveChangesWithLogAsync();
    }

    public async Task<bool> ExistByNameAsync(string name)
    {
        return await repository
            .AllReadOnly<Marker>()
            .AnyAsync(m => m.Name.ToLower() == name.ToLower() && !m.IsDeleted);
    }

    public async Task<IEnumerable<MarkerDto>> GetAllAsync(PaginationParameters paginationParams)
    {
        Expression<Func<Marker, bool>> filter = m =>
            EF.Functions.Like(m.Name, $"%{paginationParams.SearchQuery}%");
        //We can add more filters if we need more columns to search into

        var markers = await repository
            .AllReadOnly<Marker>()
            .Paginate(paginationParams, filter)
            .Select(m => new MarkerDto
            {
                Name = m.Name,
                Deliveries = m
                    .DeliveriesMarkers.Select(dm => new MarkerDeliveryDto
                    {
                        DeliveryId = dm.DeliveryId,
                        DeliverySystemNumber = dm.Delivery.SystemNumber
                    })
                    .ToList(),
                Vendors = m
                    .VendorsMarkers.Select(vm => new MarkerVendorDto
                    {
                        VendorId = vm.VendorId,
                        VendorName = vm.Vendor.Name,
                        VendorSystemNumber = vm.Vendor.SystemNumber
                    })
                    .ToList(),
                Zones = m
                    .ZonesMarkers.Select(zm => new MarkerZoneDto
                    {
                        ZoneId = zm.ZoneId,
                        ZoneName = zm.Zone.Name
                    })
                    .ToList()
            })
            .ToListAsync();

        return markers;
    }

    public async Task<MarkerDto> GetByIdAsync(int id)
    {
        var marker = await repository
            .AllReadOnly<Marker>()
            .Where(m => m.Id == id)
            .Select(m => new MarkerDto
            {
                Name = m.Name,
                Deliveries = m
                    .DeliveriesMarkers.Select(dm => new MarkerDeliveryDto
                    {
                        DeliveryId = dm.DeliveryId,
                        DeliverySystemNumber = dm.Delivery.SystemNumber
                    })
                    .ToList(),
                Vendors = m
                    .VendorsMarkers.Select(vm => new MarkerVendorDto
                    {
                        VendorId = vm.VendorId,
                        VendorName = vm.Vendor.Name,
                        VendorSystemNumber = vm.Vendor.SystemNumber
                    })
                    .ToList(),
                Zones = m
                    .ZonesMarkers.Select(zm => new MarkerZoneDto
                    {
                        ZoneId = zm.ZoneId,
                        ZoneName = zm.Zone.Name
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (marker == null)
        {
            throw new KeyNotFoundException($"{MarkerWithIdNotFound} {id}");
        }

        return marker;
    }

    public async Task RestoreAsync(int id, string userId)
    {
        var marker = await repository.AllWithDeleted<Marker>().FirstOrDefaultAsync(m => m.Id == id);

        if (marker == null)
        {
            throw new KeyNotFoundException($"{MarkerWithIdNotFound} {id}");
        }

        if (!marker.IsDeleted)
        {
            throw new InvalidOperationException($"{MarkerNotDeleted} {id}");
        }

        if (await ExistByNameAsync(marker.Name))
        {
            throw new ArgumentException($"{MarkerWithNameExist} {marker.Name}");
        }

        marker.LastModifiedByUserId = userId;
        repository.UnDelete(marker);

        await repository.SaveChangesAsync();
    }

    public async Task<IEnumerable<string>> GetDeletedMarkersAsync()
    {
        return await repository
            .AllWithDeletedReadOnly<Marker>()
            .Where(m => m.IsDeleted)
            .Select(m => m.Name)
            .ToListAsync();
    }

    public async Task<Dictionary<string, List<string>>> GetMarkerUsagesAsync(int id)
    {
        var usages = new Dictionary<string, List<string>>();

        var deliveryMarkers = await repository
            .AllReadOnly<DeliveryMarker>()
            .Where(dm => dm.MarkerId == id)
            .Include(dm => dm.Delivery)
            .Select(dm => dm.Delivery.SystemNumber)
            .ToListAsync();

        if (deliveryMarkers.Any())
        {
            usages.Add("Deliveries", deliveryMarkers);
        }

        var zoneMarkers = await repository
            .AllReadOnly<ZoneMarker>()
            .Where(zm => zm.MarkerId == id)
            .Include(zm => zm.Zone)
            .Select(zm => zm.Zone.Name)
            .ToListAsync();
        if (zoneMarkers.Any())
        {
            usages.Add("Zones", zoneMarkers);
        }

        var vendorMarkers = await repository
            .AllReadOnly<VendorMarker>()
            .Where(vm => vm.MarkerId == id)
            .Include(vm => vm.Vendor)
            .Select(vm => vm.Vendor.SystemNumber)
            .ToListAsync();
        if (vendorMarkers.Any())
        {
            usages.Add("Vendors", vendorMarkers);
        }

        return usages;
    }

    public async Task<string?> GetDeletedMarkerNameByIdAsync(int id)
    {
        var marker = await repository.AllWithDeleted<Marker>().FirstOrDefaultAsync(m => m.Id == id);

        if (marker == null)
        {
            throw new KeyNotFoundException($"Marker with ID {id} not found.");
        }

        if (marker.IsDeleted)
        {
            return marker.Name;
        }

        return null;
    }

    public async Task<bool> ExistById(int id)
    {
        return await repository.AllReadOnly<Marker>().AnyAsync(m => m.Id == id);
    }
}
