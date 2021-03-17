using System;
using System.Collections.Generic;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support
{
    internal sealed class OrderContext
    {
        private readonly Dictionary<int, IDictionary<string, OrderItemEntity>> orderItemReferenceList = new();

        public OrderContext()
        {
            AddressReferenceList = new Dictionary<int, AddressEntity>();
            CatalogueItemReferenceList = new Dictionary<string, CatalogueItemEntity>();
            ContactReferenceList = new Dictionary<int, ContactEntity>();
            OrderingPartyReferenceList = new Dictionary<Guid, OrderingPartyEntity>();
            OrderReferenceList = new Dictionary<int, OrderEntity>();
            OrderItemRecipientsReferenceList = new Dictionary<(int, string), IList<OrderItemRecipientEntity>>();
            PricingUnitReferenceList = new Dictionary<string, PricingUnitEntity>();
            ServiceRecipientReferenceList = new Dictionary<string, ServiceRecipientEntity>();
            SupplierReferenceList = new Dictionary<string, SupplierEntity>();
        }

        public IDictionary<int, AddressEntity> AddressReferenceList { get; }

        public IDictionary<string, CatalogueItemEntity> CatalogueItemReferenceList { get; }

        public IDictionary<int, ContactEntity> ContactReferenceList { get; }

        public IDictionary<Guid, OrderingPartyEntity> OrderingPartyReferenceList { get; }

        public IDictionary<int, OrderEntity> OrderReferenceList { get; }

        public IReadOnlyDictionary<int, IDictionary<string, OrderItemEntity>> OrderItemReferenceList => orderItemReferenceList;

        public IDictionary<(int OrderId, string CatalogueItemId), IList<OrderItemRecipientEntity>> OrderItemRecipientsReferenceList { get; }

        public IDictionary<string, PricingUnitEntity> PricingUnitReferenceList { get; }

        public IDictionary<string, ServiceRecipientEntity> ServiceRecipientReferenceList { get; }

        public IDictionary<string, SupplierEntity> SupplierReferenceList { get; }

        public void AddOrderItem(OrderItemEntity orderItem)
        {
            var orderId = orderItem.OrderId;

            if (!orderItemReferenceList.ContainsKey(orderId))
                orderItemReferenceList.Add(orderId, new Dictionary<string, OrderItemEntity>());

            orderItemReferenceList[orderId].Add(orderItem.CatalogueItemId, orderItem);
        }
    }
}
