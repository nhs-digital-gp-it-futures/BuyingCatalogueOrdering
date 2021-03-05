using System;
using System.Text.Json.Serialization;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class OrderListItemModel
    {
        public OrderListItemModel()
        {
        }

        internal OrderListItemModel(Order order)
        {
            OrderId = order.CallOffId.ToString();
            Description = order.Description;
            LastUpdatedBy = order.LastUpdatedByName;
            LastUpdated = order.LastUpdated;
            DateCreated = order.Created;
            DateCompleted = order.Completed;
            Status = order.OrderStatus.Name;
            OnlyGms = order.FundingSourceOnlyGms;
        }

        public string OrderId { get; init; }

        public string Description { get; init; }

        public string Status { get; init; }

        public DateTime DateCreated { get; init; }

        public DateTime? DateCompleted { get; init; }

        public DateTime LastUpdated { get; init; }

        public string LastUpdatedBy { get; init; }

        [JsonPropertyName("onlyGMS")]
        public bool? OnlyGms { get; init; }
    }
}
