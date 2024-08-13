using System.ComponentModel.DataAnnotations;
using WarehouseManagement.Common.Statuses;
using static WarehouseManagement.Common.ValidationConstants.DifferenceConstants;

namespace WarehouseManagement.Core.DTOs.Difference;

public class DifferenceFormDto
{
    [Required]
    [StringLength(ReceptionNumberMaxLength, MinimumLength = ReceptionNumberMinLength)]
    public string ReceptionNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(InternalNumberMaxLength, MinimumLength = InternalNumberMinLength)]
    public string InternalNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(ActiveNumberMaxLength, MinimumLength = ActiveNumberMinLength)]
    public string ActiveNumber { get; set; } = string.Empty;

    [StringLength(CommentMaxLength, MinimumLength = CommentMinLength)]
    public string Comment { get; set; } = string.Empty;

    [Required]
    [Range(MinCount, MaxCount)]
    public int Count { get; set; }

    [Required]
    public int DifferenceTypeId { get; set; }

    [Required]
    public int ZoneId { get; set; }

    [Required]
    public int DeliveryId { get; set; }
}
