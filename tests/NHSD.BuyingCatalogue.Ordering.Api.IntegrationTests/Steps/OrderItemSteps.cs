using System;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Data;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.EntityBuilder;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class OrderItemSteps
    {
        private readonly Settings _settings;
        private readonly OrderItemReferenceList _orderItemReferenceList;

        public OrderItemSteps(
            Settings settings,
            OrderItemReferenceList orderItemReferenceList)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _orderItemReferenceList = orderItemReferenceList ?? throw new ArgumentNullException(nameof(orderItemReferenceList));
        }

        [Given(@"Order items exist")]
        public async Task GivenOrderItemsExist(Table table)
        {
            foreach (var orderItemTableItem in table.CreateSet<OrderItemTable>())
            {
                var orderItemEntity = OrderItemEntityBuilder
                    .Create()
                    .WithOrderId(orderItemTableItem.OrderId)
                    .WithCatalogueItemId(orderItemTableItem.CatalogueItemId)
                    .WithCatalogueItemName(orderItemTableItem.CatalogueItemName)
                    .WithCatalogueItemType(orderItemTableItem.CatalogueItemType)
                    .WithOdsCode(orderItemTableItem.OdsCode)
                    .WithCurrencyCode(orderItemTableItem.CurrencyCode)
                    .WithDeliveryDate(orderItemTableItem.DeliveryDate != DateTime.MinValue ? orderItemTableItem.DeliveryDate : DateTime.UtcNow)
                    .WithEstimationPeriod(orderItemTableItem.EstimationPeriod)
                    .WithPricingUnitTierName(orderItemTableItem.CataloguePriceUnitName)
                    .WithPricingUnitDescription(orderItemTableItem.CataloguePriceUnitDescription)
                    .WithPrice(orderItemTableItem.Price)
                    .WithProvisioningType(orderItemTableItem.ProvisioningType)
                    .WithQuantity(orderItemTableItem.Quantity)
                    .Build();

                var orderItemId = await orderItemEntity.InsertAsync<int>(_settings.ConnectionString);
                orderItemEntity.OrderItemId = orderItemId;

                _orderItemReferenceList.Add(orderItemEntity.CatalogueItemName, orderItemEntity);
            }
        }

        private sealed class OrderItemTable
        {
            public string OrderId { get; set; }

            public string OdsCode { get; set; }

            public string CatalogueItemId { get; set; } = "1000-001";

            public CatalogueItemType CatalogueItemType { get; set; }

            public string CatalogueItemName { get; set; }

            public ProvisioningType ProvisioningType { get; set; } = ProvisioningType.Declarative;

            public string CataloguePriceUnitName { get; set; } = "patient";

            public string CataloguePriceUnitDescription { get; set; } = "per patient";

            public TimeUnit PriceTimeUnit { get; set; }

            public string CurrencyCode { get; set; } = "GBP";

            public int Quantity { get; private set; } = 1;

            public TimeUnit? EstimationPeriod { get; set; } = null;

            public DateTime? DeliveryDate { get; set; }

            public decimal? Price { get; set; }
        }
    }
}
