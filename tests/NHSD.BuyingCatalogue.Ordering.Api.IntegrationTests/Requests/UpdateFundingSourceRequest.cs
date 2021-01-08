using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests.Payloads;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests
{
    internal class UpdateFundingSourceRequest
    {
        private readonly Request _request;
        private readonly string _updateFundingSourceUrl;

        public string OrderId { get; set; }
        public UpdateFundingSourceRequestPayload Payload { get; private set; }

        protected IDictionary<string, Func<UpdateFundingSourceRequestPayload>> PayloadFactory = new Dictionary<string, Func<UpdateFundingSourceRequestPayload>>
        {
            { "funding-source-true", () => new UpdateFundingSourceRequestPayload { OnlyGMS = true } },
            { "funding-source-false", () => new UpdateFundingSourceRequestPayload { OnlyGMS = false } },
            { "funding-source-missing", () => new UpdateFundingSourceRequestPayload { OnlyGMS = null } },
        };

        public UpdateFundingSourceRequest(
            Request request,
            string orderingApiBaseAddress,
            string orderId)
        {
            _request = request ?? throw new ArgumentNullException(nameof(request));
            OrderId = orderId ?? throw new ArgumentNullException(nameof(orderId));

            _updateFundingSourceUrl =
                $"{orderingApiBaseAddress}/api/v1/orders/{orderId}/funding-source";
        }

        public void SetPayload(string key)
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));

            if (!PayloadFactory.TryGetValue(key, out var factory))
                Assert.Fail("Unexpected update funding source payload type.");

            Payload = factory();
        }

        public async Task ExecuteAsync()
        {
            await _request.PutJsonAsync(_updateFundingSourceUrl, Payload);
        }
    }
}
