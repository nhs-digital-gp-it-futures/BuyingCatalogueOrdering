using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AutoFixture;
using AutoFixture.Idioms;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Models
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class CreateOrderItemModelTests
    {
        [Test]
        public static void ToRequest_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new OrderCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var method = typeof(CreateOrderItemModel).GetMethod(
                nameof(CreateOrderItemModel.ToRequest),
                BindingFlags.NonPublic | BindingFlags.Instance);

            assertion.Verify(method);
        }

        [Test]
        [OrderingAutoData]
        public static void ToRequest_NullQuantity_ThrowsInvalidOperationException(
            Order order,
            CreateOrderItemModel model)
        {
            model.Quantity = null;

            Assert.Throws<InvalidOperationException>(() => model.ToRequest(order));
        }

        [Test]
        [OrderingAutoData]
        public static void ToRequest_AdditionalService_ReturnsExpectedType(
            Order order,
            CreateOrderItemModel model)
        {
            model.CatalogueItemType = CatalogueItemType.AdditionalService.ToString();

            var request = model.ToRequest(order);

            request.Should().BeOfType<CreateOrderItemAdditionalServiceRequest>();
        }

        [Test]
        [OrderingAutoData]
        public static void ToRequest_AssociatedService_ReturnsExpectedType(
            Order order,
            CreateOrderItemModel model)
        {
            model.CatalogueItemType = CatalogueItemType.AssociatedService.ToString();

            var request = model.ToRequest(order);

            request.Should().BeOfType<CreateOrderItemAssociatedServiceRequest>();
        }

        [Test]
        [OrderingAutoData]
        public static void ToRequest_Solution_ReturnsExpectedType(
            Order order,
            CreateOrderItemModel model)
        {
            model.CatalogueItemType = CatalogueItemType.Solution.ToString();

            var request = model.ToRequest(order);

            request.Should().BeOfType<CreateOrderItemSolutionRequest>();
        }

        [Test]
        [OrderingAutoData]
        public static void ToRequest_InvalidCatalogueItemType_ThrowsException(
            Order order,
            CreateOrderItemModel model)
        {
            model.CatalogueItemType = "InvalidType";

            Assert.Throws<InvalidOperationException>(() => model.ToRequest(order));
        }
    }
}
