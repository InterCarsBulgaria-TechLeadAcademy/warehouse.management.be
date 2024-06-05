using WarehouseManagement.Core.DTOs.Zone;
using WarehouseManagement.Infrastructure.Data.Models;

namespace WarehouseManagement.Core.Contracts
{
    public interface IZoneService
    {
        Task<ZoneDto> GetByIdAsync(int id);
        Task<IEnumerable<ZoneDto>> GetAllAsync();
        Task<IEnumerable<ZoneDto>> GetAllWithDeletedAsync();
        Task CreateAsync(ZoneFormDto model, string userId);
        Task EditAsync(int id, ZoneFormDto model, string userId);
        Task DeleteAsync(int id, string userId);
        Task<string> RestoreAsync(int id);
        Task<bool> ExistsByIdAsync(int id);
        Task<bool> ExistsByNameAsync(string name);
        Task<IEnumerable<ZoneEntryDto>> GetEntriesAsync(int id, Predicate<Entry> predicate);
    }
}
