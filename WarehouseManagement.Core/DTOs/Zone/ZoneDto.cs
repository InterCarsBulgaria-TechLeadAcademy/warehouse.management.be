namespace WarehouseManagement.Core.DTOs.Zone;

public class ZoneDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsFinal { get; set; }

    public string CreatedAt { get; set; } = string.Empty;

    public List<string> Markers { get; set; } = new();
}
