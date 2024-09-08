using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Common.Enums;

namespace WarehouseManagement.Core.DTOs.Delivery;

public class DeliveryChangeDto
{
    public int EntityId { get; set; }

    public DeliveryHistoryEntityPropertyChange PropertyName { get; set; }

    public string From { get; set; } = string.Empty;

    public string To { get; set; } = string.Empty;

    public DeliveryHistoryChangeType Type { get; set; }

    public LogType LogType { get; set; }

    public string ChangeDate { get; set; } = string.Empty;

    [ApiExplorerSettings(IgnoreApi = true)]
    public void UpdateChangedValuesFromItems<T>(IQueryable<T> items)
    {
        return;
    }
}