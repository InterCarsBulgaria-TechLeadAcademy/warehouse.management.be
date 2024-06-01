using WarehouseManagement.Core.DTOs.Vendor;

namespace WarehouseManagement.Core.Contracts
{
    public interface IVendorService
    {
        Task<VendorDto?> GetByIdAsync(int id);
        Task<IEnumerable<VendorDto>> GetAllAsync();
        Task<IEnumerable<VendorDto>> GetAllDeletedAsync();
        Task<int> AddAsync(VendorFormDto model, string userId);
        Task EditAsync(int id, VendorFormDto model, string userId);
        Task DeleteAsync(int id);
        Task RestoreAsync(int id);
    }
}
