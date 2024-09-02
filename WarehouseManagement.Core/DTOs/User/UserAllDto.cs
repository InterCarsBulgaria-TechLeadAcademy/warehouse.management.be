namespace WarehouseManagement.Core.DTOs.User;

public class UserAllDto
{
    public string Id { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Role { get; set; } = string.Empty;

    public string? CreatedBy { get; set; }

    public string CreatedAt { get; set; } = string.Empty;
}
