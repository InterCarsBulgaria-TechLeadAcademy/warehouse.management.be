using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Common.Statuses;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs.Entry;
using WarehouseManagement.Infrastructure.Data.Common;
using WarehouseManagement.Infrastructure.Data.Models;
using static WarehouseManagement.Common.MessageConstants.Keys.EntryMessageKey;
using static WarehouseManagement.Common.MessageConstants.Keys.ZoneMessageKeys;

namespace WarehouseManagement.Core.Services;

public class EntryService : IEntryService
{
    private readonly IRepository repository;

    public EntryService(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task CreateAsync(ICollection<EntryFormDto> model, string userId)
    {
        foreach (var entry in model)
        {
            if (HasExactlyOneTypeSet(entry) == false)
            {
                throw new ArgumentException(EntryCanHaveOnlyOneTypeSet);
            }
        }

        foreach (var entry in model)
        {
            await MapnNewEntriesAndAddToEntriesAsync(entry, userId);
        }

        await repository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id, string userId)
    {
        if (!await ExistsByIdAsync(id))
        {
            throw new KeyNotFoundException(EntryWithIdNotFound);
        }

        var entry = (await repository.GetByIdAsync<Entry>(id))!;

        repository.SoftDelete(entry);
        await repository.SaveChangesAsync();
    }

    public async Task EditAsync(int id, EntryFormDto model, string userId)
    {
        if (!await ExistsByIdAsync(id))
        {
            throw new KeyNotFoundException(EntryWithIdNotFound);
        }

        var entry = (await repository.GetByIdAsync<Entry>(id))!;

        entry.Pallets = model.Pallets;
        entry.Packages = model.Packages;
        entry.Pieces = model.Pieces;

        await repository.SaveChangesWithLogAsync();
    }

    public async Task<bool> ExistsByIdAsync(int id)
    {
        return await repository.GetByIdAsync<Entry>(id) != null;
    }

    public async Task<IEnumerable<EntryDto>> GetAllAsync(EntryStatuses[]? statuses = null)
    {
        var query = BuildQuery(statuses);

        return await query
            .Select(e => new EntryDto()
            {
                Id = e.Id,
                Pallets = e.Pallets,
                Packages = e.Packages,
                Pieces = e.Pieces,
                StartedProccessing = e.StartedProccessing,
                FinishedProccessing = e.FinishedProccessing,
                ZoneId = e.ZoneId,
                DeliveryId = e.DeliveryId
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<EntryDto>> GetAllByZoneAsync(
        int zoneId,
        EntryStatuses[]? statuses = null
    )
    {
        var query = BuildQuery(statuses);

        var entries = await query
            .Where(e => e.ZoneId == zoneId)
            .Select(e => new EntryDto()
            {
                Id = e.Id,
                Pallets = e.Pallets,
                Packages = e.Packages,
                Pieces = e.Pieces,
                StartedProccessing = e.StartedProccessing,
                FinishedProccessing = e.FinishedProccessing,
                ZoneId = e.ZoneId,
                DeliveryId = e.DeliveryId
            })
            .ToListAsync();

        return entries;
    }

    public async Task<IEnumerable<EntryDto>> GetAllWithDeletedAsync(
        int? zoneId = null,
        EntryStatuses[]? statuses = null
    )
    {
        var query = BuildQuery(statuses, true);

        if (zoneId != null)
        {
            query = query.Where(e => e.ZoneId == zoneId);
        }

        return await query
            .Select(e => new EntryDto()
            {
                Id = e.Id,
                Pallets = e.Pallets,
                Packages = e.Packages,
                Pieces = e.Pieces,
                StartedProccessing = e.StartedProccessing,
                FinishedProccessing = e.FinishedProccessing,
                ZoneId = e.ZoneId,
                DeliveryId = e.DeliveryId
            })
            .ToListAsync();
    }

    public async Task<EntryDto> GetByIdAsync(int id)
    {
        var entry = await repository.AllReadOnly<Entry>().FirstOrDefaultAsync(e => e.Id == id);

        if (entry == null)
        {
            throw new KeyNotFoundException($"{EntryWithIdNotFound} {id}");
        }

        return new EntryDto()
        {
            Id = entry.Id,
            Pallets = entry.Pallets,
            Packages = entry.Packages,
            Pieces = entry.Pieces,
            StartedProccessing = entry.StartedProccessing,
            FinishedProccessing = entry.FinishedProccessing,
            ZoneId = entry.ZoneId,
            DeliveryId = entry.DeliveryId
        };
    }

    public async Task MoveEntryToZoneWithId(int entryId, int zoneId)
    {
        if (!await ExistsByIdAsync(entryId))
        {
            throw new KeyNotFoundException(EntryWithIdNotFound);
        }

        var entry = await repository.All<Entry>().FirstAsync(e => e.Id == entryId);
        var zone = await repository.All<Zone>().FirstOrDefaultAsync(z => z.Id == zoneId);

        if (zone == null)
        {
            throw new KeyNotFoundException(ZoneWithIdNotFound);
        }

        entry.Zone = zone;
        await repository.SaveChangesWithLogAsync();
    }

    public async Task RestoreAsync(int id)
    {
        var entry = await repository.AllWithDeleted<Entry>().FirstOrDefaultAsync(z => z.Id == id);

        if (entry == null)
        {
            throw new KeyNotFoundException(EntryWithIdNotFound);
        }

        if (!entry.IsDeleted)
        {
            throw new InvalidOperationException(EntryNotDeleted);
        }

        repository.UnDelete(entry);
        await repository.SaveChangesAsync();
    }

    private IQueryable<Entry> BuildQuery(EntryStatuses[]? statuses, bool withDeleted = false)
    {
        IQueryable<Entry> query;

        if (withDeleted)
        {
            query = repository.AllWithDeletedReadOnly<Entry>();
        }
        else
        {
            query = repository.AllReadOnly<Entry>();
        }

        if (statuses != null)
        {
            if (statuses.Contains(EntryStatuses.Waiting))
            {
                query = query.Where(e =>
                    e.StartedProccessing == null && e.FinishedProccessing == null
                );
            }

            if (statuses.Contains(EntryStatuses.Processing))
            {
                query = query.Where(e =>
                    e.StartedProccessing != null && e.FinishedProccessing == null
                );
            }

            if (statuses.Contains(EntryStatuses.Finished))
            {
                query = query.Where(e => e.FinishedProccessing != null);
            }
        }

        return query;
    }

    private bool HasExactlyOneTypeSet(EntryFormDto dto)
    {
        int count = 0;

        if (dto.Pallets > 0)
            count++;
        if (dto.Packages > 0)
            count++;
        if (dto.Pieces > 0)
            count++;

        return count == 1;
    }

    private async Task MapnNewEntriesAndAddToEntriesAsync(EntryFormDto entry, string userId)
    {
        Entry newEntry;

        if (entry.Pallets > 0)
        {
            newEntry = new Entry()
            {
                Pallets = entry.Pallets,
                Packages = 0,
                Pieces = 0,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = userId,
                DeliveryId = entry.DeliveryId,
                ZoneId = entry.ZoneId,
            };
        }
        else if (entry.Packages > 0)
        {
            newEntry = new Entry()
            {
                Pallets = 0,
                Packages = entry.Packages,
                Pieces = 0,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = userId,
                DeliveryId = entry.DeliveryId,
                ZoneId = entry.ZoneId,
            };
        }
        else
        {
            newEntry = new Entry()
            {
                Pallets = 0,
                Packages = 0,
                Pieces = entry.Pieces,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = userId,
                DeliveryId = entry.DeliveryId,
                ZoneId = entry.ZoneId,
            };
        }

        await repository.AddAsync(newEntry);
    }
}
