namespace WarehouseManagement.Core.DTOs.Difference;

using System.ComponentModel.DataAnnotations;
using static WarehouseManagement.Common.ValidationConstants.DifferenceConstants;

public class DifferenceAdminCommentDto
{
    [Range(AdminCommentMinLength, AdminCommentMaxLength)]
    public string AdminComment { get; set; } = string.Empty;
}
