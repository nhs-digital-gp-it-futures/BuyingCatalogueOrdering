using System.Diagnostics.CodeAnalysis;
using AutoFixture.NUnit3;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Models
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class CreateOrderItemSolutionModelTests
    {
        [Test]
        [OrderingAutoData]
        public static void ToRequest_ReturnedCreateOrderItemRequest_HasExpectedOdsCode(
            Order order,
            [Frozen] ServiceRecipientModel serviceRecipientModel,
            CreateOrderItemSolutionModel model)
        {
            var request = model.ToRequest(order);

            request.OdsCode.Should().Be(serviceRecipientModel.OdsCode);
        }

        [Test]
        [OrderingAutoData]
        public static void ToRequest_ReturnedCreateOrderItemRequest_HasExpectedCatalogueItemType(
            Order order,
            CreateOrderItemSolutionModel model)
        {
            var request = model.ToRequest(order);

            request.CatalogueItemType.Should().Be(CatalogueItemType.Solution);
        }

        [Test]
        [OrderingAutoData]
        public static void ToRequest_ReturnedCreateOrderItemRequest_HasExpectedCatalogueItemId(
            Order order,
            CreateOrderItemSolutionModel model)
        {
            var request = model.ToRequest(order);

            request.CatalogueSolutionId.Should().BeNull();
        }

        [Test]
        [OrderingAutoData]
        public static void ToRequest_ReturnedCreateOrderItemRequest_HasExpectedPriceTimeUnit(
            [Frozen] TimeUnit timeUnit,
            Order order,
            CreateOrderItemSolutionModel model)
        {
            var request = model.ToRequest(order);

            request.PriceTimeUnit.Should().Be(timeUnit);
        }

        [Test]
        [OrderingAutoData]
        public static void ToRequest_ReturnedCreateOrderItemRequest_HasExpectedDeliveryDate(
            Order order,
            CreateOrderItemSolutionModel model)
        {
            var request = model.ToRequest(order);

            request.DeliveryDate.Should().Be(model.DeliveryDate);
        }
    }
}
