namespace WarehouseManagement.Infrastructure.Data.Models;

public class Vendor : BaseClass
{
    public string Name { get; set; } = string.Empty;
    public string SystemNumber { get; set; } = string.Empty;
    public ICollection<VendorMarker> VendorsMarkers { get; set; } = new HashSet<VendorMarker>();
    public ICollection<VendorZone> VendorsZones { get; set; } = new HashSet<VendorZone>();
    public ICollection<Delivery> Deliveries { get; set; } = new HashSet<Delivery>();
}
