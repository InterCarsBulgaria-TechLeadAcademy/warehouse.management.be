using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Core.DTOs.Delivery;

namespace WarehouseManagement.Core.Factories;

public class DeliveryChangeFactory : IDeliveryChangeFactory
{
    public DeliveryChangeDto CreateDeliveryChangeDto(int entityId, string propertyName, string entityName, string? from, string? to, DateTime changeDate)
    {
        if (entityName == "Delivery")
        {
            return new DeliveryChange(entityId, propertyName, from, to, changeDate);
        }

        if (entityName == "Difference")
        {
            return new DifferenceChange(entityId, propertyName, from, to, changeDate);
        }

        if (entityName == "Entry")
        {
            return new EntryChange(entityId, propertyName, from, to, changeDate);
        }

        throw new InvalidOperationException("Invalid entity name.");
    }
}