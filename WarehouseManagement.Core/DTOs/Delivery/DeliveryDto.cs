using WarehouseManagement.Core.DTOs.Entry;

namespace WarehouseManagement.Core.DTOs.Delivery;

public class DeliveryDto
{
    public int Id { get; set; }
    public string SystemNumber { get; set; } = string.Empty;
    public string ReceptionNumber { get; set; } = string.Empty;
    public string TruckNumber { get; set; } = string.Empty;
    public string Cmr { get; set; } = string.Empty;
    public DateTime DeliveryTime { get; set; }
    public DateTime? ApprovedOn { get; set; }
    public DateTime CreatedAt { get; set; }
    public int Pallets { get; set; }
    public int Packages { get; set; }
    public int Pieces { get; set; }
    public int EntriesWaitingProcessing { get; set; }
    public int EntriesFinishedProcessing { get; set; }
    public int VendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public ICollection<EntryDto> Entries { get; set; } = new List<EntryDto>();
    public ICollection<DeliveryMarkerDto> Markers { get; set; } = new List<DeliveryMarkerDto>();
}
