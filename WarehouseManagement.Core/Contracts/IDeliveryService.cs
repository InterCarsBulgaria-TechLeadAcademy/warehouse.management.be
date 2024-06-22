using WarehouseManagement.Core.DTOs.Delivery;

namespace WarehouseManagement.Core.Contracts;

public interface IDeliveryService
{
    Task<DeliveryDto> GetByIdAsync(int id);
    Task<ICollection<DeliveryDto>> GetAllAsync();
    Task EditAsync(int id, DeliveryFormDto model, string userId);
}
