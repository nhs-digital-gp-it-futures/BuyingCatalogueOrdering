using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Api.Validation;
using NHSD.BuyingCatalogue.Ordering.Contracts;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;
using NHSD.BuyingCatalogue.Ordering.Services;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class CreateOrderItemServiceTests
    {
        [Test]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(CreateOrderItemService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Test]
        [InMemoryDbAutoData]
        public static void Constructor_NullDbContext_ThrowsArgumentNullException(
            OrderItemValidator orderItemValidator,
            ServiceRecipientService serviceRecipientService)
        {
            Assert.Throws<ArgumentNullException>(() => _ = new CreateOrderItemService(null, orderItemValidator, serviceRecipientService));
        }

        [Test]
        [InMemoryDbAutoData]
        public static void Constructor_NullOrderItemValidator_ThrowsArgumentNullException(
            ApplicationDbContext context,
            ServiceRecipientService serviceRecipientService)
        {
            Assert.Throws<ArgumentNullException>(() => _ = new CreateOrderItemService(context, null, serviceRecipientService));
        }

        [Test]
        [InMemoryDbAutoData]
        public static void Constructor_NullServiceRecipient_ThrowsArgumentNullException(
            ApplicationDbContext context,
            OrderItemValidator orderItemValidator)
        {
            Assert.Throws<ArgumentNullException>(() => _ = new CreateOrderItemService(context, orderItemValidator, null));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task CreateAsync_InvalidAggregateValidation_SuccessIsFalse(
            [Frozen] Mock<ICreateOrderItemValidator> orderItemValidator,
            Order order,
            ErrorDetails error,
            CatalogueItemId catalogueItemId,
            CreateOrderItemModel model,
            CreateOrderItemService service)
        {
            var aggregateValidationResult = new AggregateValidationResult();
            aggregateValidationResult.AddValidationResult(new ValidationResult(error), 0);

            var catalogueItemType = Enum.Parse<CatalogueItemType>(model.CatalogueItemType, true);

            orderItemValidator.Setup(o => o.Validate(order, model, catalogueItemType)).Returns(aggregateValidationResult);

            var result = await service.CreateAsync(order, catalogueItemId, model);
            result.Success.Should().BeFalse();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task CreateAsync_ValidAggregateValidation_SuccessIsTrue(
            [Frozen] Mock<ICreateOrderItemValidator> orderItemValidator,
            Order order,
            CatalogueItemId catalogueItemId,
            CreateOrderItemModel model,
            CreateOrderItemService service)
        {
            var aggregateValidationResult = new AggregateValidationResult();
            aggregateValidationResult.AddValidationResult(new ValidationResult(), 0);

            var catalogueItemType = Enum.Parse<CatalogueItemType>(model.CatalogueItemType, true);

            orderItemValidator.Setup(o => o.Validate(order, model, catalogueItemType)).Returns(aggregateValidationResult);

            var result = await service.CreateAsync(order, catalogueItemId, model);
            result.Success.Should().BeTrue();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task CreateAsync_SetsEstimationPeriod(
            [Frozen] ApplicationDbContext context,
            [Frozen] Mock<ICreateOrderItemValidator> orderItemValidator,
            [Frozen] Mock<IServiceRecipientService> serviceRecipientService,
            Order order,
            CatalogueItemId catalogueItemId,
            CreateOrderItemModel model,
            CreateOrderItemService service)
        {
            context.Order.Add(order);
            await context.SaveChangesAsync();

            var aggregateValidationResult = new AggregateValidationResult();
            aggregateValidationResult.AddValidationResult(new ValidationResult(), 0);

            var catalogueItemType = Enum.Parse<CatalogueItemType>(model.CatalogueItemType, true);

            var provisioningType = Enum.Parse<ProvisioningType>(model.ProvisioningType, true);

            var serviceRecipients = model.ServiceRecipients.Select(r => new ServiceRecipient(r.OdsCode, r.Name));
            orderItemValidator.Setup(o => o.Validate(order, model, catalogueItemType)).Returns(aggregateValidationResult);
            serviceRecipientService.Setup(r => r.AddOrUpdateServiceRecipients(It.IsAny<IEnumerable<ServiceRecipient>>()))
                .ReturnsAsync(serviceRecipients.ToDictionary(r => r.OdsCode));

            await service.CreateAsync(order, catalogueItemId, model);

            var expectedEstimationPeriod = catalogueItemType.InferEstimationPeriod(
                provisioningType,
                OrderingEnums.ParseTimeUnit(model.EstimationPeriod));

            var expectedOrderItem = context.Set<OrderItem>().First(o => o.OrderId.Equals(order.Id));

            expectedOrderItem.EstimationPeriod.Should().Be(expectedEstimationPeriod);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task CreateAsync_RemovesDefaultDeliveryDate(
            [Frozen] ApplicationDbContext context,
            [Frozen] Mock<ICreateOrderItemValidator> orderItemValidator,
            [Frozen] Mock<IServiceRecipientService> serviceRecipientService,
            Order order,
            CatalogueItemId catalogueItemId,
            CreateOrderItemModel model,
            CreateOrderItemService service)
        {
            order.SetDefaultDeliveryDate(catalogueItemId, DateTime.Now.AddDays(10));
            context.Order.Add(order);
            await context.SaveChangesAsync();

            order.DefaultDeliveryDates.Should().NotBeEmpty();

            var aggregateValidationResult = new AggregateValidationResult();
            aggregateValidationResult.AddValidationResult(new ValidationResult(), 0);

            var catalogueItemType = Enum.Parse<CatalogueItemType>(model.CatalogueItemType, true);

            var serviceRecipients = model.ServiceRecipients.Select(r => new ServiceRecipient(r.OdsCode, r.Name));
            orderItemValidator.Setup(o => o.Validate(order, model, catalogueItemType)).Returns(aggregateValidationResult);
            serviceRecipientService.Setup(r => r.AddOrUpdateServiceRecipients(It.IsAny<IEnumerable<ServiceRecipient>>()))
                .ReturnsAsync(serviceRecipients.ToDictionary(r => r.OdsCode));

            await service.CreateAsync(order, catalogueItemId, model);

            var defaultDeliveryDateResult = context.Set<DefaultDeliveryDate>().FirstOrDefault(d => d.OrderId.Equals(order.Id));

            defaultDeliveryDateResult.Should().BeNull();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task CreateAsync_OrderItemsSavedToDb(
            [Frozen] ApplicationDbContext context,
            [Frozen] Mock<ICreateOrderItemValidator> orderItemValidator,
            [Frozen] Mock<IServiceRecipientService> serviceRecipientService,
            Order order,
            CatalogueItemId catalogueItemId,
            CreateOrderItemModel model,
            CreateOrderItemService service)
        {
            context.Order.Add(order);
            await context.SaveChangesAsync();

            order.OrderItems.Should().BeEmpty();

            var aggregateValidationResult = new AggregateValidationResult();
            aggregateValidationResult.AddValidationResult(new ValidationResult(), 0);

            var catalogueItemType = Enum.Parse<CatalogueItemType>(model.CatalogueItemType, true);

            var serviceRecipients = model.ServiceRecipients.Select(r => new ServiceRecipient(r.OdsCode, r.Name));

            orderItemValidator.Setup(o => o.Validate(order, model, catalogueItemType)).Returns(aggregateValidationResult);
            serviceRecipientService.Setup(r => r.AddOrUpdateServiceRecipients(It.IsAny<IEnumerable<ServiceRecipient>>()))
                .ReturnsAsync(serviceRecipients.ToDictionary(r => r.OdsCode));

            await service.CreateAsync(order, catalogueItemId, model);

            var expectedOrder = context.Set<Order>().First(o => o.Equals(order));

            expectedOrder.OrderItems.Should().NotBeEmpty();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task CreateAsync_PricingUnitSavedToDb(
            [Frozen] ApplicationDbContext context,
            [Frozen] Mock<ICreateOrderItemValidator> orderItemValidator,
            [Frozen] Mock<IServiceRecipientService> serviceRecipientService,
            Order order,
            CatalogueItemId catalogueItemId,
            CreateOrderItemModel model,
            CreateOrderItemService service)
        {
            context.Order.Add(order);
            await context.SaveChangesAsync();

            var aggregateValidationResult = new AggregateValidationResult();
            aggregateValidationResult.AddValidationResult(new ValidationResult(), 0);

            var catalogueItemType = Enum.Parse<CatalogueItemType>(model.CatalogueItemType, true);

            var serviceRecipients = model.ServiceRecipients.Select(r => new ServiceRecipient(r.OdsCode, r.Name));

            orderItemValidator.Setup(o => o.Validate(order, model, catalogueItemType)).Returns(aggregateValidationResult);
            serviceRecipientService.Setup(r => r.AddOrUpdateServiceRecipients(It.IsAny<IEnumerable<ServiceRecipient>>()))
                .ReturnsAsync(serviceRecipients.ToDictionary(r => r.OdsCode));

            await service.CreateAsync(order, catalogueItemId, model);

            var expectedPricingUnit = await context.FindAsync<PricingUnit>(model.ItemUnit.Name);

            expectedPricingUnit.Name.Should().Be(model.ItemUnit.Name);
            expectedPricingUnit.Description.Should().Be(model.ItemUnit.Description);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task CreateAsync_CatalogueItemSavedToDb(
            [Frozen] ApplicationDbContext context,
            [Frozen] Mock<ICreateOrderItemValidator> orderItemValidator,
            [Frozen] Mock<IServiceRecipientService> serviceRecipientService,
            Order order,
            CatalogueItemId catalogueItemId,
            CreateOrderItemModel model,
            CreateOrderItemService service)
        {
            context.Order.Add(order);
            await context.SaveChangesAsync();

            var aggregateValidationResult = new AggregateValidationResult();
            aggregateValidationResult.AddValidationResult(new ValidationResult(), 0);

            var catalogueItemType = Enum.Parse<CatalogueItemType>(model.CatalogueItemType, true);

            var serviceRecipients = model.ServiceRecipients.Select(r => new ServiceRecipient(r.OdsCode, r.Name));

            orderItemValidator.Setup(o => o.Validate(order, model, catalogueItemType)).Returns(aggregateValidationResult);
            serviceRecipientService.Setup(r => r.AddOrUpdateServiceRecipients(It.IsAny<IEnumerable<ServiceRecipient>>()))
                .ReturnsAsync(serviceRecipients.ToDictionary(r => r.OdsCode));

            await service.CreateAsync(order, catalogueItemId, model);

            var expectedOrderItem = context.Set<OrderItem>().First(o => o.CatalogueItem.Id.Equals(catalogueItemId));

            expectedOrderItem.CatalogueItem.Name.Should().Be(model.CatalogueItemName);
        }
    }
}
