namespace WarehouseManagement.Core.DTOs.Zone;

public class ZoneDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsFinal { get; set; }

    public IEnumerable<ZoneMarkerDto> Markers { get; set; } = new HashSet<ZoneMarkerDto>();

    public IEnumerable<ZoneVendorDto> Vendors { get; set; } = new HashSet<ZoneVendorDto>();

    public IEnumerable<ZoneEntryDto> Entries { get; set; } = new HashSet<ZoneEntryDto>();
}
