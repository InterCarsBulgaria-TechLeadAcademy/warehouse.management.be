using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs;
using WarehouseManagement.Core.DTOs.Difference;
using WarehouseManagement.Infrastructure.Data.Common;

namespace WarehouseManagement.Core.Services;

public class DifferenceService : IDifferenceService
{
    private readonly IRepository repository;

    public DifferenceService(IRepository repository)
    {
        this.repository = repository;
    }

    public Task CreateAsync(DifferenceFormDto model)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task EditAsync(int id, DifferenceFormDto model, string userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<DifferenceDto>> GetAllAsync(PaginationParameters paginationParams)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<DifferenceDto>> GetAllWithDeletedAsync(PaginationParameters paginationParams)
    {
        throw new NotImplementedException();
    }

    public Task<DifferenceDto> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task RestoreAsync(int id)
    {
        throw new NotImplementedException();
    }
}
