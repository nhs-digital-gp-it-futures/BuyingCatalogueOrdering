using System;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Data;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.EntityBuilder;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Entities
{
    [Binding]
    internal sealed class OrderItemEntitySteps
    {
        private readonly Settings _settings;
        private readonly OrderItemReferenceList _orderItemReferenceList;

        public OrderItemEntitySteps(
            Settings settings,
            OrderItemReferenceList orderItemReferenceList)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _orderItemReferenceList = orderItemReferenceList ?? throw new ArgumentNullException(nameof(orderItemReferenceList));
        }

        [Given(@"Order items exist")]
        public async Task GivenOrderItemsExist(Table table)
        {
            foreach (var orderItemTableItem in table.CreateSet<OrderItemsTable>())
            {
                var orderItemEntity = OrderItemEntityBuilder
                    .Create()
                    .WithOrderId(orderItemTableItem.OrderId)
                    .WithOdsCode(orderItemTableItem.OdsCode)
                    .WithCatalogueItemName(orderItemTableItem.CatalogueItemName)
                    .Build();

                var orderItemId = await orderItemEntity.InsertAsync<int>(_settings.ConnectionString);
                orderItemEntity.OrderItemId = orderItemId;

                _orderItemReferenceList.Add(orderItemEntity.CatalogueItemName, orderItemEntity);
            }
        }

        private sealed class OrderItemsTable
        {
            public string OrderId { get; set; }

            public string OdsCode { get; set; }

            public string CatalogueItemId { get; set; }

            public CatalogueItemType CatalogueItemType { get; set; }

            public string CatalogueItemName { get; set; }

            public ProvisioningType ProvisioningType { get; set; }

            public CataloguePriceType CataloguePriceType { get; set; }

            public string CataloguePriceUnitName { get; set; }

            public string CataloguePriceUnitDescription { get; set; }

            public TimeUnit PriceTimeUnit { get; set; }

            public string CurrencyCode { get; set; }

            public int Quantity { get; private set; }

            public TimeUnit EstimationPeriod { get; set; }

            public DateTime? DeliveryDate { get; set; }

            public decimal? Price { get; set; }
        }
    }
}
