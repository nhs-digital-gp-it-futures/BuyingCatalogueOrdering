using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Data;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests
{
    internal sealed class CreateCatalogueSolutionOrderItemRequest
    {
        private readonly Request _request;
        private readonly string _createCatalogueSolutionOrderItemUrl;

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

        public CreateCatalogueSolutionOrderItemRequestPayload Payload { get; set; }

        public async Task ExecuteAsync()
        {
            await _request.PostJsonAsync(_createCatalogueSolutionOrderItemUrl, new
            {
                ServiceRecipient = new 
                {
                    Payload.OdsCode
                },
                Payload.CatalogueSolutionId,
                Payload.CatalogueSolutionName,
                Payload.DeliveryDate,
                Payload.Quantity,
                EstimationPeriod = Payload.EstimationPeriod != null ? Enum.GetName(typeof(TimeUnit), Payload.EstimationPeriod) : null,
                ProvisioningType = Enum.GetName(typeof(ProvisioningType), Payload.ProvisioningType),
                Type = Enum.GetName(typeof(CataloguePriceType), Payload.PriceType),
                Payload.CurrencyCode,
                ItemUnitModel = new
                {
                    Name = Payload.ItemUnitName,
                    Description = Payload.ItemUnitNameDescription
                },
                Payload.Price
            });
        }

        public void AssertPayload(OrderItemEntity orderItem)
        {
            var actual = new
            {
                orderItem.OdsCode,
                CatalogueSolutionId = orderItem.CatalogueItemId,
                CatalogueSolutionName = orderItem.CatalogueItemName,
                orderItem.DeliveryDate,
                orderItem.Quantity,
                EstimationPeriod = orderItem.EstimationPeriodId != null ? (TimeUnit)orderItem.EstimationPeriodId : (TimeUnit?) null,
                ProvisioningType = (ProvisioningType)orderItem.ProvisioningTypeId,
                PriceType = (CataloguePriceType)orderItem.CataloguePriceTypeId,
                orderItem.CurrencyCode,
                ItemUnitName = orderItem.PricingUnitTierName,
                ItemUnitNameDescription = orderItem.PricingUnitDescription,
                orderItem.Price,
            };

            actual.Should().BeEquivalentTo(Payload);
        }
    }

    internal sealed class CreateCatalogueSolutionOrderItemRequestPayload
    {
        public string OdsCode { get; set; }

        public string CatalogueSolutionId { get; set; }

        public string CatalogueSolutionName { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public int Quantity { get; set; }

        public TimeUnit? EstimationPeriod { get; set; }

        public ProvisioningType ProvisioningType { get; set; }

        public CataloguePriceType PriceType { get; set; }

        public string CurrencyCode { get; set; }

        public string ItemUnitName { get; set; }

        public string ItemUnitNameDescription { get; set; }

        public decimal? Price { get; set; }
    }
}
