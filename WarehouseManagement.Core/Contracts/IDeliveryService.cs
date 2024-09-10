using WarehouseManagement.Core.DTOs;
using WarehouseManagement.Core.DTOs.Delivery;
using WarehouseManagement.Core.DTOs.Requests;

namespace WarehouseManagement.Core.Contracts;

public interface IDeliveryService
{
    Task<DeliveryDetailsDto> GetByIdAsync(int id);
    Task<PageDto<DeliveryDto>> GetAllAsync(PaginationParameters paginationParams, DeliveryRequest request);
    Task EditAsync(int id, DeliveryFormDto model, string userId);
    Task<int> AddAsync(DeliveryFormDto model, string userId);
    Task DeleteAsync(int id);
    Task RestoreAsync(int id);
    Task<PageDto<DeliveryDto>> GetAllDeletedAsync(PaginationParameters paginationParams, DeliveryRequest request);
    Task ChangeDeliveryStatusIfNeeded(int id);
    Task<DeliveryHistoryDto> GetHistoryAsync(int id);
    Task ApproveAsync(int id);
    Task<DeliveryPDFModelDto> GetPDFModel(int id);
}
