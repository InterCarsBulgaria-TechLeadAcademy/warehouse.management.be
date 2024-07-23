using System.ComponentModel.DataAnnotations;
using static WarehouseManagement.Common.MessageConstants.Messages;

namespace WarehouseManagement.Core.DTOs.Delivery;

public class DeliveryFormDto
{
    [Required(ErrorMessage = RequiredErrorMessage)]
    [StringLength(100, MinimumLength = 1)]
    public string SystemNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = RequiredErrorMessage)]
    [StringLength(100, MinimumLength = 1)]
    public string ReceptionNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = RequiredErrorMessage)]
    [StringLength(100, MinimumLength = 1)]
    public string TruckNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = RequiredErrorMessage)]
    [StringLength(100, MinimumLength = 1)]
    public string Cmr { get; set; } = string.Empty;
    public DateTime DeliveryTime { get; set; }
    public int Pallets { get; set; }
    public int Packages { get; set; }
    public int Pieces { get; set; }
    public int VendorId { get; set; }
    public ICollection<int> Markers { get; set; } = new List<int>();
}
