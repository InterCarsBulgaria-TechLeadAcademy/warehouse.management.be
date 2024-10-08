﻿using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WarehouseManagement.Common.Statuses;
using WarehouseManagement.Common.Utilities;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs;
using WarehouseManagement.Core.DTOs.Difference;
using WarehouseManagement.Core.Extensions;
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

    public async Task CreateAsync(DifferenceFormDto model, string userId)
    {
        var difference = new Difference()
        {
            ReceptionNumber = model.ReceptionNumber,
            InternalNumber = model.InternalNumber,
            ActiveNumber = model.ActiveNumber,
            Comment = model.Comment,
            Count = model.Count,
            Status = DifferenceStatus.Waiting,
            TypeId = model.DifferenceTypeId,
            ZoneId = model.ZoneId,
            DeliveryId = model.DeliveryId,
            CreatedByUserId = userId
        };

        await repository.AddAsync(difference);
        await repository.SaveChangesWithLogAsync();
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

    public async Task<PageDto<DifferenceDto>> GetAllAsync(PaginationParameters paginationParams)
    {
        Expression<Func<Difference, bool>> filter = v =>
            EF.Functions.Like(v.ReceptionNumber, $"%{paginationParams.SearchQuery}%")
            || EF.Functions.Like(v.InternalNumber, $"%{paginationParams.SearchQuery}%")
            || EF.Functions.Like(v.ActiveNumber, $"%{paginationParams.SearchQuery}%");

        var differences = await repository
            .AllReadOnly<Difference>()
            .Paginate(paginationParams, filter)
            .Select(d => new DifferenceDto()
            {
                Id = d.Id,
                ReceptionNumber = d.ReceptionNumber,
                InternalNumber = d.InternalNumber,
                ActiveNumber = d.ActiveNumber,
                Comment = d.Comment,
                AdminComment = d.AdminComment ?? string.Empty,
                Count = d.Count,
                Status = d.Status,
                Type = d.Type.Name,
                CreatedAt = UtcNowDateTimeStringFormatted.GetUtcNow(d.CreatedAt),
                Zone = d.Zone.Name,
                DeliverySystemNumber = d.Delivery.SystemNumber
            }).ToListAsync();

        var totalItems = repository.AllReadOnly<Difference>().Count();

        return new PageDto<DifferenceDto>()
        {
            Count = totalItems,
            Results = differences,
            HasPrevious = paginationParams.PageNumber > 1,
            HasNext = paginationParams.PageNumber * paginationParams.PageSize < totalItems
        };
    }

    public async Task<PageDto<DifferenceDto>> GetAllWithDeletedAsync(PaginationParameters paginationParams)
    {
        Expression<Func<Difference, bool>> filter = v =>
            EF.Functions.Like(v.ReceptionNumber, $"%{paginationParams.SearchQuery}%")
            || EF.Functions.Like(v.InternalNumber, $"%{paginationParams.SearchQuery}%")
            || EF.Functions.Like(v.ActiveNumber, $"%{paginationParams.SearchQuery}%");

        var differences = await repository
            .AllWithDeletedReadOnly<Difference>()
            .Paginate(paginationParams, filter)
            .Select(d => new DifferenceDto()
            {
                Id = d.Id,
                ReceptionNumber = d.ReceptionNumber,
                InternalNumber = d.InternalNumber,
                ActiveNumber = d.ActiveNumber,
                Comment = d.Comment,
                AdminComment = d.AdminComment ?? string.Empty,
                Count = d.Count,
                Status = d.Status,
                Type = d.Type.Name,
                CreatedAt = UtcNowDateTimeStringFormatted.GetUtcNow(d.CreatedAt),
                Zone = d.Zone.Name,
                DeliverySystemNumber = d.Delivery.SystemNumber
            }).ToListAsync();

        var totalItems = repository.AllReadOnly<Difference>().Count();

        return new PageDto<DifferenceDto>()
        {
            Count = totalItems,
            Results = differences,
            HasPrevious = paginationParams.PageNumber > 1,
            HasNext = paginationParams.PageNumber * paginationParams.PageSize < totalItems
        };
    }

    public async Task<DifferenceDto> GetByIdAsync(int id)
    {
        if (!await ExistsByIdAsync(id))
        {
            throw new KeyNotFoundException(DifferenceWithIdNotFound);
        }

        var model = await repository
            .AllReadOnly<Difference>()
            .Include(d => d.Type)
            .Include(d => d.Zone)
            .Include(d => d.Delivery)
            .FirstOrDefaultAsync(d => d.Id == id);

        return new DifferenceDto()
        {
            Id = id,
            ReceptionNumber = model!.ReceptionNumber,
            InternalNumber = model.InternalNumber,
            ActiveNumber = model.ActiveNumber,
            Comment = model.Comment,
            AdminComment = model.AdminComment ?? string.Empty,
            Count = model.Count,
            Status = model.Status,
            Type = model.Type.Name,
            CreatedAt = UtcNowDateTimeStringFormatted.GetUtcNow(model.CreatedAt),
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

    public async Task StartProcessing(int id)
    {
        var difference = await repository.GetByIdAsync<Difference>(id);

        if (difference == null)
        {
            throw new KeyNotFoundException(DifferenceWithIdNotFound);
        }

        if (difference.Status != DifferenceStatus.Waiting)
        {
            throw new InvalidOperationException(DifferenceCannotProceedToProcessing);
        }

        difference.Status = DifferenceStatus.Processing;

        await repository.SaveChangesWithLogAsync();
    }

    public async Task FinishProcessing(int id, DifferenceAdminCommentDto adminCommentDto)
    {
        var difference = await repository.GetByIdAsync<Difference>(id);

        if (difference == null)
        {
            throw new KeyNotFoundException(DifferenceWithIdNotFound);
        }

        if (difference.Status != DifferenceStatus.Processing)
        {
            throw new InvalidOperationException(DifferenceCannotBeFinished);
        }

        difference.Status = DifferenceStatus.Finished;
        difference.AdminComment = adminCommentDto.AdminComment;

        await repository.SaveChangesWithLogAsync();
    }

    public async Task NoDifferences(int id, DifferenceAdminCommentDto adminCommentDto)
    {
        var difference = await repository.GetByIdAsync<Difference>(id);

        if (difference == null)
        {
            throw new KeyNotFoundException(DifferenceWithIdNotFound);
        }

        if (difference.Status != DifferenceStatus.Processing)
        {
            throw new InvalidOperationException(DifferenceCannotBeFinished);
        }

        difference.Status = DifferenceStatus.NoDifferences;
        difference.AdminComment = adminCommentDto.AdminComment;

        await repository.SaveChangesWithLogAsync();
    }
}
