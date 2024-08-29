namespace WarehouseManagement.Core.DTOs.Vendor
{
    public class VendorDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SystemNumber { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        
        // TODO: Turn into List<string> when FE implements usage of VendorDetailsDto
        public ICollection<VendorMarkerDto> Markers { get; set; } = new HashSet<VendorMarkerDto>();
    }
}
