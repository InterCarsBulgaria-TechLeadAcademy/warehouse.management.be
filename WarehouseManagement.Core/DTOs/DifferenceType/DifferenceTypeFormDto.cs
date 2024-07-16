using System.ComponentModel.DataAnnotations;
using static WarehouseManagement.Common.ValidationConstants.DifferenceTypeConstants;

namespace WarehouseManagement.Core.DTOs.DifferenceType;

public class DifferenceTypeFormDto
{
    [StringLength(NameMaxLenght, MinimumLength = NameMinLenght)]
    public string Name { get; set; } = string.Empty;
}
