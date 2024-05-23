using WarehouseManagement.Core.DTOs.Marker;

namespace WarehouseManagement.Core.Contracts;

public interface IMarkerService
{
    Task<MarkerDto?> GetByIdAsync(int id);
    Task<bool> ExistByNameAsync(string name);
    Task<IEnumerable<MarkerDto>> GetAllAsync();
    Task AddAsync(MarkerFormDto model, string userId);
    Task EditAsync(int id, MarkerFormDto model, string userId);
    Task DeleteAsync(int id, string userId);
    Task RestoreAsync(int id, string userId);
    Task<bool> IsDeletedById(int id);
    Task<IEnumerable<MarkerDto>> GetDeletedMarkersAsync();
}
