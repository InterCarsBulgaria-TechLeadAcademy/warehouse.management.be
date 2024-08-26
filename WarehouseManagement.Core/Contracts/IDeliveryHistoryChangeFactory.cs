using WarehouseManagement.Core.DTOs.Delivery;
using WarehouseManagement.Infrastructure.Data.Models;

namespace WarehouseManagement.Core.Contracts;

public interface IDeliveryHistoryChangeFactory
{
    public DeliveryHistoryDto CreateDeliveryHistoryChange(EntityChange change);
}