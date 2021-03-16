using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses
{
    internal sealed class GetOrderItemResponse : GetOrderItemResponseBase
    {
        private readonly string content;

        private GetOrderItemResponse(string content)
        {
            this.content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public static async Task<GetOrderItemResponse> CreateAsync(Response response)
        {
            var content = await response.ReadBodyAsStringAsync();
            return new GetOrderItemResponse(content);
        }

        public void AssertBody(
            OrderItemEntity orderItemEntity,
            IDictionary<string, ServiceRecipientEntity> serviceRecipients,
            IList<OrderItemRecipientEntity> orderItemRecipients,
            IDictionary<string, PricingUnitEntity> pricingUnits)
        {
            var actual = ReadOrderItem(JToken.Parse(content));

            var expected = ConvertToExpectedBody(
                orderItemEntity,
                serviceRecipients,
                orderItemRecipients,
                pricingUnits);

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
