using WarehouseManagement.Common.Enums;
using WarehouseManagement.Core.DTOs.Delivery;

namespace WarehouseManagement.Core.Contracts;

public interface IDeliveryChangeFactory
{
    public DeliveryChangeDto CreateDeliveryChangeDto(int entityId, DeliveryHistoryEntityPropertyChange propertyName, DeliveryHistoryChangeType entityName, string? from, string? to, DateTime changeTime);
}
