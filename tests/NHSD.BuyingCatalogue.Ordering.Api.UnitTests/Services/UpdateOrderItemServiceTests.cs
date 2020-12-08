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
        public static void Constructors_VerifyGuardClauses()
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
        [OrderingAutoData]
        public static async Task UpdateAsync_UpdateOrderItemRequest_ReturnsSuccess(
            UpdateOrderItemRequest request,
            UpdateOrderItemService service)
        {
            var actual = await service.UpdateAsync(request, CatalogueItemType.Solution, ProvisioningType.OnDemand);

            actual.Should().Be(Result.Success());
        }

        [Test]
        [OrderingAutoData]
        public static async Task UpdateAsync_MapUpdateOrderItemRequestToOrderItem_AreEqual(
            [Frozen] Order order,
            UpdateOrderItemRequest request,
            UpdateOrderItemService service)
        {
            var orderItem = OrderItemBuilder.Create().Build();
            order.AddOrderItem(orderItem, Guid.Empty, string.Empty);
            request.OrderItemId = orderItem.OrderItemId;

            await service.UpdateAsync(request, CatalogueItemType.Solution, ProvisioningType.OnDemand);

            orderItem.Should().BeEquivalentTo(new
            {
                request.DeliveryDate,
                request.EstimationPeriod,
                request.OrderItemId,
                request.Price,
                request.Quantity,
            });
        }

        [Test]
        [OrderingAutoData]
        public static async Task UpdateAsync_OrderRepository_CalledOnce(
            [Frozen] Mock<IOrderRepository> orderRepositoryMock,
            UpdateOrderItemRequest request,
            UpdateOrderItemService service)
        {
            await service.UpdateAsync(request, CatalogueItemType.Solution, ProvisioningType.OnDemand);

            orderRepositoryMock.Verify(r => r.UpdateOrderAsync(request.Order));
        }

        [Test]
        [OrderingAutoData]
        public static async Task UpdateAsync_IdentityService_CalledTwice(
            [Frozen] Mock<IIdentityService> identityServiceMock,
            UpdateOrderItemRequest request,
            UpdateOrderItemService service)
        {
            await service.UpdateAsync(request, CatalogueItemType.Solution, ProvisioningType.OnDemand);

            identityServiceMock.Verify(i => i.GetUserName());
            identityServiceMock.Verify(i => i.GetUserIdentity());
        }
    }
}
