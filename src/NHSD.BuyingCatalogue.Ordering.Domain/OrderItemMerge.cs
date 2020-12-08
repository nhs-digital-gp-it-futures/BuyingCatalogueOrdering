using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class OrderItemMerge
    {
        private readonly HashSet<CatalogueItemType> catalogueItemTypes = new HashSet<CatalogueItemType>();
        private readonly List<OrderItem> newItems = new List<OrderItem>();
        private readonly Dictionary<int, OrderItem> updatedItems = new Dictionary<int, OrderItem>();

        public OrderItemMerge(Guid userId, string userName)
        {
            if (userName is null)
                throw new ArgumentNullException(nameof(userName));

            UserId = userId;
            UserName = string.IsNullOrWhiteSpace(userName)
                ? throw new ArgumentException($"{nameof(userName)} cannot be empty.", nameof(userName))
                : userName;
        }

        public IReadOnlyList<OrderItem> NewItems => newItems;

        public IDictionary<int, OrderItem> UpdatedItems => updatedItems;

        public Guid UserId { get; }

        public string UserName { get; }

        public bool AddOrderItems(IEnumerable<OrderItem> items)
        {
            return items.Aggregate(true, (current, orderItem) => current && AddOrderItem(orderItem));
        }

        public bool AddOrderItem(OrderItem item)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));

            if (item.OrderItemId == default)
            {
                newItems.Add(item);
                catalogueItemTypes.Add(item.CatalogueItemType);
            }
            else if (updatedItems.ContainsKey(item.OrderItemId))
            {
                return false;
            }
            else
            {
                updatedItems.Add(item.OrderItemId, item);
            }

            return true;
        }

        public void MarkOrderSectionsAsViewed(Order order)
        {
            foreach (var itemType in catalogueItemTypes)
                itemType.MarkOrderSectionAsViewed(order);
        }
    }
}
