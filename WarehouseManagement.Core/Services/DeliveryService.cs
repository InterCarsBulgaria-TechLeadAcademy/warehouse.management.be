using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Common.Statuses;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs;
using WarehouseManagement.Core.DTOs.Delivery;
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

    public async Task<DeliveryDto> GetByIdAsync(int id)
    {
        var delivery = await repository
            .AllReadOnly<Delivery>()
            .Where(d => d.Id == id)
            .Select(d => new DeliveryDto()
            {
                Id = d.Id,
                Cmr = d.Cmr,
                DeliveryTime = d.DeliveryTime,
                IsApproved = d.IsApproved,
                Packages = d.Packages,
                Pallets = d.Pallets,
                Pieces = d.Pieces,
                ReceptionNumber = d.ReceptionNumber,
                SystemNumber = d.SystemNumber,
                TruckNumber = d.TruckNumber,
                VendorId = d.VendorId,
                VendorName = d.Vendor.Name,
                Status = d.Entries.Any()
                    ? d.Entries.All(e => e.FinishedProccessing.HasValue)
                        ? d.IsApproved
                            ? DeliveryStatus.Approved.ToString()
                            : DeliveryStatus.Finished.ToString()
                        : d.Entries.Any(e => e.StartedProccessing.HasValue)
                            ? DeliveryStatus.Processing.ToString()
                            : DeliveryStatus.Waiting.ToString()
                    : DeliveryStatus.Waiting.ToString(),
                Entries = d
                    .Entries.Select(e => new DeliveryEntryDto()
                    {
                        Id = e.Id,
                        FinishedProccessing = e.FinishedProccessing,
                        StartedProccessing = e.StartedProccessing,
                        ZoneId = e.ZoneId
                    })
                    .ToList(),
                Markers = d
                    .DeliveriesMarkers.Where(dm => dm.DeliveryId == id)
                    .Select(dm => new DeliveryMarkerDto()
                    {
                        MarkerId = dm.MarkerId,
                        MarkerName = dm.Marker.Name
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (delivery == null)
        {
            throw new KeyNotFoundException(id.ToString());
        }

        return delivery;
    }

    public async Task<ICollection<DeliveryDto>> GetAllAsync(PaginationParameters paginationParams)
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
                Cmr = d.Cmr,
                DeliveryTime = d.DeliveryTime,
                IsApproved = d.IsApproved,
                Packages = d.Packages,
                Pallets = d.Pallets,
                Pieces = d.Pieces,
                ReceptionNumber = d.ReceptionNumber,
                SystemNumber = d.SystemNumber,
                TruckNumber = d.TruckNumber,
                VendorId = d.VendorId,
                VendorName = d.Vendor.Name,
                Status = d.Entries.Any()
                    ? d.Entries.All(e => e.FinishedProccessing.HasValue)
                        ? d.IsApproved
                            ? DeliveryStatus.Approved.ToString()
                            : DeliveryStatus.Finished.ToString()
                        : d.Entries.Any(e => e.StartedProccessing.HasValue)
                            ? DeliveryStatus.Processing.ToString()
                            : DeliveryStatus.Waiting.ToString()
                    : DeliveryStatus.Waiting.ToString(),
                Entries = d
                    .Entries.Select(e => new DeliveryEntryDto()
                    {
                        Id = e.Id,
                        FinishedProccessing = e.FinishedProccessing,
                        StartedProccessing = e.StartedProccessing,
                        ZoneId = e.ZoneId
                    })
                    .ToList(),
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
        deliveryToEdit.IsApproved = model.IsApproved;
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

    public async Task<int> AddASync(DeliveryFormDto model, string userId)
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

    public async Task DeleteASync(int id)
    {
        var deliveryToDelete = await repository.GetByIdAsync<Delivery>(id);

        if (deliveryToDelete == null)
        {
            throw new KeyNotFoundException($"{DeliveryWithIdNotFound} {id}");
        }

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
                Cmr = d.Cmr,
                DeliveryTime = d.DeliveryTime,
                IsApproved = d.IsApproved,
                Packages = d.Packages,
                Pallets = d.Pallets,
                Pieces = d.Pieces,
                ReceptionNumber = d.ReceptionNumber,
                SystemNumber = d.SystemNumber,
                TruckNumber = d.TruckNumber,
                VendorId = d.VendorId,
                VendorName = d.Vendor.Name,
                Status = d.Entries.Any()
                    ? d.Entries.All(e => e.FinishedProccessing.HasValue)
                        ? d.IsApproved
                            ? DeliveryStatus.Approved.ToString()
                            : DeliveryStatus.Finished.ToString()
                        : d.Entries.Any(e => e.StartedProccessing.HasValue)
                            ? DeliveryStatus.Processing.ToString()
                            : DeliveryStatus.Waiting.ToString()
                    : DeliveryStatus.Waiting.ToString(),
                Entries = d
                    .Entries.Select(e => new DeliveryEntryDto()
                    {
                        Id = e.Id,
                        FinishedProccessing = e.FinishedProccessing,
                        StartedProccessing = e.StartedProccessing,
                        ZoneId = e.ZoneId
                    })
                    .ToList(),
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
}
