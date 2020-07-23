using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
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
    internal sealed class CreateOrderItemServiceTests
    {
        [Test]
        public void Constructor_NullOrderRepository_ThrowsArgumentNullException()
        {
            static void Test()
            {
                CreateOrderItemServiceBuilder
                    .Create()
                    .WithOrderRepository(null)
                    .WithIdentityService(Mock.Of<IIdentityService>())
                    .WithValidator(Mock.Of<ICreateOrderItemValidator>())
                    .Build();
            }

            Assert.Throws<ArgumentNullException>(Test);
        }

        [Test]
        public void Constructor_NullIdentityService_ThrowsArgumentNullException()
        {
            static void Test()
            {
                CreateOrderItemServiceBuilder
                    .Create()
                    .WithOrderRepository(Mock.Of<IOrderRepository>())
                    .WithIdentityService(null)
                    .WithValidator(Mock.Of<ICreateOrderItemValidator>())
                    .Build();
            }

            Assert.Throws<ArgumentNullException>(Test);
        }

        [Test]
        public void Constructor_NullValidator_ThrowsArgumentNullException()
        {
            static void Test()
            {
                CreateOrderItemServiceBuilder
                    .Create()
                    .WithOrderRepository(Mock.Of<IOrderRepository>())
                    .WithIdentityService(Mock.Of<IIdentityService>())
                    .WithValidator(null)
                    .Build();
            }

            Assert.Throws<ArgumentNullException>(Test);
        }

        [Test]
        public void CreateAsync_NullCreateOrderItemRequest_ThrowsArgumentNullException()
        {
            static async Task Test()
            {
                var sut = CreateOrderItemServiceBuilder.Create().Build();
                await sut.CreateAsync(null);
            }

            Assert.ThrowsAsync<ArgumentNullException>(Test);
        }

        [Test]
        public async Task CreateAsync_ValidatorReturnsErrors_ReturnsFailure()
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
        public async Task CreateAsync_CreateOrderItemRequest_ReturnsSuccess()
        {
            var context = CreateOrderItemServiceTestContext.Setup();

            var createOrderItemRequest = CreateOrderItemRequestBuilder
                .Create()
                .Build();

            var actual = await context.CreateOrderItemService.CreateAsync(createOrderItemRequest);

            var expected = context.Order.OrderItems.First().OrderItemId;
            actual.Should().Be(Result.Success(expected));
        }

        [TestCase(nameof(CatalogueItemType.Solution), nameof(ProvisioningType.OnDemand), "year")]
        [TestCase(nameof(CatalogueItemType.Solution), nameof(ProvisioningType.Patient), "month")]
        [TestCase(nameof(CatalogueItemType.Solution), nameof(ProvisioningType.Declarative), "year")]
        [TestCase(nameof(CatalogueItemType.AdditionalService), nameof(ProvisioningType.OnDemand), "year")]
        [TestCase(nameof(CatalogueItemType.AdditionalService), nameof(ProvisioningType.Patient), "month")]
        [TestCase(nameof(CatalogueItemType.AdditionalService), nameof(ProvisioningType.Declarative), "year")]
        [TestCase(nameof(CatalogueItemType.AssociatedService), nameof(ProvisioningType.OnDemand), "year")]
        [TestCase(nameof(CatalogueItemType.AssociatedService), nameof(ProvisioningType.Declarative), null)]
        public async Task CreateAsync_MapCreateOrderItemRequestToOrderItem_AreEqual(
            string catalogueItemTypeNameInput, 
            string provisioningTypeNameInput,
            string expectedEstimationPeriodNameInput)
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
                ProvisioningType = ProvisioningType.FromName(createOrderItemRequest.ProvisioningTypeName),
                CataloguePriceType = CataloguePriceType.FromName(createOrderItemRequest.CataloguePriceTypeName),
                CataloguePriceUnit = CataloguePriceUnit.Create(
                    createOrderItemRequest.CataloguePriceUnitTierName, 
                    createOrderItemRequest.CataloguePriceUnitDescription),
                PriceTimeUnit = TimeUnit.FromName(createOrderItemRequest.PriceTimeUnitName),
                createOrderItemRequest.CurrencyCode,
                createOrderItemRequest.Quantity,
                EstimationPeriod = TimeUnit.FromName(expectedEstimationPeriodNameInput),
                createOrderItemRequest.DeliveryDate,
                createOrderItemRequest.Price
            });
        }

        [Test]
        public async Task CreateAsync_OrderRepository_CalledOnce()
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
        public async Task CreateAsync_OrderItemValidator_CalledOnce()
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
        public async Task CreateAsync_IdentityService_CalledTwice()
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

        [TestCase(nameof(CatalogueItemType.Solution), null)]
        [TestCase(nameof(CatalogueItemType.Solution), "month")]
        [TestCase(nameof(CatalogueItemType.Solution), "year")]
        [TestCase(nameof(CatalogueItemType.AdditionalService), null)]
        [TestCase(nameof(CatalogueItemType.AdditionalService), "month")]
        [TestCase(nameof(CatalogueItemType.AdditionalService), "year")]
        [TestCase(nameof(CatalogueItemType.AssociatedService), null)]
        [TestCase(nameof(CatalogueItemType.AssociatedService), "month")]
        [TestCase(nameof(CatalogueItemType.AssociatedService), "year")]
        public async Task CreateAsync_ProvisioningType_OnDemand_OrderItemAddedWithExpectedEstimationPeriod(
            string catalogueItemTypeNameInput,
            string inputEstimationPeriod)
        {
            var context = CreateOrderItemServiceTestContext.Setup();

            var createOrderItemRequest = CreateOrderItemRequestBuilder
                .Create()
                .WithCatalogueItemType(CatalogueItemType.FromName(catalogueItemTypeNameInput))
                .WithProvisioningTypeName(ProvisioningType.OnDemand.Name)
                .WithEstimationPeriodName(inputEstimationPeriod)
                .Build();

            await context.CreateOrderItemService.CreateAsync(createOrderItemRequest);

            var orderItem = createOrderItemRequest.Order.OrderItems.First();

            var expectedEstimationPeriod = TimeUnit.FromName(inputEstimationPeriod);
            orderItem.EstimationPeriod.Should().Be(expectedEstimationPeriod);
        }

        [TestCase(nameof(CatalogueItemType.Solution))]
        [TestCase(nameof(CatalogueItemType.AdditionalService))]
        public async Task CreateAsync_ProvisioningType_Patient_OrderItemAddedWithEstimationPeriod_PerMonth(
            string catalogueItemTypeNameInput)
        {
            var context = CreateOrderItemServiceTestContext.Setup();

            var createOrderItemRequest = CreateOrderItemRequestBuilder
                .Create()
                .WithCatalogueItemType(CatalogueItemType.FromName(catalogueItemTypeNameInput))
                .WithProvisioningTypeName(ProvisioningType.Patient.Name)
                .WithEstimationPeriodName(null)
                .Build();

            await context.CreateOrderItemService.CreateAsync(createOrderItemRequest);

            var orderItem = createOrderItemRequest.Order.OrderItems.First();
            orderItem.EstimationPeriod.Should().Be(TimeUnit.PerMonth);
        }

        [TestCase(nameof(CatalogueItemType.Solution), "year")]
        [TestCase(nameof(CatalogueItemType.AdditionalService), "year")]
        [TestCase(nameof(CatalogueItemType.AssociatedService), null)]
        public async Task CreateAsync_ProvisioningType_Declarative_OrderItemAddedWithEstimationPeriod_PerYear(
            string catalogueItemTypeNameInput,
            string expectedEstimationPeriodNameInput)
        {
            var context = CreateOrderItemServiceTestContext.Setup();

            var createOrderItemRequest = CreateOrderItemRequestBuilder
                .Create()
                .WithCatalogueItemType(CatalogueItemType.FromName(catalogueItemTypeNameInput))
                .WithProvisioningTypeName(ProvisioningType.Declarative.Name)
                .WithEstimationPeriodName(null)
                .Build();

            await context.CreateOrderItemService.CreateAsync(createOrderItemRequest);

            var orderItem = createOrderItemRequest.Order.OrderItems.First();
            orderItem.EstimationPeriod.Should().Be(TimeUnit.FromName(expectedEstimationPeriodNameInput));
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
