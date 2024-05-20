namespace WarehouseManagement.Infrastructure.Data.Models;

public class DeliveryMarker
{
    public int DeliveryId { get; set; }
    public Delivery Delivery { get; set; } = null!;
    public int MarkerId { get; set; }
    public Marker Marker { get; set; } = null!;
}
