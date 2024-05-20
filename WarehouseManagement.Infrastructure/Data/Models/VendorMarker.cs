namespace WarehouseManagement.Infrastructure.Data.Models;

public class VendorMarker
{
    public int VendorId { get; set; }
    public Vendor Vendor { get; set; } = null!;
    public int MarkerId { get; set; }
    public Marker Marker { get; set; } = null!;
}
