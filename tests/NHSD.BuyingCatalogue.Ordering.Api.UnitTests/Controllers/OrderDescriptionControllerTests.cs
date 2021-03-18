using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class OrderDescriptionControllerTests
    {
        [Test]
        public static void Constructor_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrderDescriptionController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task Get_OrderIdDoesNotExist_ReturnsNotFound(
            CallOffId callOffId,
            OrderDescriptionController controller)
        {
            var result = await controller.GetAsync(callOffId);

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task Get_OrderIdExists_ReturnsTheOrdersDescription(
            [Frozen] ApplicationDbContext context,
            [Frozen] CallOffId callOffId,
            Order order,
            OrderDescriptionController controller)
        {
            order.FundingSourceOnlyGms = true;
            context.Order.Add(order);
            await context.SaveChangesAsync();

            var expected = new OrderDescriptionModel { Description = order.Description };

            var actual = await controller.GetAsync(callOffId);

            actual.Value.Should().BeEquivalentTo(expected);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateAsync_OrderIsNull_ReturnsNotFound(
            OrderDescriptionModel model,
            OrderDescriptionController controller)
        {
            var response = await controller.UpdateAsync(null, model);

            response.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData]
        public static void UpdateAsync_ModelIsNull_ThrowsArgumentExceptionNull(
            Order order,
            OrderDescriptionController controller)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await controller.UpdateAsync(order, null));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateAsync_UpdatesDescription(
            Order order,
            OrderDescriptionModel model,
            OrderDescriptionController controller)
        {
            order.Description.Should().NotBe(model.Description);

            await controller.UpdateAsync(order, model);

            order.Description.Should().Be(model.Description);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateAsync_SuccessfulUpdate_ReturnsNoContentResult(
            Order order,
            OrderDescriptionModel model,
            OrderDescriptionController controller)
        {
            var result = await controller.UpdateAsync(order, model);

            result.Should().BeOfType<NoContentResult>();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateAsync_SavesChangesToDb(
            [Frozen] ApplicationDbContext context,
            Order order,
            OrderDescriptionModel model,
            OrderDescriptionController controller)
        {
            order.Description.Should().NotBe(model.Description);

            context.Add(order);
            await context.SaveChangesAsync();

            await controller.UpdateAsync(order, model);

            context.Set<Order>().First(o => o.Equals(order)).Description.Should().Be(model.Description);
        }
    }
}
