﻿using WarehouseManagement.Common.Enums;

namespace WarehouseManagement.Core.DTOs.Delivery;

public class DeliveryHistoryDto
{
    public int Id { get; set; }

    public ICollection<Change> Changes { get; set; } = new HashSet<Change>();

    public class Change
    {
        public int EntityId { get; set; }

        public string PropertyName { get; set; } = null!;

        public string From { get; set; } = string.Empty;

        public string To { get; set; } = string.Empty;

        public DeliveryHistoryChangeType Type { get; set; }

        public LogType LogType { get; set; }

        public DateTime ChangeDate { get; set; }
    }
}
