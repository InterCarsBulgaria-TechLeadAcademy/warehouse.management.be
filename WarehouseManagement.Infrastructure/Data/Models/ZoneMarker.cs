namespace WarehouseManagement.Infrastructure.Data.Models;

public class ZoneMarker
{
    public int ZoneId { get; set; }
    public Zone Zone { get; set; } = null!;
    public int MarkerId { get; set; }
    public Marker Marker { get; set; } = null!;
}
