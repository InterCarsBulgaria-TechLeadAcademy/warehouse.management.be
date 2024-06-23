using WarehouseManagement.Common.Statuses;
using WarehouseManagement.Core.DTOs.Entry;

namespace WarehouseManagement.Core.Contracts;

public interface IEntryService
{
    Task<EntryDto> GetByIdAsync(int id);
    Task<IEnumerable<EntryDto>> GetAllAsync(EntryStatuses[]? statuses);
    Task<IEnumerable<EntryDto>> GetAllByZoneAsync(int zoneId, EntryStatuses[]? statuses);
    Task<IEnumerable<EntryDto>> GetAllWithDeletedAsync(int? zoneId, EntryStatuses[]? statuses);
    Task CreateAsync(ICollection<EntryFormDto> model, string userId);
    Task EditAsync(int id, EntryFormDto model, string userId);
    Task DeleteAsync(int id, string userId);
    Task RestoreAsync(int id);
    Task<bool> ExistsByIdAsync(int id);
    Task MoveEntryToZoneWithId(int entryId, int zoneId);
}
