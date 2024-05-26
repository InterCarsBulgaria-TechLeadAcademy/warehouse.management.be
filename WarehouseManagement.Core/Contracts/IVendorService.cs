using WarehouseManagement.Core.DTOs.Vendor;

namespace WarehouseManagement.Core.Contracts
{
    public interface IVendorService
    {
        Task<VendorDto?> GetByIdAsync(int id);
        Task<IEnumerable<VendorDto>> GetAllAsync();
        Task EditAsync(int id, VendorFormDto model, string userId);
        Task<bool> OtherVendorWithNameExistIdAsync(int id, string name);
        Task<bool> OtherVendorWithSystemNumberExistAsync(int id, string systemNumber);
        Task<bool> ExistByIdAsync(int id);
    }
}
