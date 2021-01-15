using System;
using Newtonsoft.Json.Linq;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Data;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses
{
    internal abstract class GetOrderItemResponseBase
    {
        protected static object ConvertToExpectedBody(
            OrderItemEntity orderItemEntity,
            ServiceRecipientEntity serviceRecipient)
        {
            return new
            {
                orderItemEntity.OrderItemId,
                orderItemEntity.CatalogueItemId,
                orderItemEntity.CatalogueItemType,
                orderItemEntity.CatalogueItemName,
                orderItemEntity.CurrencyCode,
                orderItemEntity.DeliveryDate,
                EstimationPeriod = orderItemEntity.EstimationPeriod?.ToString(),
                ItemUnit = new
                {
                    Name = orderItemEntity.PricingUnitName,
                    Description = orderItemEntity.PricingUnitDescription,
                },
                orderItemEntity.Price,
                orderItemEntity.ProvisioningType,
                orderItemEntity.Quantity,
                ServiceRecipient = new
                {
                    orderItemEntity.OdsCode,
                    serviceRecipient.Name,
                },
                TimeUnit = orderItemEntity.TimeUnit is null ? null : new
                {
                    Name = orderItemEntity.TimeUnit.ToString().ToLower(),
                    Description = orderItemEntity.TimeUnit?.ToDescription(),
                },
                Type = orderItemEntity.CataloguePriceType,
            };
        }

        protected object ReadOrderItem(JToken responseBody)
        {
            return new
            {
                OrderItemId = responseBody.Value<int>("orderItemId"),
                CatalogueItemId = responseBody.Value<string>("catalogueItemId"),
                CatalogueItemType = Enum.Parse<CatalogueItemType>(responseBody.Value<string>("catalogueItemType")),
                CatalogueItemName = responseBody.Value<string>("catalogueItemName"),
                CurrencyCode = responseBody.Value<string>("currencyCode"),
                DeliveryDate = responseBody.Value<DateTime?>("DeliveryDate"),
                ItemUnit = ReadItemUnit(responseBody),
                Price = responseBody.Value<decimal?>("price"),
                ProvisioningType = Enum.Parse<ProvisioningType>(responseBody.Value<string>("provisioningType")),
                Quantity = responseBody.Value<int>("quantity"),
                ServiceRecipient = ReadServiceRecipient(responseBody),
                TimeUnit = ReadTimeUnit(responseBody),
                Type = Enum.Parse<CataloguePriceType>(responseBody.Value<string>("type")),
                EstimationPeriod = ReadEstimationPeriod(responseBody),
            };
        }

        private static object ReadItemUnit(JToken responseBody)
        {
            object itemUnit = null;

            var itemUnitToken = responseBody.SelectToken("itemUnit");
            if (itemUnitToken != null)
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
            if (timeUnitToken != null)
            {
                return Enum.Parse<TimeUnit>(timeUnitToken, true).ToString();
            }

            return null;
        }

        private static object ReadTimeUnit(JToken responseBody)
        {
            object timeUnit = null;

            var timeUnitToken = responseBody.SelectToken("timeUnit");
            if (timeUnitToken != null)
            {
                timeUnit = new
                {
                    Name = timeUnitToken.Value<string>("name"),
                    Description = timeUnitToken.Value<string>("description"),
                };
            }

            return timeUnit;
        }

        private static object ReadServiceRecipient(JToken responseBody)
        {
            object serviceRecipient = null;

            var serviceRecipientToken = responseBody.SelectToken("serviceRecipient");
            if (serviceRecipientToken != null)
            {
                serviceRecipient = new
                {
                    OdsCode = serviceRecipientToken.Value<string>("odsCode"),
                    Name = serviceRecipientToken.Value<string>("name"),
                };
            }

            return serviceRecipient;
        }
    }
}
