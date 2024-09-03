namespace WarehouseManagement.Core.DTOs.Vendor;

public class VendorDetailsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SystemNumber { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
    public ICollection<VendorMarkerDto> Markers { get; set; } = new HashSet<VendorMarkerDto>();
}