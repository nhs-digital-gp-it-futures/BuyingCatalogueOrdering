using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Errors;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Api.Validation;
using NHSD.BuyingCatalogue.Ordering.Contracts;
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
        [InMemoryDbAutoData]
        public static void AddOrUpdateAsync_NullDefaultDeliveryDateModel_ThrowsArgumentNullException(
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            DefaultDeliveryDateController controller)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => controller.AddOrUpdateAsync(callOffId, catalogueItemId, null));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task AddOrUpdateAsync_BadOrderId_ReturnsNotFound(
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            DefaultDeliveryDateModel model,
            DefaultDeliveryDateController controller)
        {
            var response = await controller.AddOrUpdateAsync(callOffId, catalogueItemId, model);

            response.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task AddOrUpdateAsync_NotValid_ReturnsExpectedResponse(
            [Frozen] Mock<IDefaultDeliveryDateService> service,
            [Frozen] CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            Order order,
            DefaultDeliveryDateModel model,
            [Frozen] Mock<IDefaultDeliveryDateValidator> validator,
            ErrorsModel errors,
            DefaultDeliveryDateController controller)
        {
            service.Setup(o => o.GetOrder(callOffId, catalogueItemId)).ReturnsAsync(order);

            validator.Setup(v => v.Validate(model, order.CommencementDate)).Returns((false, errors));

            var response = await controller.AddOrUpdateAsync(callOffId, catalogueItemId, model);
            service.Verify(o => o.GetOrder(callOffId, catalogueItemId));

            response.Should().BeOfType<BadRequestObjectResult>();
            response.As<BadRequestObjectResult>().Value.Should().Be(errors);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task AddOrUpdateAsync_AddsDefaultDeliveryDate(
            [Frozen] Mock<IDefaultDeliveryDateService> service,
            [Frozen] CallOffId callOffId,
            [Frozen] CatalogueItemId catalogueItemId,
            Order order,
            DefaultDeliveryDateModel defaultDeliveryDate,
            [Frozen] Mock<IDefaultDeliveryDateValidator> validator,
            DefaultDeliveryDateController controller)
        {
            service.Setup(o => o.GetOrder(callOffId, catalogueItemId)).ReturnsAsync(order);
            service.Setup(o => o.SetDefaultDeliveryDate(callOffId, catalogueItemId, defaultDeliveryDate.DeliveryDate.Value)).Callback(() =>
            {
                order.SetDefaultDeliveryDate(catalogueItemId, defaultDeliveryDate.DeliveryDate!.Value);
            });

            validator.Setup(v => v.Validate(defaultDeliveryDate, order.CommencementDate)).Returns((true, null));

            order.DefaultDeliveryDates.Should().BeEmpty();

            await controller.AddOrUpdateAsync(callOffId, catalogueItemId, defaultDeliveryDate);
            service.Verify(o => o.GetOrder(callOffId, catalogueItemId));
            service.Verify(o => o.SetDefaultDeliveryDate(callOffId, catalogueItemId, defaultDeliveryDate.DeliveryDate.Value));

            var expectedDeliveryDate = new DefaultDeliveryDate
            {
                OrderId = callOffId.Id,
                CatalogueItemId = catalogueItemId,
                DeliveryDate = defaultDeliveryDate.DeliveryDate.GetValueOrDefault(),
            };

            order.DefaultDeliveryDates.Should().HaveCount(1);
            order.DefaultDeliveryDates.Should().Contain(expectedDeliveryDate);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task AddOrUpdateAsync_Add_ReturnsExpectedStatusCode(
            [Frozen] Mock<IDefaultDeliveryDateService> service,
            [Frozen] CallOffId callOffId,
            [Frozen] CatalogueItemId catalogueItemId,
            Order order,
            DefaultDeliveryDateModel defaultDeliveryDate,
            [Frozen] Mock<IDefaultDeliveryDateValidator> validator,
            DefaultDeliveryDateController controller)
        {
            service.Setup(o => o.GetOrder(callOffId, catalogueItemId)).ReturnsAsync(order);
            service.Setup(o => o.SetDefaultDeliveryDate(callOffId, catalogueItemId, defaultDeliveryDate.DeliveryDate.Value)).Callback(() =>
            {
                order.SetDefaultDeliveryDate(catalogueItemId, defaultDeliveryDate.DeliveryDate!.Value);
            });

            validator.Setup(v => v.Validate(defaultDeliveryDate, order.CommencementDate)).Returns((true, null));

            var response = await controller.AddOrUpdateAsync(callOffId, catalogueItemId, defaultDeliveryDate);
            service.Verify(o => o.GetOrder(callOffId, catalogueItemId));
            service.Verify(o => o.SetDefaultDeliveryDate(callOffId, catalogueItemId, defaultDeliveryDate.DeliveryDate.Value));

            response.Should().BeOfType<CreatedAtActionResult>();
            response.As<CreatedAtActionResult>().Should().BeEquivalentTo(new
            {
                ActionName = "Get",
                RouteValues = new RouteValueDictionary
                {
                    { nameof(callOffId), callOffId.ToString() },
                    { nameof(catalogueItemId), catalogueItemId.ToString() },
                },
            });
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task AddOrUpdateAsync_Update_ReturnsExpectedStatusCode(
            [Frozen] Mock<IDefaultDeliveryDateService> service,
            [Frozen] CallOffId callOffId,
            [Frozen] CatalogueItemId catalogueItemId,
            Order order,
            DefaultDeliveryDateModel defaultDeliveryDate,
            [Frozen] Mock<IDefaultDeliveryDateValidator> validator,
            DefaultDeliveryDateController controller)
        {
            service.Setup(o => o.GetOrder(callOffId, catalogueItemId)).ReturnsAsync(order);
            service.Setup(o => o.SetDefaultDeliveryDate(callOffId, catalogueItemId, defaultDeliveryDate.DeliveryDate.Value)).ReturnsAsync(DeliveryDateResult.Updated);

            validator.Setup(v => v.Validate(defaultDeliveryDate, order.CommencementDate)).Returns((true, null));

            var response = await controller.AddOrUpdateAsync(callOffId, catalogueItemId, defaultDeliveryDate);
            service.Verify(o => o.GetOrder(callOffId, catalogueItemId));
            service.Verify(o => o.SetDefaultDeliveryDate(callOffId, catalogueItemId, defaultDeliveryDate.DeliveryDate.Value));

            response.Should().BeOfType<OkResult>();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetAsync_ReturnsNotFound(
            [Frozen] Mock<IDefaultDeliveryDateService> service,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            DefaultDeliveryDateController controller)
        {
            service.Setup(o => o.GetDefaultDeliveryDate(callOffId, catalogueItemId)).ReturnsAsync((DateTime?)null);

            var response = await controller.GetAsync(callOffId, catalogueItemId);
            service.Verify(o => o.GetDefaultDeliveryDate(callOffId, catalogueItemId));

            response.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetAsync_ReturnsExpectedResult(
            [Frozen] Mock<IDefaultDeliveryDateService> service,
            [Frozen] CallOffId callOffId,
            [Frozen] CatalogueItemId catalogueItemId,
            DateTime defaultDeliveryDate,
            DefaultDeliveryDateController controller)
        {
            service.Setup(o => o.GetDefaultDeliveryDate(callOffId, catalogueItemId)).ReturnsAsync(defaultDeliveryDate);

            var response = await controller.GetAsync(callOffId, catalogueItemId);
            service.Verify(o => o.GetDefaultDeliveryDate(callOffId, catalogueItemId));

            var actualDate = response.Value;

            actualDate.Should().NotBeNull();
            actualDate.DeliveryDate.Should().Be(defaultDeliveryDate);
        }
    }
}
