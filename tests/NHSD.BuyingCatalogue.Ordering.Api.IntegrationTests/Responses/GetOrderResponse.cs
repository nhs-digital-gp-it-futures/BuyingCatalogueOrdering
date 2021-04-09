using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Data;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses
{
    internal sealed class GetOrderResponse
    {
        private readonly string content;

        private GetOrderResponse(string content)
        {
            this.content = content;
        }

        private JToken ContentAsJson
        {
            get
            {
                return JToken.Parse(content);
            }
        }

        public static async Task<GetOrderResponse> CreateAsync(Response response)
        {
            var content = await response.ReadBodyAsStringAsync();
            return new GetOrderResponse(content);
        }

        public void AssertOrderItemCost(decimal orderItemCost)
        {
            decimal actual = ReadOrderItems(ContentAsJson).Sum(c => c.CostPerYear);

            actual.Should().Be(orderItemCost);
        }

        public void AssertOrderItemRecipientCost(string odsCode, string catalogueItemId, decimal orderItemCost)
        {
            var item = ReadOrderItems(ContentAsJson).First(i => i.CatalogueItemId == catalogueItemId);
            ExpectedServiceRecipient serviceRecipient = item.ServiceRecipients.First(i => i.OdsCode == odsCode);

            serviceRecipient.CostPerYear.Should().Be(orderItemCost);
        }

        public void AssertRecurringCost(string item, decimal recurringCost)
        {
            var responseContent = ContentAsJson;

            responseContent.Value<decimal>(item).Should().Be(recurringCost);
        }

        public void AssertServiceInstanceId(IEnumerable<object> expectedServiceInstanceIds)
        {
            var actual = ReadOrderItems(ContentAsJson).SelectMany(d => d.ServiceRecipients);
            actual.Should().BeEquivalentTo(expectedServiceInstanceIds, c => c.IncludingAllDeclaredProperties());
        }

        public void AssertOrder(
            OrderEntity orderEntity,
            OrderingPartyEntity orderingPartyEntity,
            SupplierEntity supplierEntity,
            AddressEntity orderingPartyAddress,
            ContactEntity orderPartyContact,
            AddressEntity supplierAddress,
            ContactEntity supplierContact,
            IEnumerable<OrderItemEntity> orderItems,
            IDictionary<(int OrderId, string CatalogueItemId), IList<OrderItemRecipientEntity>> serviceRecipients,
            IDictionary<string, PricingUnitEntity> pricingUnits,
            IDictionary<string, CatalogueItemEntity> catalogueItems)
        {
            var responseContent = ContentAsJson;
            var i = 1;

            var expected = new
            {
                orderEntity.Description,
                OrderParty = new
                {
                    orderingPartyEntity.Name,
                    orderingPartyEntity.OdsCode,
                    Address = new
                    {
                        orderingPartyAddress?.Line1,
                        orderingPartyAddress?.Line2,
                        orderingPartyAddress?.Line3,
                        orderingPartyAddress?.Line4,
                        orderingPartyAddress?.Line5,
                        orderingPartyAddress?.Town,
                        orderingPartyAddress?.County,
                        orderingPartyAddress?.Postcode,
                        orderingPartyAddress?.Country,
                    },
                    PrimaryContact = new
                    {
                        orderPartyContact.FirstName,
                        orderPartyContact.LastName,
                        EmailAddress = orderPartyContact.Email,
                        TelephoneNumber = orderPartyContact.Phone,
                    },
                },
                Supplier = new
                {
                    supplierEntity.Name,
                    Address = new
                    {
                        supplierAddress?.Line1,
                        supplierAddress?.Line2,
                        supplierAddress?.Line3,
                        supplierAddress?.Line4,
                        supplierAddress?.Line5,
                        supplierAddress?.Town,
                        supplierAddress?.County,
                        supplierAddress?.Postcode,
                        supplierAddress?.Country,
                    },
                    PrimaryContact = new
                    {
                        supplierContact.FirstName,
                        supplierContact.LastName,
                        EmailAddress = supplierContact.Email,
                        TelephoneNumber = supplierContact.Phone,
                    },
                },
                orderEntity.CommencementDate,
                OrderItems = orderItems.Select(orderItem => new
                {
                    CataloguePriceType = orderItem.CataloguePriceType.ToString(),
                    CatalogueItemType = catalogueItems[orderItem.CatalogueItemId].CatalogueItemType.ToString(),
                    CatalogueItemName = catalogueItems[orderItem.CatalogueItemId].Name,
                    ProvisioningType = orderItem.ProvisioningType.ToString(),
                    orderItem.Price,
                    ItemUnitDescription = pricingUnits[orderItem.PricingUnitName].Description,
                    TimeUnitDescription = orderItem.TimeUnit?.ToDescription(),
                    ServiceRecipients = serviceRecipients[(orderItem.OrderId, orderItem.CatalogueItemId)].Select(r => new
                    {
                        ItemId = $"C{orderEntity.Id:D6}-{orderEntity.Revision:D2}-{r.OdsCode}-{i++}",
                        r.DeliveryDate,
                        r.OdsCode,
                        r.Quantity,
                    }),
                    QuantityPeriodDescription = orderItem.EstimationPeriod?.ToDescription(),
                }),
                OrderStatus = orderEntity.OrderStatus.ToString(),
                orderEntity.Completed,
            };

            var actual = ReadOrder(responseContent);

            actual.Should().BeEquivalentTo(expected);
        }

        public void AssertServiceInstanceIdAsync(IEnumerable<object> expected)
        {
            var responseContent = ContentAsJson;
            var orderItems = responseContent.SelectToken("orderItems") as JArray;

            Assert.NotNull(orderItems);

            var allServiceInstanceIds = new List<object>();

            foreach (var orderItem in orderItems)
            {
                var serviceInstanceIds = ReadServiceInstanceItem(orderItem, orderItem.Value<string>("catalogueItemId"));
                allServiceInstanceIds.AddRange(serviceInstanceIds);
            }

            allServiceInstanceIds.Should().BeEquivalentTo(expected);
        }

        private static IEnumerable<object> ReadServiceInstanceItem(JToken responseBody, string catalogueItemId)
        {
            return responseBody.SelectToken("serviceRecipients")?.Select(t => new
            {
                CatalogueItemId = catalogueItemId,
                OdsCode = t.Value<string>("odsCode"),
                ServiceInstanceId = t.Value<string>("serviceInstanceId"),
            });
        }

        private static object ReadOrder(JToken responseContent)
        {
            return new
            {
                Description = responseContent.Value<string>("description"),
                OrderParty = ReadOrderingParty(responseContent.SelectToken("orderParty")),
                CommencementDate = responseContent.Value<DateTime?>("commencementDate"),
                Supplier = ReadSupplier(responseContent.SelectToken("supplier")),
                OrderItems = ReadOrderItems(responseContent),
                TotalOneOffCost = responseContent.Value<decimal>("totalOneOffCost"),
                TotalRecurringCostPerMonth = responseContent.Value<decimal>("totalRecurringCostPerMonth"),
                TotalRecurringCostPerYear = responseContent.Value<decimal>("totalRecurringCostPerYear"),
                TotalOwnershipCost = responseContent.Value<decimal>("totalOwnershipCost"),
                OrderStatus = responseContent.Value<string>("status"),
                Completed = responseContent.Value<DateTime?>("dateCompleted"),
            };
        }

        private static object ReadOrderingParty(JToken orderingPartyToken)
        {
            return new
            {
                Name = orderingPartyToken?.Value<string>("name"),
                OdsCode = orderingPartyToken?.Value<string>("odsCode"),
                Address = ReadAddress(orderingPartyToken?.SelectToken("address")),
                PrimaryContact = ReadPrimaryContact(orderingPartyToken?.SelectToken("primaryContact")),
            };
        }

        private static object ReadSupplier(JToken supplierToken)
        {
            return new
            {
                Name = supplierToken?.Value<string>("name"),
                Address = ReadAddress(supplierToken?.SelectToken("address")),
                PrimaryContact = ReadPrimaryContact(supplierToken?.SelectToken("primaryContact")),
            };
        }

        private static object ReadAddress(JToken addressToken)
        {
            return new
            {
                Line1 = addressToken?.Value<string>("line1"),
                Line2 = addressToken?.Value<string>("line2"),
                Line3 = addressToken?.Value<string>("line3"),
                Line4 = addressToken?.Value<string>("line4"),
                Line5 = addressToken?.Value<string>("line5"),
                Town = addressToken?.Value<string>("town"),
                County = addressToken?.Value<string>("county"),
                Postcode = addressToken?.Value<string>("postcode"),
                Country = addressToken?.Value<string>("country"),
            };
        }

        private static object ReadPrimaryContact(JToken primaryContactToken)
        {
            return new
            {
                FirstName = primaryContactToken?.Value<string>("firstName"),
                LastName = primaryContactToken?.Value<string>("lastName"),
                EmailAddress = primaryContactToken?.Value<string>("emailAddress"),
                TelephoneNumber = primaryContactToken?.Value<string>("telephoneNumber"),
            };
        }

        private static IEnumerable<ExpectedOrderItem> ReadOrderItems(JToken responseContent)
        {
            var orderItemsToken = responseContent.SelectToken("orderItems");

            return orderItemsToken?.Select(orderItem => new ExpectedOrderItem
            {
                CatalogueItemId = orderItem.Value<string>("catalogueItemId"),
                CataloguePriceType = orderItem.Value<string>("cataloguePriceType"),
                CatalogueItemType = orderItem.Value<string>("catalogueItemType"),
                CatalogueItemName = orderItem.Value<string>("catalogueItemName"),
                ProvisioningType = orderItem.Value<string>("provisioningType"),
                Price = orderItem.Value<decimal?>("price"),
                ItemUnitDescription = orderItem.Value<string>("itemUnitDescription"),
                TimeUnitDescription = orderItem.Value<string>("timeUnitDescription"),
                ServiceRecipients = ReadRecipients(orderItem),
                QuantityPeriodDescription = orderItem.Value<string>("quantityPeriodDescription"),
                CostPerYear = orderItem.Value<decimal>("costPerYear"),
            });
        }

        private static IEnumerable<ExpectedServiceRecipient> ReadRecipients(JToken responseContent)
        {
            return responseContent.SelectToken("serviceRecipients")?
                .Select(serviceRecipient => new ExpectedServiceRecipient
                {
                    ItemId = serviceRecipient.Value<string>("itemId"),
                    ServiceInstanceId = serviceRecipient.Value<string>("serviceInstanceId"),
                    DeliveryDate = serviceRecipient.Value<DateTime>("deliveryDate"),
                    Name = serviceRecipient.Value<string>("name"),
                    OdsCode = serviceRecipient.Value<string>("odsCode"),
                    Quantity = serviceRecipient.Value<int>("quantity"),
                    CostPerYear = serviceRecipient.Value<int>("costPerYear"),
                });
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class ExpectedOrderItem
        {
            public string CatalogueItemId { get; init; }

            public string CataloguePriceType { get; init; }

            public string CatalogueItemType { get; init; }

            public string CatalogueItemName { get; init; }

            public string ProvisioningType { get; init; }

            public decimal? Price { get; init; }

            public string ItemUnitDescription { get; init; }

            public string TimeUnitDescription { get; init; }

            public IEnumerable<ExpectedServiceRecipient> ServiceRecipients { get; init; }

            public string QuantityPeriodDescription { get; init; }

            public decimal CostPerYear { get; init; }
        }

        private sealed class ExpectedServiceRecipient
        {
            public string ItemId { get; init; }

            public string ServiceInstanceId { get; init; }

            public DateTime? DeliveryDate { get; init; }

            public string Name { get; init; }

            public string OdsCode { get; init; }

            public int? Quantity { get; init; }

            public decimal? CostPerYear { get; init; }
        }
    }
}
