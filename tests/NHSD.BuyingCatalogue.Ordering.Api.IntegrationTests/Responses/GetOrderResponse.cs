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

            var orderItems = responseContent.SelectToken("orderItems")
                .Select(orderItem => OrderItemEntityBuilder.Create()
                    .WithOrderId(expectedOrder.OrderId)
                    .WithOdsCode(orderItem.Value<string>("serviceRecipientsOdsCode"))
                    .WithCataloguePriceType(Enum.Parse<CataloguePriceType>(orderItem.Value<string>("cataloguePriceType")))
                    .WithCatalogueItemType(Enum.Parse<CatalogueItemType>(orderItem.Value<string>("catalogueItemType")))
                    .WithCatalogueItemName(orderItem.Value<string>("catalogueItemName"))
                    .WithProvisioningType(Enum.Parse<ProvisioningType>(orderItem.Value<string>("provisioningType")))
                    .WithPricingUnitDescription(orderItem.Value<string>("itemUnitDescription"))
                    .WithPrice(orderItem.Value<decimal?>("price"))
                    .WithQuantity(orderItem.Value<int>("quantity"))
                    .Build())
                .ToList();

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

            orderItems.Should().BeEquivalentTo(expectedOrderItems, x =>
                x.Excluding(orderItemEntity => orderItemEntity.CatalogueItemId)
                .Excluding(orderItemEntity => orderItemEntity.OrderItemId)
                .Excluding(orderItemEntity => orderItemEntity.DeliveryDate)
                .Excluding(orderItemEntity => orderItemEntity.EstimationPeriod)
                .Excluding(orderItemEntity => orderItemEntity.PricingUnitName)
                .Excluding(orderItemEntity => orderItemEntity.PricingUnitTierName)
                .Excluding(orderItemEntity => orderItemEntity.Created)
                .Excluding(orderItemEntity => orderItemEntity.LastUpdated));

            serviceRecipients.Should().BeEquivalentTo(expectedServiceRecipients);
        }
    }
}
