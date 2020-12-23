using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Data;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.EntityBuilder;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class OrderItemSteps
    {
        private readonly Request request;
        private readonly Response response;
        private readonly Settings settings;
        private readonly OrderContext orderContext;
        private GetOrderItemsRequest getOrderItemsRequest;
        private GetOrderItemsResponse getOrderItemsResponse;

        public OrderItemSteps(
            Request request,
            Response response,
            Settings settings,
            OrderContext orderContext)
        {
            this.request = request;
            this.response = response;
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.orderContext = orderContext;
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
                    .WithParentCatalogueItemId(orderItemTableItem.ParentCatalogueItemId)
                    .WithCatalogueItemName(orderItemTableItem.CatalogueItemName)
                    .WithCatalogueItemType(orderItemTableItem.CatalogueItemType)
                    .WithOdsCode(orderItemTableItem.OdsCode)
                    .WithCurrencyCode(orderItemTableItem.CurrencyCode)
                    .WithDeliveryDate(orderItemTableItem.DeliveryDate != DateTime.MinValue ? orderItemTableItem.DeliveryDate : DateTime.UtcNow)
                    .WithEstimationPeriod(orderItemTableItem.EstimationPeriod)
                    .WithPricingUnitName(orderItemTableItem.CataloguePriceUnitName)
                    .WithPricingUnitDescription(orderItemTableItem.CataloguePriceUnitDescription)
                    .WithPrice(orderItemTableItem.Price)
                    .WithTimeUnit(orderItemTableItem.PriceTimeUnit)
                    .WithProvisioningType(orderItemTableItem.ProvisioningType)
                    .WithQuantity(orderItemTableItem.Quantity)
                    .WithCreated(orderItemTableItem.Created ?? DateTime.UtcNow)
                    .Build();

                var orderItemId = await orderItemEntity.InsertAsync<int>(settings.ConnectionString);
                orderItemEntity.OrderItemId = orderItemId;

                orderContext.OrderItemReferenceList.Add(orderItemEntity.CatalogueItemName, orderItemEntity);
            }
        }

        [When(@"the user makes a request to retrieve a list of order items with orderID (.*) and catalogueItemType (.*)")]
        public void WhenTheUserMakesARequestToRetrieveAnOrderItemWithOrderIdAndCatalogueItemType(string orderId, string catalogueItemType)
        {
            getOrderItemsRequest = new GetOrderItemsRequest(
                request,
                settings.OrderingApiBaseUrl,
                orderId,
                catalogueItemType);
        }

        [When(@"the user makes a request to retrieve a list of order items for the order with ID (.*)")]
        public void WhenTheUserMakesARequestToRetrieveAnOrderItemWithOrderId(string orderId)
        {
            getOrderItemsRequest = new GetOrderItemsRequest(
                request,
                settings.OrderingApiBaseUrl,
                orderId,
                null);
        }

        [When(@"the user sends the retrieve a list of order items request")]
        public async Task WhenTheUserSendsTheOrderItemRequest()
        {
            getOrderItemsResponse = await getOrderItemsRequest.ExecuteAsync();
        }

        [Then(@"the order item response displays the expected order items")]
        public async Task ThenTheOrderItemResponseDisplaysTheExpectedOrderItem()
        {
            var orderItems = orderContext.OrderItemReferenceList.FindByOrderId(getOrderItemsRequest.OrderId);
            var serviceRecipients = orderContext.ServiceRecipientReferenceList.FindByOrderId(getOrderItemsRequest.OrderId);

            await getOrderItemsResponse.AssertAsync(orderItems, serviceRecipients, getOrderItemsRequest.CatalogueItemType);
        }

        [Then(@"the list order items response contains no entries")]
        public async Task ThenAnEmptyCatalogueItemListIsReturned()
        {
            var orderItems = (await response.ReadBodyAsJsonAsync());
            orderItems.Count().Should().Be(0);
        }

        [Then(@"each order item has the expected service instance ID as follows")]
        public async Task ThenEachOrderItemHasTheExpectedServiceInstanceIDAsFollows(Table table)
        {
            var expected = table.CreateSet<ServiceInstanceItem>();
            await getOrderItemsResponse.AssertServiceInstanceIdAsync(expected);
        }

        private sealed class ServiceInstanceItem
        {
            public int OrderItemId { get; set; }

            public string ServiceInstanceId { get; set; }
        }

        private sealed class OrderItemTable
        {
            public string OrderId { get; set; }

            public string OdsCode { get; set; }

            public string CatalogueItemId { get; set; } = "100001-001";

            public string ParentCatalogueItemId { get; set; }

            public CatalogueItemType CatalogueItemType { get; set; }

            public string CatalogueItemName { get; set; } = Guid.NewGuid().ToString();

            public ProvisioningType ProvisioningType { get; set; } = ProvisioningType.OnDemand;

            public string CataloguePriceUnitName { get; set; } = "patient";

            public string CataloguePriceUnitDescription { get; set; } = "per patient";

            public TimeUnit? PriceTimeUnit { get; set; } = null;

            public string CurrencyCode { get; set; } = "GBP";

            public int Quantity { get; private set; } = 1;

            public TimeUnit? EstimationPeriod { get; set; } = TimeUnit.Month;

            public DateTime? DeliveryDate { get; set; }

            public decimal? Price { get; set; }

            public DateTime? Created { get; set; }
        }
    }
}
