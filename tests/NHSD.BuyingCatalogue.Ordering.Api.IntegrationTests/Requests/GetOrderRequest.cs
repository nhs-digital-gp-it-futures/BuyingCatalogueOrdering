using System;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests
{
    internal sealed class GetOrderRequest
    {
        private readonly Request _request;
        private readonly string _getOrderUrl;

        public GetOrderRequest(
            Request request, 
            string orderingApiBaseAddress, 
            string orderId)
        {
            _request = request ?? throw new ArgumentNullException(nameof(request));
            OrderId = orderId ?? throw new ArgumentNullException(nameof(orderId));

            _getOrderUrl =
                $"{orderingApiBaseAddress}/api/v1/orders/{orderId}";
        }

        public string OrderId { get; }

        public async Task<GetOrderResponse> ExecuteAsync()
        {
            var response  = await _request.GetAsync(_getOrderUrl);
            return await GetOrderResponse.CreateAsync(response);
        }
    }
}
