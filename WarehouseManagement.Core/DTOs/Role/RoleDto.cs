﻿using WarehouseManagement.Core.DTOs.RoutePermission;

namespace WarehouseManagement.Core.DTOs.Role;

public class RoleDto
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public IEnumerable<RoutePermissionDto> RoutePermissions { get; set; } = new HashSet<RoutePermissionDto>();
}
