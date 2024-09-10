using WarehouseManagement.Common.Statuses;
using WarehouseManagement.Core.DTOs;
using WarehouseManagement.Core.DTOs.Entry;

namespace WarehouseManagement.Core.Contracts;

public interface IEntryService
{
    Task<EntryDto> GetByIdAsync(int id);
    Task<PageDto<EntryDto>> GetAllAsync(PaginationParameters paginationParams, EntryStatuses[]? statuses = null);
    Task<PageDto<EntryDto>> GetAllByZoneAsync(PaginationParameters paginationParams, int zoneId, EntryStatuses[]? statuses = null);
    Task<PageDto<EntryDto>> GetAllWithDeletedAsync(PaginationParameters paginationParams, int? zoneId = null, EntryStatuses[]? statuses = null);
    Task CreateAsync(EntryFormDto model, string userId);
    Task EditAsync(int id, EntryFormDto model, string userId);
    Task DeleteAsync(int id, string userId);
    Task RestoreAsync(int id);
    Task<bool> ExistsByIdAsync(int id);
    Task StartProcessingAsync(int entryId);
    Task FinishProcessingAsync(int entryId);
    Task MoveAsync(int id, int newZoneId, string userId);
    Task SplitAsync(int id, EntrySplitDto splitDto, string userId);
}
