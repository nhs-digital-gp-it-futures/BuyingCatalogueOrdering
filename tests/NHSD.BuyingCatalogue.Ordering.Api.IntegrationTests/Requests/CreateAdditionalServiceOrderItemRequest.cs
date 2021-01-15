using System;
using System.Collections.Generic;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests.Payloads;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Data;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests
{
    internal sealed class CreateAdditionalServiceOrderItemRequest : CreateOrderItemBaseRequest
    {
        public CreateAdditionalServiceOrderItemRequest(
            Request request,
            string orderingApiBaseAddress,
            string orderId)
            : base(request, orderingApiBaseAddress, orderId)
        {
        }

        protected override IDictionary<string, Func<CreateOrderItemRequestPayload>> PayloadFactory => new Dictionary<string, Func<CreateOrderItemRequestPayload>>
        {
            { "complete", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().Build() },
            { "high-boundary", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithPrice(999999999999999.999m).WithQuantity(int.MaxValue - 1).Build() },
            { "low-boundary", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithPrice(0).WithQuantity(1).Build() },
            { "missing-catalogue-item-type", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithCatalogueItemType(null).Build() },
            { "invalid-value-catalogue-item-type", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithCatalogueItemType(CatalogueItemType.Invalid).Build() },
            { "missing-service-recipient", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithHasServiceRecipient(false).Build() },
            { "missing-ods-code", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithOdsCode(null).Build() },
            { "missing-catalogue-item-id", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithCatalogueItemId(null).Build() },
            { "missing-catalogue-item-name", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithCatalogueItemName(null).Build() },
            { "missing-catalogue-solution-id", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithCatalogueSolutionId(null).Build() },
            { "missing-item-unit", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithHasItemUnit(false).Build() },
            { "missing-item-unit-name", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithItemUnitName(null).Build() },
            { "missing-item-unit-description", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithItemUnitNameDescription(null).Build() },
            { "missing-time-unit", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithProvisioningType(ProvisioningType.Patient).WithHasTimeUnit(false).Build() },
            { "missing-time-unit-name", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithProvisioningType(ProvisioningType.Patient).WithTimeUnitName(null).Build() },
            { "missing-time-unit-description", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithProvisioningType(ProvisioningType.Patient).WithTimeUnitDescription(null).Build() },
            { "missing-provisioning-type", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithProvisioningType(null).Build() },
            { "missing-type", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithCataloguePriceType(null).Build() },
            { "missing-currency-code", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithCurrencyCode(null).Build() },
            { "missing-quantity", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithQuantity(null).Build() },
            { "missing-estimation-period", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithEstimationPeriod(null).Build() },
            { "missing-price", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithPrice(null).Build() },
            { "invalid-value-currency-code", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithCurrencyCode("INV").Build() },
            { "invalid-value-provisioning-type", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithProvisioningType(ProvisioningType.Invalid).Build() },
            { "invalid-value-type", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithCataloguePriceType(CataloguePriceType.Invalid).Build() },
            { "invalid-value-estimation-period", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithEstimationPeriod(TimeUnit.Invalid).Build() },
            { "too-long-ods-code", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithOdsCode(new string('1', 9)).Build() },
            { "too-long-catalogue-item-id", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithCatalogueItemId(new string('1', 15)).Build() },
            { "too-long-catalogue-solution-id", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithCatalogueSolutionId(new string('1', 15)).Build() },
            { "too-long-catalogue-item-name", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithCatalogueItemName(new string('1', 256)).Build() },
            { "too-long-item-unit-name", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithItemUnitName(new string('a', 21)).Build() },
            { "too-long-item-unit-description", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithItemUnitNameDescription(new string('a', 41)).Build() },
            { "less-than-min-quantity", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithQuantity(0).Build() },
            { "greater-than-max-quantity", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithQuantity(int.MaxValue).Build() },
            { "less-than-min-price", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithPrice(-1).Build() },
            { "greater-than-max-price", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithPrice(1000000000000000m).Build() },
            { "on-demand-per-month", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithProvisioningType(ProvisioningType.OnDemand).WithEstimationPeriod(TimeUnit.Month).Build() },
            { "on-demand-per-year", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithProvisioningType(ProvisioningType.OnDemand).WithEstimationPeriod(TimeUnit.Year).Build() },
            { "patient", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithProvisioningType(ProvisioningType.Patient).WithEstimationPeriod(null).Build() },
            { "declarative", () => CreateOrderItemRequestPayloadBuilder.CreateAdditionalService().WithProvisioningType(ProvisioningType.Declarative).WithEstimationPeriod(null).Build() },
        };
    }
}
