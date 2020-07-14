using System;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests
{
    internal sealed class GetOrderItemRequest
    {
        private readonly Request _request;
        private readonly string _getOrderItemUrl;

        public GetOrderItemRequest(
            Request request,
            string orderingApiBaseAddress,
            string orderId,
            int? orderItemId)
        {
            _request = request ?? throw new ArgumentNullException(nameof(request));

            OrderId = orderId ?? throw new ArgumentNullException(nameof(orderId));
            OrderItemId = orderItemId;

            _getOrderItemUrl = $"{orderingApiBaseAddress}/api/v1/orders/{orderId}/order-items/{orderItemId.GetValueOrDefault()}";
        }

        public string OrderId { get; }

        public int? OrderItemId { get; }

        public async Task<GetOrderItemResponse> ExecuteAsync()
        {
            var response = await _request.GetAsync(_getOrderItemUrl);

            return await GetOrderItemResponse.CreateAsync(response);
        }
    }
}
