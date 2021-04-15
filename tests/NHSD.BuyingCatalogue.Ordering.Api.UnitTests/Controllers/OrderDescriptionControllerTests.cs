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
        public static async Task Get_OrderIdDoesNotExist_ReturnsNull(
            CallOffId callOffId,
            OrderDescriptionController controller)
        {
            var result = await controller.GetAsync(callOffId);

            result.Result.Should().BeNull();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task Get_OrderIdExists_ReturnsTheOrdersDescription(
            [Frozen] Mock<IOrderDescriptionService> service,
            [Frozen] CallOffId callOffId,
            Order order,
            OrderDescriptionController controller)
        {
            service.Setup(o => o.GetOrderDescription(callOffId)).ReturnsAsync(order.Description);

            var expected = new OrderDescriptionModel { Description = order.Description };

            var actual = await controller.GetAsync(callOffId);
            service.Verify(o => o.GetOrderDescription(callOffId));

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
            [Frozen] Mock<IOrderDescriptionService> service,
            Order order,
            OrderDescriptionModel model,
            OrderDescriptionController controller)
        {
            order.Description.Should().NotBe(model.Description);
            service.Setup(o => o.SetOrderDescription(order, model.Description)).Callback(() =>
            {
                order.Description = model.Description;
            });

            await controller.UpdateAsync(order, model);
            service.Verify(o => o.SetOrderDescription(order, model.Description));

            order.Description.Should().Be(model.Description);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateAsync_SuccessfulUpdate_ReturnsNoContentResult(
            [Frozen] Mock<IOrderDescriptionService> service,
            Order order,
            OrderDescriptionModel model,
            OrderDescriptionController controller)
        {
            service.Setup(o => o.SetOrderDescription(order, model.Description)).Callback(() =>
            {
                order.Description = model.Description;
            });

            var result = await controller.UpdateAsync(order, model);
            service.Verify(o => o.SetOrderDescription(order, model.Description));

            result.Should().BeOfType<NoContentResult>();
        }
    }
}
