using iText.Layout.Element;

namespace WarehouseManagement.Core.DTOs.Vendor
{
    public class VendorDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SystemNumber { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public List<string> Markers { get; set; } = new();
    }
}
