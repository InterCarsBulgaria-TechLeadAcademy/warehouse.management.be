using WarehouseManagement.Common.Enums;
using WarehouseManagement.Common.Utilities;
using WarehouseManagement.Core.DTOs.Delivery;

namespace WarehouseManagement.Core.Factories;

public class DeliveryChange : DeliveryChangeDto
{
    public DeliveryChange(int entityId, string propertyName, string? from, string? to, DateTime changeDate)
    {
        EntityId = entityId;
        PropertyName = propertyName;
        From = from ?? string.Empty;
        To = to ?? string.Empty;
        Type = DeliveryHistoryChangeType.Delivery;
        LogType = GetLogType(propertyName);
        ChangeDate = UtcNowDateTimeStringFormatted.GetUtcNow(changeDate);
    }

    private static LogType GetLogType(string propertyName)
    {
        return propertyName switch
        {
            "StartedProcessing" => LogType.Empty,
            "FinishedProcessing" => LogType.Empty,
            "Status" => LogType.DeliveryStatusChange,
            _ => LogType.Empty
        };
    }
}