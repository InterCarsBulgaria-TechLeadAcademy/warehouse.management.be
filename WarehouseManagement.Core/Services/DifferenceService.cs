using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs;
using WarehouseManagement.Core.DTOs.Difference;
using WarehouseManagement.Infrastructure.Data.Common;
using WarehouseManagement.Infrastructure.Data.Models;
using static WarehouseManagement.Common.MessageConstants.Keys.DifferenceMessageKeys;

namespace WarehouseManagement.Core.Services;

public class DifferenceService : IDifferenceService
{
    private readonly IRepository repository;

    public DifferenceService(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task CreateAsync(DifferenceFormDto model)
    {
        var difference = new Difference()
        {
            ReceptionNumber = model.ReceptionNumber,
            InternalNumber = model.InternalNumber,
            ActiveNumber = model.ActiveNumber,
            Comment = model.Comment,
            Count = model.Count,
            Status = model.Status,
            TypeId = model.DifferenceTypeId,
            ZoneId = model.ZoneId,
            DeliveryId = model.DeliveryId
        };

        await repository.AddAsync(difference);
        await repository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        if (!await ExistsByIdAsync(id))
        {
            throw new KeyNotFoundException(DifferenceWithIdNotFound);
        }

        await repository.SoftDeleteById<Difference>(id);
        await repository.SaveChangesAsync();
    }

    public async Task EditAsync(int id, DifferenceFormDto model, string userId)
    {
        if (!await ExistsByIdAsync(id))
        {
            throw new KeyNotFoundException(DifferenceWithIdNotFound);
        }

        var difference = await repository.GetByIdAsync<Difference>(id);
        
        difference!.ReceptionNumber = model.ReceptionNumber;
        difference.InternalNumber = model.InternalNumber;
        difference.ActiveNumber = model.ActiveNumber;
        difference.Comment = model.Comment;
        difference.Count = model.Count;
        difference.Status = model.Status;
        difference.TypeId = model.DifferenceTypeId;
        difference.ZoneId = model.ZoneId;
        difference.DeliveryId = model.DeliveryId;
        difference.LastModifiedAt = DateTime.UtcNow;
        difference.LastModifiedByUserId = userId;

        await repository.SaveChangesWithLogAsync();
    }

    public async Task<bool> ExistsByIdAsync(int id)
    {
        return await repository.GetByIdAsync<Difference>(id) != null;
    }

    public async Task<IEnumerable<DifferenceDto>> GetAllAsync(PaginationParameters paginationParams)
    {
        var models = await repository
            .AllReadOnly<Difference>()
            .Select(d => new DifferenceDto()
            {
                Id = d.Id,
                ReceptionNumber = d.ReceptionNumber,
                InternalNumber = d.InternalNumber,
                ActiveNumber = d.ActiveNumber,
                Comment = d.Comment,
                Count = d.Count,
                Status = d.Status,
                Type = d.Type.Name,
                Zone = d.Zone.Name,
                DeliverySystemNumber = d.Delivery.SystemNumber
            }).ToListAsync();

        return models;
    }

    public async Task<IEnumerable<DifferenceDto>> GetAllWithDeletedAsync(PaginationParameters paginationParams)
    {
        var models = await repository
            .AllWithDeletedReadOnly<Difference>()
            .Select(d => new DifferenceDto()
            {
                Id = d.Id,
                ReceptionNumber = d.ReceptionNumber,
                InternalNumber = d.InternalNumber,
                ActiveNumber = d.ActiveNumber,
                Comment = d.Comment,
                Count = d.Count,
                Status = d.Status,
                Type = d.Type.Name,
                Zone = d.Zone.Name,
                DeliverySystemNumber = d.Delivery.SystemNumber
            }).ToListAsync();

        return models;
    }

    public async Task<DifferenceDto> GetByIdAsync(int id)
    {
        if (!await ExistsByIdAsync(id))
        {
            throw new KeyNotFoundException(DifferenceWithIdNotFound);
        }

        var model = await repository.GetByIdAsync<Difference>(id);

        return new DifferenceDto()
        {
            Id = id,
            ReceptionNumber = model!.ReceptionNumber,
            InternalNumber = model.InternalNumber,
            ActiveNumber = model.ActiveNumber,
            Comment = model.Comment,
            Count = model.Count,
            Status = model.Status,
            Type = model.Type.Name,
            Zone = model.Zone.Name,
            DeliverySystemNumber = model.Delivery.SystemNumber
        };
    }

    public async Task RestoreAsync(int id)
    {
        var difference = await repository.GetByIdAsync<Difference>(id);

        if (difference == null)
        {
            throw new KeyNotFoundException(DifferenceWithIdNotFound);
        }

        if (!difference.IsDeleted)
        {
            throw new InvalidOperationException(DifferenceNotDeleted);
        }

        repository.UnDelete(difference);
        await repository.SaveChangesAsync();
    }
}
