namespace WarehouseManagement.Core.DTOs.Delivery;

public class DeliveryDetailsDto
{
    public int Id { get; set; }
    public string SystemNumber { get; set; } = string.Empty;
    public string ReceptionNumber { get; set; } = string.Empty;
    public string TruckNumber { get; set; } = string.Empty;
    public string Cmr { get; set; } = string.Empty;
    public string DeliveryTime { get; set; } = string.Empty;
    public string? ApprovedOn { get; set; }
    public DateTime CreatedAt { get; set; }
    public int Pallets { get; set; }
    public int Packages { get; set; }
    public int Pieces { get; set; }
    public int EntriesWaitingProcessing { get; set; }
    public int EntriesFinishedProcessing { get; set; }
    public EntriesProcessingDetails EntriesWaitingProcessingDetails { get; set; } = null!;
    public EntriesProcessingDetails EntriesFinishedProcessingDetails { get; set; } = null!;
    public int VendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public ICollection<DeliveryEntryDetailsDto> Entries { get; set; } = new List<DeliveryEntryDetailsDto>();
    public ICollection<DeliveryMarkerDto> Markers { get; set; } = new List<DeliveryMarkerDto>();
}