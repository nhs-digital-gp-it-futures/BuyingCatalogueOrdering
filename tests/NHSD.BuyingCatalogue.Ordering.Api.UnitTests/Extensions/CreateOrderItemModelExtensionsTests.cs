﻿using System;
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
    internal sealed class CreateOrderItemModelExtensionsTests
    {
        [Test]
        public void ToRequest_NullOrder_ThrowsArgumentNullException()
        {
            var builder = CreateOrderItemModelBuilder.Create().BuildSolution();
            
            Assert.Throws<ArgumentNullException>(() => builder.ToRequest(null, CatalogueItemType.Solution));
        }

        [Test]
        public void ToRequest_NullCatalogueItemType_ThrowsArgumentNullException()
        {
            var builder = CreateOrderItemModelBuilder.Create().BuildSolution();

            Assert.Throws<ArgumentNullException>(() => builder.ToRequest(OrderBuilder.Create().Build(), null));
        }

        [Test]
        public void ToRequest_NullQuantity_ThrowsArgumentException()
        {
            var builder = CreateOrderItemModelBuilder.Create().WithQuantity(null).BuildSolution();
            var order = OrderBuilder.Create().Build();

            Assert.Throws<ArgumentException>(() => builder.ToRequest(order, CatalogueItemType.Solution));
        }

        [Test]
        public void ToRequest_Order_CatalogueSolutionType_ReturnsCreateOrderItemRequest()
        {
            var model = CreateOrderItemModelBuilder
                .Create()
                .BuildSolution();

            var order = OrderBuilder
                .Create()
                .Build();

            var catalogueItemType = CatalogueItemType.Solution;

            var actual = model.ToRequest(order, catalogueItemType);

            var expected = CreateOrderItemRequestBuilder
                .Create()
                .WithOrder(order)
                .WithOdsCode(model.ServiceRecipient?.OdsCode)
                .WithCatalogueItemId(model.CatalogueSolutionId)
                .WithCatalogueItemType(catalogueItemType)
                .WithCatalogueItemName(model.CatalogueSolutionName)
                .WithProvisioningTypeName(model.ProvisioningType)
                .WithCataloguePriceTypeName(model.Type)
                .WithCataloguePriceUnitTierName(model.ItemUnit?.Name)
                .WithCataloguePriceUnitDescription(model.ItemUnit?.Description)
                .WithPriceTimeUnitName(null)
                .WithCurrencyCode(model.CurrencyCode)
                .WithQuantity(model.Quantity.Value)
                .WithEstimationPeriodName(model.EstimationPeriod)
                .WithDeliveryDate(model.DeliveryDate)
                .WithPrice(model.Price)
                .Build();

            actual.Should().BeEquivalentTo(expected);

        }
    }
}
