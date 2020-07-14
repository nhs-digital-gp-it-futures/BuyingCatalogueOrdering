using System;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class CreateOrderItemBaseModelTests
    {
        [Test]
        public void ToRequest_NullOrder_ThrowsArgumentNullException()
        {
            var builder = CreateOrderItemModelBuilder.Create().BuildSolution();
            
            Assert.Throws<ArgumentNullException>(() => builder.ToRequest(null));
        }

        [Test]
        public void ToRequest_NullQuantity_ThrowsInvalidOperationException()
        {
            var builder = CreateOrderItemModelBuilder.Create().WithQuantity(null).BuildSolution();
            var order = OrderBuilder.Create().Build();

            Assert.Throws<InvalidOperationException>(() => builder.ToRequest(order));
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

            var actual = model.ToRequest(order);

            var expected = CreateOrderItemRequestBuilder
                .Create()
                .WithOrder(order)
                .WithOdsCode(model.ServiceRecipient?.OdsCode)
                .WithCatalogueItemId(model.CatalogueItemId)
                .WithCatalogueItemType(CatalogueItemType.Solution)
                .WithCatalogueItemName(model.CatalogueItemName)
                .WithCatalogueSolutionId(null)
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
