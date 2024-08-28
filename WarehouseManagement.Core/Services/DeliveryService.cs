using System.Globalization;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Common.Enums;
using WarehouseManagement.Common.Statuses;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs;
using WarehouseManagement.Core.DTOs.Delivery;
using WarehouseManagement.Core.DTOs.Entry;
using WarehouseManagement.Core.DTOs.Zone;
using WarehouseManagement.Core.Extensions;
using WarehouseManagement.Infrastructure.Data.Common;
using WarehouseManagement.Infrastructure.Data.Models;
using static WarehouseManagement.Common.MessageConstants.Keys.DeliveryMessageKeys;

namespace WarehouseManagement.Core.Services;

public class DeliveryService : IDeliveryService
{
    private readonly IRepository repository;

    public DeliveryService(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task<DeliveryDetailsDto> GetByIdAsync(int id)
    {
        var delivery = await repository
            .AllReadOnly<Delivery>()
            .Where(d => d.Id == id)
            .Select(d => new DeliveryDetailsDto()
            {
                Id = d.Id,
                Cmr = d.Cmr,
                DeliveryTime = d.DeliveryTime.ToString("s") + "Z",
                Packages = d.Packages,
                Pallets = d.Pallets,
                Pieces = d.Pieces,
                ReceptionNumber = d.ReceptionNumber,
                SystemNumber = d.SystemNumber,
                TruckNumber = d.TruckNumber,
                VendorId = d.VendorId,
                VendorName = d.Vendor.Name,
                Status = d.Status.ToString(),
                CreatedAt = d.CreatedAt,
                ApprovedOn = d.ApprovedOn.HasValue ? d.ApprovedOn.Value.ToString("s") + "Z" : null,
                Entries = d
                    .Entries.Select(e => new DeliveryEntryDetailsDto()
                    {
                        Id = e.Id,
                        Packages = e.Packages,
                        Pallets = e.Pallets,
                        Pieces = e.Pieces,
                        FinishedProccessing = e.FinishedProcessing,
                        StartedProccessing = e.StartedProcessing,
                        ZoneName = e.Zone.Name
                    })
                    .ToList(),
                Markers = d
                    .DeliveriesMarkers.Where(dm => dm.DeliveryId == id)
                    .Select(dm => new DeliveryMarkerDto()
                    {
                        MarkerId = dm.MarkerId,
                        MarkerName = dm.Marker.Name
                    })
                    .ToList(),
                EntriesFinishedProcessing = d
                    .Entries
                    .Count(e => e.FinishedProcessing.HasValue),
                EntriesWaitingProcessing = d
                    .Entries
                    .Count(e => !e.FinishedProcessing.HasValue)
            })
            .FirstOrDefaultAsync();

        if (delivery == null)
        {
            throw new KeyNotFoundException(id.ToString());
        }

        return delivery;
    }

    public async Task<PageDto<DeliveryDto>> GetAllAsync(PaginationParameters paginationParams)
    {
        Expression<Func<Delivery, bool>> filter = v =>
            EF.Functions.Like(v.ReceptionNumber, $"%{paginationParams.SearchQuery}%")
            || EF.Functions.Like(v.SystemNumber, $"%{paginationParams.SearchQuery}%")
            || EF.Functions.Like(v.TruckNumber, $"%{paginationParams.SearchQuery}%");

        var deliveries = await repository
            .AllReadOnly<Delivery>()
            .Paginate(paginationParams, filter)
            .Select(d => new DeliveryDto()
            {
                Id = d.Id,
                ReceptionNumber = d.ReceptionNumber,
                SystemNumber = d.SystemNumber,
                DeliveryTime = d.DeliveryTime.ToString("s") + "Z",
                VendorName = d.Vendor.Name,
                Status = d.Status.ToString(),
                CreatedAt = d.CreatedAt,
                ApprovedOn = d.ApprovedOn.HasValue ? d.ApprovedOn.Value.ToString("s") + "Z" : null,
                Markers = d
                    .DeliveriesMarkers.Select(dm => new DeliveryMarkerDto()
                    {
                        MarkerId = dm.MarkerId,
                        MarkerName = dm.Marker.Name
                    })
                    .ToList(),
                EntriesFinishedProcessingDetails = GetEntriesProcessingDetails(d.Entries, true),
                EntriesWaitingProcessingDetails = GetEntriesProcessingDetails(d.Entries, false),
                EntriesFinishedProcessing = d
                    .Entries
                    .Count(e => e.FinishedProcessing.HasValue),
                EntriesWaitingProcessing = d
                    .Entries
                    .Count(e => !e.FinishedProcessing.HasValue)
            })
            .ToListAsync();

        var totalItems = repository.AllReadOnly<Delivery>().Count();

        return new PageDto<DeliveryDto>()
        {
            Count = totalItems,
            Results = deliveries,
            HasPrevious = paginationParams.PageNumber > 1,
            HasNext = paginationParams.PageNumber * paginationParams.PageSize < totalItems
        };
    }

    private static EntriesProcessingDetails GetEntriesProcessingDetails(ICollection<Entry> entries, bool isFinished)
    {
        var entriesToProcess = entries
            .Where(e => isFinished ? e.FinishedProcessing.HasValue : e.FinishedProcessing == null)
            .ToList();
        
        return new EntriesProcessingDetails()
        {
            Pallets = entriesToProcess
                .Sum(e => e.Pallets),
            Packages = entriesToProcess
                .Sum(e => e.Packages),
            Pieces = entriesToProcess
                .Sum(e => e.Pieces)
        };
    }

    public async Task EditAsync(int id, DeliveryFormDto model, string userId)
    {
        var deliveryToEdit = repository
            .All<Delivery>()
            .Where(d => d.Id == id)
            .Include(d => d.DeliveriesMarkers)
            .FirstOrDefault();

        if (deliveryToEdit == null)
        {
            throw new KeyNotFoundException();
        }

        deliveryToEdit.TruckNumber = model.TruckNumber;
        deliveryToEdit.SystemNumber = model.SystemNumber;
        deliveryToEdit.ReceptionNumber = model.ReceptionNumber;
        deliveryToEdit.VendorId = model.VendorId;
        deliveryToEdit.Cmr = model.Cmr;
        deliveryToEdit.Packages = model.Packages;
        deliveryToEdit.Pieces = model.Pieces;
        deliveryToEdit.Pallets = model.Pallets;
        deliveryToEdit.DeliveryTime = model.DeliveryTime;
        deliveryToEdit.LastModifiedAt = DateTime.UtcNow;
        deliveryToEdit.LastModifiedByUserId = userId;

        repository.DeleteRange(deliveryToEdit.DeliveriesMarkers);

        foreach (var markerId in model.Markers)
        {
            var newDeliveryMarker = new DeliveryMarker()
            {
                DeliveryId = deliveryToEdit.Id,
                MarkerId = markerId
            };

            deliveryToEdit.DeliveriesMarkers.Add(newDeliveryMarker);
        }

        await repository.SaveChangesWithLogAsync();
    }

    public async Task<int> AddAsync(DeliveryFormDto model, string userId)
    {
        var delivery = new Delivery()
        {
            Cmr = model.Cmr,
            TruckNumber = model.TruckNumber,
            ReceptionNumber = model.ReceptionNumber,
            SystemNumber = model.SystemNumber,
            VendorId = model.VendorId,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = userId,
            DeliveryTime = model.DeliveryTime,
            IsApproved = false,
            IsDeleted = false,
            Packages = model.Packages,
            Pallets = model.Pallets,
            Pieces = model.Pieces,
        };

        await repository.AddAsync(delivery);
        await repository.SaveChangesAsync();

        foreach (var markerId in model.Markers)
        {
            var newDeliveryMarker = new DeliveryMarker()
            {
                DeliveryId = delivery.Id,
                MarkerId = markerId
            };

            delivery.DeliveriesMarkers.Add(newDeliveryMarker);
        }

        await repository.SaveChangesAsync();

        return delivery.Id;
    }

    public async Task DeleteAsync(int id)
    {
        var deliveryToDelete = await RetrieveByIdAsync(id);

        repository.SoftDelete(deliveryToDelete);

        await repository.SaveChangesAsync();
    }

    public async Task RestoreAsync(int id)
    {
        var delivery = await repository
            .AllWithDeleted<Delivery>()
            .FirstOrDefaultAsync(d => d.Id == id);

        if (delivery == null)
        {
            throw new KeyNotFoundException($"{DeliveryWithIdNotFound} {id}");
        }

        if (!delivery.IsDeleted)
        {
            throw new InvalidOperationException($"{DeliveryNotDeleted} {id}");
        }

        repository.UnDelete(delivery);

        await repository.SaveChangesAsync();
    }

    public async Task<ICollection<DeliveryDto>> GetAllDeletedAsync()
    {
        var deliveries = await repository
            .AllWithDeletedReadOnly<Delivery>()
            .Where(d => d.IsDeleted)
            .Select(d => new DeliveryDto()
            {
                Id = d.Id,
                ReceptionNumber = d.ReceptionNumber,
                SystemNumber = d.SystemNumber,
                DeliveryTime = d.DeliveryTime.ToString("s") + "Z",
                VendorName = d.Vendor.Name,
                Status = d.Status.ToString(),
                CreatedAt = d.CreatedAt,
                ApprovedOn = d.ApprovedOn.HasValue ? d.ApprovedOn.Value.ToString("s") + "Z" : null,
                Markers = d
                    .DeliveriesMarkers.Select(dm => new DeliveryMarkerDto()
                    {
                        MarkerId = dm.MarkerId,
                        MarkerName = dm.Marker.Name
                    })
                    .ToList()
            })
            .ToListAsync();

        return deliveries;
    }

    public async Task ApproveAsync(int id)
    {
        var deliveryToApprove = await repository
            .All<Delivery>()
            .Include(d => d.Entries)
            .Where(d => d.Id == id)
            .FirstOrDefaultAsync();

        if (deliveryToApprove == null)
        {
            throw new KeyNotFoundException($"{DeliveryWithIdNotFound} {id}");
        }

        var entriesAreFinished = AreAllEntriesFinished(deliveryToApprove.Entries);
        
        if (entriesAreFinished == false)
        {
            throw new ArgumentException(DeliveryHasNotFinishedEntries);
        }

        if (deliveryToApprove.IsApproved == true)
        {
            throw new InvalidOperationException(DeliveryIsAlreadyApproved);
        }

        deliveryToApprove.IsApproved = true;
        deliveryToApprove.ApprovedOn = DateTime.UtcNow;
        deliveryToApprove.Status = DeliveryStatus.Approved;

        await repository.SaveChangesWithLogAsync();
    }

    public async Task<DeliveryHistoryDto> GetHistoryAsync(int id)
    {
        var delivery = await RetrieveByIdAsync(id);

        var relatedEntriesIds = await repository
            .AllReadOnly<Entry>()
            .Where(e => e.DeliveryId == delivery.Id)
            .Select(e => e.Id)
            .ToListAsync();

        var changes = await repository.AllReadOnly<EntityChange>().ToListAsync();

        changes = changes
            .Where(change =>
                int.Parse(change.EntityId) == delivery.Id
                || relatedEntriesIds.Any(id => id == int.Parse(change.EntityId))
            )
            .ToList();

        var deliveryHistory = new DeliveryHistoryDto
        {
            Id = delivery.Id,
            Changes = changes
                .Select(change => new DeliveryHistoryDto.Change
                {
                    EntityId = int.Parse(change.EntityId),
                    PropertyName = change.PropertyName,
                    From = change.OldValue!,
                    To = change.NewValue!,
                    Type =
                        change.EntityName == "Delivery"
                            ? DeliveryHistoryChangeType.Delivery
                            : DeliveryHistoryChangeType.Entry,
                    LogType =
                        (
                            change.PropertyName == "StartedProcessing"
                            || change.PropertyName == "FinishedProcessing"
                        )
                        && change.EntityName == "Entry"
                            ? LogType.EntryStatusChange
                            : change.PropertyName == "Status" && change.EntityName == "Delivery"
                                ? LogType.DeliveryStatusChange
                                : change.PropertyName == "ZoneId"
                                    ? LogType.ZoneChange
                                    : LogType.Split, // May be refactored based on the Move functionality
                    ChangeDate = change.ChangedAt
                })
                .ToList()
        };

        return deliveryHistory;
    }

    public async Task ChangeDeliveryStatusIfNeeded(int id)
    {
        var delivery = await RetrieveByIdAsync(id);

        DeliveryStatus expectedStatus = GetExpectedStatus(delivery);

        if (delivery.Status != expectedStatus)
        {
            delivery.Status = expectedStatus;

            await SetDatesForDeliveryAsync(delivery, expectedStatus);
        }
    }

    private async Task<Delivery> RetrieveByIdAsync(int id)
    {
        var delivery = await repository
            .All<Delivery>()
            .Include(d => d.Entries)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (delivery == null)
        {
            throw new KeyNotFoundException(DeliveryWithIdNotFound);
        }

        return delivery;
    }

    private DeliveryStatus GetExpectedStatus(Delivery delivery)
    {
        if (AreAllEntriesWaiting(delivery.Entries))
        {
            return DeliveryStatus.Waiting;
        }
        else if (IsProcessingInProgress(delivery.Entries))
        {
            return DeliveryStatus.Processing;
        }
        else if (AreAllEntriesFinished(delivery.Entries))
        {
            return DeliveryStatus.Finished;
        }
        else
        {
            throw new InvalidOperationException("The delivery entries are in an invalid state.");
        }
    }

    private bool AreAllEntriesWaiting(ICollection<Entry> entries)
    {
        return entries.All(e => e.StartedProcessing == null);
    }

    private bool IsProcessingInProgress(ICollection<Entry> entries)
    {
        return entries.Any(e => e.StartedProcessing != null)
            && entries.Any(e => e.FinishedProcessing == null);
    }

    private bool AreAllEntriesFinished(ICollection<Entry> entries)
    {
        if(entries.Count == 0)
        {
            return false;
        }
        
        return entries.All(e => e.FinishedProcessing != null);
    }

    private async Task SetDatesForDeliveryAsync(Delivery delivery, DeliveryStatus status)
    {
        if (status == DeliveryStatus.Waiting)
        {
            delivery.StartedProcessing = null;
            delivery.FinishedProcessing = null; // Assure that delivery is not in finished state
        }
        else if (status == DeliveryStatus.Processing)
        {
            delivery.FinishedProcessing = null; // Assure that delivery is not in finished state

            var startedProcessing = delivery
                .Entries.OrderBy(e => e.StartedProcessing)
                .First()
                .StartedProcessing;

            delivery.StartedProcessing = startedProcessing;
        }
        else if (status == DeliveryStatus.Finished)
        {
            var startedProcessing = delivery
                .Entries.OrderBy(e => e.StartedProcessing)
                .First()
                .StartedProcessing;

            delivery.StartedProcessing = startedProcessing; // TODO: Ask if necessary

            var finishedProcessing = delivery
                .Entries.OrderByDescending(e => e.FinishedProcessing)
                .First()
                .FinishedProcessing;

            delivery.FinishedProcessing = finishedProcessing;
        }
        else
        {
            // TODO: Handle Approve if needed (probably approve will be handled by another method ApproveDelivery() and will not be part of this functionality)
            throw new InvalidOperationException(
                "Operation for delivery with status 'Approved' currently not supported."
            );
        }

        await repository.SaveChangesWithLogAsync();
    }

    public async Task<DeliveryPDFModelDto> GetPDFModel(int id)
    {
        var delivery = await repository
            .All<Delivery>()
            .Where(d => d.Id == id)
            .Select(d => new DeliveryPDFModelDto
            {
                Id = d.Id,
                DeliveredOn = d.DeliveryTime,
                Packages = d.Packages,
                Pallets = d.Pallets,
                Pieces = d.Pieces,
                ReceptionNumber = d.ReceptionNumber,
                SystemNumber = d.SystemNumber,
                VendorName = d.Vendor.Name,
                VendorSystemNumber = d.Vendor.SystemNumber,
                Zones = d.Vendor.VendorsZones.Select(vz => vz.Zone.Name).ToList()
            })
            .FirstOrDefaultAsync();

        if (delivery == null)
        {
            throw new KeyNotFoundException($"{DeliveryWithIdNotFound} {id}");
        }

        return delivery;
    }
}
