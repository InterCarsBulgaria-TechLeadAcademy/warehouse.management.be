namespace WarehouseManagement.Core.DTOs.Delivery;

public class DeliveryPDFModelDto
{
    public int Id { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public string VendorSystemNumber { get; set; } = string.Empty;
    public ICollection<string> Zones { get; set; } = new List<string>();
    public DateTime DeliveredOn { get; set; }
    public string SystemNumber { get; set; } = string.Empty;
    public string ReceptionNumber { get; set; } = string.Empty;
    public int Pallets { get; set; }
    public int Packages { get; set; }
    public int Pieces { get; set; }
}
