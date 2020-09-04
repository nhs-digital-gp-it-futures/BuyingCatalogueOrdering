using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Data;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses
{
    internal sealed class GetOrderResponse
    {
        private readonly string _content;

        private JToken ContentAsJson
        {
            get
            {
                return JToken.Parse(_content);
            }
        }

        private GetOrderResponse(string content)
        {
            _content = content;
        }

        public static async Task<GetOrderResponse> CreateAsync(Response response)
        {
            var content = await response.ReadBodyAsStringAsync();
            return new GetOrderResponse(content);
        }

        public void AssertOrderItemCost(decimal orderItemCost)
        {
            var item = ReadOrderItems(ContentAsJson).First(x => x.CatalogueItemName != null);
            decimal actual = item.CostPerYear;

            actual.Should().Be(orderItemCost);
        }

        public void AssertRecurringCost(string item, decimal recurringCost)
        {
            var responseContent = ContentAsJson;

            responseContent.Value<decimal>(item).Should().Be(recurringCost);
        }

        public void AssertOrder(
            OrderEntity orderEntity,
            AddressEntity orderingPartyAddress, 
            ContactEntity orderPartyContact,
            AddressEntity supplierAddress,
            ContactEntity supplierContact,
            IEnumerable<OrderItemEntity> orderItems,
            IEnumerable<ServiceRecipientEntity> serviceRecipients)
        {
            var responseContent = ContentAsJson;

            var expected = new
            {
                orderEntity.Description,
                OrderParty = new
                {
                    Name = orderEntity.OrganisationName,
                    OdsCode = orderEntity.OrganisationOdsCode,
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
                        orderingPartyAddress?.Country
                    },
                    PrimaryContact = new
                    {
                        orderPartyContact.FirstName,
                        orderPartyContact.LastName,
                        EmailAddress = orderPartyContact.Email,
                        TelephoneNumber = orderPartyContact.Phone
                    }
                },
                orderEntity.CommencementDate,
                Supplier = new
                {
                    Name = orderEntity.SupplierName,
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
                        supplierAddress?.Country
                    },
                    PrimaryContact = new
                    {
                        supplierContact.FirstName,
                        supplierContact.LastName,
                        EmailAddress = supplierContact.Email,
                        TelephoneNumber = supplierContact.Phone
                    }
                },
                OrderItems = orderItems.Select(orderItem => new
                {
                    ItemId = $"{orderEntity.OrderId}-{orderItem.OdsCode}-{orderItem.OrderItemId}",
                    ServiceRecipientsOdsCode = orderItem.OdsCode,
                    CataloguePriceType = orderItem.CataloguePriceType.ToString(),
                    CatalogueItemType = orderItem.CatalogueItemType.ToString(),
                    orderItem.CatalogueItemName,
                    ProvisioningType = orderItem.ProvisioningType.ToString(),
                    ItemUnitDescription = orderItem.PricingUnitDescription,
                    TimeUnitDescription = orderItem.TimeUnit?.ToDescription(),
                    QuantityPeriodDescription = orderItem.EstimationPeriod?.ToDescription(),
                    orderItem.Price,
                    orderItem.Quantity,
                    orderItem.DeliveryDate
                }),
                ServiceRecipients = serviceRecipients.Select(serviceRecipient => new
                {
                    serviceRecipient.Name,
                    serviceRecipient.OdsCode
                }),
                OrderStatus = orderEntity.OrderStatus.ToString(),
                orderEntity.Completed
            };

            var actual = ReadOrder(responseContent);

            actual.Should().BeEquivalentTo(expected);
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
                ServiceRecipients = ReadServiceRecipients(responseContent),
                OrderStatus = responseContent.Value<string>("status"),
                TotalOneOffCost = responseContent.Value<decimal>("totalOneOffCost"),
                TotalRecurringCostPerMonth = responseContent.Value<decimal>("totalRecurringCostPerMonth"),
                TotalRecurringCostPerYear = responseContent.Value<decimal>("totalRecurringCostPerYear"),
                TotalOwnershipCost = responseContent.Value<decimal>("totalOwnershipCost"),
                Completed = responseContent.Value<DateTime?>("dateCompleted")
            };
        }

        private static object ReadOrderingParty(JToken orderingPartyToken)
        {
            return new 
            {
                Name = orderingPartyToken?.Value<string>("name"),
                OdsCode = orderingPartyToken?.Value<string>("odsCode"),
                Address = ReadAddress(orderingPartyToken?.SelectToken("address")),
                PrimaryContact = ReadPrimaryContact(orderingPartyToken?.SelectToken("primaryContact"))
            };
        }

        private static object ReadSupplier(JToken supplierToken)
        {
            return new 
            {
                Name = supplierToken?.Value<string>("name"),
                Address = ReadAddress(supplierToken?.SelectToken("address")),
                PrimaryContact = ReadPrimaryContact(supplierToken?.SelectToken("primaryContact"))
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
                Country = addressToken?.Value<string>("country")
            };
        }

        private static object ReadPrimaryContact(JToken primaryContactToken)
        {
            return new
            {
                FirstName = primaryContactToken?.Value<string>("firstName"),
                LastName = primaryContactToken?.Value<string>("lastName"),
                EmailAddress = primaryContactToken?.Value<string>("emailAddress"),
                TelephoneNumber = primaryContactToken?.Value<string>("telephoneNumber")
            };
        }

        private static IEnumerable<dynamic> ReadOrderItems(JToken responseContent)
        {
            var orderItemsToken = responseContent.SelectToken("orderItems");

            return orderItemsToken?.Select(orderItem => new
            {
                ItemId = orderItem.Value<string>("itemId"),
                ServiceRecipientsOdsCode = orderItem.Value<string>("serviceRecipientsOdsCode"),
                CataloguePriceType = orderItem.Value<string>("cataloguePriceType"),
                CatalogueItemType = orderItem.Value<string>("catalogueItemType"),
                CatalogueItemName = orderItem.Value<string>("catalogueItemName"),
                ProvisioningType = orderItem.Value<string>("provisioningType"),
                ItemUnitDescription = orderItem.Value<string>("itemUnitDescription"),
                TimeUnitDescription = orderItem.Value<string>("timeUnitDescription"),
                QuantityPeriodDescription = orderItem.Value<string>("quantityPeriodDescription"),
                Price = orderItem.Value<decimal?>("price"),
                Quantity = orderItem.Value<int>("quantity"),
                DeliveryDate = orderItem.Value<DateTime?>("deliveryDate"),
                CostPerYear = orderItem.Value<decimal>("costPerYear")
            });
        }

        private static IEnumerable<object> ReadServiceRecipients(JToken responseContent)
        {
            return responseContent.SelectToken("serviceRecipients")?
                .Select(serviceRecipient => new
                {
                    OdsCode = serviceRecipient.Value<string>("odsCode"),
                    Name = serviceRecipient.Value<string>("name")
                });
        }
    }
}
