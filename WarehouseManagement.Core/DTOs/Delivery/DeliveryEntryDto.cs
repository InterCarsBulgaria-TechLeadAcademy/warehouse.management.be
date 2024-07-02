namespace WarehouseManagement.Core.DTOs.Delivery;

public class DeliveryEntryDto
{
    public int Id { get; set; }
    public int ZoneId { get; set; }
    public DateTime? StartedProccessing { get; set; }
    public DateTime? FinishedProccessing { get; set; }
}
