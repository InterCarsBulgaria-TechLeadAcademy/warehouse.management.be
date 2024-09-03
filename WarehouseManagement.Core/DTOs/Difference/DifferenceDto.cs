using WarehouseManagement.Common.Statuses;

namespace WarehouseManagement.Core.DTOs.Difference;

public class DifferenceDto
{
    public int Id { get; set; }

    public string ReceptionNumber { get; set; } = string.Empty;

    public string InternalNumber { get; set; } = string.Empty;

    public string ActiveNumber { get; set; } = string.Empty;

    public string Comment { get; set; } = string.Empty;

    public string AdminComment { get; set; } = string.Empty;

    public int Count { get; set; }

    public DifferenceStatus Status { get; set; }

    public string Type { get; set; } = string.Empty;

    public string CreatedAt { get; set; } = string.Empty;

    public string Zone { get; set; } = string.Empty;

    public string DeliverySystemNumber { get; set; } = string.Empty;
}
