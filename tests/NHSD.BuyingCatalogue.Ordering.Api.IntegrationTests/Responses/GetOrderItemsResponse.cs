using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses
{
    internal sealed class GetOrderItemsResponse : GetOrderItemResponseBase
    {
        private readonly Response _response;

        public GetOrderItemsResponse(Response response)
        {
            _response = response ?? throw new ArgumentNullException(nameof(response));
        }

        public async Task AssertAsync(
            IEnumerable<OrderItemEntity> expectedOrderItems, 
            IEnumerable<ServiceRecipientEntity> expectedServiceRecipients, 
            string catalogueItemType)
        {
            var responseContext = await _response.ReadBodyAsJsonAsync();

            var orderItems = responseContext.Select(ReadOrderItem);

            var expectedItems = expectedOrderItems
                .Where(x => 
                    catalogueItemType is null ||
                    catalogueItemType.Equals(x.CatalogueItemType.ToString(), StringComparison.OrdinalIgnoreCase))
                .OrderBy(expectedItem => expectedItem.Created)
                .Select(expectedItem =>
                {
                    var serviceRecipient = expectedServiceRecipients.FirstOrDefault(item =>
                        string.Equals(expectedItem.OdsCode,
                            item.OdsCode, StringComparison.OrdinalIgnoreCase));

                    return ConvertToExpectedBody(expectedItem, serviceRecipient);
                });

            orderItems.Should().BeEquivalentTo(expectedItems, options => options.WithStrictOrdering());
        }
    }
}
