﻿using System;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class OrderListItemModel
    {
        public string OrderId { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DateCompleted { get; set; }

        public DateTime LastUpdated { get; set; }

        public string LastUpdatedBy { get; set; }
    }
}
