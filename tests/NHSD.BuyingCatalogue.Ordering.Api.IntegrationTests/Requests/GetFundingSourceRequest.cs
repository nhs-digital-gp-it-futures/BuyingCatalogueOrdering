using System;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests
{
    internal sealed class GetFundingSourceRequest
    {
        private readonly Request _request;
        private readonly string _getFundingSourceUrl;

        public GetFundingSourceRequest(
            Request request,
            string orderingApiBaseAddress,
            string orderId)
        {
            _request = request ?? throw new ArgumentNullException(nameof(request));
            OrderId = orderId ?? throw new ArgumentNullException(nameof(orderId));

            _getFundingSourceUrl = $"{orderingApiBaseAddress}/api/v1/orders/{orderId}/funding-source";
        }

        public string OrderId { get; }

        public async Task<GetFundingSourceResponse> ExecuteAsync()
        {
            var response = await _request.GetAsync(_getFundingSourceUrl);

            return await GetFundingSourceResponse.CreateAsync(response);
        }
    }
}
