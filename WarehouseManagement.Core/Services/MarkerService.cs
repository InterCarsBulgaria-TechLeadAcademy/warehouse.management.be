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
            await repository.SaveChangesWithLogAsync();
        }
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
}
