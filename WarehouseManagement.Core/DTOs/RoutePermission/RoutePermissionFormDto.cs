using System.ComponentModel.DataAnnotations;
using static WarehouseManagement.Common.ValidationConstants.RoutePermissionConstants;

namespace WarehouseManagement.Core.DTOs.RoutePermission;

public class RoutePermissionFormDto
{
    [StringLength(NameMaxLength, MinimumLength = NameMinLength)]
    public string Name { get; set; } = string.Empty;

    public string ControllerName { get; set; } = string.Empty;

    public string ActionName { get; set; } = string.Empty;
}
