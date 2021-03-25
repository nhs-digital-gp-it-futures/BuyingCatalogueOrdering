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
        private static Mock<IOrderService> moqorderService;

        [SetUp]
        public static void Setup()
        {
            moqorderService = new Mock<IOrderService>();
        }

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

            result.Result.Should().BeOfType<NotFoundResult>();
            result.Value.Should().BeNull();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetAsync_ReturnsExpectedResult(
            [Frozen] ApplicationDbContext context,
            Order order,
            CommencementDateController controller)
        {
            controller = new CommencementDateController(context, moqorderService.Object);

            var expectedResult = new CommencementDateModel { CommencementDate = order.CommencementDate };
            moqorderService.Setup(o => o.GetCommencementDate(order.CallOffId)).ReturnsAsync(order.CommencementDate);

            var result = await controller.GetAsync(order.CallOffId);
            moqorderService.Verify(o => o.GetCommencementDate(order.CallOffId));

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
