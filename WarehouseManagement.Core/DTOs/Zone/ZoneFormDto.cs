using System.ComponentModel.DataAnnotations;
using static WarehouseManagement.Common.MessageConstants.Messages;
using static WarehouseManagement.Common.ValidationConstants.ZoneConstants;

namespace WarehouseManagement.Core.DTOs.Zone;

public class ZoneFormDto
{
    [StringLength(NameMaxLength, MinimumLength = NameMinLength, ErrorMessage = LengthErrorMessage)]
    public string Name { get; set; } = string.Empty;
}
