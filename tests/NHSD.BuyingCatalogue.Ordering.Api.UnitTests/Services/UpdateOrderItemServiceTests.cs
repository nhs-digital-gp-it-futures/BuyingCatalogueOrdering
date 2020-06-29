using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders.Services;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Application.Services;
using NHSD.BuyingCatalogue.Ordering.Application.Services.UpdateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class UpdateOrderItemServiceTests
    {
        [Test]
        public void Constructor_NullOrderRepository_ThrowsArgumentNullException()
        {
            static void Test()
            {
                UpdateOrderItemServiceBuilder
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
                UpdateOrderItemServiceBuilder
                    .Create()
                    .WithOrderRepository(Mock.Of<IOrderRepository>())
                    .WithIdentityService(null)
                    .Build();
            }

            Assert.Throws<ArgumentNullException>(Test);
        }

        [Test]
        public void UpdateAsync_NullUpdateOrderItemRequest_ThrowsArgumentNullException()
        {
            static async Task Test()
            {
                var sut = UpdateOrderItemServiceBuilder.Create().Build();
                await sut.UpdateAsync(null);
            }

            Assert.ThrowsAsync<ArgumentNullException>(Test);
        }

        [Test]
        public async Task UpdateAsync_UpdateOrderItemRequest_ReturnsSuccess()
        {
            var context = UpdateOrderItemServiceTestContext.Setup();

            var updateOrderItemRequest = UpdateOrderItemRequestBuilder
                .Create()
                .Build();

            var actual = await context.UpdateOrderItemService.UpdateAsync(updateOrderItemRequest);

            actual.Should().Be(Result.Success());
        }

        [Test]
        public async Task UpdateAsync_MapUpdateOrderItemRequestToOrderItem_AreEqual()
        {
            var context = UpdateOrderItemServiceTestContext.Setup();

            var updateOrderItemRequest = UpdateOrderItemRequestBuilder
                .Create()
                .WithOrder(context.Order)
                .WithOrderItemId(context.OrderItem.OrderItemId)
                .Build();

            await context.UpdateOrderItemService.UpdateAsync(updateOrderItemRequest);

            context.OrderItem.Should().BeEquivalentTo(new
            {
                updateOrderItemRequest.DeliveryDate,
                EstimationPeriod = TimeUnit.FromName(updateOrderItemRequest.EstimationPeriodName),
                updateOrderItemRequest.OrderItemId,
                updateOrderItemRequest.Price,
                updateOrderItemRequest.Quantity
            });
        }

        [Test]
        public async Task UpdateAsync_OrderRepository_CalledOnce()
        {
            var context = UpdateOrderItemServiceTestContext.Setup();

            var updateOrderItemRequest = UpdateOrderItemRequestBuilder
                .Create()
                .Build();

            await context.UpdateOrderItemService.UpdateAsync(updateOrderItemRequest);

            context.OrderRepositoryMock.Verify(orderRepository => 
                orderRepository.UpdateOrderAsync(updateOrderItemRequest.Order), Times.Once);
        }

        [Test]
        public async Task UpdateAsync_IdentityService_CalledTwice()
        {
            var context = UpdateOrderItemServiceTestContext.Setup();

            var updateOrderItemRequest = UpdateOrderItemRequestBuilder
                .Create()
                .Build();

            await context.UpdateOrderItemService.UpdateAsync(updateOrderItemRequest);

            context.IdentityServiceMock.Verify(identityService => 
                identityService.GetUserName(), Times.Once);

            context.IdentityServiceMock.Verify(identityService => 
                identityService.GetUserIdentity(), Times.Once);
        }

        private sealed class UpdateOrderItemServiceTestContext
        {
            private UpdateOrderItemServiceTestContext()
            {
                UserId = Guid.NewGuid();
                UserName = "Bob";

                OrderItem = OrderItemBuilder
                    .Create()
                    .WithOrderItemId(123)
                    .Build();

                Order = OrderBuilder
                    .Create()
                    .WithOrderItem(OrderItem)
                    .Build();

                OrderRepositoryMock = new Mock<IOrderRepository>();
                OrderRepositoryMock.Setup(x => x.UpdateOrderAsync(It.IsAny<Order>())).Callback<Order>(x => Order = x);

                IdentityServiceMock = new Mock<IIdentityService>();
                IdentityServiceMock.Setup(identityService => identityService.GetUserIdentity()).Returns(() => UserId);
                IdentityServiceMock.Setup(identityService => identityService.GetUserName()).Returns(() => UserName);

                UpdateOrderItemService = UpdateOrderItemServiceBuilder
                    .Create()
                    .WithOrderRepository(OrderRepositoryMock.Object)
                    .WithIdentityService(IdentityServiceMock.Object)
                    .Build();
            }


            internal UpdateOrderItemService UpdateOrderItemService { get; }

            internal Mock<IOrderRepository> OrderRepositoryMock { get; }

            internal Mock<IIdentityService> IdentityServiceMock { get; }

            internal Order Order { get; private set; }

            internal OrderItem OrderItem { get; private set; }

            private string UserName { get; }

            private Guid UserId { get; }

            internal static UpdateOrderItemServiceTestContext Setup() => 
                new UpdateOrderItemServiceTestContext();
        }
    }
}
