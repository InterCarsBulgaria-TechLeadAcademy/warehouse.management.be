using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs;
using WarehouseManagement.Core.DTOs.Zone;
using WarehouseManagement.Core.Extensions;
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
            throw new ArgumentException(ZoneWithNameExists);
        }

        var zone = new Zone()
        {
            Name = model.Name,
            IsFinal = model.IsFinal ?? false,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = userId
        };

        await repository.AddAsync(zone);

        var markers = await repository
            .All<Marker>()
            .Where(m => model.MarkerIds.Contains(m.Id))
            .ToListAsync();

        foreach (var marker in markers)
        {
            await repository.AddAsync(new ZoneMarker()
            {
                Zone = zone,
                Marker = marker
            });
        }

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

        var zone = (await repository.GetByIdAsync<Zone>(id))!;

        if (await ExistsByNameAsync(model.Name) && model.Name != zone.Name)
        {
            throw new ArgumentException(ZoneWithNameExists);
        }

        zone.Name = model.Name;
        zone.IsFinal = model.IsFinal ?? false;
        zone.LastModifiedAt = DateTime.UtcNow;
        zone.LastModifiedByUserId = userId;

        var zoneMarkers = await repository
            .All<Marker>()
            .Where(m => model.MarkerIds.Contains(m.Id))
            .Select(m => new ZoneMarker
            {
                Zone = zone,
                Marker = m
            }).ToListAsync();

        zone.ZonesMarkers = zoneMarkers;

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
                IsFinal = z.IsFinal,
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
                    EntryId = e.Id,
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

    public async Task<IEnumerable<ZoneDto>> GetAllAsync(PaginationParameters paginationParams)
    {
        Expression<Func<Zone, bool>> filter = z =>
    EF.Functions.Like(z.Name, $"%{paginationParams.SearchQuery}%");

        return await repository
            .AllReadOnly<Zone>()
            .Paginate(paginationParams, filter)
            .Select(z => new ZoneDto()
            {
                Id = z.Id,
                Name = z.Name,
                IsFinal = z.IsFinal,
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
                    EntryId = e.Id,
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
                IsFinal = z.IsFinal,
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
                    EntryId = e.Id,
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

    public async Task<IEnumerable<ZoneDto>> GetAllWithDeletedAsync(PaginationParameters paginationParams)
    {
        Expression<Func<Zone, bool>> filter = z =>
    EF.Functions.Like(z.Name, $"%{paginationParams.SearchQuery}%");

        return await repository
            .AllWithDeletedReadOnly<Zone>()
            .Paginate(paginationParams, filter)
            .Select(z => new ZoneDto()
            {
                Id = z.Id,
                Name = z.Name,
                IsFinal = z.IsFinal,
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
                    EntryId = e.Id,
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

        var zone = (await repository.AllReadOnly<Zone>().FirstAsync(z => z.Id == id))!;

        return new ZoneDto()
        {
            Id = zone.Id,
            Name = zone.Name,
            IsFinal = zone.IsFinal,
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
                EntryId = e.Id,
                Pallets = e.Pallets,
                Packages = e.Packages,
                Pieces = e.Pieces,
                StartedProccessing = e.StartedProccessing,
                FinishedProccessing = e.FinishedProccessing,
                DeliveryId = e.DeliveryId
            })
        };
    }

    public async Task RestoreAsync(int id)
    {
        var zone = await repository.AllWithDeleted<Zone>().FirstOrDefaultAsync(v => v.Id == id);

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
            throw new InvalidOperationException(ZoneWithNameExists);
        }

        repository.UnDelete(zone);
        await repository.SaveChangesAsync();
    }
}
