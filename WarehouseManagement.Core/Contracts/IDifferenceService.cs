using WarehouseManagement.Core.DTOs;
using WarehouseManagement.Core.DTOs.Difference;

namespace WarehouseManagement.Core.Contracts;

public interface IDifferenceService
{
    Task<DifferenceDto> GetByIdAsync(int id);

    Task<PageDto<DifferenceDto>> GetAllAsync(PaginationParameters paginationParams);

    Task<PageDto<DifferenceDto>> GetAllWithDeletedAsync(PaginationParameters paginationParams);

    Task CreateAsync(DifferenceFormDto model, string userId);

    Task EditAsync(int id, DifferenceFormDto model, string userId);

    Task DeleteAsync(int id);

    Task RestoreAsync(int id);

    Task<bool> ExistsByIdAsync(int id);

    Task StartProcessing(int id);

    Task FinishProcessing(DifferenceAdminCommentDto adminCommentDto);

    Task NoDifferences(DifferenceAdminCommentDto adminCommentDto);
}
