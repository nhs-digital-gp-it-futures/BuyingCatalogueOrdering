using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal class GetCatalogueSolutionOrderItemSteps
    {
        private readonly Request _request;
        private readonly Response _response;
        private readonly OrderItemReferenceList _orderItemReferenceList;

        private readonly string _orderCatalogueSolutionsUrl;

        public GetCatalogueSolutionOrderItemSteps(Request request, Response response, OrderItemReferenceList orderItemReferenceList, Settings settings)
        {
            _request = request;
            _response = response;
            _orderItemReferenceList = orderItemReferenceList;
            _orderCatalogueSolutionsUrl = settings.OrderingApiBaseUrl + "/api/v1/orders/{0}/sections/catalogue-solutions";
        }
        
        [When(@"the user makes a request to retrieve an order catalogue solution With orderID (.*) and CatalogueItemName (.*)")]
        public async Task WhenAGetRequestIsMadeForASingleOrdersCatalogueSolutionWithOrderIdAndOrderItemId(string orderId, string name)
        {
            var url = string.Format(_orderCatalogueSolutionsUrl, orderId);
            var orderItemId = _orderItemReferenceList.GetByCatalogueSolutionItemName(name).OrderItemId;

            await _request.GetAsync($"{url}/{orderItemId}");
        }

        [When(@"the user makes a request to retrieve an order catalogue solution With orderID (.*) and Invalid OrderItemID (.*)")]
        public async Task WhenAGetRequestIsMadeForASingleOrdersCatalogueSolutionWithOrderIdAndOrderItemId(string orderId, int orderItemId)
        {
            var url = string.Format(_orderCatalogueSolutionsUrl, orderId);

            await _request.GetAsync($"{url}/{orderItemId}");
        }

        [Then(@"the catalogue solutions response contains a single solution")]
        public async Task ThenTheCatalogueSolutionsResponseContainsASingleSolution(Table table)
        {
            var expected = table.CreateSet<GetOrderItemModel>().FirstOrDefault();

            var response = await _response.ReadBodyAsJsonAsync();

            var actual = new GetOrderItemModel
            {
                ServiceRecipientOdsCode = response.SelectToken("serviceRecipient").Value<string>("odsCode"),
                CatalogueSolutionId = response.Value<string>("catalogueSolutionId"),
                CatalogueSolutionName = response.Value<string>("catalogueSolutionName"),
                CurrencyCode = response.Value<string>("currencyCode"),
                DeliveryDate = response.Value<DateTime?>("deliveryDate"),
                EstimationPeriod = response.Value<string>("estimationPeriod"),
                ItemUnitDescription = response.SelectToken("itemUnitModel").Value<string>("description"),
                ItemUnitName = response.SelectToken("itemUnitModel").Value<string>("name"),
                Price = response.Value<decimal?>("price"),
                ProvisioningType = response.Value<string>("provisioningType"),
                Quantity = response.Value<int>("quantity"),
                Type = response.Value<string>("type")
            };

            actual.Should().BeEquivalentTo(expected);
        }

        private sealed class GetOrderItemModel
        {
            public string ServiceRecipientOdsCode { get; set; }
            public string CatalogueSolutionId { get; set; }
            public string CatalogueSolutionName { get; set; }
            public string CurrencyCode { get; set; }
            public DateTime? DeliveryDate { get; set; }
            public string EstimationPeriod { get; set; }
            public string ItemUnitDescription { get; set; }
            public string ItemUnitName { get; set; }
            public decimal? Price { get; set; }
            public string ProvisioningType { get; set; }
            public int Quantity { get; set; }
            public string Type { get; set; }
        }
    }
}
