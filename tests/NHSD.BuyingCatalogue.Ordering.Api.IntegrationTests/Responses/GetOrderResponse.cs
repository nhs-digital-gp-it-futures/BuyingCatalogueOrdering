using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Data;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.EntityBuilder;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses
{
    internal sealed class GetOrderResponse
    {
        private readonly Response _response;

        public GetOrderResponse(Response response)
        {
            _response = response ?? throw new ArgumentNullException(nameof(response));
        }

        public async Task AssertOrderItemCost(string orderItemId, decimal orderItemCost)
        {
            var responseContent = await _response.ReadBodyAsJsonAsync();
            var orderItems = responseContent.SelectToken("orderItems");
            var item = orderItems.FirstOrDefault(x => x.SelectToken("catalogueItemName") != null);
            item.Should().NotBeNull();
            item.Value<decimal>("costPerYear").Should().Be(orderItemCost);
        }

        public async Task AssertAsync(OrderEntity expectedOrder, IEnumerable<OrderItemEntity> expectedOrderItems, IEnumerable<ServiceRecipientEntity> expectedServiceRecipients)
        {
            var responseContent = await _response.ReadBodyAsJsonAsync();

            var order = OrderEntityBuilder
                .Create()
                .WithOrderId(expectedOrder.OrderId)
                .WithDescription(responseContent.Value<string>("description"))
                .WithOrganisationName(responseContent.SelectToken("orderParty").Value<string>("name"))
                .WithOrganisationOdsCode(responseContent.SelectToken("orderParty").Value<string>("odsCode"))
                .WithCommencementDate(responseContent.Value<DateTime?>("commencementDate"))
                .WithSupplierName(responseContent.SelectToken("supplier").Value<string>("name"))
                .Build();

            var orderItems = responseContent.SelectToken("orderItems").Select(orderItem => new
            {
                ItemId = orderItem.Value<string>("itemId"),
                ServiceRecipientsOdsCode = orderItem.Value<string>("serviceRecipientsOdsCode"),
                CataloguePriceType = orderItem.Value<string>("cataloguePriceType"),
                CatalogueItemType = orderItem.Value<string>("catalogueItemType"),
                CatalogueItemName = orderItem.Value<string>("catalogueItemName"),
                ProvisioningType = orderItem.Value<string>("provisioningType"),
                ItemUnitDescription = orderItem.Value<string>("itemUnitDescription"),
                TimeUnitDescription = orderItem.Value<string>("timeUnitDescription"),
                QuantityPeriodDescription = orderItem.Value<string>("quantityPeriodDescription"),
                Price = orderItem.Value<decimal?>("price"),
                Quantity = orderItem.Value<int>("quantity"),
                DeliveryDate = orderItem.Value<DateTime?>("deliveryDate")
            });

            var serviceRecipients = responseContent.SelectToken("serviceRecipients")
                .Select(serviceRecipient => ServiceRecipientBuilder.Create()
                    .WithOrderId(expectedOrder.OrderId)
                    .WithOdsCode(serviceRecipient.Value<string>("odsCode"))
                    .WithName(serviceRecipient.Value<string>("name"))
                    .Build()).ToList();

            order.Should().BeEquivalentTo(expectedOrder, x=> 
                x.Excluding(orderEntity => orderEntity.OrganisationId)
                .Excluding(orderEntity => orderEntity.OrderStatusId)
                .Excluding(orderEntity => orderEntity.Created)
                .Excluding(orderEntity => orderEntity.LastUpdatedBy)
                .Excluding(orderEntity => orderEntity.LastUpdatedByName)
                .Excluding(orderEntity => orderEntity.LastUpdated)
                .Excluding(orderEntity => orderEntity.OrganisationAddressId)
                .Excluding(orderEntity => orderEntity.OrganisationContactId)
                .Excluding(orderEntity => orderEntity.SupplierAddressId)
                .Excluding(orderEntity => orderEntity.SupplierContactId));

            var convertedExpectedOrderItems = expectedOrderItems.Select(orderItem => new
            {
                ItemId = $"{expectedOrder.OrderId}-{orderItem.OdsCode}-{orderItem.OrderItemId}",
                ServiceRecipientsOdsCode = orderItem.OdsCode,
                CataloguePriceType = orderItem.CataloguePriceType.ToString(),
                CatalogueItemType = orderItem.CatalogueItemType.ToString(),
                orderItem.CatalogueItemName,
                ProvisioningType = orderItem.ProvisioningType.ToString(),
                ItemUnitDescription = orderItem.PricingUnitDescription,
                TimeUnitDescription = orderItem.TimeUnit?.ToDescription(),
                QuantityPeriodDescription = orderItem.EstimationPeriod?.ToDescription(),
                orderItem.Price,
                orderItem.Quantity,
                orderItem.DeliveryDate
            });

            orderItems.Should().BeEquivalentTo(convertedExpectedOrderItems);

            serviceRecipients.Should().BeEquivalentTo(expectedServiceRecipients);
        }
    }
}
