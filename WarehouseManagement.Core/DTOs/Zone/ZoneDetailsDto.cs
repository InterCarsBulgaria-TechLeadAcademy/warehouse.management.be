namespace WarehouseManagement.Core.DTOs.Zone;

public class ZoneDetailsDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsFinal { get; set; }

    public DateTime CreatedAt { get; set; }

    public IEnumerable<ZoneMarkerDto> Markers { get; set; } = new HashSet<ZoneMarkerDto>();
}