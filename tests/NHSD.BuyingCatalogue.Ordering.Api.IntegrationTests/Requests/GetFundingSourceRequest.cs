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
            int orderId)
        {
            this.request = request ?? throw new ArgumentNullException(nameof(request));
            OrderId = orderId;

            getFundingSourceUrl = $"{orderingApiBaseAddress}/api/v1/orders/C{orderId}-01/funding-source";
        }

        public int OrderId { get; }

        public async Task<GetFundingSourceResponse> ExecuteAsync()
        {
            var response = await request.GetAsync(getFundingSourceUrl);

            return await GetFundingSourceResponse.CreateAsync(response);
        }
    }
}
