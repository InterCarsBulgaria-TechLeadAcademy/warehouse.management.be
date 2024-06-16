using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs.Delivery;
using WarehouseManagement.Core.Enums;
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
                            ? DeliveryStatus.InProgress.ToString()
                            : DeliveryStatus.NotStarted.ToString()
                    : DeliveryStatus.None.ToString(),
                Entries = d
                    .Entries.Select(e => new DeliveryEntryDto()
                    {
                        Id = e.Id,
                        FinishedProccessing = e.FinishedProccessing,
                        StartedProccessing = e.StartedProccessing,
                        ZoneId = e.ZoneId
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
                            ? DeliveryStatus.InProgress.ToString()
                            : DeliveryStatus.NotStarted.ToString()
                    : DeliveryStatus.None.ToString(),
                Entries = d
                    .Entries.Select(e => new DeliveryEntryDto()
                    {
                        Id = e.Id,
                        FinishedProccessing = e.FinishedProccessing,
                        StartedProccessing = e.StartedProccessing,
                        ZoneId = e.ZoneId
                    })
                    .ToList()
            })
            .ToListAsync();

        return deliverise;
    }

    //   context.Deliveries.Select(x => new DeliveryDto()
    //   {
    //       Id = x.Id,
    //Name = x.Name,
    //Status = x.Entries.All(e => e.FinishedDate.HasValue)
    //           ? x.IsApproved
    //               ? Status.Approved
    //               : Status.Finished
    //           : x.Entries.Any(e => e.StartedDate.HasValue)
    //               ? Status.InProgress
    //               : Status.NotStarted,
    //StartedDate = x.Entries.Min(e => e.StartedDate),
    //FinishedDate = x.Entries.Max(e => e.FinishedDate),
}
