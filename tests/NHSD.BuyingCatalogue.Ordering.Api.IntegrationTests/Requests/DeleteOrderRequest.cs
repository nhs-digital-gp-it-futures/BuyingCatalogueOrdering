using System;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests
{
    internal sealed class DeleteOrderRequest
    {
        private readonly Request _request;
        private readonly string _deleteOrderUrl;

        public DeleteOrderRequest(
            Request request,
            string orderingApiBaseAddress,
            string orderId)
        {
            _request = request ?? throw new ArgumentNullException(nameof(request));
            OrderId = orderId ?? throw new ArgumentNullException(nameof(orderId));

            _deleteOrderUrl =
                $"{orderingApiBaseAddress}/api/v1/orders/{orderId}";
        }

        public string OrderId { get; }

        public async Task<Response> ExecuteAsync()
        {
            return await _request.DeleteAsync(_deleteOrderUrl);
        }
    }
}
