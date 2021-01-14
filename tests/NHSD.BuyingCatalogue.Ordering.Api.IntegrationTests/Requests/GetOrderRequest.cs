using System;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests
{
    internal sealed class GetOrderRequest
    {
        private readonly Request request;
        private readonly string getOrderUrl;

        public GetOrderRequest(
            Request request,
            string orderingApiBaseAddress,
            string orderId)
        {
            this.request = request ?? throw new ArgumentNullException(nameof(request));
            OrderId = orderId ?? throw new ArgumentNullException(nameof(orderId));

            getOrderUrl =
                $"{orderingApiBaseAddress}/api/v1/orders/{orderId}";
        }

        public string OrderId { get; }

        public async Task<GetOrderResponse> ExecuteAsync()
        {
            var response = await request.GetAsync(getOrderUrl);
            return await GetOrderResponse.CreateAsync(response);
        }
    }
}
