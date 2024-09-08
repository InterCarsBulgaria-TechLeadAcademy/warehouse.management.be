using WarehouseManagement.Common.Enums;
using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs.Delivery;

namespace WarehouseManagement.Core.Factories;

public class DeliveryChangeFactory : IDeliveryChangeFactory
{
    public DeliveryChangeDto CreateDeliveryChangeDto(int entityId, DeliveryHistoryEntityPropertyChange propertyName, DeliveryHistoryChangeType entityName, string? from, string? to, DateTime changeDate)
    {
        return entityName switch
        {
            DeliveryHistoryChangeType.Delivery => new DeliveryChange(entityId, propertyName, from, to, changeDate),
            DeliveryHistoryChangeType.Difference => new DifferenceChange(entityId, propertyName, from, to, changeDate),
            DeliveryHistoryChangeType.Entry => new EntryChange(entityId, propertyName, from, to, changeDate),
            _ => throw new InvalidOperationException("Invalid entity name.")
        };
    }
}