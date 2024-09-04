using WarehouseManagement.Common.Enums;
using WarehouseManagement.Common.Utilities;
using WarehouseManagement.Core.DTOs.Delivery;

namespace WarehouseManagement.Core.Factories;

public class EntryChange : DeliveryChangeDto
{
    public EntryChange(int entityId, string propertyName, string? from, string? to, DateTime changeDate)
    {
        var logType = GetLogType(propertyName);
        
        EntityId = entityId;
        PropertyName = propertyName;
        From = logType == LogType.EntryStatusChange && !string.IsNullOrEmpty(from) ? UtcNowDateTimeStringFormatted.GetUtcNow(Convert.ToDateTime(from)) : from ?? string.Empty;
        To = logType == LogType.EntryStatusChange && !string.IsNullOrEmpty(to) ? UtcNowDateTimeStringFormatted.GetUtcNow(Convert.ToDateTime(to)) : to ?? string.Empty;
        Type = DeliveryHistoryChangeType.Entry;
        LogType = logType;
        ChangeDate = UtcNowDateTimeStringFormatted.GetUtcNow(changeDate);
    }

    private static LogType GetLogType(string propertyName)
    {
        return propertyName switch
        {
            "StartedProcessing" => LogType.EntryStatusChange,
            "FinishedProcessing" => LogType.EntryStatusChange,
            "Status" => LogType.EntryStatusChange,
            "ZoneId" => LogType.ZoneChange,
            _ => LogType.Empty
        };
    }
}