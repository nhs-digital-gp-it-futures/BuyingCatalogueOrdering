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
            var merge = new OrderItemMerge(userId, userName);
            merge.UserId.Should().Be(userId);
        }

        [Test]
        [AutoData]
        public static void Constructor_InitializesUserName(string userName)
        {
            var merge = new OrderItemMerge(Guid.Empty, userName);
            merge.UserName.Should().Be(userName);
        }

        [Test]
        public static void Constructor_NullUserName_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new OrderItemMerge(Guid.Empty, null));
        }

        [TestCase("")]
        [TestCase("\t")]
        public static void Constructor_EmptyUserName_ThrowsException(string userName)
        {
            Assert.Throws<ArgumentException>(() => _ = new OrderItemMerge(Guid.Empty, userName));
        }

        [Test]
        [AutoData]
        public static void AddOrderItem_NullOrderItem_ThrowsException(OrderItemMerge merge)
        {
            Assert.Throws<ArgumentNullException>(() => merge.AddOrderItem(null));
        }

        [Test]
        [AutoData]
        public static void AddOrderItem_WithoutId_AddsToNewItems(OrderItemMerge merge)
        {
            var orderItem = OrderItemBuilder.Create().Build();

            var result = merge.AddOrderItem(orderItem);

            result.Should().BeTrue();
            merge.NewItems.Should().HaveCount(1);
            merge.NewItems.Should().Contain(orderItem);
            merge.UpdatedItems.Should().HaveCount(0);
        }

        [Test]
        [AutoData]
        public static void AddOrderItem_WithId_AddsToUpdatedItems(int orderItemId, OrderItemMerge merge)
        {
            var orderItem = OrderItemBuilder.Create().WithOrderItemId(orderItemId).Build();

            var result = merge.AddOrderItem(orderItem);

            result.Should().BeTrue();
            merge.NewItems.Should().HaveCount(0);
            merge.UpdatedItems.Should().HaveCount(1);
            merge.UpdatedItems.Should().ContainKey(orderItemId);
        }

        [Test]
        [AutoData]
        public static void AddOrderItem_WithExistingId_IsNotAdded(int orderItemId, OrderItemMerge merge)
        {
            var orderItem1 = OrderItemBuilder.Create().WithOrderItemId(orderItemId).Build();
            var orderItem2 = OrderItemBuilder.Create().WithOrderItemId(orderItemId).Build();

            merge.AddOrderItem(orderItem1).Should().BeTrue();
            merge.AddOrderItem(orderItem2).Should().BeFalse();

            merge.UpdatedItems.Should().HaveCount(1);
        }

        [Test]
        [AutoData]
        public static void MarkOrderSectionsAsViewed_MarksExpectedSections(OrderItemMerge merge)
        {
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
    }
}
