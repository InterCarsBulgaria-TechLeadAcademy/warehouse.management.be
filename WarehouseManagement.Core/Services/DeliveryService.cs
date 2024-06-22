using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Common.Statuses;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs.Delivery;
using WarehouseManagement.Infrastructure.Data.Common;
using WarehouseManagement.Infrastructure.Data.Models;

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

    public async Task<ICollection<DeliveryDto>> GetAllAsync()
    {
        var deliverise = await repository
            .AllReadOnly<Delivery>()
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

        return deliverise;
    }

    public async Task EditAsync(int id, DeliveryFormDto model, string userId)
    {
        var deliveryToEdit = await repository.GetByIdAsync<Delivery>(id);

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

        await repository.SaveChangesWithLogAsync();
    }
}
