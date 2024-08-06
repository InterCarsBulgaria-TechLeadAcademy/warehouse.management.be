using WarehouseManagement.Core.DTOs;
using WarehouseManagement.Core.DTOs.Delivery;

namespace WarehouseManagement.Core.Contracts;

public interface IDeliveryService
{
    Task<DeliveryDto> GetByIdAsync(int id);
    Task<PageDto<DeliveryDto>> GetAllAsync(PaginationParameters paginationParams);
    Task EditAsync(int id, DeliveryFormDto model, string userId);
    Task<int> AddAsync(DeliveryFormDto model, string userId);
    Task DeleteAsync(int id);
    Task RestoreAsync(int id);
    Task<ICollection<DeliveryDto>> GetAllDeletedAsync();
    Task ChangeDeliveryStatusIfNeeded(int id);
    Task<DeliveryHistoryDto> GetHistoryAsync(int id);
    Task ApproveAsync(int id);
    Task<DeliveryPDFModelDto> GetPDFModel(int id);
}
