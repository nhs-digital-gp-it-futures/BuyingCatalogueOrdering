using System;
using System.Linq;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Models
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class OrderModelTests
    {
        [Test]
        public static void Create_NullOrder_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = OrderModel.Create(null));
        }

        [Test]
        [InMemoryDbFullOrderAutoData]
        public static void Create_ValidOrder_SetsCommencementDate(Order order)
        {
            var actual = OrderModel.Create(order);

            actual.CommencementDate.Should().Be(order.CommencementDate);
        }

        [Test]
        [InMemoryDbFullOrderAutoData]
        public static void Create_ValidOrder_SetsDateCompleted(Order order)
        {
            var actual = OrderModel.Create(order);

            actual.DateCompleted.Should().Be(order.Completed);
        }

        [Test]
        [InMemoryDbFullOrderAutoData]
        public static void Create_ValidOrder_SetsDescription(Order order)
        {
            var actual = OrderModel.Create(order);

            actual.Description.Should().Be(order.Description);
        }

        [Test]
        [InMemoryDbFullOrderAutoData]
        public static void Create_ValidOrder_SetsOrderParty(Order order)
        {
            var actual = OrderModel.Create(order);

            actual.OrderParty.Should()
                .BeEquivalentTo(new OrderingPartyModel(order.OrderingParty, order.OrderingPartyContact));
        }

        [Test]
        [InMemoryDbFullOrderAutoData]
        public static void Create_ValidOrder_SetsStatusName(Order order)
        {
            var actual = OrderModel.Create(order);

            actual.Status.Should().Be(order.OrderStatus.Name);
        }

        [Test]
        [InMemoryDbFullOrderAutoData]
        public static void Create_ValidOrder_SetsSupplier(Order order)
        {
            var actual = OrderModel.Create(order);

            actual.Supplier.Should().BeEquivalentTo(new SupplierModel(order.Supplier, order.SupplierContact));
        }

        [Test]
        [InMemoryDbFullOrderAutoData]
        public static void Create_ValidOrder_SetsTotalOneOffCost(Order order)
        {
            var actual = OrderModel.Create(order);

            actual.TotalOneOffCost.Should().Be(order.CalculateCostPerYear(CostType.OneOff));
        }

        [Test]
        [InMemoryDbFullOrderAutoData]
        public static void Create_ValidOrder_SetsTotalRecurringCostPerMonth(Order order)
        {
            var actual = OrderModel.Create(order);

            actual.TotalRecurringCostPerMonth.Should().Be(order.CalculateCostPerYear(CostType.Recurring) / 12);
        }

        [Test]
        [InMemoryDbFullOrderAutoData]
        public static void Create_ValidOrder_SetsTotalRecurringCostPerYear(Order order)
        {
            var actual = OrderModel.Create(order);

            actual.TotalRecurringCostPerYear.Should().Be(order.CalculateCostPerYear(CostType.Recurring));
        }

        [Test]
        [InMemoryDbFullOrderAutoData]
        public static void Create_OrderItems_CountSameAsForOrder(Order order)
        {
            var actual = OrderModel.Create(order);

            actual.OrderItems.Count.Should().Be(order.OrderItems.Count);
        }

        [Test]
        [InMemoryDbFullOrderAutoData]
        public static void Create_OrderItems_SetsCatalogueItemId(Order order)
        {
            var actual = OrderModel.Create(order);

            for (int i = 0; i < actual.OrderItems.Count; i++)
            {
                actual.OrderItems[i].CatalogueItemId.Should().Be(order.OrderItems[i].CatalogueItem.Id.ToString());
            }
        }

        [Test]
        [InMemoryDbFullOrderAutoData]
        public static void Create_OrderItems_SetsCatalogueItemName(Order order)
        {
            var actual = OrderModel.Create(order);

            for (int i = 0; i < actual.OrderItems.Count; i++)
            {
                actual.OrderItems[i].CatalogueItemName.Should().Be(order.OrderItems[i].CatalogueItem.Name.ToString());
            }
        }

        [Test]
        [InMemoryDbFullOrderAutoData]
        public static void Create_OrderItems_SetsCatalogueItemType(Order order)
        {
            var actual = OrderModel.Create(order);

            for (int i = 0; i < actual.OrderItems.Count; i++)
            {
                actual.OrderItems[i]
                    .CatalogueItemType.Should()
                    .Be(order.OrderItems[i].CatalogueItem.CatalogueItemType.ToString());
            }
        }

        [Test]
        [InMemoryDbFullOrderAutoData]
        public static void Create_OrderItems_SetsCataloguePriceType(Order order)
        {
            var actual = OrderModel.Create(order).OrderItems;

            for (int i = 0; i < actual.Count; i++)
            {
                actual[i].CataloguePriceType.Should().Be(order.OrderItems[i].CataloguePriceType.ToString());
            }
        }

        [Test]
        [InMemoryDbFullOrderAutoData]
        public static void Create_OrderItems_SetsCostPerYear(Order order)
        {
            var actual = OrderModel.Create(order).OrderItems;

            for (int i = 0; i < actual.Count; i++)
            {
                actual[i].CostPerYear.Should().Be(order.OrderItems[i].CalculateTotalCostPerYear());
            }
        }

        [Test]
        [InMemoryDbFullOrderAutoData]
        public static void Create_OrderItems_SetsItemUnitDescription(Order order)
        {
            var actual = OrderModel.Create(order).OrderItems;

            for (int i = 0; i < actual.Count; i++)
            {
                actual[i].ItemUnitDescription.Should().Be(order.OrderItems[i].PricingUnit.Description);
            }
        }

        [Test]
        [InMemoryDbFullOrderAutoData]
        public static void Create_OrderItems_SetsPrice(Order order)
        {
            var actual = OrderModel.Create(order).OrderItems;

            for (int i = 0; i < actual.Count; i++)
            {
                actual[i].Price.Should().Be(order.OrderItems[i].Price);
            }
        }

        [Test]
        [InMemoryDbFullOrderAutoData]
        public static void Create_OrderItems_SetsProvisioningType(Order order)
        {
            var actual = OrderModel.Create(order).OrderItems;

            for (int i = 0; i < actual.Count; i++)
            {
                actual[i].ProvisioningType.Should().Be(order.OrderItems[i].ProvisioningType.ToString());
            }
        }

        [Test]
        [InMemoryDbFullOrderAutoData]
        public static void Create_OrderItems_SetsTimeUnitDescription(Order order)
        {
            var actual = OrderModel.Create(order).OrderItems;

            for (int i = 0; i < actual.Count; i++)
            {
                actual[i].TimeUnitDescription.Should().Be(order.OrderItems[i].PriceTimeUnit?.Description());
            }
        }

        [Test]
        [InMemoryDbFullOrderAutoData]
        public static void Create_OrderItems_SetsQuantityPeriodDescription(Order order)
        {
            var actual = OrderModel.Create(order).OrderItems;

            for (int i = 0; i < actual.Count; i++)
            {
                actual[i]
                    .QuantityPeriodDescription.Should()
                    .Be(order.OrderItems[i].EstimationPeriod?.Description());
            }
        }

        [Test]
        [InMemoryDbFullOrderAutoData]
        public static void Create_OrderItems_SetsAllRecipientsFromOrder(Order order)
        {
            var actual = OrderModel.Create(order).OrderItems;

            for (int i = 0; i < actual.Count; i++)
            {
                actual[i].ServiceRecipients.Count.Should().Be(order.OrderItems[i].OrderItemRecipients.Count);
            }
        }

        [Test]
        [InMemoryDbFullOrderAutoData]
        public static void Create_OrderItems_Recipients_SetsCostPerYear(Order order)
        {
            var actual = OrderModel.Create(order).OrderItems;

            for (int i = 0; i < actual.Count; i++)
            {
                for (int j = 0; j < actual[i].ServiceRecipients.Count; j++)
                {
                    var priceTimeUnit = order.OrderItems[i].ProvisioningType == ProvisioningType.OnDemand ?
                        order.OrderItems[i].EstimationPeriod : order.OrderItems[i].PriceTimeUnit;

                    actual[i]
                        .ServiceRecipients[j]
                        .CostPerYear.Should()
                        .Be(
                            order.OrderItems[i]
                                .OrderItemRecipients[j]
                                .CalculateTotalCostPerYear(order.OrderItems[i].Price ?? 0, priceTimeUnit));
                }
            }
        }

        [Test]
        [InMemoryDbFullOrderAutoData]
        public static void Create_OrderItems_Recipients_SetsDeliveryDate(Order order)
        {
            var actual = OrderModel.Create(order).OrderItems;

            for (int i = 0; i < actual.Count; i++)
            {
                for (int j = 0; j < actual[i].ServiceRecipients.Count; j++)
                {
                    actual[i]
                        .ServiceRecipients[j]
                        .DeliveryDate.Should()
                        .Be(order.OrderItems[i].OrderItemRecipients[j].DeliveryDate);
                }
            }
        }

        [Test]
        [InMemoryDbFullOrderAutoData]
        public static void Create_OrderItems_Recipients_SetsItemId(Order order)
        {
            var actual = OrderModel.Create(order).OrderItems;

            int count = 1;
            for (int i = 0; i < actual.Count; i++)
            {
                for (int j = 0; j < actual[i].ServiceRecipients.Count; j++)
                {
                    actual[i]
                        .ServiceRecipients[j]
                        .ItemId.Should()
                        .Be($"{order.CallOffId}-{order.OrderItems[i].OrderItemRecipients[j].Recipient.OdsCode}-{count}");

                    count++;
                }
            }
        }

        [Test]
        [InMemoryDbFullOrderAutoData]
        public static void Create_OrderItems_Recipients_SetsName(Order order)
        {
            var actual = OrderModel.Create(order).OrderItems;

            for (int i = 0; i < actual.Count; i++)
            {
                for (int j = 0; j < actual[i].ServiceRecipients.Count; j++)
                {
                    actual[i]
                        .ServiceRecipients[j]
                        .Name.Should()
                        .Be(order.OrderItems[i].OrderItemRecipients[j].Recipient.Name);
                }
            }
        }

        [Test]
        [InMemoryDbFullOrderAutoData]
        public static void Create_OrderItems_Recipients_SetsOdsCode(Order order)
        {
            var actual = OrderModel.Create(order).OrderItems;

            for (int i = 0; i < actual.Count; i++)
            {
                for (int j = 0; j < actual[i].ServiceRecipients.Count; j++)
                {
                    actual[i]
                        .ServiceRecipients[j]
                        .OdsCode.Should()
                        .Be(order.OrderItems[i].OrderItemRecipients[j].Recipient.OdsCode);
                }
            }
        }

        [Test]
        [InMemoryDbFullOrderAutoData]
        public static void Create_OrderItems_Recipients_SetsQuantity(Order order)
        {
            var actual = OrderModel.Create(order).OrderItems;

            for (int i = 0; i < actual.Count; i++)
            {
                for (int j = 0; j < actual[i].ServiceRecipients.Count; j++)
                {
                    actual[i]
                        .ServiceRecipients[j]
                        .Quantity.Should()
                        .Be(order.OrderItems[i].OrderItemRecipients[j].Quantity);
                }
            }
        }
    }
}
