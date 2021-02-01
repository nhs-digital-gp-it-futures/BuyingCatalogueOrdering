using System;
using System.Diagnostics.CodeAnalysis;
using AutoFixture.NUnit3;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Domain.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class OrderItemMergeTests
    {
        [Test]
        [AutoData]
        public static void Constructor_InitializesUserId(Guid userId, string userName)
        {
            var merge = new OrderItemMerge(Array.Empty<ServiceRecipient>(), userId, userName);
            merge.UserId.Should().Be(userId);
        }

        [Test]
        [AutoData]
        public static void Constructor_InitializesUserName(string userName)
        {
            var merge = new OrderItemMerge(Array.Empty<ServiceRecipient>(), Guid.Empty, userName);
            merge.UserName.Should().Be(userName);
        }

        [Test]
        public static void Constructor_NullUserName_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new OrderItemMerge(
                Array.Empty<ServiceRecipient>(),
                Guid.Empty,
                null));
        }

        [TestCase("")]
        [TestCase("\t")]
        public static void Constructor_EmptyUserName_ThrowsException(string userName)
        {
            Assert.Throws<ArgumentException>(() => _ = new OrderItemMerge(
                Array.Empty<ServiceRecipient>(),
                Guid.Empty,
                userName));
        }

        [Test]
        [AutoData]
        public static void AddOrderItem_NullOrderItem_ThrowsException(Guid userId, string userName)
        {
            var merge = new OrderItemMerge(Array.Empty<ServiceRecipient>(), userId, userName);
            Assert.Throws<ArgumentNullException>(() => merge.AddOrderItem(null));
        }

        [Test]
        [AutoData]
        public static void AddOrderItem_WithoutId_AddsToNewItems(Guid userId, string userName)
        {
            var merge = new OrderItemMerge(Array.Empty<ServiceRecipient>(), userId, userName);
            var orderItem = OrderItemBuilder.Create().Build();
            var result = merge.AddOrderItem(orderItem);

            result.Should().BeTrue();
            merge.NewItems.Should().HaveCount(1);
            merge.NewItems.Should().Contain(orderItem);
            merge.UpdatedItems.Should().HaveCount(0);
        }

        [Test]
        [AutoData]
        public static void AddOrderItem_WithId_AddsToUpdatedItems(int orderItemId, Guid userId, string userName)
        {
            var merge = new OrderItemMerge(Array.Empty<ServiceRecipient>(), userId, userName);
            var orderItem = OrderItemBuilder.Create().WithOrderItemId(orderItemId).Build();

            var result = merge.AddOrderItem(orderItem);

            result.Should().BeTrue();
            merge.NewItems.Should().HaveCount(0);
            merge.UpdatedItems.Should().HaveCount(1);
            merge.UpdatedItems.Should().ContainKey(orderItemId);
        }

        [Test]
        [AutoData]
        public static void AddOrderItem_WithExistingId_IsNotAdded(int orderItemId, Guid userId, string userName)
        {
            var merge = new OrderItemMerge(Array.Empty<ServiceRecipient>(), userId, userName);
            var orderItem1 = OrderItemBuilder.Create().WithOrderItemId(orderItemId).Build();
            var orderItem2 = OrderItemBuilder.Create().WithOrderItemId(orderItemId).Build();

            merge.AddOrderItem(orderItem1).Should().BeTrue();
            merge.AddOrderItem(orderItem2).Should().BeFalse();

            merge.UpdatedItems.Should().HaveCount(1);
        }

        [Test]
        [AutoData]
        public static void MarkOrderSectionsAsViewed_MarksExpectedSections(Guid userId, string userName)
        {
            var merge = new OrderItemMerge(Array.Empty<ServiceRecipient>(), userId, userName);
            var order = OrderBuilder.Create().Build();
            var orderItem1 = OrderItemBuilder.Create().WithCatalogueItemType(CatalogueItemType.AdditionalService).Build();
            var orderItem2 = OrderItemBuilder.Create().WithCatalogueItemType(CatalogueItemType.Solution).Build();
            merge.AddOrderItem(orderItem1);
            merge.AddOrderItem(orderItem2);

            order.AdditionalServicesViewed.Should().BeFalse();
            order.AssociatedServicesViewed.Should().BeFalse();
            order.CatalogueSolutionsViewed.Should().BeFalse();

            merge.MarkOrderSectionsAsViewed(order);

            order.AdditionalServicesViewed.Should().BeTrue();
            order.AssociatedServicesViewed.Should().BeFalse();
            order.CatalogueSolutionsViewed.Should().BeTrue();
        }

        [Test]
        public static void ServiceRecipients_InitializedWithExpectedValues()
        {
            var recipient1 = new ServiceRecipient { OdsCode = "ODS1", Name = "Recipient 1" };
            var recipient2 = new ServiceRecipient { OdsCode = "ODS2", Name = "Recipient 2" };

            var merge = new OrderItemMerge(new[] { recipient1, recipient1, recipient2 }, Guid.Empty, "Foo");

            merge.Recipients.Should().BeEquivalentTo(recipient1, recipient2);
        }
    }
}
