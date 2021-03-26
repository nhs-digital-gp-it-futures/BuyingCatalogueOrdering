using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Contracts;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
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
        [InMemoryDbAutoData]
        public static async Task GetAsync_OrderNotFound_ReturnsNotFound(CommencementDateController controller)
        {
            var result = await controller.GetAsync(default);

            result.Value.CommencementDate.Should().BeNull();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetAsync_ReturnsExpectedResult(
            [Frozen] Mock<IOrderService> orderService,
            Order order,
            CommencementDateController controller)
        {
            controller = new CommencementDateController(orderService.Object);

            var expectedResult = new CommencementDateModel { CommencementDate = order.CommencementDate };
            orderService.Setup(o => o.GetCommencementDate(order.CallOffId)).ReturnsAsync(order.CommencementDate);

            var result = await controller.GetAsync(order.CallOffId);
            orderService.Verify(o => o.GetCommencementDate(order.CallOffId));

            result.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateAsync_NullOrder_ReturnsNotFound(
            CommencementDateModel model,
            CommencementDateController controller)
        {
            var result = await controller.UpdateAsync(null, model);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData]
        public static void UpdateAsync_NullModel_ThrowsException(
            Order order,
            CommencementDateController controller)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await controller.UpdateAsync(order, null));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateAsync_UpdatesCommencementDate(
            [Frozen] Mock<IOrderService> orderService,
            Order order,
            CommencementDateModel model,
            CommencementDateController controller)
        {
            order.CommencementDate.Should().NotBeSameDateAs(model.CommencementDate.GetValueOrDefault());

            orderService.Setup(o => o.SetCommencementDate(order, model.CommencementDate)).Callback(() =>
            {
                order.CommencementDate = model.CommencementDate;
            });

            await controller.UpdateAsync(order, model);
            orderService.Verify(o => o.SetCommencementDate(order, model.CommencementDate));

            order.CommencementDate.Should().Be(model.CommencementDate);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateAsync_SuccessfulUpdate_ReturnsNoContentResult(
            Order order,
            CommencementDateModel model,
            CommencementDateController controller)
        {
            var result = await controller.UpdateAsync(order, model);

            result.Should().BeOfType<NoContentResult>();
        }
    }
}
