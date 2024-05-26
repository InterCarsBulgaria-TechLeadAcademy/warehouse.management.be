namespace WarehouseManagement.Core.DTOs.Vendor
{
    public class VendorZoneDto
    {
        public int ZoneId { get; set; }
        public string ZoneName { get; set; } = string.Empty;
        public bool IsFinal { get; set; }
    }
}
