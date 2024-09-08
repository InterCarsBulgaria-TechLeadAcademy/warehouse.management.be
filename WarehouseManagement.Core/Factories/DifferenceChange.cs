using WarehouseManagement.Common.Enums;
using WarehouseManagement.Common.Utilities;
using WarehouseManagement.Core.DTOs.Delivery;

namespace WarehouseManagement.Core.Factories;

public class DifferenceChange : DeliveryChangeDto
{
    public DifferenceChange(int entityId, DeliveryHistoryEntityPropertyChange propertyName, string? from, string? to, DateTime changeDate)
    {
        EntityId = entityId;
        PropertyName = propertyName;
        From = from ?? string.Empty;
        To = to ?? string.Empty;
        Type = DeliveryHistoryChangeType.Difference;
        LogType = GetLogType(propertyName);
        ChangeDate = UtcNowDateTimeStringFormatted.GetUtcNow(changeDate);
    }

    private static LogType GetLogType(DeliveryHistoryEntityPropertyChange propertyName)
    {
        return propertyName switch
        {
            DeliveryHistoryEntityPropertyChange.Status => LogType.DifferenceStatusChange,
            DeliveryHistoryEntityPropertyChange.AdminComment => LogType.DifferenceAdminComment,
            _ => LogType.Empty
        };
    }
}