using System;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses
{
    internal sealed class GetOrderItemResponse : GetOrderItemResponseBase
    {
        private readonly string _content;

        private GetOrderItemResponse(string content)
        {
            _content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public static async Task<GetOrderItemResponse> CreateAsync(Response response)
        {
            var content = await response.ReadBodyAsStringAsync();
            return new GetOrderItemResponse(content);
        }

        public void AssertBody(
            OrderItemEntity orderItemEntity,
            ServiceRecipientEntity serviceRecipient)
        {
            var actual = ReadOrderItem(JToken.Parse(_content));

            var expected = ConvertToExpectedBody(orderItemEntity, serviceRecipient);
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
