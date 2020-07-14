using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses
{
    internal sealed class GetOrderItemsResponse
    {
        private readonly Response _response;

        public GetOrderItemsResponse(Response response)
        {
            _response = response ?? throw new ArgumentNullException(nameof(response));
        }

        public async Task AssertAsync(IEnumerable<OrderItemEntity> expectedOrderItems, IEnumerable<ServiceRecipientEntity> expectedServiceRecipients, string catalogueItemType)
        {
            var responseContext = await _response.ReadBodyAsJsonAsync();

            const string serviceRecipientToken = "serviceRecipient";

            var orderItems = responseContext.Select(orderItem => new
            {
                ItemId = orderItem.Value<string>("itemId"),
                ServiceRecipientName = orderItem.SelectToken(serviceRecipientToken).Value<string>("name"),
                ServiceRecipientOdsCode = orderItem.SelectToken(serviceRecipientToken).Value<string>("odsCode"),
                CataloguePriceType = orderItem.Value<string>("cataloguePriceType"),
                CatalogueItemType = orderItem.Value<string>("catalogueItemType"),
                CatalogueItemName = orderItem.Value<string>("catalogueItemName"),
                CatalogueItemId = orderItem.Value<string>("catalogueItemId")
            });

            var expectedItems = expectedOrderItems
                .OrderBy(expectedItem => expectedItem.Created)
                .Select(expectedItem => new
            {
                ItemId = $"{expectedItem.OrderId}-{expectedItem.OdsCode}-{expectedItem.OrderItemId}",
                ServiceRecipientName = expectedServiceRecipients.FirstOrDefault(serviceRecipient =>
                    string.Equals(expectedItem.OdsCode,
                        serviceRecipient.OdsCode, StringComparison.OrdinalIgnoreCase))?.Name,
                ServiceRecipientOdsCode = expectedItem.OdsCode,
                CataloguePriceType = expectedItem.CataloguePriceType.ToString(),
                CatalogueItemType = expectedItem.CatalogueItemType.ToString(),
                expectedItem.CatalogueItemName,
                expectedItem.CatalogueItemId
            });

            if (catalogueItemType != null)
            {
                expectedItems = expectedItems.Where(x => x.CatalogueItemType.Equals(
                    catalogueItemType, StringComparison.OrdinalIgnoreCase));
            }

            orderItems.Should().BeEquivalentTo(expectedItems, options => options.WithStrictOrdering());
        }
    }
}
