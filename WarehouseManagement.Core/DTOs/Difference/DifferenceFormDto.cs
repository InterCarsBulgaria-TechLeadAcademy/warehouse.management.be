using System.ComponentModel.DataAnnotations;
using WarehouseManagement.Common.Statuses;
using static WarehouseManagement.Common.ValidationConstants.DifferenceConstants;

namespace WarehouseManagement.Core.DTOs.Difference;

public class DifferenceFormDto
{
    [Required]
    [Range(ReceptionNumberMinLength, ReceptionNumberMaxLength)]
    public string ReceptionNumber { get; set; } = string.Empty;

    [Required]
    [Range(InternalNumberMinLength, InternalNumberMaxLength)]
    public string InternalNumber { get; set; } = string.Empty;

    [Required]
    [Range(ActiveNumberMinLength, ActiveNumberMaxLength)]
    public string ActiveNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(CommentMaxLength, MinimumLength = CommentMinLength)]
    public string Comment { get; set; } = string.Empty;

    [Required]
    [Range(MinCount, MaxCount)]
    public int Count { get; set; }

    public DifferenceStatus? Status { get; set; }

    [Required]
    public int DifferenceTypeId { get; set; }

    [Required]
    public int ZoneId { get; set; }

    [Required]
    public int DeliveryId { get; set; }
}
