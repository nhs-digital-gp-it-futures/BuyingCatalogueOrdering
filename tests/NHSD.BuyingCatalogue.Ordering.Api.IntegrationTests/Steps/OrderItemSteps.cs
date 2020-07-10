using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
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
        private readonly Request _request;
        private readonly Response _response;
        private readonly Settings _settings;
        private readonly OrderContext _orderContext;
        private GetOrderItemsRequest _getOrderItemsRequest;
        private GetOrderItemsResponse _getOrderItemsResponse;

        public OrderItemSteps(
            Request request,
            Response response,
            Settings settings,
            OrderContext orderContext)
        {
            _request = request;
            _response = response;
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _orderContext = orderContext;
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
                    .WithPricingUnitName(orderItemTableItem.CataloguePriceUnitName)
                    .WithPricingUnitDescription(orderItemTableItem.CataloguePriceUnitDescription)
                    .WithPrice(orderItemTableItem.Price)
                    .WithTimeUnit(orderItemTableItem.PriceTimeUnit)
                    .WithProvisioningType(orderItemTableItem.ProvisioningType)
                    .WithQuantity(orderItemTableItem.Quantity)
                    .WithCreated(orderItemTableItem.Created ?? DateTime.UtcNow)
                    .Build();

                var orderItemId = await orderItemEntity.InsertAsync<int>(_settings.ConnectionString);
                orderItemEntity.OrderItemId = orderItemId;

                // temporarily removing these from the reference list as they are not returned by the GET
                orderItemEntity.TimeUnit = null;
                orderItemEntity.EstimationPeriod = null;
                _orderContext.OrderItemReferenceList.Add(orderItemEntity.CatalogueItemName, orderItemEntity);
            }
        }

        [When(@"the user makes a request to retrieve a list of order items with orderID (.*) and catalogueItemType (.*)")]
        public void WhenTheUserMakesARequestToRetrieveAnOrderItemWithOrderIDAndCatalogueItemType(string orderId, string catalogueItemType)
        {
            _getOrderItemsRequest = new GetOrderItemsRequest(
                _request,
                _settings.OrderingApiBaseUrl,
                orderId,
                catalogueItemType);
        }

        [When(@"the user sends the retrieve a list of order items request")]
        public async Task WhenTheUserSendsTheOrderItemRequest()
        {
            _getOrderItemsResponse = await _getOrderItemsRequest.ExecuteAsync();
        }

        [Then(@"the order item response displays the expected order items")]
        public async Task ThenTheOrderItemResponseDisplaysTheExpectedOrderItem()
        {
            var orderItems = _orderContext.OrderItemReferenceList.FindByOrderId(_getOrderItemsRequest.OrderId);
            var serviceRecipients = _orderContext.ServiceRecipientReferenceList.FindByOrderId(_getOrderItemsRequest.OrderId);

            await _getOrderItemsResponse.AssertAsync(orderItems, serviceRecipients, _getOrderItemsRequest.CatalogueItemType);
        }

        [Then(@"the list order items response contains no entries")]
        public async Task ThenAnEmptyCatalougeItemListIsReturned()
        {
            var orderItems = (await _response.ReadBodyAsJsonAsync());
            orderItems.Count().Should().Be(0);
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

            public TimeUnit? PriceTimeUnit { get; set; } = null;

            public string CurrencyCode { get; set; } = "GBP";

            public int Quantity { get; private set; } = 1;

            public TimeUnit? EstimationPeriod { get; set; } = null;

            public DateTime? DeliveryDate { get; set; }

            public decimal? Price { get; set; }

            public DateTime? Created { get; set; }
        }
    }
}
