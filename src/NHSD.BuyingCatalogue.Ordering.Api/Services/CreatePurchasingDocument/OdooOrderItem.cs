using System;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreatePurchasingDocument
{
    public sealed class OdooOrderItem
    {
        public OdooOrderItem(Order order, OrderItem orderItem, string serviceRecipientName)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            if (orderItem is null)
                throw new ArgumentNullException(nameof(orderItem));

            CallOffAgreementId = order.OrderId;
            CallOffOrderingPartyId = order.OrganisationOdsCode;
            CallOffOrderingPartyName = order.OrganisationName;
            CallOffCommencementDate = order.CommencementDate;
            ServiceRecipientId = orderItem.OdsCode;
            ServiceRecipientName = serviceRecipientName;
            ServiceRecipientItemId = $"{order.OrderId}-{orderItem.OdsCode}-{orderItem.OrderItemId}";
            SupplierId = order.SupplierId;
            SupplierName = order.SupplierName;
            ProductId = orderItem.CatalogueItemId;
            ProductName = orderItem.CatalogueItemName;
            ProductType = orderItem.CatalogueItemType.DisplayName;
            QuantityOrdered = orderItem.Quantity;
            UnitOfOrder = orderItem.CataloguePriceUnit.Description;
            UnitTime = orderItem.PriceTimeUnit?.Description;

            EstimationPeriod = orderItem.ProvisioningType.Equals(ProvisioningType.Declarative)
                ? null
                : orderItem.EstimationPeriod?.Description;

            Price = orderItem.Price.GetValueOrDefault();
            OrderType = orderItem.ProvisioningType.Id;
            M1Planned = orderItem.DeliveryDate;
        }

        public string CallOffAgreementId { get; }

        public string CallOffOrderingPartyId { get; }

        public string CallOffOrderingPartyName { get; }

        public DateTime? CallOffCommencementDate { get; }

        public string ServiceRecipientId { get; }

        public string ServiceRecipientName { get; }

        public string ServiceRecipientItemId { get; }

        public string SupplierId { get; }

        public string SupplierName { get; }

        public string ProductId { get; }

        public string ProductName { get; }

        public string ProductType { get; }

        public int QuantityOrdered { get; }

        public string UnitOfOrder { get; }

        public string UnitTime { get; }

        public string EstimationPeriod { get; }

        public decimal Price { get; }

        public int OrderType { get; }

        public string FundingType { get; } = "Central";

        public DateTime? M1Planned { get; }

        public DateTime? ActualM1Date { get; } = null;

        public DateTime? BuyerVerificationDate { get; } = null;

        public DateTime? CeaseDate { get; } = null;
    }
}
