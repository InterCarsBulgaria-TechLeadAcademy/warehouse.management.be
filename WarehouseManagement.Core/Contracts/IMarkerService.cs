using WarehouseManagement.Core.DTOs.Marker;

namespace WarehouseManagement.Core.Contracts;

public interface IMarkerService
{
    Task<MarkerDto?> GetByIdAsync(int id);
}
