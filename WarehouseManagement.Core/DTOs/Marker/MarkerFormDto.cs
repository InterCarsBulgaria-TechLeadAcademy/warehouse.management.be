using System.ComponentModel.DataAnnotations;
using static WarehouseManagement.Common.MessageConstants.Messages;
using static WarehouseManagement.Common.ValidationConstants.MarkerConstants;

namespace WarehouseManagement.Core.DTOs.Marker;

public class MarkerFormDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = RequiredErrorMessage)]
    [StringLength(NameMaxLenght, MinimumLength = NameMinLenght, ErrorMessage = LengthErrorMessage)]
    public string Name { get; set; } = string.Empty;
}
