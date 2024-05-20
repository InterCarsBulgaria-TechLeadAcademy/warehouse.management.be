namespace WarehouseManagement.Infrastructure.Data.Models;

public class Zone : BaseClass
{
    public string Name { get; set; } = string.Empty;
    public bool IsFinal { get; set; }
    public ICollection<ZoneMarker> ZonesMarkers { get; set; } = new HashSet<ZoneMarker>();
    public ICollection<VendorZone> VendorsZones { get; set; } = new HashSet<VendorZone>();
    public ICollection<Entry> Entries { get; set; } = new HashSet<Entry>();
}
