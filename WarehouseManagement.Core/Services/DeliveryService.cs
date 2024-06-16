using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Infrastructure.Data.Common;

namespace WarehouseManagement.Core.Services;

public class DeliveryService : IDeliveryService
{
    private readonly IRepository repository;

    public DeliveryService(IRepository repository)
    {
        this.repository = repository;
    }
}
