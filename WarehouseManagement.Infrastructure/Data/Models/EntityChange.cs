namespace WarehouseManagement.Infrastructure.Data.Models;

public class EntityChange
{
    public EntityChange()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string PropertyName { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime ChangedAt { get; set; }
    public string ChangedByUserId { get; set; } = string.Empty;
    public string Action { get; set; } = "Created";
}
