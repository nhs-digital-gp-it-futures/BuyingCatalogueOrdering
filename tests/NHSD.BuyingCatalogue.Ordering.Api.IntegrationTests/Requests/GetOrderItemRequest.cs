using System;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests
{
    internal sealed class GetOrderItemRequest
    {
        private readonly Request request;
        private readonly string getOrderItemUrl;

        public GetOrderItemRequest(
            Request request,
            string orderingApiBaseAddress,
            int orderId,
            string catalogueItemId)
        {
            this.request = request ?? throw new ArgumentNullException(nameof(request));

            OrderId = orderId;
            CatalogueItemId = catalogueItemId;

            getOrderItemUrl = $"{orderingApiBaseAddress}/api/v1/orders/C{orderId}-01/order-items/{catalogueItemId}";
        }

        public int OrderId { get; }

        public string CatalogueItemId { get; }

        public async Task<GetOrderItemResponse> ExecuteAsync()
        {
            var response = await request.GetAsync(getOrderItemUrl);

            return await GetOrderItemResponse.CreateAsync(response);
        }
    }
}
