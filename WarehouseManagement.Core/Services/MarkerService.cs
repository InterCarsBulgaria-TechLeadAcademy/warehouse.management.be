using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs.Marker;
using WarehouseManagement.Infrastructure.Data.Common;
using WarehouseManagement.Infrastructure.Data.Models;

namespace WarehouseManagement.Core.Services;

public class MarkerService : IMarkerService
{
    private readonly IRepository repository;

    public MarkerService(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task AddAsync(MarkerFormDto model, string userId)
    {
        if (model != null)
        {
            var marker = new Marker
            {
                Name = model.Name,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = userId,
            };
            await repository.AddAsync(marker);
            await repository.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int id, string userId)
    {
        var marker = await repository.GetByIdAsync<Marker>(id);
        if (marker == null)
        {
            throw new KeyNotFoundException($"Marker with ID {id} not found.");
        }

        marker.IsDeleted = true;
        marker.DeletedAt = DateTime.UtcNow;
        marker.DeletedByUserId = userId;

        repository.SoftDelete(marker);
        await repository.SaveChangesAsync();
    }

    public async Task EditAsync(int id, MarkerFormDto model, string userId)
    {
        var marker = await repository.GetByIdAsync<Marker>(id);
        if (marker == null)
        {
            throw new KeyNotFoundException($"Marker with ID {id} not found.");
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
            .AnyAsync(m => m.Name.ToLower() == name.ToLower());
    }

    public async Task<IEnumerable<MarkerDto>> GetAllAsync()
    {
        return await repository
            .AllReadOnly<Marker>()
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
    }

    public async Task<MarkerDto?> GetByIdAsync(int id)
    {
        return await repository
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
    }

    public async Task RestoreAsync(int id, string userId)
    {
        var marker = await repository.GetByIdWithDeletedAsync<Marker>(id);

        if (marker == null)
        {
            throw new KeyNotFoundException($"Marker with ID {id} not found.");
        }

        marker.LastModifiedByUserId = userId; //TODO: Should the user that undeleted be assigned to LastModified user ?
        repository.UnDelete(marker);

        await repository.SaveChangesWithLogAsync(); //TODO: Should we log the Undeletion process or not ?
    }

    public async Task<bool> IsDeletedById(int id)
    {
        return await repository.AllWithDeleted<Marker>().AnyAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<MarkerDto>> GetDeletedMarkersAsync()
    {
        return await repository
            .AllWithDeletedReadOnly<Marker>()
            .Where(m => m.IsDeleted)
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
    }

    public async Task<List<string>> IsMarkerInUseAsync(int markerId)
    {
        var usages = new List<string>();

        if (await repository.AllReadOnly<DeliveryMarker>().AnyAsync(dm => dm.MarkerId == markerId))
        {
            usages.Add("DeliveryMarkers");
        }

        if (await repository.AllReadOnly<VendorMarker>().AnyAsync(vm => vm.MarkerId == markerId))
        {
            usages.Add("VendorMarkers");
        }

        if (await repository.AllReadOnly<ZoneMarker>().AnyAsync(zm => zm.MarkerId == markerId))
        {
            usages.Add("ZoneMarkers");
        }

        return usages;
    }
}
