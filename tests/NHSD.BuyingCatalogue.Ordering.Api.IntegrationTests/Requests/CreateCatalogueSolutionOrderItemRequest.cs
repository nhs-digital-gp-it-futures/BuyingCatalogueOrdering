using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests.Payloads;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Data;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.EntityBuilder;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests
{
    internal sealed class CreateCatalogueSolutionOrderItemRequest
    {
        private readonly Request _request;
        private readonly string _createCatalogueSolutionOrderItemUrl;

        private static readonly IDictionary<string, Func<CreateCatalogueSolutionOrderItemRequestPayload>> PayloadFactory =
            new Dictionary<string, Func<CreateCatalogueSolutionOrderItemRequestPayload>>()
            {
                {
                    "complete", () => CreateCatalogueSolutionOrderItemRequestPayloadBuilder.Create().Build()
                }
            };

        public CreateCatalogueSolutionOrderItemRequest(
            Request request,
            string orderingApiBaseAddress,
            string orderId)
        {
            _request = request ?? throw new ArgumentNullException(nameof(request));
            OrderId = orderId ?? throw new ArgumentNullException(nameof(orderId));

            _createCatalogueSolutionOrderItemUrl =
                $"{orderingApiBaseAddress}/api/v1/orders/{orderId}/sections/catalogue-solutions";
        }

        public string OrderId { get; set; }

        public CreateCatalogueSolutionOrderItemRequestPayload Payload { get; private set; }

        public async Task<CreateCatalogueSolutionOrderItemResponse> ExecuteAsync()
        {
            object payload = null;
            if (Payload != null)
            {
                payload = new
                {
                    ServiceRecipient = new {Payload.OdsCode},
                    Payload.CatalogueSolutionId,
                    Payload.CatalogueSolutionName,
                    Payload.DeliveryDate,
                    Payload.Quantity,
                    EstimationPeriod = Payload.EstimationPeriod?.ToString(),
                    ProvisioningType = Payload.ProvisioningType?.ToString(),
                    Type = Payload.CataloguePriceType?.ToString(),
                    Payload.CurrencyCode,
                    ItemUnitModel =
                        new
                        {
                            Name = Payload.ItemUnitName, 
                            Description = Payload.ItemUnitNameDescription
                        },
                    Payload.Price
                };
            }

            var response = await _request.PostJsonAsync(_createCatalogueSolutionOrderItemUrl, payload);
            
            return new CreateCatalogueSolutionOrderItemResponse(response);
        }

        public void SetPayload(string key)
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));

            if (!PayloadFactory.TryGetValue(key, out var factory))
                Assert.Fail("Unexpected create catalogue solution order item payload type.");

            Payload = factory();
        }

        public void AssertPayload(OrderItemEntity actual)
        {
            var expected = OrderItemEntityBuilder
                .Create()
                .WithOrderId(OrderId)
                .WithOdsCode(Payload.OdsCode)
                .WithCatalogueItemId(Payload.CatalogueSolutionId)
                .WithCatalogueItemType(CatalogueItemType.Solution)
                .WithCatalogueItemName(Payload.CatalogueSolutionName)
                .WithDeliveryDate(Payload.DeliveryDate)
                .WithQuantity(Payload.Quantity.GetValueOrDefault())
                .WithEstimationPeriod(Payload.EstimationPeriod)
                .WithProvisioningType(Payload.ProvisioningType.GetValueOrDefault())
                .WithCataloguePriceType(Payload.CataloguePriceType.GetValueOrDefault())
                .WithCurrencyCode(Payload.CurrencyCode)
                .WithPricingUnitTierName(Payload.ItemUnitName)
                .WithPricingUnitDescription(Payload.ItemUnitNameDescription)
                .WithPrice(Payload.Price)
                .Build();

            actual.Should().BeEquivalentTo(expected, 
                config => 
                    config
                        .Excluding(entity => entity.OrderItemId)
                        .Excluding(entity => entity.Created)
                        .Excluding(entity => entity.LastUpdated));
        }
    }
}
