namespace WarehouseManagement.Infrastructure.Data.Models;

public abstract class BaseClass
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedByUserId { get; set; } = string.Empty;

    public DateTime? LastModifiedAt { get; set; }

    public string? LastModifiedByUserId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? DeletedByUserId { get; set; }
}
