namespace WarehouseManagement.Infrastructure.Data.Models;

public class RefreshToken
{
    public int Id { get; set; }

    public string Token { get; set; } = string.Empty;

    public DateTime ExpirationDate { get; set; }

    public bool IsRevoked { get; set; }

    public Guid UserId { get; set; }

    public ApplicationUser User { get; set; } = null!;
}
