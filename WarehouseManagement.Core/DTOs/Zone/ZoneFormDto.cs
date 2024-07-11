using System.ComponentModel.DataAnnotations;
using static WarehouseManagement.Common.MessageConstants.Messages;
using static WarehouseManagement.Common.ValidationConstants.ZoneConstants;

namespace WarehouseManagement.Core.DTOs.Zone;

public class ZoneFormDto
{
    [Required]
    [StringLength(NameMaxLength, MinimumLength = NameMinLength, ErrorMessage = LengthErrorMessage)]
    public string Name { get; set; } = string.Empty;

    public bool? IsFinal { get; set; }

    public IEnumerable<int> MarkerIds { get; set; } = new List<int>();
}
