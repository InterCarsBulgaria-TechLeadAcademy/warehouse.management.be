using WarehouseManagement.Core.Contracts;
using WarehouseManagement.Infrastructure.Data.Common;

namespace WarehouseManagement.Core.Services
{
    public class VendorService : IVendorService
    {
        private readonly IRepository repository;

        public VendorService(IRepository repository)
        {
            this.repository = repository;
        }
    }
}
