using System.Threading.Tasks;
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

        public OrderItemSteps(Settings settings)
        {
            _settings = settings;
        }

        [Given(@"Order Items exist")]
        public async Task GivenOrderItemExist(Table table)
        {
            foreach (var orderItemTable in table.CreateSet<OrderItemTable>())
            {
                var orderItem = OrderItemEntityBuilder
                    .Create()
                    .WithOrderId(orderItemTable.OrderId)
                    .WithCatalogueItemName(orderItemTable.CatalogueItemName)
                    .WithCatalogueItemType(orderItemTable.CatalogueItemTypeEnum)
                    .WithOdsCode(orderItemTable.OdsCode)
                    .Build();

                await orderItem.InsertAsync(_settings.ConnectionString);
            }
        }

        private sealed class OrderItemTable
        {
            public string OrderId { get; set; }
            public string CatalogueItemName { get; set; }
            public CatalogueItemType CatalogueItemTypeEnum { get; set; }
            public string OdsCode { get; set; }
        }
    }
}
