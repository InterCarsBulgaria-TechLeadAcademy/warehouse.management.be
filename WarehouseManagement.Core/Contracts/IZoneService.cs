using WarehouseManagement.Core.DTOs;
using WarehouseManagement.Core.DTOs.Zone;

namespace WarehouseManagement.Core.Contracts
{
    public interface IZoneService
    {
        Task<ZoneDto> GetByIdAsync(int id);
        Task<IEnumerable<ZoneDto>> GetAllAsync();
        Task<IEnumerable<ZoneDto>> GetAllAsync(PaginationParameters paginationParams);
        Task<IEnumerable<ZoneDto>> GetAllWithDeletedAsync();
        Task<IEnumerable<ZoneDto>> GetAllWithDeletedAsync(PaginationParameters paginationParams);
        Task CreateAsync(ZoneFormDto model, string userId);
        Task EditAsync(int id, ZoneFormDto model, string userId);
        Task DeleteAsync(int id, string userId);
        Task RestoreAsync(int id);
        Task<bool> ExistsByIdAsync(int id);
        Task<bool> ExistsByNameAsync(string name);
    }
}
