using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses
{
    internal sealed class GetOrderItemsResponse : GetOrderItemResponseBase
    {
        private readonly Response response;

        public GetOrderItemsResponse(Response response)
        {
            this.response = response ?? throw new ArgumentNullException(nameof(response));
        }

        public async Task AssertAsync(
            IEnumerable<OrderItemEntity> expectedOrderItems,
            IEnumerable<ServiceRecipientEntity> expectedServiceRecipients,
            string catalogueItemType)
        {
            var responseContext = await response.ReadBodyAsJsonAsync();

            var orderItems = responseContext.Select(ReadOrderItem);

            var expectedItems = expectedOrderItems
                .Where(i =>
                    catalogueItemType is null ||
                    catalogueItemType.Equals(i.CatalogueItemType.ToString(), StringComparison.OrdinalIgnoreCase))
                .OrderBy(expectedItem => expectedItem.Created)
                .Select(expectedItem =>
                {
                    var serviceRecipient = expectedServiceRecipients.FirstOrDefault(item => string.Equals(
                        expectedItem.OdsCode,
                        item.OdsCode,
                        StringComparison.OrdinalIgnoreCase));

                    return ConvertToExpectedBody(expectedItem, serviceRecipient);
                });

            orderItems.Should().BeEquivalentTo(expectedItems, options => options.WithStrictOrdering());
        }

        public async Task AssertServiceInstanceIdAsync(IEnumerable<object> expected)
        {
            var responseContent = await response.ReadBodyAsJsonAsync();
            var orderItems = responseContent.Select(ReadServiceInstanceItem);

            orderItems.Should().BeEquivalentTo(expected);
        }

        private static object ReadServiceInstanceItem(JToken responseBody)
        {
            return new
            {
                OrderItemId = responseBody.Value<int>("orderItemId"),
                ServiceInstanceId = responseBody.Value<string>("serviceInstanceId"),
            };
        }
    }
}
