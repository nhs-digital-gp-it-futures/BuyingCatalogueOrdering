using System;
using NHSD.BuyingCatalogue.Ordering.Application;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreatePurchasingDocument
{
    public sealed class OdooOrderItem
    {
        public OdooOrderItem(FlattenedOrderItem orderItem)
        {
            if (orderItem is null)
                throw new ArgumentNullException(nameof(orderItem));

            var order = orderItem.Order;
            var recipient = orderItem.Recipient;

            CallOffAgreementId = order.CallOffId.ToString();
            CallOffOrderingPartyId = order.OrderingParty.OdsCode;
            CallOffOrderingPartyName = order.OrderingParty.Name;
            CallOffCommencementDate = order.CommencementDate;
            ServiceRecipientId = recipient.OdsCode;
            ServiceRecipientName = recipient.Name;
            ServiceRecipientItemId = $"{order.CallOffId}-{recipient.OdsCode}-{orderItem.ItemId}";
            SupplierId = order.Supplier.Id;
            SupplierName = order.Supplier.Name;
            ProductId = orderItem.CatalogueItem.Id.ToString();
            ProductName = orderItem.CatalogueItem.Name;
            ProductType = orderItem.CatalogueItem.CatalogueItemType.DisplayName();
            QuantityOrdered = orderItem.Quantity;
            UnitOfOrder = orderItem.PricingUnit.Description;
            UnitTime = orderItem.PriceTimeUnit?.Description();

            EstimationPeriod = orderItem.ProvisioningType.Equals(ProvisioningType.Declarative)
                ? null
                : orderItem.EstimationPeriod?.Description();

            Price = orderItem.Price.GetValueOrDefault();
            OrderType = (int)orderItem.ProvisioningType;
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

        // The remaining properties are required fields in the finance CSV file but they are not populated
        // ReSharper disable once UnassignedGetOnlyAutoProperty (used by CsvHelper to generate empty column)
        public DateTime? ActualM1Date { get; }

        // ReSharper disable once UnassignedGetOnlyAutoProperty (used by CsvHelper to generate empty column)
        public DateTime? BuyerVerificationDate { get; }

        // ReSharper disable once UnassignedGetOnlyAutoProperty (used by CsvHelper to generate empty column)
        public DateTime? CeaseDate { get; }
    }
}
