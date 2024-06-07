namespace WarehouseManagement.Core.DTOs.Entry;

public class EntryDto
{
    public int Id { get; set; }
    public int Pallets { get; set; }
    public int Packages { get; set; }
    public int Pieces { get; set; }
    public DateTime? StartedProccessing { get; set; }
    public DateTime? FinishedProccessing { get; set; }
    public int DeliveryId { get; set; }
}
