﻿using System.ComponentModel.DataAnnotations;
using static WarehouseManagement.Common.MessageConstants.Messages;

namespace WarehouseManagement.Core.DTOs.Vendor
{
    public class VendorFormDto
    {
        [Required(ErrorMessage = RequiredErrorMessage)]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredErrorMessage)]
        [StringLength(100, MinimumLength = 1)]
        public string SystemNumber { get; set; } = string.Empty;

        public int? DefaultZoneId { get; set; }

        public ICollection<int> MarkerIds { get; set; } = new List<int>();
    }
}
