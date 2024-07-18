namespace WarehouseManagement.Infrastructure.Data.Models;

public class Entry : BaseClass
{
    public int Pallets { get; set; }
    public int Packages { get; set; }
    public int Pieces { get; set; }

    public DateTime? StartedProcessing { get; set; }
    public DateTime? FinishedProcessing { get; set; }
    public int ZoneId { get; set; }
    public Zone Zone { get; set; } = null!;
    public int DeliveryId { get; set; }
    public Delivery Delivery { get; set; } = null!;
}
