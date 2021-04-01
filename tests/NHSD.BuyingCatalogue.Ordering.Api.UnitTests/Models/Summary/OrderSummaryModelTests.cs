using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Summary;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Models.Summary
{
    [TestFixture]
    [Parallelizable(ParallelScope.Children)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class OrderSummaryModelTests
    {
        private const string Complete = "complete";
        private const string Incomplete = "incomplete";

        [Test]
        public static void Create_NullOrder_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => OrderSummaryModel.Create(null));
        }

        [Test]
        [CommonAutoData]
        public static void Create_SetsExpectedOrderId(Order order)
        {
            order.OrderStatus = OrderStatus.Incomplete;

            var model = OrderSummaryModel.Create(order);

            model.OrderId.Should().Be(order.CallOffId.ToString());
        }

        [Test]
        [CommonAutoData]
        public static void Create_NullOrderingParty_SetsExpectedOrganisationId(Supplier supplier)
        {
            var order = new Order
            {
                OrderStatus = OrderStatus.Incomplete,
                Supplier = supplier,
            };

            var model = OrderSummaryModel.Create(order);

            model.OrganisationId.Should().Be(Guid.Empty);
        }

        [Test]
        [CommonAutoData]
        public static void Create_SetsExpectedOrganisationId(Order order)
        {
            order.OrderStatus = OrderStatus.Incomplete;

            var model = OrderSummaryModel.Create(order);

            model.OrganisationId.Should().Be(order.OrderingParty.Id.ToString());
        }

        [Test]
        [CommonAutoData]
        public static void Create_SetsExpectedDescription(Order order)
        {
            order.OrderStatus = OrderStatus.Incomplete;

            var model = OrderSummaryModel.Create(order);

            model.Description.Should().Be(order.Description);
        }

        [Test]
        [CommonAutoData]
        public static void Create_SetsExpectedDescriptionSectionStatus(Order order)
        {
            order.OrderingPartyContact = null;
            order.OrderStatus = OrderStatus.Incomplete;

            var model = OrderSummaryModel.Create(order);

            model.Sections.Should().ContainEquivalentOf(SectionModel.Description);
        }

        [Test]
        [CommonAutoData]
        public static void Create_NullOrderingPartyContact_SetsExpectedOrderingPartySectionStatus(Order order)
        {
            order.OrderingPartyContact = null;
            order.OrderStatus = OrderStatus.Incomplete;

            var model = OrderSummaryModel.Create(order);

            model.Sections.Should().ContainEquivalentOf(SectionModel.OrderingParty.WithStatus(Incomplete));
        }

        [Test]
        [CommonAutoData]
        public static void Create_SetsExpectedOrderingPartySectionStatus(Order order)
        {
            order.OrderStatus = OrderStatus.Incomplete;

            var model = OrderSummaryModel.Create(order);

            model.Sections.Should().ContainEquivalentOf(SectionModel.OrderingParty.WithStatus(Complete));
        }

        [Test]
        [CommonAutoData]
        public static void Create_NullSupplierContact_SetsExpectedSupplierSectionStatus(Order order)
        {
            order.SupplierContact = null;
            order.OrderStatus = OrderStatus.Incomplete;

            var model = OrderSummaryModel.Create(order);

            model.Sections.Should().ContainEquivalentOf(SectionModel.Supplier.WithStatus(Incomplete));
        }

        [Test]
        [CommonAutoData]
        public static void Create_SetsExpectedSupplierSectionStatus(Order order)
        {
            order.OrderStatus = OrderStatus.Incomplete;

            var model = OrderSummaryModel.Create(order);

            model.Sections.Should().ContainEquivalentOf(SectionModel.Supplier.WithStatus(Complete));
        }

        [Test]
        [CommonAutoData]
        public static void Create_NullCommencementDate_SetsExpectedCommencementDateSectionStatus(Order order)
        {
            order.CommencementDate = null;
            order.OrderStatus = OrderStatus.Incomplete;

            var model = OrderSummaryModel.Create(order);

            model.Sections.Should().ContainEquivalentOf(SectionModel.CommencementDate.WithStatus(Incomplete));
        }

        [Test]
        [CommonAutoData]
        public static void Create_SetsExpectedCommencementDateSectionStatus(Order order)
        {
            order.OrderStatus = OrderStatus.Incomplete;

            var model = OrderSummaryModel.Create(order);

            model.Sections.Should().ContainEquivalentOf(SectionModel.CommencementDate.WithStatus(Complete));
        }

        [Test]
        [CommonAutoData]
        public static void Create_AssociatedServicesNotViewed_SetsExpectedAssociatedServicesSectionStatus(Order order)
        {
            order.Progress.AssociatedServicesViewed = false;
            order.OrderStatus = OrderStatus.Incomplete;

            var model = OrderSummaryModel.Create(order);

            model.Sections.Should().ContainEquivalentOf(SectionModel.AssociatedServices.WithStatus(Incomplete).WithCount(0));
        }

        [Test]
        [CommonAutoData]
        public static void Create_SetsExpectedAssociatedServicesSectionStatus(Order order)
        {
            order.Progress.AssociatedServicesViewed = true;
            order.OrderStatus = OrderStatus.Incomplete;
            order.AddOrUpdateOrderItem(new OrderItem
            {
                CatalogueItem = new CatalogueItem { CatalogueItemType = CatalogueItemType.AssociatedService },
            });

            var model = OrderSummaryModel.Create(order);

            model.Sections.Should().ContainEquivalentOf(SectionModel.AssociatedServices.WithStatus(Complete).WithCount(1));
        }

        [Test]
        [CommonAutoData]
        public static void Create_ServiceRecipientsNotViewed_SetsExpectedServiceRecipientsSectionStatus(Order order)
        {
            order.Progress.ServiceRecipientsViewed = false;
            order.OrderStatus = OrderStatus.Incomplete;

            var model = OrderSummaryModel.Create(order);

            model.Sections.Should().ContainEquivalentOf(SectionModel.ServiceRecipients.WithStatus(Incomplete).WithCount(0));
        }

        [Test]
        [CommonAutoData]
        public static void Create_SetsExpectedServiceRecipientsSectionStatus(
            IReadOnlyList<SelectedServiceRecipient> recipients,
            Order order)
        {
            order.Progress.ServiceRecipientsViewed = true;
            order.OrderStatus = OrderStatus.Incomplete;
            order.SetSelectedServiceRecipients(recipients);

            var model = OrderSummaryModel.Create(order);

            model.Sections.Should().ContainEquivalentOf(
                SectionModel.ServiceRecipients.WithStatus(Complete).WithCount(recipients.Count));
        }

        [Test]
        [CommonAutoData]
        public static void Create_CatalogueSolutionsNotViewed_SetsExpectedCatalogueSolutionsSectionStatus(Order order)
        {
            order.Progress.CatalogueSolutionsViewed = false;
            order.OrderStatus = OrderStatus.Incomplete;

            var model = OrderSummaryModel.Create(order);

            model.Sections.Should().ContainEquivalentOf(SectionModel.CatalogueSolutions.WithStatus(Incomplete).WithCount(0));
        }

        [Test]
        [CommonAutoData]
        public static void Create_SetsExpectedCatalogueSolutionsSectionStatus(Order order)
        {
            order.Progress.CatalogueSolutionsViewed = true;
            order.OrderStatus = OrderStatus.Incomplete;
            order.AddOrUpdateOrderItem(new OrderItem
            {
                CatalogueItem = new CatalogueItem { CatalogueItemType = CatalogueItemType.Solution },
            });

            var model = OrderSummaryModel.Create(order);

            model.Sections.Should().ContainEquivalentOf(SectionModel.CatalogueSolutions.WithStatus(Complete).WithCount(1));
        }

        [Test]
        [CommonAutoData]
        public static void Create_AdditionalServicesNotViewed_SetsExpectedAdditionalServicesSectionStatus(Order order)
        {
            order.Progress.AdditionalServicesViewed = false;
            order.OrderStatus = OrderStatus.Incomplete;

            var model = OrderSummaryModel.Create(order);

            model.Sections.Should().ContainEquivalentOf(SectionModel.AdditionalServices.WithStatus(Incomplete).WithCount(0));
        }

        [Test]
        [CommonAutoData]
        public static void Create_SetsExpectedAdditionalServicesSectionStatus(Order order)
        {
            order.Progress.AdditionalServicesViewed = true;
            order.OrderStatus = OrderStatus.Incomplete;
            order.AddOrUpdateOrderItem(new OrderItem
            {
                OrderId = 22,
                CatalogueItem = new CatalogueItem { CatalogueItemType = CatalogueItemType.AdditionalService },
            });
            order.AddOrUpdateOrderItem(new OrderItem
            {
                OrderId = 42,
                CatalogueItem = new CatalogueItem { CatalogueItemType = CatalogueItemType.Solution },
            });

            var model = OrderSummaryModel.Create(order);

            var expected = SectionModel.AdditionalServices.WithStatus(Complete).WithCount(1);
            model.Sections.Should().ContainEquivalentOf(expected);
        }

        [Test]
        [CommonAutoData]
        public static void Create_NullFundingSource_SetsExpectedFundingSourceSectionStatus(Order order)
        {
            order.FundingSourceOnlyGms = null;
            order.OrderStatus = OrderStatus.Incomplete;

            var model = OrderSummaryModel.Create(order);

            model.Sections.Should().ContainEquivalentOf(SectionModel.FundingSource.WithStatus(Incomplete));
        }

        [Test]
        [CommonAutoData]
        public static void Create_SetsExpectedFundingSourceSectionStatus(Order order)
        {
            order.FundingSourceOnlyGms = true;
            order.OrderStatus = OrderStatus.Incomplete;

            var model = OrderSummaryModel.Create(order);

            model.Sections.Should().ContainEquivalentOf(SectionModel.FundingSource.WithStatus(Complete));
        }

        [Test]
        [CommonAutoData]
        public static void Create_IncompleteOrder_SetsExpectedSectionStatus(Order order)
        {
            order.FundingSourceOnlyGms = null;
            order.OrderStatus = OrderStatus.Incomplete;

            var model = OrderSummaryModel.Create(order);

            model.SectionStatus.Should().Be(Incomplete);
        }

        [Test]
        [CommonAutoData]
        public static void Create_CompleteOrder_SetsExpectedSectionStatus(Order order)
        {
            order.Progress.AssociatedServicesViewed = true;
            order.Progress.CatalogueSolutionsViewed = true;
            order.OrderStatus = OrderStatus.Incomplete;
            order.AddOrUpdateOrderItem(new OrderItem
            {
                CatalogueItem = new CatalogueItem { CatalogueItemType = CatalogueItemType.Solution },
            });

            var model = OrderSummaryModel.Create(order);

            model.SectionStatus.Should().Be(Complete);
        }

        [Test]
        [CommonAutoData]
        public static void Create_SetsExpectedStatus(Order order)
        {
            var status = OrderStatus.Complete;
            order.OrderStatus = status;

            var model = OrderSummaryModel.Create(order);

            model.Status.Should().Be(status.Name);
        }
    }
}
