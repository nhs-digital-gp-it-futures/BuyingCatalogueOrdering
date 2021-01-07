using System;
using System.Collections.Generic;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests.Payloads;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Data;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests
{
    internal sealed class CreateAssociatedServiceOrderItemRequest : CreateOrderItemBaseRequest
    {
        protected override IDictionary<string, Func<CreateOrderItemRequestPayload>> PayloadFactory => new Dictionary<string, Func<CreateOrderItemRequestPayload>>
        {
            { "complete", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().Build() },
            { "high-boundary", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithPrice(999999999999999.999m).WithQuantity(int.MaxValue - 1).Build() },
            { "low-boundary", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithPrice(0).WithQuantity(1).Build() },
            { "missing-catalogue-item-type", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithCatalogueItemType(null).Build() },
            { "invalid-value-catalogue-item-type", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithCatalogueItemType(CatalogueItemType.Invalid).Build() },
            { "missing-service-recipient", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithHasServiceRecipient(false).Build() },
            { "missing-ods-code", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithOdsCode(null).Build() },
            { "missing-catalogue-item-id", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithCatalogueItemId(null).Build() },
            { "missing-catalogue-item-name", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithCatalogueItemName(null).Build() },
            { "missing-catalogue-solution-id", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithCatalogueSolutionId(null).Build() },
            { "missing-item-unit", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithHasItemUnit(false).Build() },
            { "missing-item-unit-name", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithItemUnitName(null).Build() },
            { "missing-item-unit-description", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithItemUnitNameDescription(null).Build() },
            { "missing-time-unit", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithProvisioningType(ProvisioningType.Patient).WithHasTimeUnit(false).Build() },
            { "missing-time-unit-name", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithProvisioningType(ProvisioningType.Patient).WithTimeUnitName(null).Build() },
            { "missing-time-unit-description", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithProvisioningType(ProvisioningType.Patient).WithTimeUnitDescription(null).Build() },
            { "missing-provisioning-type", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithProvisioningType(null).Build() },
            { "missing-type", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithCataloguePriceType(null).Build() },
            { "missing-currency-code", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithCurrencyCode(null).Build() },
            { "missing-quantity", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithQuantity(null).Build() },
            { "missing-estimation-period", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithEstimationPeriod(null).Build() },
            { "missing-price", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithPrice(null).Build() },
            { "invalid-value-currency-code", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithCurrencyCode("INV").Build() },
            { "invalid-value-provisioning-type", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithProvisioningType(ProvisioningType.Invalid).Build() },
            { "invalid-value-type", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithCataloguePriceType(CataloguePriceType.Invalid).Build() },
            { "invalid-value-estimation-period", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithEstimationPeriod(TimeUnit.Invalid).Build() },
            { "too-long-ods-code", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithOdsCode(new string('1', 9)).Build() },
            { "too-long-catalogue-item-id", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithCatalogueItemId(new string('1', 15)).Build() },
            { "too-long-catalogue-solution-id", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithCatalogueSolutionId(new string('1', 15)).Build() },
            { "too-long-catalogue-item-name", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithCatalogueItemName(new string('1', 256)).Build() },
            { "too-long-item-unit-name", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithItemUnitName(new string('a', 21)).Build() },
            { "too-long-item-unit-description", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithItemUnitNameDescription(new string('a', 41)).Build() },
            { "less-than-min-quantity", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithQuantity(0).Build() },
            { "greater-than-max-quantity", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithQuantity(int.MaxValue).Build() },
            { "less-than-min-price", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithPrice(-1).Build() },
            { "greater-than-max-price", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithPrice(1000000000000000m).Build() },
            { "on-demand-per-month", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithProvisioningType(ProvisioningType.OnDemand).WithEstimationPeriod(TimeUnit.Month).Build() },
            { "on-demand-per-year", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithProvisioningType(ProvisioningType.OnDemand).WithEstimationPeriod(TimeUnit.Year).Build() },
            { "patient", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithProvisioningType(ProvisioningType.Patient).WithEstimationPeriod(null).Build() },
            { "declarative", () => CreateOrderItemRequestPayloadBuilder.CreateAssociatedService().WithProvisioningType(ProvisioningType.Declarative).WithEstimationPeriod(null).Build() },
        };

        public CreateAssociatedServiceOrderItemRequest(
            Request request,
            string orderingApiBaseAddress,
            string orderId)
            : base(request, orderingApiBaseAddress, orderId)
        {
        }
    }
}
