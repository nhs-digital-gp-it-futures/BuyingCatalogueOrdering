using System;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests
{
    internal sealed class DeleteOrderRequest
    {
        private readonly Request request;
        private readonly string deleteOrderUrl;

        public DeleteOrderRequest(
            Request request,
            string orderingApiBaseAddress,
            string orderId)
        {
            this.request = request ?? throw new ArgumentNullException(nameof(request));
            OrderId = orderId ?? throw new ArgumentNullException(nameof(orderId));

            deleteOrderUrl =
                $"{orderingApiBaseAddress}/api/v1/orders/{orderId}";
        }

        public string OrderId { get; }

        public async Task<Response> ExecuteAsync()
        {
            return await request.DeleteAsync(deleteOrderUrl);
        }
    }
}
