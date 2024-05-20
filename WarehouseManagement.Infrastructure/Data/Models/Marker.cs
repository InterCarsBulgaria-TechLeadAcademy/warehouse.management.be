namespace WarehouseManagement.Infrastructure.Data.Models;

public class Marker : BaseClass
{
    public string Name { get; set; } = string.Empty;
    public ICollection<DeliveryMarker> DeliveriesMarkers { get; set; } =
        new HashSet<DeliveryMarker>();
    public ICollection<ZoneMarker> ZonesMarkers { get; set; } = new HashSet<ZoneMarker>();
    public ICollection<VendorMarker> VendorsMarkers { get; set; } = new HashSet<VendorMarker>();
}
