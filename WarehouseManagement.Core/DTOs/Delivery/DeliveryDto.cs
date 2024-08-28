using WarehouseManagement.Core.DTOs.Entry;

namespace WarehouseManagement.Core.DTOs.Delivery;

public class DeliveryDto
{
    public int Id { get; set; }
    public string SystemNumber { get; set; } = string.Empty;
    public string ReceptionNumber { get; set; } = string.Empty;
    public string DeliveryTime { get; set; } = string.Empty;
    public string? ApprovedOn { get; set; }
    public DateTime CreatedAt { get; set; }
    public int EntriesWaitingProcessing { get; set; }
    public int EntriesFinishedProcessing { get; set; }
    public EntriesProcessingDetails EntriesWaitingProcessingDetails { get; set; } = null!;
    public EntriesProcessingDetails EntriesFinishedProcessingDetails { get; set; } = null!;
    public string VendorName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public ICollection<DeliveryMarkerDto> Markers { get; set; } = new List<DeliveryMarkerDto>();
}
