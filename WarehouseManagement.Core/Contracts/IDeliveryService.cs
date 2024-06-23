using WarehouseManagement.Core.DTOs;
using WarehouseManagement.Core.DTOs.Delivery;

namespace WarehouseManagement.Core.Contracts;

public interface IDeliveryService
{
    Task<DeliveryDto> GetByIdAsync(int id);
    Task<ICollection<DeliveryDto>> GetAllAsync(PaginationParameters paginationParams);
    Task EditAsync(int id, DeliveryFormDto model, string userId);
    Task<int> AddASync(DeliveryFormDto model, string userId);
    Task DeleteASync(int id);
}
