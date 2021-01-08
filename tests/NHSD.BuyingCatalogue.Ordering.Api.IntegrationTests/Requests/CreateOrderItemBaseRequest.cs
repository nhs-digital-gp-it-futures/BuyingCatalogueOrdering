using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Equivalency;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests.Payloads;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Data;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.EntityBuilder;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests
{
    internal abstract class CreateOrderItemBaseRequest
    {
        private static readonly Dictionary<string, Func<Request, string, string, CreateOrderItemBaseRequest>> CreateFunctions
            = new()
            {
                {
                    "catalogue solution", (request, url, orderId) => new CreateCatalogueSolutionOrderItemRequest(
                        request,
                        url,
                        orderId)
                },
                {
                    "additional service", (request, url, orderId) => new CreateAdditionalServiceOrderItemRequest(
                        request,
                        url,
                        orderId)
                },
                {
                    "associated service", (request, url, orderId) => new CreateAssociatedServiceOrderItemRequest(
                        request,
                        url,
                        orderId)
                },
            };

        private readonly Request request;
        private readonly string createOrderItemUrl;

        protected CreateOrderItemBaseRequest(
            Request request,
            string orderingApiBaseAddress,
            string orderId)
        {
            this.request = request ?? throw new ArgumentNullException(nameof(request));
            OrderId = orderId ?? throw new ArgumentNullException(nameof(orderId));

            createOrderItemUrl = $"{orderingApiBaseAddress}/api/v1/orders/{orderId}/order-items";
        }

        public string OrderId { get; set; }

        public OrderItemEntity OriginalEntity { get; set; }

        public CreateOrderItemRequestPayload Payload { get; private set; }

        protected abstract IDictionary<string, Func<CreateOrderItemRequestPayload>> PayloadFactory { get; }

        public static CreateOrderItemBaseRequest Create(string orderItemType, Request request, string baseUrl, string orderId)
        {
            if (!CreateFunctions.TryGetValue(orderItemType, out var factory))
                Assert.Fail("Unexpected create order item request type.");

            return factory(request, baseUrl, orderId);
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
            if (Payload.HasOrderItemId)
                AssertUpdatePayload(actual);
            else
                AssertCreatePayload(actual);
        }

        public object CreatePayload()
        {
            if (Payload is null)
                return null;

            return new
            {
                Payload.OrderItemId,
                ServiceRecipient = CreateServiceRecipient(),
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
                ItemUnit = Payload.HasItemUnit
                    ? new
                    {
                        Name = Payload.ItemUnitName,
                        Description = Payload.ItemUnitNameDescription,
                    }
                    : null,
                Payload.Price,
                TimeUnit = Payload.HasTimeUnit ?
                    new
                    {
                        Name = Payload.TimeUnitName,
                        Description = Payload.TimeUnitDescription,
                    }
                    : null,
            };
        }

        public async Task<CreateOrderItemResponse> ExecuteAsync()
        {
            object payload = CreatePayload();

            var response = await request.PostJsonAsync(createOrderItemUrl, payload);

            return new CreateOrderItemResponse(response);
        }

        private void AssertCreatePayload(OrderItemEntity actual)
        {
            var timeUnit = Payload.HasTimeUnit
                ? (TimeUnit?)Enum.Parse(typeof(TimeUnit), Payload.TimeUnitName, true)
                : null;

            var expectedBuilder = OrderItemEntityBuilder
                .Create()
                .WithOrderId(OrderId)
                .WithOrderItemId(Payload.OrderItemId.GetValueOrDefault())
                .WithOdsCode(Payload.OdsCode)
                .WithCatalogueItemId(Payload.CatalogueItemId)
                .WithParentCatalogueItemId(Payload.CatalogueSolutionId)
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
                .WithTimeUnit(timeUnit);

            var expected = expectedBuilder.Build();
            static EquivalencyAssertionOptions<OrderItemEntity> AssertionOptions(EquivalencyAssertionOptions<OrderItemEntity> options) => options
                .Excluding(entity => entity.OrderItemId)
                .Excluding(entity => entity.PricingUnitTierName)
                .Excluding(entity => entity.Created)
                .Excluding(entity => entity.LastUpdated);

            actual.Should().BeEquivalentTo(expected, AssertionOptions);
        }

        private void AssertUpdatePayload(OrderItemEntity actual)
        {
            var expectedEstimationPeriod = OriginalEntity.ProvisioningType != ProvisioningType.OnDemand
                ? OriginalEntity.EstimationPeriod
                : Payload.EstimationPeriod;

            var expected = OrderItemEntityBuilder
                .Create(OriginalEntity)
                .WithOrderId(OrderId)
                .WithOrderItemId(Payload.OrderItemId.GetValueOrDefault())
                .WithDeliveryDate(Payload.DeliveryDate)
                .WithEstimationPeriod(expectedEstimationPeriod)
                .WithPrice(Payload.Price)
                .WithQuantity(Payload.Quantity.GetValueOrDefault())
                .Build();

            actual.Should().BeEquivalentTo(expected, o => o.Excluding(e => e.LastUpdated));
            actual.LastUpdated.Should().BeAfter(OriginalEntity.LastUpdated);
        }

        private object CreateServiceRecipient()
        {
            if (!string.IsNullOrWhiteSpace(Payload.ServiceRecipientName))
            {
                return new
                {
                    Payload.OdsCode,
                    Name = Payload.ServiceRecipientName,
                };
            }

            return Payload.HasServiceRecipient ? new { Payload.OdsCode } : null;
        }
    }
}
