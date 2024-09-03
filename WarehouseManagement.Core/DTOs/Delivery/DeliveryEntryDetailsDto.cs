namespace WarehouseManagement.Core.DTOs.Delivery;

public class DeliveryEntryDetailsDto
{
    public int Id { get; set; }
    public int Pallets { get; set; }
    public int Packages { get; set; }
    public int Pieces { get; set; }
    public string? StartedProccessing { get; set; }
    public string? FinishedProccessing { get; set; }
    public string ZoneName { get; set; } = string.Empty;
}