using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests
{
    internal sealed class GetOrderItemsRequest
    {
        private readonly Request _request;
        private readonly string _getOrderItemUrl;

        public GetOrderItemsRequest(
            Request request,
            string orderingApiBaseAddress,
            string orderId,
            string catalogueItemType)
        {
            _request = request;
            OrderId = orderId;
            CatalogueItemType = catalogueItemType;

            _getOrderItemUrl = $"{orderingApiBaseAddress}/api/v1/orders/{orderId}/order-items";

            if (!string.IsNullOrWhiteSpace(CatalogueItemType))
            {
                _getOrderItemUrl += $"?catalogueItemType={CatalogueItemType}";
            }
        }

        public string OrderId { get; }

        public string CatalogueItemType { get; }

        public async Task<GetOrderItemsResponse> ExecuteAsync()
        {
            var response = await _request.GetAsync(_getOrderItemUrl);
            return new GetOrderItemsResponse(response);
        }
    }
}
