namespace WarehouseManagement.Core.DTOs.Vendor
{
    public class VendorDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SystemNumber { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public ICollection<VendorMarkerDto> Markers { get; set; } = new HashSet<VendorMarkerDto>();
        public ICollection<VendorZoneDto> Zones { get; set; } = new HashSet<VendorZoneDto>();
    }
}
