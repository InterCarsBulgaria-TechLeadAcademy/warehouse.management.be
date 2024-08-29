using WarehouseManagement.Core.DTOs.Zone;

namespace WarehouseManagement.Core.DTOs.Entry;

public class EntryDto
{
    public int Id { get; set; }
    public int Pallets { get; set; }
    public int Packages { get; set; }
    public int Pieces { get; set; }
    public string? StartedProccessing { get; set; } = string.Empty;
    public string? FinishedProccessing { get; set; }
    public string CreatedAt { get; set; } = string.Empty;
    public EntryZoneDto Zone { get; set; } = null!;
    public EntryDeliveryDetailsDto DeliveryDetails { get; set; } = null!;
}
