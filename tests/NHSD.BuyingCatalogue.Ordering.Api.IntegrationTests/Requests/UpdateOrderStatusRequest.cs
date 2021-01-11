using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests.Payloads;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests
{
    internal class UpdateOrderStatusRequest
    {
        private readonly Request _request;
        private readonly string _updateOrderStatusUrl;

        public string OrderId { get; set; }

        public UpdateOrderStatusRequestPayload Payload { get; private set; }

        protected IDictionary<string, Func<UpdateOrderStatusRequestPayload>> payloadFactory = new Dictionary<string, Func<UpdateOrderStatusRequestPayload>>
        {
            { "order-status-complete", () => new UpdateOrderStatusRequestPayload { Status = "Complete" } },
            { "order-status-incomplete", () => new UpdateOrderStatusRequestPayload { Status = "Incomplete" } },
            { "order-status-invalid", () => new UpdateOrderStatusRequestPayload { Status = "INVALID" } },
            { "order-status-missing", () => new UpdateOrderStatusRequestPayload { Status = null } },
        };

        public UpdateOrderStatusRequest(Request request, string orderingApiBaseAddress, string orderId)
        {
            _request = request ?? throw new ArgumentNullException(nameof(request));
            OrderId = orderId ?? throw new ArgumentNullException(nameof(orderId));

            _updateOrderStatusUrl =
                $"{orderingApiBaseAddress}/api/v1/orders/{orderId}/status";
        }

        public void SetPayload(string key)
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));

            if (!payloadFactory.TryGetValue(key, out var factory))
                Assert.Fail("Unexpected update funding source payload type.");

            Payload = factory();
        }

        public async Task ExecuteAsync()
        {
            await _request.PutJsonAsync(_updateOrderStatusUrl, Payload);
        }
    }
}
