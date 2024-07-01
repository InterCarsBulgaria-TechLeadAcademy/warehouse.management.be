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

    public async Task CreateAsync(EntryFormDto model, string userId)
    {
        var entry = new Entry()
        {
            Pallets = model.Pallets,
            Packages = model.Packages,
            Pieces = model.Pieces,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = userId
        };

        await repository.AddAsync(entry);
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
        int? zoneId,
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
        if (!await ExistsByIdAsync(id))
        {
            throw new KeyNotFoundException(EntryWithIdNotFound);
        }

        var entry = await repository.AllReadOnly<Entry>().FirstAsync(e => e.Id == id);

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

        var entry = await repository.AllReadOnly<Entry>().FirstAsync(e => e.Id == entryId);
        var zoneExists = await repository.AllReadOnly<Zone>().AnyAsync(z => z.Id == zoneId);

        if (!zoneExists)
        {
            throw new KeyNotFoundException(ZoneWithIdNotFound);
        }

        entry.ZoneId = zoneId;
        await repository.SaveChangesWithLogAsync();
    }

    public async Task RestoreAsync(int id)
    {
        var entry = await repository.All<Entry>().FirstOrDefaultAsync(z => z.Id == id);

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

    public async Task StartProccessingAsync(int entryId)
    {
        if (!await ExistsByIdAsync(entryId))
        {
            throw new KeyNotFoundException(EntryWithIdNotFound);
        }

        var entry = (await repository.GetByIdAsync<Entry>(entryId))!;

        ValidateStartProccessingOfEntry(entry);

        entry.StartedProccessing = DateTime.UtcNow;
        await repository.SaveChangesWithLogAsync();
    }

    public async Task FinishProccessingAsync(int entryId)
    {
        if (await ExistsByIdAsync(entryId))
        {
            throw new KeyNotFoundException(EntryWithIdNotFound);
        }

        var entry = (await repository.GetByIdAsync<Entry>(entryId))!;

        ValidateFinishProccessingOfEntry(entry);

        entry.FinishedProccessing = DateTime.UtcNow;
        await repository.SaveChangesWithLogAsync();
    }

    private void ValidateStartProccessingOfEntry(Entry entry)
    {
        if (entry.StartedProccessing != null)
        {
            throw new InvalidOperationException($"{EntryHasAlreadyStartedProccessing} {entry.Id}");
        }
        else if (entry.FinishedProccessing != null)
        {
            throw new InvalidOperationException($"{EntryHasAlreadyFinishedProccessing} {entry.Id}");
        }
    }

    private void ValidateFinishProccessingOfEntry(Entry entry)
    {
        if (entry.FinishedProccessing != null)
        {
            throw new InvalidOperationException($"{EntryHasAlreadyFinishedProccessing} {entry.Id}");
        }
        else if (entry.StartedProccessing == null)
        {
            throw new InvalidOperationException($"{EntryHasNotStartedProccessing} {entry.Id}");
        }
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
}
