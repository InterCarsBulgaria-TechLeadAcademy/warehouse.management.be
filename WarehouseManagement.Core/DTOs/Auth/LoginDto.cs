using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.Core.DTOs.Auth;

public class LoginDto
{
    [StringLength(100, MinimumLength = 1)]
    public string Username { get; set; } = null!;

    [StringLength(250, MinimumLength = 5)]
    public string Password { get; set; } = null!;
}
