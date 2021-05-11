using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Data;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses
{
    internal abstract class GetOrderItemResponseBase
    {
        [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "TimeUnit name must be lower case")]
        protected static object ConvertToExpectedBody(
            OrderItemEntity orderItemEntity,
            IDictionary<string, ServiceRecipientEntity> serviceRecipients,
            IList<OrderItemRecipientEntity> orderItemRecipients,
            IDictionary<string, PricingUnitEntity> pricingUnits)
        {
            return new
            {
                orderItemEntity.CatalogueItemId,
                orderItemEntity.CurrencyCode,
                EstimationPeriod = orderItemEntity.EstimationPeriod?.ToString(),
                ItemUnit = new
                {
                    Name = orderItemEntity.PricingUnitName,
                    pricingUnits[orderItemEntity.PricingUnitName].Description,
                },
                orderItemEntity.PriceId,
                orderItemEntity.Price,
                orderItemEntity.ProvisioningType,
                ServiceRecipients = orderItemRecipients.Select(r => new
                {
                    r.DeliveryDate,
                    serviceRecipients[r.OdsCode].Name,
                    r.OdsCode,
                    r.Quantity,
                }).ToArray(),
                TimeUnit = orderItemEntity.TimeUnit is null ? null : new
                {
                    Name = orderItemEntity.TimeUnit.ToString()?.ToLowerInvariant(),
                    Description = orderItemEntity.TimeUnit?.ToDescription(),
                },
                Type = orderItemEntity.CataloguePriceType,
            };
        }

        protected static object ReadOrderItem(JToken responseBody)
        {
            return new
            {
                CatalogueItemId = responseBody.Value<string>("catalogueItemId"),
                CurrencyCode = responseBody.Value<string>("currencyCode"),
                DeliveryDate = responseBody.Value<DateTime?>("DeliveryDate"),
                ItemUnit = ReadItemUnit(responseBody),
                PriceId = responseBody.Value<int?>("priceId"),
                Price = responseBody.Value<decimal?>("price"),
                ProvisioningType = Enum.Parse<ProvisioningType>(responseBody.Value<string>("provisioningType")),
                Quantity = responseBody.Value<int>("quantity"),
                ServiceRecipients = responseBody.SelectToken("serviceRecipients")?.Select(ReadOrderItemRecipient).ToArray(),
                TimeUnit = ReadTimeUnit(responseBody),
                Type = Enum.Parse<CataloguePriceType>(responseBody.Value<string>("type")),
                EstimationPeriod = ReadEstimationPeriod(responseBody),
            };
        }

        private static object ReadItemUnit(JToken responseBody)
        {
            object itemUnit = null;

            var itemUnitToken = responseBody.SelectToken("itemUnit");
            if (itemUnitToken is not null)
            {
                itemUnit = new
                {
                    Name = itemUnitToken.Value<string>("name"),
                    Description = itemUnitToken.Value<string>("description"),
                };
            }

            return itemUnit;
        }

        private static object ReadEstimationPeriod(JToken responseBody)
        {
            var timeUnitToken = responseBody.Value<string>("estimationPeriod");
            return timeUnitToken is not null ? Enum.Parse<TimeUnit>(timeUnitToken, true).ToString() : null;
        }

        private static object ReadTimeUnit(JToken responseBody)
        {
            object timeUnit = null;

            var timeUnitToken = responseBody.SelectToken("timeUnit");
            if (timeUnitToken is not null)
            {
                timeUnit = new
                {
                    Name = timeUnitToken.Value<string>("name"),
                    Description = timeUnitToken.Value<string>("description"),
                };
            }

            return timeUnit;
        }

        private static object ReadOrderItemRecipient(JToken responseBody)
        {
            return new
            {
                DeliveryDate = DateTime.Parse(responseBody.Value<string>("deliveryDate"), CultureInfo.InvariantCulture),
                Name = responseBody.Value<string>("name"),
                OdsCode = responseBody.Value<string>("odsCode"),
                Quantity = int.Parse(responseBody.Value<string>("quantity"), CultureInfo.InvariantCulture),
            };
        }
    }
}
