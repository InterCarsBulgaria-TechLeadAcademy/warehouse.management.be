using System.ComponentModel.DataAnnotations;
using WarehouseManagement.Common.Statuses;

namespace WarehouseManagement.Infrastructure.Data.Models;

public class Difference : BaseClass
{
    public string ReceptionNumber { get; set; } = string.Empty;

    public string InternalNumber { get; set; } = string.Empty;

    public string ActiveNumber { get; set; } = string.Empty;

    public string Comment { get; set; } = string.Empty;

    public string AdminComment { get; set; } = string.Empty;

    public int Count { get; set; }

    public DifferenceStatus Status { get; set; }

    public int TypeId { get; set; }

    public DifferenceType Type { get; set; } = null!;

    public int DeliveryId { get; set; }

    public Delivery Delivery { get; set; } = null!;

    public int ZoneId { get; set; }

    public Zone Zone { get; set; } = null!;
}
