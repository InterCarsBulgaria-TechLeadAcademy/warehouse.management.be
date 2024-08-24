using WarehouseManagement.Common.Enums;

namespace WarehouseManagement.Core.DTOs.Delivery;

public class DeliveryHistoryDto
{
    public int Id { get; set; }

    public ICollection<DeliveryChangeDto> Changes { get; set; } = new HashSet<DeliveryChangeDto>();
}
