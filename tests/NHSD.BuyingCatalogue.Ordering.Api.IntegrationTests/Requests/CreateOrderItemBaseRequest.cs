using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests.Payloads;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Data;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.EntityBuilder;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests
{
    internal abstract class CreateOrderItemBaseRequest
    {
        private readonly Request _request;
        private readonly string _createOrderItemUrl;

        private static readonly Dictionary<string, Func<Request, string, string, CreateOrderItemBaseRequest>> CreateFunctions 
            = new Dictionary<string, Func<Request, string, string, CreateOrderItemBaseRequest>>
        {
            { "catalogue solution", (request, url, orderId) => new CreateCatalogueSolutionOrderItemRequest(
                request,
                url,
                orderId)
            },
            { "additional service", (request, url, orderId) => new CreateAdditionalServiceOrderItemRequest(
                request,
                url,
                orderId)
            }
        };

        protected abstract IDictionary<string, Func<CreateOrderItemRequestPayload>> PayloadFactory { get; }
        public string OrderId { get; set; }
        public CreateOrderItemRequestPayload Payload { get; private set; }

        protected CreateOrderItemBaseRequest(
            Request request,
            string orderingApiBaseAddress,
            string orderId)
        {
            _request = request ?? throw new ArgumentNullException(nameof(request));
            OrderId = orderId ?? throw new ArgumentNullException(nameof(orderId));

            _createOrderItemUrl =
                $"{orderingApiBaseAddress}/api/v1/orders/{orderId}/order-items";
        }

        public void SetPayload(string key)
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));

            if (!PayloadFactory.TryGetValue(key, out var factory))
                Assert.Fail("Unexpected create order item payload type.");

            Payload = factory();
        }

        public void AssertPayload(OrderItemEntity actual)
        {
            var expectedBuilder = OrderItemEntityBuilder
                .Create()
                .WithOrderId(OrderId)
                .WithOdsCode(Payload.OdsCode)
                .WithCatalogueItemId(Payload.CatalogueItemId)
                .WithCatalogueItemType(Payload.CatalogueItemType.GetValueOrDefault())
                .WithCatalogueItemName(Payload.CatalogueItemName)
                .WithQuantity(Payload.Quantity.GetValueOrDefault())
                .WithDeliveryDate(Payload.DeliveryDate)
                .WithEstimationPeriod(Payload.EstimationPeriod)
                .WithProvisioningType(Payload.ProvisioningType.GetValueOrDefault())
                .WithCataloguePriceType(Payload.CataloguePriceType.GetValueOrDefault())
                .WithCurrencyCode(Payload.CurrencyCode)
                .WithPricingUnitName(Payload.ItemUnitName)
                .WithPricingUnitDescription(Payload.ItemUnitNameDescription)
                .WithPrice(Payload.Price)
                .WithTimeUnit((TimeUnit?)Enum.Parse(typeof(TimeUnit), Payload.TimeUnitName, true));
            
            var expected = expectedBuilder.Build();
            actual.Should().BeEquivalentTo(expected,
                config =>
                    config
                        .Excluding(entity => entity.OrderItemId)
                        .Excluding(entity => entity.PricingUnitTierName)
                        .Excluding(entity => entity.Created)
                        .Excluding(entity => entity.LastUpdated));
        }

        public async Task<CreateOrderItemResponse> ExecuteAsync()
        {
            object payload = null;

            if (Payload != null)
            {
                payload = new
                {
                    ServiceRecipient = Payload.HasServiceRecipient ? new { Payload.OdsCode } : null,
                    Payload.CatalogueItemId,
                    Payload.CatalogueItemName,
                    Payload.CatalogueSolutionId,
                    CatalogueItemType = Payload.CatalogueItemType?.ToString(),
                    Payload.DeliveryDate,
                    Payload.Quantity,
                    EstimationPeriod = Payload.EstimationPeriod?.ToString(),
                    ProvisioningType = Payload.ProvisioningType?.ToString(),
                    Type = Payload.CataloguePriceType?.ToString(),
                    Payload.CurrencyCode,
                    ItemUnit = Payload.HasItemUnit ?
                        new
                        {
                            Name = Payload.ItemUnitName,
                            Description = Payload.ItemUnitNameDescription
                        } : null,
                    Payload.Price,
                    TimeUnit = Payload.HasTimeUnit ?
                        new
                        {
                            Name = Payload.TimeUnitName,
                            Description = Payload.TimeUnitDescription
                        } : null
                };
            }

            var response = await _request.PostJsonAsync(_createOrderItemUrl, payload);

            return new CreateOrderItemResponse(response);
        }

        public static CreateOrderItemBaseRequest Create(string orderItemType, Request request, string baseUrl, string orderId)
        {
            if (!CreateFunctions.TryGetValue(orderItemType, out var factory))
                Assert.Fail("Unexpected create order item request type.");

            return factory(request, baseUrl, orderId);
        }
    }
}
