using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs.DifferenceType;
using WarehouseManagement.Infrastructure.Data.Common;
using WarehouseManagement.Infrastructure.Data.Models;
using static WarehouseManagement.Common.MessageConstants.Keys.DifferenceTypeMessageKeys;

namespace WarehouseManagement.Core.Services;

public class DifferenceTypeService : IDifferenceTypeService
{
    private readonly IRepository repository;

    public DifferenceTypeService(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task CreateAsync(DifferenceTypeFormDto model)
    {
        if (await ExistsByNameAsync(model.Name))
        {
            throw new ArgumentException(DifferenceTypeWithNameExist);
        }

        var differenceType = new DifferenceType
        {
            Name = model.Name
        };

        await repository.AddAsync(differenceType);
        await repository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        if (!await ExistsByIdAsync(id))
        {
            throw new KeyNotFoundException(DifferenceTypeWithIdNotFound);
        }

        await repository.SoftDeleteById<DifferenceType>(id);
        await repository.SaveChangesAsync();
    }

    public async Task EditAsync(int id, DifferenceTypeFormDto model, string userId)
    {
        if (!await ExistsByIdAsync(id))
        {
            throw new KeyNotFoundException(DifferenceTypeWithIdNotFound);
        }

        var differenceType = await repository.
            GetByIdAsync<DifferenceType>(id);

        if (await ExistsByNameAsync(model.Name) && model.Name != differenceType!.Name)
        {
            throw new ArgumentException(DifferenceTypeWithNameExist);
        }

        differenceType!.Name = model.Name;
        differenceType!.LastModifiedByUserId = userId;
        differenceType!.LastModifiedAt = DateTime.UtcNow;

        await repository.SaveChangesWithLogAsync();
    }

    public async Task<bool> ExistsByIdAsync(int id)
    {
        return await repository
            .AllReadOnly<DifferenceType>()
            .AnyAsync(dt => dt.Id == id);
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await repository
            .AllReadOnly<DifferenceType>()
            .AnyAsync(dt => dt.Name == name);
    }

    public async Task<IEnumerable<DifferenceTypeDto>> GetAllAsync()
    {
        return await repository
            .AllReadOnly<DifferenceType>()
            .Select(dt => new DifferenceTypeDto
            {
                Id = dt.Id,
                Name = dt.Name,
                CreatedAt = dt.CreatedAt
            }).ToListAsync();
    }

    public async Task<IEnumerable<DifferenceTypeDto>> GetAllWithDeletedAsync()
    {
        return await repository
            .AllWithDeletedReadOnly<DifferenceType>()
            .Select(dt => new DifferenceTypeDto
            {
                Id = dt.Id,
                Name = dt.Name,
                CreatedAt = dt.CreatedAt
            }).ToListAsync();
    }

    public async Task<DifferenceTypeDto> GetByIdAsync(int id)
    {
        if (!await ExistsByIdAsync(id))
        {
            throw new KeyNotFoundException(DifferenceTypeWithIdNotFound);
        }

        var differenceType = await repository.GetByIdAsync<DifferenceType>(id);

        return new DifferenceTypeDto
        {
            Id = differenceType!.Id,
            Name = differenceType.Name,
            CreatedAt = differenceType.CreatedAt
        };
    }

    public async Task RestoreAsync(int id, string userId)
    {
        if (!await ExistsByIdAsync(id))
        {
            throw new KeyNotFoundException(DifferenceTypeWithIdNotFound);
        }

        var differenceType = await repository.GetByIdAsync<DifferenceType>(id);

        if (await ExistsByNameAsync(differenceType!.Name))
        {
            throw new ArgumentException(DifferenceTypeWithNameExist);
        }

        repository.UnDelete(differenceType);

        differenceType.LastModifiedByUserId = userId;
        differenceType.LastModifiedAt = DateTime.UtcNow;

        await repository.SaveChangesAsync();
    }
}
