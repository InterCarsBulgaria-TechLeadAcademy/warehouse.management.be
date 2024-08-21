namespace WarehouseManagement.Core.DTOs.Zone;

public class ZoneDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsFinal { get; set; }

    public DateTime CreatedAt { get; set; }

    // TODO: Turn into List<string> when FE implements usage of ZoneDetailsDto
    public IEnumerable<ZoneMarkerDto> Markers { get; set; } = new HashSet<ZoneMarkerDto>();
}
