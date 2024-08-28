namespace WarehouseManagement.Core.DTOs.Zone;

public class ZoneDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsFinal { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<string> Markers { get; set; } = new();
}
