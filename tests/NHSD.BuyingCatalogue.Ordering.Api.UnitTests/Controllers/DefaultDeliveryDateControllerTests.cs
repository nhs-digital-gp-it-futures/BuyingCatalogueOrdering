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
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;
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
            [Frozen] ApplicationDbContext context,
            [Frozen] CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            Order order,
            DefaultDeliveryDateModel model,
            [Frozen] Mock<IDefaultDeliveryDateValidator> validator,
            ErrorsModel errors,
            DefaultDeliveryDateController controller)
        {
            context.Add(order);
            await context.SaveChangesAsync();

            validator.Setup(v => v.Validate(model, order.CommencementDate)).Returns((false, errors));

            var response = await controller.AddOrUpdateAsync(callOffId, catalogueItemId, model);

            response.Should().BeOfType<BadRequestObjectResult>();
            response.As<BadRequestObjectResult>().Value.Should().Be(errors);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task AddOrUpdateAsync_AddsDefaultDeliveryDate(
            [Frozen] ApplicationDbContext context,
            [Frozen] CallOffId callOffId,
            [Frozen] CatalogueItemId catalogueItemId,
            OrderItem item,
            Order order,
            DefaultDeliveryDateModel defaultDeliveryDate,
            [Frozen] Mock<IDefaultDeliveryDateValidator> validator,
            DefaultDeliveryDateController controller)
        {
            order.AddOrUpdateOrderItem(item);
            context.Add(order);
            await context.SaveChangesAsync();

            validator.Setup(v => v.Validate(defaultDeliveryDate, order.CommencementDate)).Returns((true, null));

            order.DefaultDeliveryDates.Should().BeEmpty();

            await controller.AddOrUpdateAsync(callOffId, catalogueItemId, defaultDeliveryDate);

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
        public static async Task AddOrUpdateAsync_SavesToDb(
            [Frozen] ApplicationDbContext context,
            [Frozen] CallOffId callOffId,
            [Frozen] CatalogueItemId catalogueItemId,
            OrderItem item,
            Order order,
            DefaultDeliveryDateModel defaultDeliveryDate,
            [Frozen] Mock<IDefaultDeliveryDateValidator> validator,
            DefaultDeliveryDateController controller)
        {
            order.AddOrUpdateOrderItem(item);
            context.Add(order);
            await context.SaveChangesAsync();

            validator.Setup(v => v.Validate(defaultDeliveryDate, order.CommencementDate)).Returns((true, null));

            context.Set<DefaultDeliveryDate>().Should().BeEmpty();

            await controller.AddOrUpdateAsync(callOffId, catalogueItemId, defaultDeliveryDate);

            var expectedDeliveryDate = new DefaultDeliveryDate
            {
                OrderId = callOffId.Id,
                CatalogueItemId = catalogueItemId,
                DeliveryDate = defaultDeliveryDate.DeliveryDate.GetValueOrDefault(),
            };

            context.Set<DefaultDeliveryDate>().Should().HaveCount(1);
            context.Set<DefaultDeliveryDate>().Should().Contain(expectedDeliveryDate);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task AddOrUpdateAsync_Add_ReturnsExpectedStatusCode(
            [Frozen] ApplicationDbContext context,
            [Frozen] CallOffId callOffId,
            [Frozen] CatalogueItemId catalogueItemId,
            OrderItem item,
            Order order,
            DefaultDeliveryDateModel defaultDeliveryDate,
            [Frozen] Mock<IDefaultDeliveryDateValidator> validator,
            DefaultDeliveryDateController controller)
        {
            order.AddOrUpdateOrderItem(item);
            context.Add(order);
            await context.SaveChangesAsync();

            validator.Setup(v => v.Validate(defaultDeliveryDate, order.CommencementDate)).Returns((true, null));

            var response = await controller.AddOrUpdateAsync(callOffId, catalogueItemId, defaultDeliveryDate);

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
            [Frozen] ApplicationDbContext context,
            [Frozen] CallOffId callOffId,
            [Frozen] CatalogueItemId catalogueItemId,
            OrderItem item,
            Order order,
            DefaultDeliveryDateModel defaultDeliveryDate,
            [Frozen] Mock<IDefaultDeliveryDateValidator> validator,
            DefaultDeliveryDateController controller)
        {
            order.AddOrUpdateOrderItem(item);
            order.SetDefaultDeliveryDate(catalogueItemId, DateTime.UtcNow);
            context.Add(order);
            await context.SaveChangesAsync();

            validator.Setup(v => v.Validate(defaultDeliveryDate, order.CommencementDate)).Returns((true, null));

            var response = await controller.AddOrUpdateAsync(callOffId, catalogueItemId, defaultDeliveryDate);

            response.Should().BeOfType<OkResult>();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetAsync_NotFound_ReturnsExpectedStatusCode(
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            DefaultDeliveryDateController controller)
        {
            var response = await controller.GetAsync(callOffId, catalogueItemId);

            response.Should().NotBeNull();
            response.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetAsync_ReturnsExpectedResult(
            [Frozen] ApplicationDbContext context,
            [Frozen] CallOffId callOffId,
            [Frozen] CatalogueItemId catalogueItemId,
            DateTime defaultDeliveryDate,
            OrderItem item,
            Order order,
            DefaultDeliveryDateController controller)
        {
            order.AddOrUpdateOrderItem(item);
            order.SetDefaultDeliveryDate(catalogueItemId, defaultDeliveryDate);
            context.Add(order);
            await context.SaveChangesAsync();

            var response = await controller.GetAsync(callOffId, catalogueItemId);
            var actualDate = response.Value;

            actualDate.Should().NotBeNull();
            actualDate.DeliveryDate.Should().Be(defaultDeliveryDate);
        }
    }
}
