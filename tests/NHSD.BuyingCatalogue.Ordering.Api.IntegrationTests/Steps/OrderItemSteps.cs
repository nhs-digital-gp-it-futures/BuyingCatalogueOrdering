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
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;
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

        [Given(@"order items exist")]
        public async Task GivenOrderItemsExist(Table table)
        {
            foreach (var orderItemTableItem in table.CreateSet<OrderItemTable>())
            {
                var orderItemEntity = OrderItemEntityBuilder
                    .Create()
                    .WithOrderId(orderItemTableItem.OrderId)
                    .WithCatalogueItemId(orderItemTableItem.CatalogueItemId)
                    .WithProvisioningType(orderItemTableItem.ProvisioningType)
                    .WithPricingUnitName(orderItemTableItem.CataloguePriceUnitName)
                    .WithTimeUnit(orderItemTableItem.PriceTimeUnit)
                    .WithEstimationPeriod(orderItemTableItem.EstimationPeriod)
                    .WithCurrencyCode(orderItemTableItem.CurrencyCode)
                    .WithPrice(orderItemTableItem.Price)
                    .WithCreated(orderItemTableItem.Created ?? DateTime.UtcNow)
                    .Build();

                await orderItemEntity.InsertAsync(settings.ConnectionString);
                orderContext.AddOrderItem(orderItemEntity);
            }
        }

        [When(@"the user makes a request to retrieve a list of order items with orderID (\d{1,6}) and catalogueItemType (.*)")]
        public void WhenTheUserMakesARequestToRetrieveAnOrderItemWithOrderIdAndCatalogueItemType(int orderId, string catalogueItemType)
        {
            getOrderItemsRequest = new GetOrderItemsRequest(
                request,
                settings.OrderingApiBaseUrl,
                orderId,
                catalogueItemType);
        }

        [When(@"the user makes a request to retrieve a list of order items for the order with ID (\d{1,6})")]
        public void WhenTheUserMakesARequestToRetrieveAnOrderItemWithOrderId(int orderId)
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
            var orderItems = orderContext.OrderItemReferenceList[getOrderItemsRequest.OrderId].Values;
            var serviceRecipients = orderContext.ServiceRecipientReferenceList;

            await getOrderItemsResponse.AssertAsync(
                orderItems,
                serviceRecipients,
                orderContext.OrderItemRecipientsReferenceList,
                orderContext.PricingUnitReferenceList,
                orderContext.CatalogueItemReferenceList,
                getOrderItemsRequest.CatalogueItemType);
        }

        [Then(@"the list order items response contains no entries")]
        public async Task ThenAnEmptyCatalogueItemListIsReturned()
        {
            var orderItems = await response.ReadBodyAsJsonAsync();
            orderItems.Count().Should().Be(0);
        }

        [Then(@"the following order items exist for the order with ID (\d{1,6})")]
        public async Task ThenTheFollowingOrderItemsExist(int orderId, Table table)
        {
            var orderItems = await OrderItemEntity.FetchByOrderId(settings.ConnectionString, orderId);
            table.CompareToSet(orderItems);
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class ServiceInstanceItem
        {
            public string CatalogueItemId { get; init; }

            public string OdsCode { get; init; }

            public string ServiceInstanceId { get; init; }
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class OrderItemTable
        {
            public int OrderId { get; init; }

            public string CatalogueItemId { get; init; } = "100001-001";

            public ProvisioningType ProvisioningType { get; init; } = ProvisioningType.OnDemand;

            public string CataloguePriceUnitName { get; init; } = "patient";

            public TimeUnit? PriceTimeUnit { get; init; }

            public string CurrencyCode { get; init; } = "GBP";

            public TimeUnit? EstimationPeriod { get; init; } = TimeUnit.Month;

            public decimal? Price { get; init; }

            public DateTime? Created { get; init; }
        }
    }
}
