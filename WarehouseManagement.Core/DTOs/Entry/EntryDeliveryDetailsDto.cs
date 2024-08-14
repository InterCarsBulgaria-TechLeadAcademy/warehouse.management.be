namespace WarehouseManagement.Core.DTOs.Entry;

public class EntryDeliveryDetailsDto
{
    public int Id { get; set; }
    public string VendorName { get; set; } = null!;
    public string SystemNumber { get; set; } = null!;
    public string ReceptionNumber { get; set; } = null!;
}