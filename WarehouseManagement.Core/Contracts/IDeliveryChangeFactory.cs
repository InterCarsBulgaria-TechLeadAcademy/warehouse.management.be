using WarehouseManagement.Core.DTOs.Delivery;

namespace WarehouseManagement.Core.Contracts;

public interface IDeliveryChangeFactory
{
    public DeliveryChangeDto CreateDeliveryChangeDto(int entityId, string propertyName, string entityName, string? from, string? to, DateTime changeTime);
}
