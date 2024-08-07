﻿using WarehouseManagement.Infrastructure.Data.Models;

namespace WarehouseManagement.Core.DTOs.Marker;

public class MarkerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public ICollection<MarkerDeliveryDto> Deliveries { get; set; } =
        new HashSet<MarkerDeliveryDto>();
    public ICollection<MarkerZoneDto> Zones { get; set; } = new HashSet<MarkerZoneDto>();
    public ICollection<MarkerVendorDto> Vendors { get; set; } = new HashSet<MarkerVendorDto>();
}
