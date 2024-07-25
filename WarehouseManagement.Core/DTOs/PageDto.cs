namespace WarehouseManagement.Core.DTOs;

public class PageDto<T>
    where T : class
{
    public int Count { get; set; }

    public IEnumerable<T> Results { get; set; } = new HashSet<T>();
}
