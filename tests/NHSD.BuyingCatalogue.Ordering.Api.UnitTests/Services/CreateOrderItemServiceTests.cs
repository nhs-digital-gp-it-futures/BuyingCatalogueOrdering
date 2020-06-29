using System;
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

        [Test]
        public async Task CreateAsync_MapCreateOrderItemRequestToOrderItem_AreEqual()
        {
            var context = CreateOrderItemServiceTestContext.Setup();

            var createOrderItemRequest = CreateOrderItemRequestBuilder
                .Create()
                .Build();

            await context.CreateOrderItemService.CreateAsync(createOrderItemRequest);

            var actual = context.Order.OrderItems.First();
            actual.Should().BeEquivalentTo(new
            {
                createOrderItemRequest.OdsCode,
                createOrderItemRequest.CatalogueItemId,
                createOrderItemRequest.CatalogueItemName,
                ProvisioningType = ProvisioningType.FromName(createOrderItemRequest.ProvisioningTypeName),
                CataloguePriceType = CataloguePriceType.FromName(createOrderItemRequest.CataloguePriceTypeName),
                CataloguePriceUnit = CataloguePriceUnit.Create(
                    createOrderItemRequest.CataloguePriceUnitTierName, 
                    createOrderItemRequest.CataloguePriceUnitDescription),
                PriceTimeUnit = TimeUnit.FromName(createOrderItemRequest.PriceTimeUnitName),
                createOrderItemRequest.CurrencyCode,
                createOrderItemRequest.Quantity,
                EstimationPeriod = TimeUnit.FromName(createOrderItemRequest.EstimationPeriodName),
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

                CreateOrderItemService = CreateOrderItemServiceBuilder
                    .Create()
                    .WithOrderRepository(OrderRepositoryMock.Object)
                    .WithIdentityService(IdentityServiceMock.Object)
                    .Build();
            }


            internal ICreateOrderItemService CreateOrderItemService { get; }

            internal Mock<IOrderRepository> OrderRepositoryMock { get; }

            internal Mock<IIdentityService> IdentityServiceMock { get; }

            internal Order Order { get; private set; }

            private string UserName { get; }

            private Guid UserId { get; }

            internal static CreateOrderItemServiceTestContext Setup() => 
                new CreateOrderItemServiceTestContext();
        }
    }
}
