using WarehouseManagement.Common.Enums;

namespace WarehouseManagement.Core.DTOs.Delivery;

public class DeliveryChangeDto
{
    public int EntityId { get; set; }

    public string PropertyName { get; set; } = null!;

    public string From { get; set; } = string.Empty;

    public string To { get; set; } = string.Empty;

    public DeliveryHistoryChangeType Type { get; set; }

    public LogType LogType { get; set; }

    public string ChangeDate { get; set; } = string.Empty;
}