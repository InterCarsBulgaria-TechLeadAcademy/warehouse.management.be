using WarehouseManagement.Common.Statuses;

namespace WarehouseManagement.Core.DTOs.Requests;

public class DeliveryRequest
{
    public DeliveryStatus Status { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Vendor { get; set; } = string.Empty;
}
