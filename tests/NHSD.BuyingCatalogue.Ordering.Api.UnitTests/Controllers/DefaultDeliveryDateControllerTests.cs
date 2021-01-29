using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Errors;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Api.Validation;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class DefaultDeliveryDateControllerTests
    {
        [Test]
        public static void Constructor_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(DefaultDeliveryDateController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Test]
        [OrderingAutoData]
        public static void AddOrUpdateAsync_NullDefaultDeliveryDateModel_ThrowsArgumentNullException(
            string orderId,
            string catalogueItemId,
            int priceId,
            DefaultDeliveryDateController controller)
        {
            Assert.ThrowsAsync<ArgumentNullException>(
                () => controller.AddOrUpdateAsync(orderId, catalogueItemId, priceId, null));
        }

        [Test]
        [OrderingAutoData]
        public static async Task AddOrUpdateAsync_BadOrderId_ReturnsNotFound(
            string orderId,
            string catalogueItemId,
            int priceId,
            DefaultDeliveryDateModel defaultDeliveryDate,
            DefaultDeliveryDateController controller)
        {
            var response = await controller.AddOrUpdateAsync(orderId, catalogueItemId, priceId, defaultDeliveryDate);

            response.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [OrderingAutoData]
        public static async Task AddOrUpdateAsync_NotValid_ReturnsExpectedResponse(
            string orderId,
            string catalogueItemId,
            int priceId,
            Order order,
            DefaultDeliveryDateModel defaultDeliveryDate,
            [Frozen] Mock<IOrderRepository> orderRepository,
            [Frozen] Mock<IDefaultDeliveryDateValidator> validator,
            ErrorsModel errors,
            DefaultDeliveryDateController controller)
        {
            orderRepository.Setup(o => o.GetOrderByIdAsync(It.IsAny<string>())).ReturnsAsync(order);
            validator.Setup(v => v.Validate(defaultDeliveryDate, order.CommencementDate)).Returns((false, errors));

            var response = await controller.AddOrUpdateAsync(orderId, catalogueItemId, priceId, defaultDeliveryDate);

            response.Should().BeOfType<BadRequestObjectResult>();
            response.As<BadRequestObjectResult>().Value.Should().Be(errors);
        }

        [Test]
        [OrderingAutoData]
        public static async Task AddOrUpdateAsync_InvokesAddOrUpdateAsync(
            string orderId,
            string catalogueItemId,
            int priceId,
            Order order,
            DefaultDeliveryDateModel defaultDeliveryDate,
            [Frozen] Mock<IOrderRepository> orderRepository,
            [Frozen] Mock<IDefaultDeliveryDateRepository> repository,
            [Frozen] Mock<IDefaultDeliveryDateValidator> validator,
            DefaultDeliveryDateController controller)
        {
            orderRepository.Setup(o => o.GetOrderByIdAsync(It.IsAny<string>())).ReturnsAsync(order);
            validator.Setup(v => v.Validate(defaultDeliveryDate, order.CommencementDate)).Returns((true, null));

            await controller.AddOrUpdateAsync(orderId, catalogueItemId, priceId, defaultDeliveryDate);

            var expectedDeliveryDate = new DefaultDeliveryDate
            {
                OrderId = orderId,
                CatalogueItemId = catalogueItemId,
                PriceId = priceId,
                DeliveryDate = defaultDeliveryDate.DeliveryDate.GetValueOrDefault(),
            };

            repository.Verify(r => r.AddOrUpdateAsync(It.Is<DefaultDeliveryDate>(d => VerifyDeliveryDate(expectedDeliveryDate, d))));
        }

        [Test]
        [OrderingAutoData]
        public static async Task AddOrUpdateAsync_Add_ReturnsExpectedStatusCode(
            string orderId,
            string catalogueItemId,
            int priceId,
            Order order,
            DefaultDeliveryDateModel defaultDeliveryDate,
            [Frozen] Mock<IOrderRepository> orderRepository,
            [Frozen] Mock<IDefaultDeliveryDateRepository> repository,
            [Frozen] Mock<IDefaultDeliveryDateValidator> validator,
            DefaultDeliveryDateController controller)
        {
            orderRepository.Setup(o => o.GetOrderByIdAsync(It.IsAny<string>())).ReturnsAsync(order);
            repository.Setup(r => r.AddOrUpdateAsync(It.IsAny<DefaultDeliveryDate>())).ReturnsAsync(true);
            validator.Setup(v => v.Validate(defaultDeliveryDate, order.CommencementDate)).Returns((true, null));

            var response = await controller.AddOrUpdateAsync(orderId, catalogueItemId, priceId, defaultDeliveryDate);

            response.Should().BeOfType<StatusCodeResult>();
            response.As<StatusCodeResult>().StatusCode.Should().Be(StatusCodes.Status201Created);
        }

        [Test]
        [OrderingAutoData]
        public static async Task AddOrUpdateAsync_Update_ReturnsExpectedStatusCode(
            string orderId,
            string catalogueItemId,
            int priceId,
            Order order,
            DefaultDeliveryDateModel defaultDeliveryDate,
            [Frozen] Mock<IOrderRepository> orderRepository,
            [Frozen] Mock<IDefaultDeliveryDateValidator> validator,
            DefaultDeliveryDateController controller)
        {
            orderRepository.Setup(o => o.GetOrderByIdAsync(It.IsAny<string>())).ReturnsAsync(order);
            validator.Setup(v => v.Validate(defaultDeliveryDate, order.CommencementDate)).Returns((true, null));

            var response = await controller.AddOrUpdateAsync(orderId, catalogueItemId, priceId, defaultDeliveryDate);

            response.Should().BeOfType<OkResult>();
        }

        [Test]
        [OrderingAutoData]
        public static async Task GetAsync_InvokesGetAsync(
                string orderId,
                string catalogueItemId,
                int priceId,
                DefaultDeliveryDate defaultDeliveryDate,
                [Frozen] Mock<IDefaultDeliveryDateRepository> repository,
                DefaultDeliveryDateController controller)
        {
            repository
                .Setup(r => r.GetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(defaultDeliveryDate);

            await controller.GetAsync(orderId, catalogueItemId, priceId);

            repository
                .Verify(r => r.GetAsync(
                    It.Is<string>(o => o == orderId),
                    It.Is<string>(c => c == catalogueItemId),
                    It.Is<int>(p => p == priceId)));
        }

        [Test]
        [OrderingAutoData]
        public static async Task GetAsync_ReturnsExpectedResult(
            string orderId,
            string catalogueItemId,
            int priceId,
            DefaultDeliveryDate defaultDeliveryDate,
            [Frozen] Mock<IDefaultDeliveryDateRepository> repository,
            DefaultDeliveryDateController controller)
        {
            repository
                .Setup(r => r.GetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(defaultDeliveryDate);

            var response = await controller.GetAsync(orderId, catalogueItemId, priceId);
            var actualDate = response.Value;

            actualDate.Should().NotBeNull();
            actualDate.DeliveryDate.Should().Be(defaultDeliveryDate.DeliveryDate);
        }

        [Test]
        [OrderingAutoData]
        public static async Task GetAsync_NotFound_ReturnsExpectedStatusCode(
            string orderId,
            string catalogueItemId,
            int priceId,
            DefaultDeliveryDateController controller)
        {
            var response = await controller.GetAsync(orderId, catalogueItemId, priceId);

            response.Should().NotBeNull();
            response.Result.Should().BeOfType<NotFoundResult>();
        }

        private static bool VerifyDeliveryDate(DefaultDeliveryDate expectedDeliveryDate, DefaultDeliveryDate actualDeliveryDate)
        {
            actualDeliveryDate.Should().BeEquivalentTo(expectedDeliveryDate);
            return true;
        }
    }
}
