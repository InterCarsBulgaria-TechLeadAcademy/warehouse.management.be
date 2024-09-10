using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Common.Statuses;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs.Entry;
using WarehouseManagement.Infrastructure.Data.Common;
using WarehouseManagement.Infrastructure.Data.Models;
using WarehouseManagement.Core.DTOs;
using WarehouseManagement.Core.Extensions;
using static WarehouseManagement.Common.MessageConstants.Keys.EntryMessageKey;
using static WarehouseManagement.Common.MessageConstants.Keys.ZoneMessageKeys;
using static WarehouseManagement.Common.MessageConstants.Keys.DeliveryMessageKeys;
using WarehouseManagement.Core.DTOs.Zone;
using WarehouseManagement.Common.Utilities;

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
        if (!HasExactlyOneTypeSet(model))
        {
            throw new ArgumentException(EntryCanHaveOnlyOneTypeSet);
        }

        await MapAndAddNewEntryAsync(model, userId);

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

    public async Task<PageDto<EntryDto>> GetAllAsync(PaginationParameters paginationParams, EntryStatuses[]? statuses = null)
    {
        var query = BuildQuery(statuses);

        var count = await query.CountAsync();
            
        query = query.Paginate(paginationParams, null);

        var entries = await query
            .Select(e => new EntryDto()
            {
                Id = e.Id,
                Pallets = e.Pallets,
                Packages = e.Packages,
                Pieces = e.Pieces,
                StartedProccessing = e.StartedProcessing.HasValue ? UtcNowDateTimeStringFormatted.GetUtcNow(e.StartedProcessing.Value) : null,
                FinishedProccessing = e.FinishedProcessing.HasValue ? UtcNowDateTimeStringFormatted.GetUtcNow(e.FinishedProcessing.Value) : null,
                CreatedAt = UtcNowDateTimeStringFormatted.GetUtcNow(e.CreatedAt),
                Zone = new EntryZoneDto()
                {
                    ZoneName = e.Zone.Name,
                    IsFinal = e.Zone.IsFinal
                },
                DeliveryDetails = new EntryDeliveryDetailsDto()
                {
                    Id = e.DeliveryId,
                    ReceptionNumber = e.Delivery.ReceptionNumber,
                    SystemNumber = e.Delivery.SystemNumber,
                    VendorName = e.Delivery.Vendor.Name,
                }
                
            })
            .ToListAsync();

        return new PageDto<EntryDto>
        {
            Count = count,
            Results = entries,
            HasPrevious = paginationParams.PageNumber > 1,
            HasNext = paginationParams.PageNumber * paginationParams.PageSize < count
        };
    }

    public async Task<PageDto<EntryDto>> GetAllByZoneAsync(
        PaginationParameters paginationParams,
        int zoneId,
        EntryStatuses[]? statuses = null
    )
    {
        var query = BuildQuery(statuses)
            .Where(e => e.ZoneId == zoneId);

        var count = await query.CountAsync();

        query = query.Paginate(paginationParams, null);

        var entries = await query
            .Select(e => new EntryDto()
            {
                Id = e.Id,
                Pallets = e.Pallets,
                Packages = e.Packages,
                Pieces = e.Pieces,
                StartedProccessing = e.StartedProcessing.HasValue ? UtcNowDateTimeStringFormatted.GetUtcNow(e.StartedProcessing.Value) : null,
                FinishedProccessing = e.FinishedProcessing.HasValue ? UtcNowDateTimeStringFormatted.GetUtcNow(e.FinishedProcessing.Value) : null,
                CreatedAt = UtcNowDateTimeStringFormatted.GetUtcNow(e.CreatedAt),
                Zone = new EntryZoneDto()
                {
                    ZoneName = e.Zone.Name,
                    IsFinal = e.Zone.IsFinal
                },
                DeliveryDetails = new EntryDeliveryDetailsDto()
                {
                    Id = e.DeliveryId,
                    ReceptionNumber = e.Delivery.ReceptionNumber,
                    SystemNumber = e.Delivery.SystemNumber,
                    VendorName = e.Delivery.Vendor.Name,
                }
            })
            .ToListAsync();

        return new PageDto<EntryDto>
        {
            Count = count,
            Results = entries,
            HasPrevious = paginationParams.PageNumber > 1,
            HasNext = paginationParams.PageNumber * paginationParams.PageSize < count
        };
    }

    public async Task<PageDto<EntryDto>> GetAllWithDeletedAsync(
        PaginationParameters paginationParams,
        int? zoneId = null,
        EntryStatuses[]? statuses = null
    )
    {
        var query = BuildQuery(statuses, true);

        if (zoneId != null)
        {
            query = query.Where(e => e.ZoneId == zoneId);
        }

        var count = await query.CountAsync();

        query = query.Paginate(paginationParams, null);

        var entries = await query
            .Select(e => new EntryDto()
            {
                Id = e.Id,
                Pallets = e.Pallets,
                Packages = e.Packages,
                Pieces = e.Pieces,
                StartedProccessing = e.StartedProcessing.HasValue ? UtcNowDateTimeStringFormatted.GetUtcNow(e.StartedProcessing.Value) : null,
                FinishedProccessing = e.FinishedProcessing.HasValue ? UtcNowDateTimeStringFormatted.GetUtcNow(e.FinishedProcessing.Value) : null,
                CreatedAt = UtcNowDateTimeStringFormatted.GetUtcNow(e.CreatedAt),
                Zone = new EntryZoneDto()
                {
                    ZoneName = e.Zone.Name,
                    IsFinal = e.Zone.IsFinal
                },
                DeliveryDetails = new EntryDeliveryDetailsDto()
                {
                    Id = e.DeliveryId,
                    ReceptionNumber = e.Delivery.ReceptionNumber,
                    SystemNumber = e.Delivery.SystemNumber,
                    VendorName = e.Delivery.Vendor.Name,
                }
            })
            .ToListAsync();

        return new PageDto<EntryDto>
        {
            Count = count,
            Results = entries,
            HasPrevious = paginationParams.PageNumber > 1,
            HasNext = paginationParams.PageNumber * paginationParams.PageSize < count
        };
    }

    public async Task<EntryDto> GetByIdAsync(int id)
    {
        var entry = await repository
            .AllReadOnly<Entry>()
            .Include(e => e.Zone)
            .Include(e => e.Delivery)
            .ThenInclude(d => d.Vendor)
            .FirstOrDefaultAsync(e => e.Id == id);

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
            StartedProccessing = entry.StartedProcessing.HasValue ? UtcNowDateTimeStringFormatted.GetUtcNow(entry.StartedProcessing.Value) : null,
            FinishedProccessing = entry.FinishedProcessing.HasValue ? UtcNowDateTimeStringFormatted.GetUtcNow(entry.FinishedProcessing.Value) : null,
            CreatedAt = UtcNowDateTimeStringFormatted.GetUtcNow(entry.CreatedAt),
            Zone = new EntryZoneDto()
            {
                ZoneName = entry.Zone.Name,
                IsFinal = entry.Zone.IsFinal
            },
            DeliveryDetails = new EntryDeliveryDetailsDto()
            {
                Id = entry.DeliveryId,
                ReceptionNumber = entry.Delivery.ReceptionNumber,
                SystemNumber = entry.Delivery.SystemNumber,
                VendorName = entry.Delivery.Vendor.Name,
            }
        };
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

    public async Task StartProcessingAsync(int entryId)
    {
        if (!await ExistsByIdAsync(entryId))
        {
            throw new KeyNotFoundException(EntryWithIdNotFound);
        }

        var entry = (await repository.GetByIdAsync<Entry>(entryId))!;

        ValidateStartProcessingOfEntry(entry);

        entry.StartedProcessing = DateTime.UtcNow;

        await repository.SaveChangesWithLogAsync();
    }

    private void ValidateStartProcessingOfEntry(Entry entry)
    { 
        if (entry.FinishedProcessing != null)
        {
            throw new InvalidOperationException($"{EntryHasAlreadyFinishedProcessing} {entry.Id}");
        }
        else if (entry.StartedProcessing != null)
        {
            throw new InvalidOperationException($"{EntryHasAlreadyStartedProcessing} {entry.Id}");
        }
    }

    public async Task FinishProcessingAsync(int entryId)
    {
        if (!await ExistsByIdAsync(entryId))
        {
            throw new KeyNotFoundException(EntryWithIdNotFound);
        }

        var entry = (await repository.GetByIdAsync<Entry>(entryId))!;

        ValidateFinishProcessingOfEntry(entry);

        entry.FinishedProcessing = DateTime.UtcNow;

        await repository.SaveChangesWithLogAsync();
    }

    public async Task MoveAsync(int id, int newZoneId, string userId)
    {
        var entry = await repository.GetByIdAsync<Entry>(id);

        if (entry == null)
        {
            throw new KeyNotFoundException(EntryWithIdNotFound);
        }

        if (entry.FinishedProcessing != null)
        {
            throw new InvalidOperationException(EntryHasFinishedProcessingAndCannotBeMoved);
        }

        var newZone = await repository.GetByIdAsync<Zone>(newZoneId);

        if (newZone == null)
        {
            throw new KeyNotFoundException(ZoneWithIdNotFound);
        }

        if (newZoneId == entry.ZoneId)
        {
            throw new InvalidOperationException(EntryCannotBeMovedToSameZone);
        }

        entry.Zone = newZone;
        entry.StartedProcessing = null;
        entry.LastModifiedAt = DateTime.UtcNow;
        entry.LastModifiedByUserId = userId;

        await repository.SaveChangesWithLogAsync();
    }

    public async Task SplitAsync(int id, EntrySplitDto splitDto, string userId)
    {
        var entry = await repository.GetByIdAsync<Entry>(id);

        if (entry == null)
        {
            throw new KeyNotFoundException(EntryWithIdNotFound);
        }

        if (entry.FinishedProcessing != null)
        {
            throw new InvalidOperationException(EntryHasFinishedProcessingAndCannotBeSplit);
        }

        var zone = await repository.GetByIdAsync<Zone>(splitDto.NewZoneId);

        if (zone == null)
        {
            throw new KeyNotFoundException(ZoneWithIdNotFound);
        }

        var isCountValid = splitDto.Count > 0 &&
            (splitDto.Count < entry.Pallets ||
            splitDto.Count < entry.Packages ||
            splitDto.Count < entry.Pieces);

        if (!isCountValid)
        {
            throw new InvalidOperationException(InsufficientAmountToSplit);
        }

        var newEntry = SplitItemsAndCreateNewEntry(entry, splitDto.Count, splitDto.NewZoneId, userId);;

        await repository.AddAsync(newEntry);
        await repository.SaveChangesAsync();
    }

    private Entry SplitItemsAndCreateNewEntry(Entry entry, int countToSplit, int newZoneId, string userId)
    {
        var newEntry = new Entry()
        {
            ZoneId = newZoneId,
            DeliveryId = entry.DeliveryId,
            CreatedByUserId = userId
        };

        if (entry.Pallets > 0)
        {
            entry.Pallets -= countToSplit;
            newEntry.Pallets = countToSplit;
        }

        if (entry.Packages > 0)
        {
            entry.Packages -= countToSplit;
            newEntry.Packages = countToSplit;
        }

        if (entry.Pieces > 0)
        {
            entry.Pieces -= countToSplit;
            newEntry.Pieces = countToSplit;
        }

        entry.LastModifiedAt = DateTime.UtcNow;
        entry.LastModifiedByUserId = userId;

        return newEntry;
    }

    private void ValidateFinishProcessingOfEntry(Entry entry)
    {
        if (entry.FinishedProcessing != null)
        {
            throw new InvalidOperationException($"{EntryHasAlreadyFinishedProcessing} {entry.Id}");
        }
        else if (entry.StartedProcessing == null)
        {
            throw new InvalidOperationException($"{EntryHasNotStartedProcessing} {entry.Id}");
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
                    e.StartedProcessing == null && e.FinishedProcessing == null
                );
            }
            else if (statuses.Contains(EntryStatuses.Processing))
            {
                query = query.Where(e =>
                    e.StartedProcessing != null && e.FinishedProcessing == null
                );
            }
            else if (statuses.Contains(EntryStatuses.Finished))
            {
                query = query.Where(e => e.FinishedProcessing != null);
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

    private async Task MapAndAddNewEntryAsync(EntryFormDto entry, string userId)
    {
        var delivery = await repository.GetByIdAsync<Delivery>(entry.DeliveryId);
        var zone = await repository.GetByIdAsync<Zone>(entry.ZoneId);

        if (delivery == null)
        {
            throw new KeyNotFoundException(DeliveryWithIdNotFound);
        }

        if (zone == null)
        {
            throw new KeyNotFoundException(ZoneWithIdNotFound);
        }

        Entry newEntry;

        if (entry.Pallets > 0)
        {
            newEntry = new Entry()
            {
                Pallets = entry.Pallets,
                Packages = 0,
                Pieces = 0,
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
                CreatedByUserId = userId,
                DeliveryId = entry.DeliveryId,
                ZoneId = entry.ZoneId,
            };
        }

        await repository.AddAsync(newEntry);
    }
}
