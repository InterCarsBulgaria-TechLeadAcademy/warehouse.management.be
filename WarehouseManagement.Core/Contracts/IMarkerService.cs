using System.Threading.Tasks;
using WarehouseManagement.Core.DTOs;
using WarehouseManagement.Core.DTOs.Marker;

namespace WarehouseManagement.Core.Contracts;

public interface IMarkerService
{
    Task<MarkerDto> GetByIdAsync(int id);
    Task<bool> ExistByNameAsync(string name);
    Task<IEnumerable<MarkerDto>> GetAllAsync(PaginationParameters paginationParams);
    Task<int> AddAsync(MarkerFormDto model, string userId);
    Task EditAsync(int id, MarkerFormDto model, string userId);
    Task DeleteAsync(int id);
    Task RestoreAsync(int id, string userId);
    Task<IEnumerable<string>> GetDeletedMarkersAsync();
    Task<Dictionary<string, List<string>>> GetMarkerUsagesAsync(int id);
    Task<string?> GetDeletedMarkerNameByIdAsync(int id);
    Task<bool> ExistById(int id);
    Task<ICollection<int>> MarkersExistAsync(ICollection<int> makrkerIds);
}
