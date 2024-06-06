using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs.Zone;
using WarehouseManagement.Infrastructure.Data.Common;
using WarehouseManagement.Infrastructure.Data.Models;
using static WarehouseManagement.Common.MessageConstants.Keys.ZoneMessageKeys;

namespace WarehouseManagement.Core.Services;

public class ZoneService : IZoneService
{
    private readonly IRepository repository;

    public ZoneService(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task CreateAsync(ZoneFormDto model, string userId)
    {
        if (await ExistsByNameAsync(model.Name))
        {
            throw new ArgumentException(ZoneWithIdNotFound);
        }

        var zone = new Zone()
        {
            Name = model.Name,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = userId
        };

        await repository.AddAsync(zone);
        await repository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id, string userId)
    {
        if (!await ExistsByIdAsync(id))
        {
            throw new KeyNotFoundException(ZoneWithIdNotFound);
        }

        var zone = (await repository.GetByIdAsync<Zone>(id))!;

        if (zone.ZonesMarkers.Any())
        {
            var markers = zone.ZonesMarkers.Select(zm => zm.Marker.Name);

            throw new InvalidOperationException(ZoneHasMarkers);
        }

        if (zone.VendorsZones.Any())
        {
            var vendors = zone.VendorsZones.Select(vz => vz.Vendor.Name);

            throw new InvalidOperationException(ZoneHasVendors);
        }

        if (zone.Entries.Any())
        {
            throw new InvalidOperationException(ZoneHasEntries);
        }

        repository.SoftDelete(zone);
        await repository.SaveChangesAsync();
    }

    public async Task EditAsync(int id, ZoneFormDto model, string userId)
    {
        if (!await ExistsByIdAsync(id))
        {
            throw new KeyNotFoundException(ZoneWithIdNotFound);
        }

        if (!await ExistsByNameAsync(model.Name))
        {
            throw new ArgumentException(ZoneWithNameExist);
        }

        var zone = (await repository.GetByIdAsync<Zone>(id))!;

        zone.Name = model.Name;
        zone.LastModifiedAt = DateTime.UtcNow;
        zone.LastModifiedByUserId = userId;

        await repository.SaveChangesWithLogAsync();
    }

    public async Task<bool> ExistsByIdAsync(int id)
    {
        return await repository.GetByIdAsync<Zone>(id) != null;
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await repository.AllReadOnly<Zone>().AnyAsync(z => z.Name == name);
    }

    public async Task<IEnumerable<ZoneDto>> GetAllAsync()
    {
        return await repository
            .AllReadOnly<Zone>()
            .Select(z => new ZoneDto()
            {
                Id = z.Id,
                Name = z.Name,
                Markers = z.ZonesMarkers.Select(zm => new ZoneMarkerDto()
                {
                    MarkerId = zm.MarkerId,
                    MarkerName = zm.Marker.Name,
                }),
                Vendors = z.VendorsZones.Select(vz => new ZoneVendorDto()
                {
                    VendorId = vz.VendorId,
                    VendorName = vz.Vendor.Name,
                    VendorSystemNumber = vz.Vendor.SystemNumber
                }),
                Entries = z.Entries.Select(e => new ZoneEntryDto()
                {
                    Pallets = e.Pallets,
                    Packages = e.Packages,
                    Pieces = e.Pieces,
                    StartedProccessing = e.StartedProccessing,
                    FinishedProccessing = e.FinishedProccessing,
                    DeliveryId = e.DeliveryId
                })
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<ZoneDto>> GetAllWithDeletedAsync()
    {
        return await repository
            .AllWithDeletedReadOnly<Zone>()
            .Select(z => new ZoneDto()
            {
                Id = z.Id,
                Name = z.Name,
                Markers = z.ZonesMarkers.Select(zm => new ZoneMarkerDto()
                {
                    MarkerId = zm.MarkerId,
                    MarkerName = zm.Marker.Name,
                }),
                Vendors = z.VendorsZones.Select(vz => new ZoneVendorDto()
                {
                    VendorId = vz.VendorId,
                    VendorName = vz.Vendor.Name,
                    VendorSystemNumber = vz.Vendor.SystemNumber
                }),
                Entries = z.Entries.Select(e => new ZoneEntryDto()
                {
                    Pallets = e.Pallets,
                    Packages = e.Packages,
                    Pieces = e.Pieces,
                    StartedProccessing = e.StartedProccessing,
                    FinishedProccessing = e.FinishedProccessing,
                    DeliveryId = e.DeliveryId
                })
            })
            .ToListAsync();
    }

    public async Task<ZoneDto> GetByIdAsync(int id)
    {
        if (!await ExistsByIdAsync(id))
        {
            throw new KeyNotFoundException(ZoneWithIdNotFound);
        }

        var zone = (await repository.AllReadOnly<Zone>().FirstOrDefaultAsync(z => z.Id == id))!;

        return new ZoneDto()
        {
            Id = zone.Id,
            Name = zone.Name,
            Markers = zone.ZonesMarkers.Select(zm => new ZoneMarkerDto()
            {
                MarkerId = zm.MarkerId,
                MarkerName = zm.Marker.Name,
            }),
            Vendors = zone.VendorsZones.Select(vz => new ZoneVendorDto()
            {
                VendorId = vz.VendorId,
                VendorName = vz.Vendor.Name,
                VendorSystemNumber = vz.Vendor.SystemNumber
            }),
            Entries = zone.Entries.Select(e => new ZoneEntryDto()
            {
                Pallets = e.Pallets,
                Packages = e.Packages,
                Pieces = e.Pieces,
                StartedProccessing = e.StartedProccessing,
                FinishedProccessing = e.FinishedProccessing,
                DeliveryId = e.DeliveryId
            })
        };
    }

    public async Task<IEnumerable<ZoneEntryDto>> GetFinishedEntries(int zoneId)
    {
        var currentZone = await this.repository.GetByIdAsync<Zone>(zoneId);

        if (currentZone == null)
        {
            throw new KeyNotFoundException(ZoneWithIdNotFound);
        }

        var entries = currentZone.Entries.Where(e => e.FinishedProccessing != null);

        return entries.Select(e => new ZoneEntryDto()
        {
            Pallets = e.Pallets,
            Packages = e.Packages,
            Pieces = e.Pieces,
            StartedProccessing = e.StartedProccessing,
            FinishedProccessing = e.FinishedProccessing,
            DeliveryId = e.DeliveryId
        });
    }

    public async Task<IEnumerable<ZoneEntryDto>> GetProccessingEntries(int zoneId)
    {
        var currentZone = await this.repository.GetByIdAsync<Zone>(zoneId);

        if (currentZone == null)
        {
            throw new KeyNotFoundException(ZoneWithIdNotFound);
        }

        var entries = currentZone.Entries.Where(e =>
            e.StartedProccessing != null && e.FinishedProccessing == null
        );

        return entries.Select(e => new ZoneEntryDto()
        {
            Pallets = e.Pallets,
            Packages = e.Packages,
            Pieces = e.Pieces,
            StartedProccessing = e.StartedProccessing,
            FinishedProccessing = e.FinishedProccessing,
            DeliveryId = e.DeliveryId
        });
    }

    public async Task<IEnumerable<ZoneEntryDto>> GetWaitingEntries(int zoneId)
    {
        var currentZone = await this.repository.GetByIdAsync<Zone>(zoneId);

        if (currentZone == null)
        {
            throw new KeyNotFoundException(ZoneWithIdNotFound);
        }

        var entries = currentZone.Entries.Where(e =>
            e.StartedProccessing == null && e.FinishedProccessing == null
        );

        return entries.Select(e => new ZoneEntryDto()
        {
            Pallets = e.Pallets,
            Packages = e.Packages,
            Pieces = e.Pieces,
            StartedProccessing = e.StartedProccessing,
            FinishedProccessing = e.FinishedProccessing,
            DeliveryId = e.DeliveryId
        });
    }

    public async Task<string> RestoreAsync(int id)
    {
        var zone = await repository.All<Zone>().FirstOrDefaultAsync(v => v.Id == id);

        if (zone == null)
        {
            throw new KeyNotFoundException(ZoneWithIdNotFound);
        }

        if (!zone.IsDeleted)
        {
            throw new InvalidOperationException(ZoneNotDeleted);
        }

        if (await ExistsByNameAsync(zone.Name))
        {
            throw new InvalidOperationException(ZoneWithNameExist);
        }

        repository.UnDelete(zone);
        await repository.SaveChangesAsync();

        return zone.Name;
    }
}
