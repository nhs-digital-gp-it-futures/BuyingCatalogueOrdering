using System.Diagnostics.CodeAnalysis;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Domain.UnitTests
{
    [TestFixture]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class DefaultDeliveryDateTests
    {
        [Test]
        [AutoData]
        public static void Equals_DefaultDeliveryDate_ReturnsExpectedResult(
            DefaultDeliveryDate defaultDeliveryDate)
        {
            var otherDefaultDeliveryDate = new DefaultDeliveryDate
            {
                CatalogueItemId = defaultDeliveryDate.CatalogueItemId.ToUpperInvariant(),
                OrderId = defaultDeliveryDate.OrderId.ToUpperInvariant(),
                PriceId = defaultDeliveryDate.PriceId,
            };

            defaultDeliveryDate.Should().Be(otherDefaultDeliveryDate);
        }

        [Test]
        [AutoData]
        public static void Equals_DefaultDeliveryDate_DifferentCatalogueItemId_ReturnsExpectedResult(
            DefaultDeliveryDate defaultDeliveryDate)
        {
            var otherDefaultDeliveryDate = new DefaultDeliveryDate
            {
                CatalogueItemId = null,
                OrderId = defaultDeliveryDate.OrderId,
                PriceId = defaultDeliveryDate.PriceId,
            };

            defaultDeliveryDate.Should().NotBe(otherDefaultDeliveryDate);
        }

        [Test]
        [AutoData]
        public static void Equals_DefaultDeliveryDate_DifferentOrderId_ReturnsExpectedResult(
            DefaultDeliveryDate defaultDeliveryDate)
        {
            var otherDefaultDeliveryDate = new DefaultDeliveryDate
            {
                CatalogueItemId = defaultDeliveryDate.CatalogueItemId,
                OrderId = null,
                PriceId = defaultDeliveryDate.PriceId,
            };

            defaultDeliveryDate.Should().NotBe(otherDefaultDeliveryDate);
        }

        [Test]
        [AutoData]
        public static void Equals_DefaultDeliveryDate_DifferentPriceId_ReturnsExpectedResult(
            DefaultDeliveryDate defaultDeliveryDate)
        {
            var otherDefaultDeliveryDate = new DefaultDeliveryDate
            {
                CatalogueItemId = defaultDeliveryDate.CatalogueItemId,
                OrderId = defaultDeliveryDate.OrderId,
                PriceId = 0,
            };

            defaultDeliveryDate.Should().NotBe(otherDefaultDeliveryDate);
        }

        [Test]
        [AutoData]
        public static void Equals_DefaultDeliveryDate_DifferentType_ReturnsExpectedResult(
            DefaultDeliveryDate defaultDeliveryDate,
            string somethingElse)
        {
            defaultDeliveryDate.Should().NotBe(somethingElse);
        }

        [Test]
        [AutoData]
        public static void Equals_DefaultDeliveryDate_NullObject_ReturnsExpectedResult(
            DefaultDeliveryDate defaultDeliveryDate)
        {
            defaultDeliveryDate.Should().NotBe(null);
        }

        [Test]
        [AutoData]
        public static void Equals_DefaultDeliveryDate_SameObject_ReturnsExpectedResult(
            DefaultDeliveryDate defaultDeliveryDate)
        {
            defaultDeliveryDate.Should().Be(defaultDeliveryDate);
        }
    }
}
