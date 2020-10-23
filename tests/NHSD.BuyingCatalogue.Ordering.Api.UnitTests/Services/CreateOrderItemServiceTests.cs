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
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders.Services;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Application.Services;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class CreateOrderItemServiceTests
    {
        [Test]
        public static void Contructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(CreateOrderItemService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Test]
        public static void CreateAsync_NullCreateOrderItemRequest_ThrowsArgumentNullException()
        {
            var sut = CreateOrderItemServiceBuilder.Create().Build();

            Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.CreateAsync(null));
        }

        [Test]
        public static async Task CreateAsync_ValidatorReturnsErrors_ReturnsFailure()
        {
            var context = CreateOrderItemServiceTestContext.Setup();
            var expectedErrors = new List<ErrorDetails>
                {
                    new ErrorDetails("Error1", "ErrorField"), new ErrorDetails("Error2", "ErrorField")
                };

            var createOrderItemRequest = CreateOrderItemRequestBuilder
                .Create()
                .Build();

            context.OrderItemValidatorMock.Setup(x => x.Validate(createOrderItemRequest)).Returns(
                expectedErrors);

            var actual = await context.CreateOrderItemService.CreateAsync(createOrderItemRequest);
            actual.Should().Be(Result.Failure<int>(expectedErrors));
        }

        [Test]
        public static async Task CreateAsync_CreateOrderItemRequest_ReturnsSuccess()
        {
            var context = CreateOrderItemServiceTestContext.Setup();

            var createOrderItemRequest = CreateOrderItemRequestBuilder
                .Create()
                .Build();

            var actual = await context.CreateOrderItemService.CreateAsync(createOrderItemRequest);

            var expected = context.Order.OrderItems.First().OrderItemId;
            actual.Should().Be(Result.Success(expected));
        }

        [TestCase(nameof(CatalogueItemType.Solution), nameof(ProvisioningType.OnDemand), TimeUnit.PerYear)]
        [TestCase(nameof(CatalogueItemType.Solution), nameof(ProvisioningType.Patient), TimeUnit.PerMonth)]
        [TestCase(nameof(CatalogueItemType.Solution), nameof(ProvisioningType.Declarative), TimeUnit.PerYear)]
        [TestCase(nameof(CatalogueItemType.AdditionalService), nameof(ProvisioningType.OnDemand), TimeUnit.PerYear)]
        [TestCase(nameof(CatalogueItemType.AdditionalService), nameof(ProvisioningType.Patient), TimeUnit.PerMonth)]
        [TestCase(nameof(CatalogueItemType.AdditionalService), nameof(ProvisioningType.Declarative), TimeUnit.PerYear)]
        [TestCase(nameof(CatalogueItemType.AssociatedService), nameof(ProvisioningType.OnDemand), TimeUnit.PerYear)]
        [TestCase(nameof(CatalogueItemType.AssociatedService), nameof(ProvisioningType.Declarative), null)]
        public static async Task CreateAsync_MapCreateOrderItemRequestToOrderItem_AreEqual(
            string catalogueItemTypeNameInput,
            string provisioningTypeNameInput,
            TimeUnit? expectedEstimationPeriodNameInput)
        {
            var context = CreateOrderItemServiceTestContext.Setup();

            var createOrderItemRequest = CreateOrderItemRequestBuilder
                .Create()
                .WithCatalogueItemType(CatalogueItemType.FromName(catalogueItemTypeNameInput))
                .WithProvisioningTypeName(provisioningTypeNameInput)
                .Build();

            await context.CreateOrderItemService.CreateAsync(createOrderItemRequest);

            var actual = context.Order.OrderItems.First();
            actual.Should().BeEquivalentTo(new
            {
                createOrderItemRequest.OdsCode,
                createOrderItemRequest.CatalogueItemId,
                createOrderItemRequest.CatalogueItemName,
                ParentCatalogueItemId = createOrderItemRequest.CatalogueSolutionId,
                ProvisioningType = createOrderItemRequest.ProvisioningType.Value,
                createOrderItemRequest.CataloguePriceType,
                CataloguePriceUnit = CataloguePriceUnit.Create(
                    createOrderItemRequest.CataloguePriceUnitTierName,
                    createOrderItemRequest.CataloguePriceUnitDescription),
                createOrderItemRequest.PriceTimeUnit,
                createOrderItemRequest.CurrencyCode,
                createOrderItemRequest.Quantity,
                EstimationPeriod = expectedEstimationPeriodNameInput,
                createOrderItemRequest.DeliveryDate,
                createOrderItemRequest.Price
            });
        }

        [Test]
        public static async Task CreateAsync_OrderRepository_CalledOnce()
        {
            var context = CreateOrderItemServiceTestContext.Setup();

            var createOrderItemRequest = CreateOrderItemRequestBuilder
                .Create()
                .Build();

            await context.CreateOrderItemService.CreateAsync(createOrderItemRequest);

            context.OrderRepositoryMock.Verify(orderRepository =>
                orderRepository.UpdateOrderAsync(createOrderItemRequest.Order), Times.Once);
        }

        [Test]
        public static async Task CreateAsync_OrderItemValidator_CalledOnce()
        {
            var context = CreateOrderItemServiceTestContext.Setup();

            var createOrderItemRequest = CreateOrderItemRequestBuilder
                .Create()
                .Build();

            await context.CreateOrderItemService.CreateAsync(createOrderItemRequest);

            context.OrderItemValidatorMock.Verify(validator =>
                validator.Validate(createOrderItemRequest), Times.Once);
        }

        [Test]
        public static async Task CreateAsync_IdentityService_CalledTwice()
        {
            var context = CreateOrderItemServiceTestContext.Setup();

            var createOrderItemRequest = CreateOrderItemRequestBuilder
                .Create()
                .Build();

            await context.CreateOrderItemService.CreateAsync(createOrderItemRequest);

            context.IdentityServiceMock.Verify(identityService =>
                identityService.GetUserName(), Times.Once);

            context.IdentityServiceMock.Verify(identityService =>
                identityService.GetUserIdentity(), Times.Once);
        }

        [TestCase(nameof(CatalogueItemType.Solution), null, null)]
        [TestCase(nameof(CatalogueItemType.Solution), "month", TimeUnit.PerMonth)]
        [TestCase(nameof(CatalogueItemType.Solution), "year", TimeUnit.PerYear)]
        [TestCase(nameof(CatalogueItemType.AdditionalService), null, null)]
        [TestCase(nameof(CatalogueItemType.AdditionalService), "month", TimeUnit.PerMonth)]
        [TestCase(nameof(CatalogueItemType.AdditionalService), "year", TimeUnit.PerYear)]
        [TestCase(nameof(CatalogueItemType.AssociatedService), null, null)]
        [TestCase(nameof(CatalogueItemType.AssociatedService), "month", TimeUnit.PerMonth)]
        [TestCase(nameof(CatalogueItemType.AssociatedService), "year", TimeUnit.PerYear)]
        public static async Task CreateAsync_ProvisioningType_OnDemand_OrderItemAddedWithExpectedEstimationPeriod(
            string catalogueItemTypeNameInput,
            string inputEstimationPeriod,
            TimeUnit? expectedEstimationPeriod)
        {
            var context = CreateOrderItemServiceTestContext.Setup();

            var createOrderItemRequest = CreateOrderItemRequestBuilder
                .Create()
                .WithCatalogueItemType(CatalogueItemType.FromName(catalogueItemTypeNameInput))
                .WithProvisioningTypeName(ProvisioningType.OnDemand.ToString())
                .WithEstimationPeriodName(inputEstimationPeriod)
                .Build();

            await context.CreateOrderItemService.CreateAsync(createOrderItemRequest);

            var orderItem = createOrderItemRequest.Order.OrderItems.First();

            orderItem.EstimationPeriod.Should().Be(expectedEstimationPeriod);
        }

        [TestCase(nameof(CatalogueItemType.Solution))]
        [TestCase(nameof(CatalogueItemType.AdditionalService))]
        public static async Task CreateAsync_ProvisioningType_Patient_OrderItemAddedWithEstimationPeriod_PerMonth(
            string catalogueItemTypeNameInput)
        {
            var context = CreateOrderItemServiceTestContext.Setup();

            var createOrderItemRequest = CreateOrderItemRequestBuilder
                .Create()
                .WithCatalogueItemType(CatalogueItemType.FromName(catalogueItemTypeNameInput))
                .WithProvisioningTypeName(ProvisioningType.Patient.ToString())
                .WithEstimationPeriodName(null)
                .Build();

            await context.CreateOrderItemService.CreateAsync(createOrderItemRequest);

            var orderItem = createOrderItemRequest.Order.OrderItems.First();
            orderItem.EstimationPeriod.Should().Be(TimeUnit.PerMonth);
        }

        [TestCase(nameof(CatalogueItemType.Solution), TimeUnit.PerYear)]
        [TestCase(nameof(CatalogueItemType.AdditionalService), TimeUnit.PerYear)]
        [TestCase(nameof(CatalogueItemType.AssociatedService), null)]
        public static async Task CreateAsync_ProvisioningType_Declarative_OrderItemAddedWithEstimationPeriod_PerYear(
            string catalogueItemTypeNameInput,
            TimeUnit? expectedEstimationPeriodNameInput)
        {
            var context = CreateOrderItemServiceTestContext.Setup();

            var createOrderItemRequest = CreateOrderItemRequestBuilder
                .Create()
                .WithCatalogueItemType(CatalogueItemType.FromName(catalogueItemTypeNameInput))
                .WithProvisioningTypeName(ProvisioningType.Declarative.ToString())
                .WithEstimationPeriodName(null)
                .Build();

            await context.CreateOrderItemService.CreateAsync(createOrderItemRequest);

            var orderItem = createOrderItemRequest.Order.OrderItems.First();
            orderItem.EstimationPeriod.Should().Be(expectedEstimationPeriodNameInput);
        }

        [Test]
        [OrderingAutoData]
        public static void CreateAsync_IEnumerable_CreateOrderItemRequest_NullOrder_ThrowsException(
            IEnumerable<CreateOrderItemRequest> requests,
            CreateOrderItemService service)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => service.CreateAsync(null, requests));
        }

        [Test]
        [OrderingAutoData]
        public static void CreateAsync_IEnumerable_CreateOrderItemRequest_NullRequests_ThrowsException(
            Order order,
            CreateOrderItemService service)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => service.CreateAsync(order, null));
        }

        [Test]
        [OrderingAutoData]
        public static async Task CreateAsync_Order_IEnumerable_CreateOrderItemRequest_AddsExpectedOrderItemToOrder(
            [Frozen] Mock<IOrderItemFactory> orderItemFactory,
            [Frozen] Mock<IIdentityService> identityService,
            string userName,
            Order order,
            OrderItem item,
            CreateOrderItemRequest request,
            CreateOrderItemService service)
        {
            identityService.Setup(i => i.GetUserName()).Returns(userName);
            orderItemFactory.Setup(f => f.Create(request)).Returns(item);

            await service.CreateAsync(order, new[] { request });

            order.OrderItems.Should().HaveCount(1);
            order.OrderItems.Should().Contain(item);
        }

        [Test]
        [OrderingAutoData]
        public static async Task CreateAsync_Order_IEnumerable_CreateOrderItemRequest_UsesExpectedUserId(
            [Frozen] Mock<IOrderItemFactory> orderItemFactory,
            [Frozen] Mock<IIdentityService> identityService,
            Guid userId,
            string userName,
            Order order,
            OrderItem item,
            CreateOrderItemRequest request,
            CreateOrderItemService service)
        {
            identityService.Setup(i => i.GetUserIdentity()).Returns(userId);
            identityService.Setup(i => i.GetUserName()).Returns(userName);
            orderItemFactory.Setup(f => f.Create(request)).Returns(item);

            await service.CreateAsync(order, new[] { request });

            order.LastUpdatedBy.Should().Be(userId);
        }

        [Test]
        [OrderingAutoData]
        public static async Task CreateAsync_Order_IEnumerable_CreateOrderItemRequest_UsesExpectedUserName(
            [Frozen] Mock<IOrderItemFactory> orderItemFactory,
            [Frozen] Mock<IIdentityService> identityService,
            string userName,
            Order order,
            OrderItem item,
            CreateOrderItemRequest request,
            CreateOrderItemService service)
        {
            identityService.Setup(i => i.GetUserName()).Returns(userName);
            orderItemFactory.Setup(f => f.Create(request)).Returns(item);

            await service.CreateAsync(order, new[] { request });

            order.LastUpdatedByName.Should().Be(userName);
        }

        [Test]
        [OrderingAutoData]
        public static async Task CreateAsync_Order_IEnumerable_CreateOrderItemRequest_InvokesUpdateOrderAsync(
            [Frozen] Mock<IOrderItemFactory> orderItemFactory,
            [Frozen] Mock<IIdentityService> identityService,
            [Frozen] Mock<IOrderRepository> repository,
            string userName,
            Order order,
            OrderItem item,
            CreateOrderItemRequest request,
            CreateOrderItemService service)
        {
            identityService.Setup(i => i.GetUserName()).Returns(userName);
            orderItemFactory.Setup(f => f.Create(request)).Returns(item);

            await service.CreateAsync(order, new[] { request });

            repository.Verify(r => r.UpdateOrderAsync(order));
        }

        private sealed class CreateOrderItemServiceTestContext
        {
            private CreateOrderItemServiceTestContext()
            {
                UserId = Guid.NewGuid();
                UserName = "Bob";

                OrderRepositoryMock = new Mock<IOrderRepository>();
                OrderRepositoryMock.Setup(x => x.UpdateOrderAsync(It.IsAny<Order>())).Callback<Order>(x => Order = x);

                IdentityServiceMock = new Mock<IIdentityService>();
                IdentityServiceMock.Setup(identityService => identityService.GetUserIdentity()).Returns(() => UserId);
                IdentityServiceMock.Setup(identityService => identityService.GetUserName()).Returns(() => UserName);

                OrderItemValidatorMock = new Mock<ICreateOrderItemValidator>();

                CreateOrderItemService = CreateOrderItemServiceBuilder
                    .Create()
                    .WithOrderRepository(OrderRepositoryMock.Object)
                    .WithIdentityService(IdentityServiceMock.Object)
                    .WithValidator(OrderItemValidatorMock.Object)
                    .Build();
            }

            internal ICreateOrderItemService CreateOrderItemService { get; }

            internal Mock<IOrderRepository> OrderRepositoryMock { get; }

            internal Mock<IIdentityService> IdentityServiceMock { get; }

            internal Mock<ICreateOrderItemValidator> OrderItemValidatorMock { get; }

            internal Order Order { get; private set; }

            private string UserName { get; }

            private Guid UserId { get; }

            internal static CreateOrderItemServiceTestContext Setup() =>
                new CreateOrderItemServiceTestContext();
        }
    }
}
