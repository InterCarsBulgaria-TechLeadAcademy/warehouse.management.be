namespace WarehouseManagement.Infrastructure.Data.Models;

public class VendorZone
{
    public int VendorId { get; set; }
    public Vendor Vendor { get; set; } = null!;
    public int ZoneId { get; set; }
    public Zone Zone { get; set; } = null!;
}
