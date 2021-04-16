using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class OrderExtensionsTests
    {
        [Test]
        [CommonAutoData]
        public static void IsSupplierSectionComplete_HasPrimaryContact_ReturnsTrue(Order order)
        {
            order.IsSupplierSectionComplete().Should().BeTrue();
        }

        [Test]
        public static void IsSupplierSectionComplete_NullPrimaryContact_ReturnsFalse()
        {
            var order = OrderBuilder
                .Create()
                .WithSupplierContact(null)
                .Build();

            order.IsSupplierSectionComplete().Should().BeFalse();
        }

        [Test]
        public static void IsSupplierSectionComplete_NullOrder_ReturnsFalse()
        {
            var actual = OrderExtensions.IsSupplierSectionComplete(null);
            actual.Should().BeFalse();
        }

        [Test]
        public static void IsCommencementDateSectionComplete_HasCommencementDate_ReturnsTrue()
        {
            var order = OrderBuilder
                .Create()
                .WithCommencementDate(new DateTime(2020, 05, 31))
                .Build();

            order.IsCommencementDateSectionComplete().Should().BeTrue();
        }

        [Test]
        public static void IsCommencementDateSectionComplete_NullPrimaryContact_ReturnsFalse()
        {
            var order = OrderBuilder
                .Create()
                .WithCommencementDate(null)
                .Build();

            order.IsCommencementDateSectionComplete().Should().BeFalse();
        }

        [Test]
        public static void IsCommencementDateSectionComplete_NullOrder_ReturnsFalse()
        {
            var actual = OrderExtensions.IsCommencementDateSectionComplete(null);
            actual.Should().BeFalse();
        }

        [Test]
        public static void IsServiceRecipientsSectionComplete_NullOrder_ReturnsFalse()
        {
            var actual = OrderExtensions.IsServiceRecipientsSectionComplete(null);
            actual.Should().BeFalse();
        }

        [Test]
        public static void IsServiceRecipientsSectionComplete_ServiceRecipientsViewed_ReturnsTrue()
        {
            var order = OrderBuilder.Create().WithServiceRecipientsViewed(true).Build();
            var actual = order.IsServiceRecipientsSectionComplete();
            actual.Should().BeTrue();
        }

        [Test]
        [CommonAutoData]
        public static void IsServiceRecipientsSectionComplete_ServiceRecipientsViewedFalse_ReturnsFalse(Order order)
        {
            order.Progress.ServiceRecipientsViewed = false;
            var actual = order.IsServiceRecipientsSectionComplete();
            actual.Should().BeFalse();
        }

        [Test]
        public static void IsCatalogueSolutionsSectionComplete_NullOrder_ReturnsFalse()
        {
            var actual = OrderExtensions.IsCatalogueSolutionsSectionComplete(null);
            actual.Should().BeFalse();
        }

        [Test]
        [CommonAutoData]
        public static void IsCatalogueSolutionsSectionComplete_CatalogueSolutionsViewed_ReturnsTrue(Order order)
        {
            order.Progress.CatalogueSolutionsViewed = true;
            var actual = order.IsCatalogueSolutionsSectionComplete();
            actual.Should().BeTrue();
        }

        [Test]
        public static void IsCatalogueSolutionsSectionComplete_CatalogueSolutionsNotViewed_ReturnsFalse()
        {
            var order = OrderBuilder.Create().WithCatalogueSolutionsViewed(false).Build();
            var actual = order.IsCatalogueSolutionsSectionComplete();
            actual.Should().BeFalse();
        }

        [Test]
        public static void IsAssociatedServicesSectionComplete_NullOrder_ReturnsFalse()
        {
            var actual = OrderExtensions.IsAssociatedServicesSectionComplete(null);
            actual.Should().BeFalse();
        }

        [TestCase(false)]
        [TestCase(true)]
        public static void IsAssociatedServicesSectionComplete_ToggleViewed_ReturnsExpectedValue(bool viewed)
        {
            var order = OrderBuilder.Create().WithAssociatedServicesViewed(viewed).Build();
            var actual = order.IsAssociatedServicesSectionComplete();
            actual.Should().Be(viewed);
        }

        [Test]
        public static void IsAdditionalServicesSectionComplete_NullOrder_ReturnsFalse()
        {
            var actual = OrderExtensions.IsAdditionalServicesSectionComplete(null);
            actual.Should().BeFalse();
        }

        [TestCase(false)]
        [TestCase(true)]
        public static void IsAdditionalServicesSectionComplete_OrderHasSolution_ToggleViewed_ReturnsExpectedValue(bool viewed)
        {
            var order = OrderBuilder.Create()
                .WithCatalogueSolution()
                .WithAdditionalServicesViewed(viewed).Build();
            var actual = order.IsAdditionalServicesSectionComplete();
            actual.Should().Be(viewed);
        }

        [TestCase(false)]
        [TestCase(true)]
        public static void IsAdditionalServicesSectionComplete_OrderHasNoSolution_ToggleViewed_ReturnsFalse(bool viewed)
        {
            var order = OrderBuilder.Create().WithAdditionalServicesViewed(viewed).Build();
            var actual = order.IsAdditionalServicesSectionComplete();
            actual.Should().BeFalse();
        }

        [Test]
        public static void IsFundingSourceComplete_OrderFundingViewed_WithCatalogueSolution_ReturnsTrue()
        {
            var order = OrderBuilder.Create()
                .WithCatalogueSolution().WithFundingSourceOnlyGms(true).Build();
            var actual = order.IsFundingSourceComplete();
            actual.Should().BeTrue();
        }

        [Test]
        public static void IsFundingSourceComplete_OrderFundingViewed_WithAssociatedService_ReturnsTrue()
        {
            var order = OrderBuilder.Create()
                .WithOrderItem(OrderItemBuilder.Create().WithCatalogueItem(new CatalogueItem
                {
                    CatalogueItemType = CatalogueItemType.AssociatedService,
                }).Build()).WithFundingSourceOnlyGms(true).Build();
            var actual = order.IsFundingSourceComplete();
            actual.Should().BeTrue();
        }

        [Test]
        public static void IsFundingSourceComplete_OrderFundingViewed_WithNoItem_ReturnsFalse()
        {
            var order = OrderBuilder.Create().WithFundingSourceOnlyGms(true).Build();
            var actual = order.IsFundingSourceComplete();
            actual.Should().BeFalse();
        }

        [Test]
        public static void IsFundingSourceComplete_FundingSourceOnlyGmsIsNull_ReturnsFalse()
        {
            var order = OrderBuilder.Create()
                .WithCatalogueSolution().WithFundingSourceOnlyGms(null).Build();
            var actual = order.IsFundingSourceComplete();
            actual.Should().BeFalse();
        }

        [Test]
        public static void IsFundingSourceComplete_NullOrder_ReturnsFalse()
        {
            var actual = OrderExtensions.IsFundingSourceComplete(null);
            actual.Should().BeFalse();
        }

        [Test]
        public static void IsSectionStatusCompleteComplete_NullOrder_ReturnsFalse()
        {
            var actual = OrderExtensions.IsSectionStatusComplete(null);
            actual.Should().BeFalse();
        }

        [Test]
        [CommonInlineAutoData(null, false, false, false, false, false, false)]
        [CommonInlineAutoData(true, true, true, true, true, false, true)]
        [CommonInlineAutoData(true, true, true, true, true, true, true)]
        [CommonInlineAutoData(true, true, true, true, false, true, true)]
        [CommonInlineAutoData(true, true, false, false, false, false, false)]
        [CommonInlineAutoData(true, true, false, true, true, false, false)]
        [CommonInlineAutoData(true, false, true, false, false, true, false)]
        public static void IsSectionStatusCompleteComplete_whenCalled_ReturnsCorrectResult(
            bool? fundingComplete,
            bool recipientViewed,
            bool associatedViewed,
            bool solutionViewed,
            bool hasSolution,
            bool hasAssociated,
            bool expectedResult,
            IReadOnlyList<CatalogueItemId> itemIds,
            Order order)
        {
            order.FundingSourceOnlyGms = fundingComplete;
            order.Progress.AssociatedServicesViewed = associatedViewed;
            order.Progress.CatalogueSolutionsViewed = solutionViewed;
            order.Progress.ServiceRecipientsViewed = recipientViewed;

            if (hasSolution)
            {
                order.AddOrUpdateOrderItem(new OrderItem
                {
                    CatalogueItem = new CatalogueItem
                    {
                        Id = itemIds[0],
                        CatalogueItemType = CatalogueItemType.Solution,
                    },
                });
            }

            if (hasAssociated)
            {
                order.AddOrUpdateOrderItem(new OrderItem
                {
                    CatalogueItem = new CatalogueItem
                    {
                        Id = itemIds[1],
                        CatalogueItemType = CatalogueItemType.AssociatedService,
                    },
                });
            }

            var actual = order.IsSectionStatusComplete();

            actual.Should().Be(expectedResult);
        }
    }
}
