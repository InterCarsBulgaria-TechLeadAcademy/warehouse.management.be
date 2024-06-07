using WarehouseManagement.Core.DTOs.Entry;

namespace WarehouseManagement.Core.Contracts;

public interface IEntryService
{
    Task<EntryDto> GetByIdAsync(int id);
    Task<IEnumerable<EntryDto>> GetAllAsync();
    Task<IEnumerable<EntryDto>> GetAllWithDeletedAsync();
    Task CreateAsync(EntryFormDto model, string userId);
    Task EditAsync(int id, EntryFormDto model, string userId);
    Task DeleteAsync(int id, string userId);
    Task RestoreAsync(int id);
    Task<bool> ExistsByIdAsync(int id);
    Task MoveEntryToZoneWithId(int entryId, int zoneId);
}
