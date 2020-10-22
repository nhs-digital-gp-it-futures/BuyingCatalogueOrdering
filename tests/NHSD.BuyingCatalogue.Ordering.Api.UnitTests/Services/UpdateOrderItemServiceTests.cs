using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Api.Services.UpdateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders.Services;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Application.Services;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class UpdateOrderItemServiceTests
    {
        [Test]
        public static void Contructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(UpdateOrderItemService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Test]
        [OrderingAutoData]
        public static void UpdateAsync_NullUpdateOrderItemRequest_ThrowsArgumentNullException(
            CatalogueItemType catalogueItemType,
            ProvisioningType provisioningType,
            UpdateOrderItemService service)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.UpdateAsync(
                null,
                catalogueItemType,
                provisioningType));
        }

        [Test]
        [OrderingAutoData]
        public static async Task UpdateAsync_ValidationFailure_ReturnsFailure(
            CatalogueItemType catalogueItemType,
            ProvisioningType provisioningType,
            [Frozen] Mock<IUpdateOrderItemValidator> validator,
            UpdateOrderItemRequest request,
            UpdateOrderItemService service)
        {
            var errors = new[] { new ErrorDetails("ErrorId") };

            validator.Setup(v => v.Validate(request, catalogueItemType, provisioningType))
                .Returns(errors);

            var result = await service.UpdateAsync(
                request,
                catalogueItemType,
                provisioningType);

            result.Should().BeEquivalentTo(Result.Failure(errors));
        }

        [Test]
        public static async Task UpdateAsync_UpdateOrderItemRequest_ReturnsSuccess()
        {
            var context = UpdateOrderItemServiceTestContext.Setup();

            var updateOrderItemRequest = UpdateOrderItemRequestBuilder
                .Create()
                .Build();

            var actual = await context.UpdateOrderItemService.UpdateAsync(updateOrderItemRequest, CatalogueItemType.Solution, ProvisioningType.OnDemand);

            actual.Should().Be(Result.Success());
        }

        [Test]
        public static async Task UpdateAsync_MapUpdateOrderItemRequestToOrderItem_AreEqual()
        {
            var context = UpdateOrderItemServiceTestContext.Setup();

            var updateOrderItemRequest = UpdateOrderItemRequestBuilder
                .Create()
                .WithOrder(context.Order)
                .WithOrderItemId(context.OrderItem.OrderItemId)
                .Build();

            await context.UpdateOrderItemService.UpdateAsync(updateOrderItemRequest, CatalogueItemType.Solution, ProvisioningType.OnDemand);

            context.OrderItem.Should().BeEquivalentTo(new
            {
                updateOrderItemRequest.DeliveryDate,
                updateOrderItemRequest.EstimationPeriod,
                updateOrderItemRequest.OrderItemId,
                updateOrderItemRequest.Price,
                updateOrderItemRequest.Quantity
            });
        }

        [Test]
        public static async Task UpdateAsync_OrderRepository_CalledOnce()
        {
            var context = UpdateOrderItemServiceTestContext.Setup();

            var updateOrderItemRequest = UpdateOrderItemRequestBuilder
                .Create()
                .Build();

            await context.UpdateOrderItemService.UpdateAsync(updateOrderItemRequest, CatalogueItemType.Solution, ProvisioningType.OnDemand);

            context.OrderRepositoryMock.Verify(orderRepository =>
                orderRepository.UpdateOrderAsync(updateOrderItemRequest.Order), Times.Once);
        }

        [Test]
        public static async Task UpdateAsync_IdentityService_CalledTwice()
        {
            var context = UpdateOrderItemServiceTestContext.Setup();

            var updateOrderItemRequest = UpdateOrderItemRequestBuilder
                .Create()
                .Build();

            await context.UpdateOrderItemService.UpdateAsync(updateOrderItemRequest, CatalogueItemType.Solution, ProvisioningType.OnDemand);

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
