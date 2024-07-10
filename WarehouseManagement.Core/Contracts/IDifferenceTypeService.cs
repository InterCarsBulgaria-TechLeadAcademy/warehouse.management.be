using WarehouseManagement.Core.DTOs.DifferenceType;

namespace WarehouseManagement.Core.Contracts
{
    public interface IDifferenceTypeService
    {
        Task CreateAsync(DifferenceTypeFormDto model);
        Task<IEnumerable<DifferenceTypeDto>> GetAllAsync();
        Task<IEnumerable<DifferenceTypeDto>> GetAllWithDeletedAsync();
        Task<DifferenceTypeDto> GetByIdAsync(int id);
        Task EditAsync(int id, DifferenceTypeFormDto model, string userId);
        Task DeleteAsync(int id);
        Task<bool> ExistsByIdAsync(int id);
        Task<bool> ExistsByNameAsync(string name);
        Task RestoreAsync(int id, string userId);
    }
}
