using WarehouseManagement.Common.Enums;
using WarehouseManagement.Common.Utilities;
using WarehouseManagement.Core.DTOs.Delivery;
using WarehouseManagement.Core.DTOs.Zone;
using WarehouseManagement.Infrastructure.Data.Models;

namespace WarehouseManagement.Core.Factories;

public class EntryChange : DeliveryChangeDto
{
    public EntryChange(int entityId, DeliveryHistoryEntityPropertyChange propertyName, string? from, string? to, DateTime changeDate)
    {
        var logType = GetLogType(propertyName);
        
        EntityId = entityId;
        PropertyName = propertyName;
        From = GetChangeValue(logType, from);
        To = GetChangeValue(logType, to);
        Type = DeliveryHistoryChangeType.Entry;
        LogType = logType;
        ChangeDate = UtcNowDateTimeStringFormatted.GetUtcNow(changeDate);
    }

    public void UpdateChangedValuesFromItems(IQueryable<Zone> zones)
    {
        From = zones.FirstOrDefault(zone => zone.Id == Convert.ToInt32(From))?.Name ?? From;
        To = zones.FirstOrDefault(zone => zone.Id == Convert.ToInt32(To))?.Name ?? To;
    }

    private static LogType GetLogType(DeliveryHistoryEntityPropertyChange propertyName)
    {
        return propertyName switch
        {
            DeliveryHistoryEntityPropertyChange.StartedProcessing => LogType.EntryStatusChange,
            DeliveryHistoryEntityPropertyChange.FinishedProcessing => LogType.EntryStatusChange,
            DeliveryHistoryEntityPropertyChange.Status => LogType.EntryStatusChange,
            DeliveryHistoryEntityPropertyChange.ZoneId => LogType.ZoneChange,
            _ => LogType.Empty
        };
    }
    
    private static string GetChangeValue(LogType logType, string? value)
    {
        if(logType == LogType.EntryStatusChange && !string.IsNullOrEmpty(value))
        {
            return UtcNowDateTimeStringFormatted.GetUtcNow(Convert.ToDateTime(value));
        }
        
        return value ?? string.Empty;
    }
}