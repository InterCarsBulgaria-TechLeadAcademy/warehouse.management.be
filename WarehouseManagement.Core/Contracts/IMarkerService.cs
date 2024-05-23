using WarehouseManagement.Core.DTOs.Marker;

namespace WarehouseManagement.Core.Contracts;

public interface IMarkerService
{
    Task<MarkerDto?> GetByIdAsync(int id);
    Task<bool> ExistByNameAsync(string name);
    Task<IEnumerable<MarkerDto>> GetAllAsync();
    Task AddAsync(MarkerFormDto model, string userId);
}
