using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
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
            var orderItems = await response.ReadBodyAsJsonAsync();
            orderItems.Count().Should().Be(0);
        }

        [Then(@"each order item has the expected service instance ID as follows")]
        public async Task ThenEachOrderItemHasTheExpectedServiceInstanceIdAsFollows(Table table)
        {
            var expected = table.CreateSet<ServiceInstanceItem>();
            await getOrderItemsResponse.AssertServiceInstanceIdAsync(expected);
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class ServiceInstanceItem
        {
            public int OrderItemId { get; init; }

            public string ServiceInstanceId { get; init; }
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class OrderItemTable
        {
            public string OrderId { get; init; }

            public string OdsCode { get; init; }

            public string CatalogueItemId { get; init; } = "100001-001";

            public string ParentCatalogueItemId { get; init; }

            public CatalogueItemType CatalogueItemType { get; init; }

            public string CatalogueItemName { get; init; } = Guid.NewGuid().ToString();

            public ProvisioningType ProvisioningType { get; init; } = ProvisioningType.OnDemand;

            public string CataloguePriceUnitName { get; init; } = "patient";

            public string CataloguePriceUnitDescription { get; init; } = "per patient";

            public TimeUnit? PriceTimeUnit { get; init; }

            public string CurrencyCode { get; init; } = "GBP";

            public int Quantity { get; init; } = 1;

            public TimeUnit? EstimationPeriod { get; init; } = TimeUnit.Month;

            public DateTime? DeliveryDate { get; init; }

            public decimal? Price { get; init; }

            public DateTime? Created { get; init; }
        }
    }
}
