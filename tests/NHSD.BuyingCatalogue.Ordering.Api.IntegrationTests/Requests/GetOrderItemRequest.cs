using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests
{
    internal sealed class GetOrderItemRequest
    {
        private readonly Request _request;
        private string _getOrderItemUrl;

        public GetOrderItemRequest(Request request,
            string orderingApiBaseAddress,
            string orderId,
            string catalogueItemType)
        {
            _request = request;
            OrderId = orderId;
            CatalogueItemType = catalogueItemType;

            _getOrderItemUrl = $"{orderingApiBaseAddress}/api/v1/orders/{orderId}/order-items";
        }

        public string OrderId { get; }

        public string CatalogueItemType { get; }

        public async Task<GetOrderItemResponse> ExecuteAsync()
        {
            if (!string.IsNullOrWhiteSpace(CatalogueItemType))
            {
                _getOrderItemUrl += $"?catalogueItemType={CatalogueItemType}";
            }

            var response = await _request.GetAsync(_getOrderItemUrl);
            return new GetOrderItemResponse(response);
        }
    }
}
