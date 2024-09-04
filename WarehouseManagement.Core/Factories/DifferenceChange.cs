using WarehouseManagement.Common.Enums;
using WarehouseManagement.Common.Utilities;
using WarehouseManagement.Core.DTOs.Delivery;

namespace WarehouseManagement.Core.Factories;

public class DifferenceChange : DeliveryChangeDto
{
    public DifferenceChange(int entityId, string propertyName, string? from, string? to, DateTime changeDate)
    {
        EntityId = entityId;
        PropertyName = propertyName;
        From = from ?? string.Empty;
        To = to ?? string.Empty;
        Type = DeliveryHistoryChangeType.Difference;
        LogType = GetLogType(propertyName);
        ChangeDate = UtcNowDateTimeStringFormatted.GetUtcNow(changeDate);
    }

    private static LogType GetLogType(string propertyName)
    {
        return propertyName switch
        {
            "Status" => LogType.DifferenceStatusChange,
            "AdminComment" => LogType.DifferenceAdminComment,
            _ => LogType.Empty
        };
    }
}