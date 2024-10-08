﻿using WarehouseManagement.Infrastructure.Data.Models;

namespace WarehouseManagement.Core.DTOs.Marker;

public class MarkerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
}
