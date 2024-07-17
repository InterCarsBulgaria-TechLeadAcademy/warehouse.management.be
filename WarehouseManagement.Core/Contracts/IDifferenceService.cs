using WarehouseManagement.Core.DTOs;
using WarehouseManagement.Core.DTOs.Difference;

namespace WarehouseManagement.Core.Contracts;

public interface IDifferenceService
{
    Task<DifferenceDto> GetByIdAsync(int id);

    Task<IEnumerable<DifferenceDto>> GetAllAsync(PaginationParameters paginationParams);

    Task<IEnumerable<DifferenceDto>> GetAllWithDeletedAsync(PaginationParameters paginationParams);

    Task CreateAsync(DifferenceFormDto model);

    Task EditAsync(int id, DifferenceFormDto model, string userId);

    Task DeleteAsync(int id);

    Task RestoreAsync(int id);

    Task<bool> ExistsByIdAsync(int id);
}
