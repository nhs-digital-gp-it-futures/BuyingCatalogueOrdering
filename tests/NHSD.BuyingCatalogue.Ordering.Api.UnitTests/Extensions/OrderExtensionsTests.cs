using System;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class OrderExtensionsTests
    {
        [Test]
        public void IsSupplierSectionComplete_HasPrimaryContact_ReturnsTrue()
        {
            var order = OrderBuilder
                .Create()
                .WithSupplierContact(ContactBuilder.Create().Build())
                .Build();

            order.IsSupplierSectionComplete().Should().BeTrue();
        }

        [Test]
        public void IsSupplierSectionComplete_NullPrimaryContact_ReturnsFalse()
        {
            var order = OrderBuilder
                .Create()
                .WithSupplierContact(null)
                .Build();

            order.IsSupplierSectionComplete().Should().BeFalse();
        }

        [Test]
        public void IsSupplierSectionComplete_NullOrder_ReturnsFalse()
        {
            var actual = OrderExtensions.IsSupplierSectionComplete(null);
            actual.Should().BeFalse();
        }

        [Test]
        public void IsCommencementDateSectionComplete_HasCommencementDate_ReturnsTrue()
        {
            var order = OrderBuilder
                .Create()
                .WithCommencementDate(new DateTime(2020, 05, 31))
                .Build();

            order.IsCommencementDateSectionComplete().Should().BeTrue();
        }

        [Test]
        public void IsCommencementDateSectionComplete_NullPrimaryContact_ReturnsFalse()
        {
            var order = OrderBuilder
                .Create()
                .WithCommencementDate(null)
                .Build();

            order.IsCommencementDateSectionComplete().Should().BeFalse();
        }

        [Test]
        public void IsCommencementDateSectionComplete_NullOrder_ReturnsFalse()
        {
            var actual = OrderExtensions.IsCommencementDateSectionComplete(null);
            actual.Should().BeFalse();
        }

        [Test]
        public void IsServiceRecipientsSectionComplete_NullOrder_ReturnsFalse()
        {
            var actual = OrderExtensions.IsServiceRecipientsSectionComplete(null);
            actual.Should().BeFalse();
        }

        [Test]
        public void IsServiceRecipientsSectionComplete_ServiceRecipientsViewed_ReturnsTrue()
        {
            var order = OrderBuilder.Create().WithServiceRecipientsViewed(true).Build();
            var actual = OrderExtensions.IsServiceRecipientsSectionComplete(order);
            actual.Should().BeTrue();
        }

        [Test]
        public void IsServiceRecipientsSectionComplete_ServiceRecipientsViewedFalse_ReturnsFalse()
        {
            var order = OrderBuilder.Create().WithServiceRecipientsViewed(false).Build();
            var actual = OrderExtensions.IsServiceRecipientsSectionComplete(order);
            actual.Should().BeFalse();
        }

        [Test]
        public void IsCatalogueSolutionsSectionComplete_NullOrder_ReturnsFalse()
        {
            var actual = OrderExtensions.IsCatalogueSolutionsSectionComplete(null);
            actual.Should().BeFalse();
        }

        [Test]
        public void IsCatalogueSolutionsSectionComplete_CatalogueSolutionsViewed_ReturnsTrue()
        {
            var order = OrderBuilder.Create().WithCatalogueSolutionsViewed(true).Build();
            var actual = OrderExtensions.IsCatalogueSolutionsSectionComplete(order);
            actual.Should().BeTrue();
        }

        [Test]
        public void IsCatalogueSolutionsSectionComplete_CatalogueSolutionsNotViewed_ReturnsFalse()
        {
            var order = OrderBuilder.Create().WithCatalogueSolutionsViewed(false).Build();
            var actual = OrderExtensions.IsCatalogueSolutionsSectionComplete(order);
            actual.Should().BeFalse();
        }

        [Test]
        public void IsAssociatedServicesSectionComplete_NullOrder_ReturnsFalse()
        {
            var actual = OrderExtensions.IsAssociatedServicesSectionComplete(null);
            actual.Should().BeFalse();
        }

        [TestCase(false)]
        [TestCase(true)]
        public void IsAssociatedServicesSectionComplete_ToggleViewed_ReturnsBool(bool viewed)
        {
            var order = OrderBuilder.Create().WithAdditionalServicesViewed(viewed).Build();
            var actual = order.IsAssociatedServicesSectionComplete();
            actual.Should().BeFalse();
        }

        [Test]
        public void IsFundingSourceComplete_OrderFundingViewed_ReturnsTrue()
        {
            var order = OrderBuilder.Create().WithFundingSourceOnlyGms(true).Build();
            var actual = OrderExtensions.IsFundingSourceComplete(order);
            actual.Should().BeTrue();
        }

        [Test]
        public void IsFundingSourceComplete_NullOrder_ReturnsFalse()
        {
            var actual = OrderExtensions.IsFundingSourceComplete(null);
            actual.Should().BeFalse();
        }

        [Test]
        public void IsSectionStatusCompleteComplete_NullOrder_ReturnsFalse()
        {
            var actual = OrderExtensions.IsSectionStatusComplete(null);
            actual.Should().BeFalse();
        }

        [TestCase(null, false, false, false, false, false, false)]
        [TestCase(true, true, true, true, true, false, true)]
        [TestCase(true, true, true, true, true, true, true)]
        [TestCase(true, true, true, true, false, true, true)]
        [TestCase(true, true, true, true, false, true, true)]
        [TestCase(true, true, false, false, false, false, false)]
        [TestCase(true, true, false, true, true, false, false)]
        [TestCase(true, false, true, false, false, true, false)]
        public void IsSectionStatusCompleteComplete_whenCalled_ReturnsCorrectResult(
            bool? fundingComplete,
            bool recipientViewed,
            bool associatedViewed,
            bool solutionViewed,
            bool hasSolution,
            bool hasAssociated,
            bool expectedResult)
        {
            var orderBuilder = OrderBuilder
                .Create()
                .WithFundingSourceOnlyGms(fundingComplete)
                .WithServiceRecipientsViewed(recipientViewed)
                .WithAssociatedServicesViewed(associatedViewed)
                .WithCatalogueSolutionsViewed(solutionViewed);

            if (hasSolution)
            {
                var orderItem = OrderItemBuilder.Create();
                orderItem.WithCatalogueItemType(CatalogueItemType.Solution);
                orderBuilder.WithOrderItem(orderItem.Build());
            }

            if (hasAssociated)
            {
                var orderItem = OrderItemBuilder.Create();
                orderItem.WithCatalogueItemType(CatalogueItemType.AssociatedService);
                orderBuilder.WithOrderItem(orderItem.Build());
            }

            var order = orderBuilder.Build();
            var actual = order.IsSectionStatusComplete();
            actual.Should().Be(expectedResult);
        }
    }
}
