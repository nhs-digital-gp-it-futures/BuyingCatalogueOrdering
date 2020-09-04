using System;
using System.Collections.Generic;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests.Payloads;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Data;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests
{
    internal sealed class CreateCatalogueSolutionOrderItemRequest : CreateOrderItemBaseRequest
    {
        private const int MaximumDeliveryDateOffsetDays = 1282;

        protected override IDictionary<string, Func<CreateOrderItemRequestPayload>> PayloadFactory =>
            new Dictionary<string, Func<CreateOrderItemRequestPayload>>()
            {
                {"complete", () => CreateOrderItemRequestPayloadBuilder.CreateSolution().Build()},
                {"missing-catalogue-item-type", () => CreateOrderItemRequestPayloadBuilder.CreateSolution().WithCatalogueItemType(null).Build()},
                {"invalid-value-catalogue-item-type", () => CreateOrderItemRequestPayloadBuilder.CreateSolution().WithCatalogueItemType(CatalogueItemType.Invalid).Build()},
                {"missing-service-recipient", () => CreateOrderItemRequestPayloadBuilder.CreateSolution().WithHasServiceRecipient(false).Build()},
                {"missing-ods-code", () => CreateOrderItemRequestPayloadBuilder.CreateSolution().WithOdsCode(null).Build()},
                {"missing-catalogue-item-id", () => CreateOrderItemRequestPayloadBuilder.CreateSolution().WithCatalogueItemId(null).Build()},
                {"missing-catalogue-item-name", () => CreateOrderItemRequestPayloadBuilder.CreateSolution().WithCatalogueItemName(null).Build()},
                {"missing-item-unit", () => CreateOrderItemRequestPayloadBuilder.CreateSolution().WithHasItemUnit(false).Build()},
                {"missing-item-unit-name", () => CreateOrderItemRequestPayloadBuilder.CreateSolution().WithItemUnitName(null).Build()},
                {"missing-item-unit-description", () => CreateOrderItemRequestPayloadBuilder.CreateSolution().WithItemUnitNameDescription(null).Build()},
                {"missing-time-unit", () => CreateOrderItemRequestPayloadBuilder.CreateSolution().WithProvisioningType(ProvisioningType.Patient).WithHasTimeUnit(false).Build()},
                {"missing-time-unit-name", () => CreateOrderItemRequestPayloadBuilder.CreateSolution().WithProvisioningType(ProvisioningType.Patient).WithTimeUnitName(null).Build()},
                {"missing-time-unit-description", () => CreateOrderItemRequestPayloadBuilder.CreateSolution().WithProvisioningType(ProvisioningType.Patient).WithTimeUnitDescription(null).Build()},
                {"missing-provisioning-type", () => CreateOrderItemRequestPayloadBuilder.CreateSolution().WithProvisioningType(null).Build()},
                {"missing-type", () => CreateOrderItemRequestPayloadBuilder.CreateSolution().WithCataloguePriceType(null).Build()},
                {"missing-currency-code", () => CreateOrderItemRequestPayloadBuilder.CreateSolution().WithCurrencyCode(null).Build()},
                {"missing-delivery-date", () => CreateOrderItemRequestPayloadBuilder.CreateSolution().WithDeliveryDate(null).Build()},
                {"missing-quantity", () => CreateOrderItemRequestPayloadBuilder.CreateSolution().WithQuantity(null).Build()},
                {"missing-estimation-period", () => CreateOrderItemRequestPayloadBuilder.CreateSolution().WithEstimationPeriod(null).Build()},
                {"missing-price", () => CreateOrderItemRequestPayloadBuilder.CreateSolution().WithPrice(null).Build()},
                {"invalid-value-currency-code", () => CreateOrderItemRequestPayloadBuilder.CreateSolution().WithCurrencyCode("INV").Build()},
                {"invalid-value-provisioning-type", () => CreateOrderItemRequestPayloadBuilder.CreateSolution().WithProvisioningType(ProvisioningType.Invalid).Build()},
                {"invalid-value-type", () => CreateOrderItemRequestPayloadBuilder.CreateSolution().WithCataloguePriceType(CataloguePriceType.Invalid).Build()},
                {"invalid-value-estimation-period", () => CreateOrderItemRequestPayloadBuilder.CreateSolution().WithEstimationPeriod(TimeUnit.Invalid).Build()},
                {"too-long-ods-code", () => CreateOrderItemRequestPayloadBuilder.CreateSolution().WithOdsCode(new string('1', 9)).Build()},
                {"too-long-catalogue-item-id", () => CreateOrderItemRequestPayloadBuilder.CreateSolution().WithCatalogueItemId(new string('1', 15)).Build()},
                {"too-long-catalogue-item-name", () => CreateOrderItemRequestPayloadBuilder.CreateSolution().WithCatalogueItemName(new string('1', 256)).Build()},
                {"above-delivery-window", () => CreateOrderItemRequestPayloadBuilder.CreateSolution().WithDeliveryDate(new DateTime(2021,1,1).AddDays(MaximumDeliveryDateOffsetDays)).Build()},
                {"below-delivery-window", () => CreateOrderItemRequestPayloadBuilder.CreateSolution().WithDeliveryDate(new DateTime(2021,1,1).AddDays(-1)).Build()},
                {"less-than-min-quantity", () => CreateOrderItemRequestPayloadBuilder.CreateSolution().WithQuantity(0).Build()},
                {"greater-than-max-quantity", () => CreateOrderItemRequestPayloadBuilder.CreateSolution().WithQuantity(int.MaxValue).Build()},
                {"less-than-min-price", () => CreateOrderItemRequestPayloadBuilder.CreateSolution().WithPrice(-1).Build()}, 
                {"greater-than-max-price", () => CreateOrderItemRequestPayloadBuilder.CreateSolution().WithPrice(1000000000000000m).Build()},
                {"on-demand-per-month", () => CreateOrderItemRequestPayloadBuilder.CreateSolution().WithProvisioningType(ProvisioningType.OnDemand).WithEstimationPeriod(TimeUnit.Month).Build()},
                {"on-demand-per-year", () => CreateOrderItemRequestPayloadBuilder.CreateSolution().WithProvisioningType(ProvisioningType.OnDemand).WithEstimationPeriod(TimeUnit.Year).Build()},
                {"patient", () => CreateOrderItemRequestPayloadBuilder.CreateSolution().WithProvisioningType(ProvisioningType.Patient).WithEstimationPeriod(null).Build()},
                {"declarative", () => CreateOrderItemRequestPayloadBuilder.CreateSolution().WithProvisioningType(ProvisioningType.Declarative).WithEstimationPeriod(null).Build()}
            };

        public CreateCatalogueSolutionOrderItemRequest(
            Request request,
            string orderingApiBaseAddress,
            string orderId) : base(request, orderingApiBaseAddress, orderId)
        {
        }
    }
}
