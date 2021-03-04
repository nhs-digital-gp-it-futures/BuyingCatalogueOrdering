using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.AutoFixture;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Domain.UnitTests
{
    [TestFixture]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class DefaultDeliveryDateTests
    {
        [Test]
        [CommonAutoData]
        public static void Equals_DefaultDeliveryDate_ReturnsExpectedResult(
            DefaultDeliveryDate defaultDeliveryDate)
        {
            var otherDefaultDeliveryDate = new DefaultDeliveryDate
            {
                CatalogueItemId = defaultDeliveryDate.CatalogueItemId,
                OrderId = defaultDeliveryDate.OrderId,
            };

            defaultDeliveryDate.Should().Be(otherDefaultDeliveryDate);
        }

        [Test]
        [CommonAutoData]
        public static void Equals_DefaultDeliveryDate_DifferentCatalogueItemId_ReturnsExpectedResult(
            DefaultDeliveryDate defaultDeliveryDate)
        {
            var otherDefaultDeliveryDate = new DefaultDeliveryDate
            {
                CatalogueItemId = default,
                OrderId = defaultDeliveryDate.OrderId,
            };

            defaultDeliveryDate.Should().NotBe(otherDefaultDeliveryDate);
        }

        [Test]
        [CommonAutoData]
        public static void Equals_DefaultDeliveryDate_DifferentOrderId_ReturnsExpectedResult(
            DefaultDeliveryDate defaultDeliveryDate)
        {
            var otherDefaultDeliveryDate = new DefaultDeliveryDate
            {
                CatalogueItemId = defaultDeliveryDate.CatalogueItemId,
                OrderId = defaultDeliveryDate.OrderId + 1,
            };

            defaultDeliveryDate.Should().NotBe(otherDefaultDeliveryDate);
        }

        [Test]
        [CommonAutoData]
        public static void Equals_DefaultDeliveryDate_DifferentType_ReturnsExpectedResult(
            DefaultDeliveryDate defaultDeliveryDate,
            string somethingElse)
        {
            defaultDeliveryDate.Should().NotBe(somethingElse);
        }

        [Test]
        [CommonAutoData]
        public static void Equals_DefaultDeliveryDate_NullObject_ReturnsExpectedResult(
            DefaultDeliveryDate defaultDeliveryDate)
        {
            defaultDeliveryDate.Should().NotBe(null);
        }

        [Test]
        [CommonAutoData]
        public static void Equals_DefaultDeliveryDate_SameObject_ReturnsExpectedResult(
            DefaultDeliveryDate defaultDeliveryDate)
        {
            defaultDeliveryDate.Should().Be(defaultDeliveryDate);
        }

        [Test]
        [CommonAutoData]
        public static void GetHashCode_Equal_ReturnsExpectedValue(int orderId, CatalogueItemId itemId)
        {
            var date1 = new DefaultDeliveryDate { OrderId = orderId, CatalogueItemId = itemId };
            var date2 = new DefaultDeliveryDate { OrderId = orderId, CatalogueItemId = itemId };

            var hash1 = date1.GetHashCode();
            var hash2 = date2.GetHashCode();

            hash1.Should().Be(hash2);
        }

        [Test]
        [CommonAutoData]
        public static void GetHashCode_DifferentOrderId_ReturnsExpectedValue(
            int orderId1,
            int orderId2,
            CatalogueItemId itemId)
        {
            var date1 = new DefaultDeliveryDate { OrderId = orderId1, CatalogueItemId = itemId };
            var date2 = new DefaultDeliveryDate { OrderId = orderId2, CatalogueItemId = itemId };

            var hash1 = date1.GetHashCode();
            var hash2 = date2.GetHashCode();

            hash1.Should().NotBe(hash2);
        }

        [Test]
        [CommonAutoData]
        public static void GetHashCode_DifferentCatalogueItemId_ReturnsExpectedValue(
            int orderId,
            CatalogueItemId itemId1,
            CatalogueItemId itemId2)
        {
            var date1 = new DefaultDeliveryDate { OrderId = orderId, CatalogueItemId = itemId1 };
            var date2 = new DefaultDeliveryDate { OrderId = orderId, CatalogueItemId = itemId2 };

            var hash1 = date1.GetHashCode();
            var hash2 = date2.GetHashCode();

            hash1.Should().NotBe(hash2);
        }
    }
}
