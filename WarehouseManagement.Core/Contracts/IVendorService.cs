using WarehouseManagement.Core.DTOs.Vendor;

namespace WarehouseManagement.Core.Contracts
{
    public interface IVendorService
    {
        Task<VendorDto?> GetByIdAsync(int id);
        Task<IEnumerable<VendorDto>> GetAllAsync();
        Task AddAsync(VendorFormDto model, string userId);
        Task EditAsync(int id, VendorFormDto model, string userId);
        Task DeleteAsync(int id, string userId);
        Task<bool> AnotherVendorWithNameExistAsync(int id, string name);
        Task<bool> ExistByNameAsync(string name);
        Task<bool> ExistBySystemNumberAsync(string systemNumber);
        Task<bool> AnotherVendorWithSystemNumberExistAsync(int id, string systemNumber);
        Task<bool> ExistByIdAsync(int id);
    }
}
