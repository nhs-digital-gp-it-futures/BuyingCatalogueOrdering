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
    [Parallelizable(ParallelScope.Children)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class CommencementDateControllerTests
    {
        [Test]
        public static void Constructor_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(CommencementDateController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Test]
        [InMemoryDbAutoData(nameof(GetAsync_OrderNotFound_ReturnsNotFound))]
        public static async Task GetAsync_OrderNotFound_ReturnsNotFound(CommencementDateController controller)
        {
            var result = await controller.GetAsync(default);

            result.Result.Should().BeOfType<NotFoundResult>();
            result.Value.Should().BeNull();
        }

        [Test]
        [InMemoryDbAutoData(nameof(GetAsync_ReturnsExpectedResult))]
        public static async Task GetAsync_ReturnsExpectedResult(
            [Frozen] ApplicationDbContext context,
            Order order1,
            Order order2,
            CommencementDateController controller)
        {
            context.Order.AddRange(order1, order2);
            await context.SaveChangesAsync();

            var expectedResult = new CommencementDateModel { CommencementDate = order2.CommencementDate };

            var result = await controller.GetAsync(order2.CallOffId);

            result.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        [InMemoryDbAutoData(nameof(UpdateAsync_NullOrder_ReturnsNotFound))]
        public static async Task UpdateAsync_NullOrder_ReturnsNotFound(
            CommencementDateModel model,
            CommencementDateController controller)
        {
            var result = await controller.UpdateAsync(null, model);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData(nameof(UpdateAsync_NullModel_ThrowsException))]
        public static void UpdateAsync_NullModel_ThrowsException(
            Order order,
            CommencementDateController controller)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await controller.UpdateAsync(order, null));
        }

        [Test]
        [InMemoryDbAutoData(nameof(UpdateAsync_UpdatesCommencementDate))]
        public static async Task UpdateAsync_UpdatesCommencementDate(
            Order order,
            CommencementDateModel model,
            CommencementDateController controller)
        {
            order.CommencementDate.Should().NotBeSameDateAs(model.CommencementDate.GetValueOrDefault());

            await controller.UpdateAsync(order, model);

            order.CommencementDate.Should().Be(model.CommencementDate);
        }

        [Test]
        [InMemoryDbAutoData(nameof(UpdateAsync_SuccessfulUpdate_ReturnsNoContentResult))]
        public static async Task UpdateAsync_SuccessfulUpdate_ReturnsNoContentResult(
            Order order,
            CommencementDateModel model,
            CommencementDateController controller)
        {
            var result = await controller.UpdateAsync(order, model);

            result.Should().BeOfType<NoContentResult>();
        }

        [Test]
        [InMemoryDbAutoData(nameof(UpdateAsync_SavesChangesToDb))]
        public static async Task UpdateAsync_SavesChangesToDb(
            [Frozen] ApplicationDbContext context,
            Order order,
            CommencementDateModel model,
            CommencementDateController controller)
        {
            context.Add(order);
            await context.SaveChangesAsync();

            await controller.UpdateAsync(order, model);

            context.Set<Order>().First(o => o.Equals(order)).CommencementDate.Should().Be(model.CommencementDate);
        }
    }
}
