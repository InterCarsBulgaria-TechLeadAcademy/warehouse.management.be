using WarehouseManagement.Common.Statuses;

namespace WarehouseManagement.Core.DTOs.Requests;

public class EntryRequest
{
    public int? ZoneId { get; set; }

    public ZoneEntryStatuses[]? Statuses { get; set; }
}
