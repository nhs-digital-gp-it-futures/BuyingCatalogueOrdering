using System;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests
{
    internal sealed class GetFundingSourceRequest
    {
        private readonly Request request;
        private readonly string getFundingSourceUrl;

        public GetFundingSourceRequest(
            Request request,
            string orderingApiBaseAddress,
            string orderId)
        {
            this.request = request ?? throw new ArgumentNullException(nameof(request));
            OrderId = orderId ?? throw new ArgumentNullException(nameof(orderId));

            getFundingSourceUrl = $"{orderingApiBaseAddress}/api/v1/orders/{orderId}/funding-source";
        }

        public string OrderId { get; }

        public async Task<GetFundingSourceResponse> ExecuteAsync()
        {
            var response = await request.GetAsync(getFundingSourceUrl);

            return await GetFundingSourceResponse.CreateAsync(response);
        }
    }
}
