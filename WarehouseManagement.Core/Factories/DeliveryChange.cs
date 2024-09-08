using WarehouseManagement.Common.Enums;
using WarehouseManagement.Common.Utilities;
using WarehouseManagement.Core.DTOs.Delivery;

namespace WarehouseManagement.Core.Factories;

public class DeliveryChange : DeliveryChangeDto
{
    public DeliveryChange(int entityId, DeliveryHistoryEntityPropertyChange propertyName, string? from, string? to, DateTime changeDate)
    {
        EntityId = entityId;
        PropertyName = propertyName;
        From = from ?? string.Empty;
        To = to ?? string.Empty;
        Type = DeliveryHistoryChangeType.Delivery;
        LogType = GetLogType(propertyName);
        ChangeDate = UtcNowDateTimeStringFormatted.GetUtcNow(changeDate);
    }

    private static LogType GetLogType(DeliveryHistoryEntityPropertyChange propertyName)
    {
        return propertyName switch
        {
            DeliveryHistoryEntityPropertyChange.StartedProcessing => LogType.Empty,
            DeliveryHistoryEntityPropertyChange.FinishedProcessing => LogType.Empty,
            DeliveryHistoryEntityPropertyChange.Status => LogType.DeliveryStatusChange,
            _ => LogType.Empty
        };
    }
}