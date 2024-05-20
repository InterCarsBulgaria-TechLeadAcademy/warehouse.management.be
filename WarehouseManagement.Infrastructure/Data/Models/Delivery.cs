namespace WarehouseManagement.Infrastructure.Data.Models;

public class Delivery : BaseClass
{
    public string SystemNumber { get; set; } = string.Empty;
    public string ReceptionNumber { get; set; } = string.Empty;
    public string TruckNumber { get; set; } = string.Empty;
    public string Cmr { get; set; } = string.Empty;
    public DateTime DeliveryTime { get; set; }
    public int Pallets { get; set; }
    public int Packages { get; set; }
    public int Pieces { get; set; }
    public bool IsApproved { get; set; }
    public int VendorId { get; set; }
    public Vendor Vendor { get; set; } = null!;
    public ICollection<DeliveryMarker> DeliveriesMarkers { get; set; } =
        new HashSet<DeliveryMarker>();
    public ICollection<Entry> Entries { get; set; } = new HashSet<Entry>();
}
