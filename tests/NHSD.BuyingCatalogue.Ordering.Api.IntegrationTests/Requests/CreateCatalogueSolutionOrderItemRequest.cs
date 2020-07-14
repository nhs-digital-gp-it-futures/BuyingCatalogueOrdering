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
        private const int MaximumDeliveryDateOffsetDays = 1282;

        private readonly Request _request;
        private readonly string _createCatalogueSolutionOrderItemUrl;

        private static readonly IDictionary<string, Func<CreateCatalogueSolutionOrderItemRequestPayload>> PayloadFactory =
            new Dictionary<string, Func<CreateCatalogueSolutionOrderItemRequestPayload>>()
            {
                {"complete", () => CreateCatalogueSolutionOrderItemRequestPayloadBuilder.Create().Build()},
                {"missing-service-recipient", () => CreateCatalogueSolutionOrderItemRequestPayloadBuilder.Create().WithHasServiceRecipient(false).Build()},
                {"missing-ods-code", () => CreateCatalogueSolutionOrderItemRequestPayloadBuilder.Create().WithOdsCode(null).Build()},
                {"missing-catalogue-solution-id", () => CreateCatalogueSolutionOrderItemRequestPayloadBuilder.Create().WithCatalogueSolutionId(null).Build()},
                {"missing-catalogue-solution-name", () => CreateCatalogueSolutionOrderItemRequestPayloadBuilder.Create().WithCatalogueSolutionName(null).Build()},
                {"missing-item-unit", () => CreateCatalogueSolutionOrderItemRequestPayloadBuilder.Create().WithHasItemUnit(false).Build()},
                {"missing-item-unit-name", () => CreateCatalogueSolutionOrderItemRequestPayloadBuilder.Create().WithItemUnitName(null).Build()},
                {"missing-item-unit-description", () => CreateCatalogueSolutionOrderItemRequestPayloadBuilder.Create().WithItemUnitNameDescription(null).Build()},
                {"missing-time-unit", () => CreateCatalogueSolutionOrderItemRequestPayloadBuilder.Create().WithProvisioningType(ProvisioningType.Patient).WithHasTimeUnit(false).Build()},
                {"missing-time-unit-name", () => CreateCatalogueSolutionOrderItemRequestPayloadBuilder.Create().WithProvisioningType(ProvisioningType.Patient).WithTimeUnitName(null).Build()},
                {"missing-time-unit-description", () => CreateCatalogueSolutionOrderItemRequestPayloadBuilder.Create().WithProvisioningType(ProvisioningType.Patient).WithTimeUnitDescription(null).Build()},
                {"missing-provisioning-type", () => CreateCatalogueSolutionOrderItemRequestPayloadBuilder.Create().WithProvisioningType(null).Build()},
                {"missing-type", () => CreateCatalogueSolutionOrderItemRequestPayloadBuilder.Create().WithCataloguePriceType(null).Build()},
                {"missing-currency-code", () => CreateCatalogueSolutionOrderItemRequestPayloadBuilder.Create().WithCurrencyCode(null).Build()},
                {"missing-delivery-date", () => CreateCatalogueSolutionOrderItemRequestPayloadBuilder.Create().WithDeliveryDate(null).Build()},
                {"missing-quantity", () => CreateCatalogueSolutionOrderItemRequestPayloadBuilder.Create().WithQuantity(null).Build()},
                {"missing-estimation-period", () => CreateCatalogueSolutionOrderItemRequestPayloadBuilder.Create().WithEstimationPeriod(null).Build()},
                {"missing-price", () => CreateCatalogueSolutionOrderItemRequestPayloadBuilder.Create().WithPrice(null).Build()},
                {"invalid-value-currency-code", () => CreateCatalogueSolutionOrderItemRequestPayloadBuilder.Create().WithCurrencyCode("YOLO").Build()},
                {"invalid-value-provisioning-type", () => CreateCatalogueSolutionOrderItemRequestPayloadBuilder.Create().WithProvisioningType(ProvisioningType.Invalid).Build()},
                {"invalid-value-type", () => CreateCatalogueSolutionOrderItemRequestPayloadBuilder.Create().WithCataloguePriceType(CataloguePriceType.Invalid).Build()},
                {"invalid-value-estimation-period", () => CreateCatalogueSolutionOrderItemRequestPayloadBuilder.Create().WithEstimationPeriod(TimeUnit.Invalid).Build()},
                {"too-long-ods-code", () => CreateCatalogueSolutionOrderItemRequestPayloadBuilder.Create().WithOdsCode(new string('1', 9)).Build()},
                {"too-long-catalogue-solution-id", () => CreateCatalogueSolutionOrderItemRequestPayloadBuilder.Create().WithCatalogueSolutionId(new string('1', 15)).Build()},
                {"too-long-catalogue-solution-name", () => CreateCatalogueSolutionOrderItemRequestPayloadBuilder.Create().WithCatalogueSolutionName(new string('1', 256)).Build()},
                {"above-delivery-window", () => CreateCatalogueSolutionOrderItemRequestPayloadBuilder.Create().WithDeliveryDate(new DateTime(2021,1,1).AddDays(MaximumDeliveryDateOffsetDays)).Build()},
                {"below-delivery-window", () => CreateCatalogueSolutionOrderItemRequestPayloadBuilder.Create().WithDeliveryDate(new DateTime(2021,1,1).AddDays(-1)).Build()},
                {"less-than-min-quantity", () => CreateCatalogueSolutionOrderItemRequestPayloadBuilder.Create().WithQuantity(0).Build()},
                {"greater-than-max-quantity", () => CreateCatalogueSolutionOrderItemRequestPayloadBuilder.Create().WithQuantity(int.MaxValue).Build()},
                {"less-than-min-price", () => CreateCatalogueSolutionOrderItemRequestPayloadBuilder.Create().WithPrice(-1).Build()},
                {"greater-than-max-price", () => CreateCatalogueSolutionOrderItemRequestPayloadBuilder.Create().WithPrice(1000000000000000m).Build()} ,
                {
                    "on-demand-per-month", () => CreateCatalogueSolutionOrderItemRequestPayloadBuilder
                        .Create()
                        .WithProvisioningType(ProvisioningType.OnDemand)
                        .WithEstimationPeriod(TimeUnit.Month)
                        .Build()
                },
                {
                    "on-demand-per-year", () => CreateCatalogueSolutionOrderItemRequestPayloadBuilder
                        .Create()
                        .WithProvisioningType(ProvisioningType.OnDemand)
                        .WithEstimationPeriod(TimeUnit.Year)
                        .Build()
                },
                {
                    "patient", () => CreateCatalogueSolutionOrderItemRequestPayloadBuilder
                        .Create()
                        .WithProvisioningType(ProvisioningType.Patient)
                        .WithEstimationPeriod(null)
                        .Build()
                },
                {
                    "declarative", () => CreateCatalogueSolutionOrderItemRequestPayloadBuilder
                        .Create()
                        .WithProvisioningType(ProvisioningType.Declarative)
                        .WithEstimationPeriod(null)
                        .Build()
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
                    ServiceRecipient = Payload.HasServiceRecipient ? new { Payload.OdsCode } : null,
                    Payload.CatalogueSolutionId,
                    Payload.CatalogueSolutionName,
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
                .WithPricingUnitName(Payload.ItemUnitName)
                .WithPricingUnitDescription(Payload.ItemUnitNameDescription)
                .WithPrice(Payload.Price)
                .WithTimeUnit((TimeUnit?)Enum.Parse(typeof(TimeUnit), Payload.TimeUnitName, true))
                .Build();

            actual.Should().BeEquivalentTo(expected,
                config =>
                    config
                        .Excluding(entity => entity.OrderItemId)
                        .Excluding(entity => entity.PricingUnitTierName)
                        .Excluding(entity => entity.Created)
                        .Excluding(entity => entity.LastUpdated));
        }
    }
}
