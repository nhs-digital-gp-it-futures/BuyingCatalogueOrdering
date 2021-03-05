using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Errors;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Summary;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CompleteOrder;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.Children)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class OrdersControllerTests
    {
        [Test]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrdersController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Test]
        [InMemoryDbAutoData(nameof(GetAsync_OrderDoesNotExist_ReturnsNotFound))]
        public static async Task GetAsync_OrderDoesNotExist_ReturnsNotFound(
            CallOffId callOffId,
            OrdersController controller)
        {
            var response = await controller.GetAsync(callOffId);

            response.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData(nameof(GetAsync_OrderExists_ReturnsExpectedResult))]
        public static async Task GetAsync_OrderExists_ReturnsExpectedResult(
            [Frozen] ApplicationDbContext context,
            [Frozen] CallOffId callOffId,
            OrderItem orderItem,
            Order order,
            OrdersController controller)
        {
            order.AddOrUpdateOrderItem(orderItem);
            context.Order.Add(order);
            await context.SaveChangesAsync();

            var expectedResult = OrderModel.Create(order);

            var response = await controller.GetAsync(callOffId);

            response.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        [InMemoryDbAutoData(nameof(GetAllAsync_NoOrdersExist_ReturnsEmptyResult))]
        public static async Task GetAllAsync_NoOrdersExist_ReturnsEmptyResult(
            Guid organisationId,
            OrdersController controller)
        {
            var result = await controller.GetAllAsync(organisationId);

            result.Value.Should().BeEmpty();
        }

        [Test]
        [InMemoryDbAutoData(nameof(GetAllAsync_OrdersExist_ReturnsExpectedResult))]
        public static async Task GetAllAsync_OrdersExist_ReturnsExpectedResult(
            [Frozen] ApplicationDbContext context,
            [Frozen] OrderingParty orderingParty,
            IReadOnlyList<Order> orders,
            OrdersController controller)
        {
            context.Order.AddRange(orders);
            await context.SaveChangesAsync();

            var expectedResult = orders.Select(o => new OrderListItemModel(o));

            var result = await controller.GetAllAsync(orderingParty.Id);

            result.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        [InMemoryDbAutoData(nameof(GetOrderSummaryAsync_OrderNotFound_ReturnsNotFound))]
        public static async Task GetOrderSummaryAsync_OrderNotFound_ReturnsNotFound(
            CallOffId callOffId,
            OrdersController controller)
        {
            var response = await controller.GetOrderSummaryAsync(callOffId);

            response.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData(nameof(GetOrderSummaryAsync_ReturnsExpectedResult))]
        public static async Task GetOrderSummaryAsync_ReturnsExpectedResult(
            [Frozen] ApplicationDbContext context,
            [Frozen] CallOffId callOffId,
            IReadOnlyList<SelectedServiceRecipient> serviceRecipients,
            IReadOnlyList<OrderItem> orderItems,
            Order order,
            OrdersController controller)
        {
            order.SetSelectedServiceRecipients(serviceRecipients);
            foreach (var orderItem in orderItems)
                order.AddOrUpdateOrderItem(orderItem);

            context.Order.AddRange(order);
            await context.SaveChangesAsync();

            var expectedResult = OrderSummaryModel.Create(order);

            var result = await controller.GetOrderSummaryAsync(callOffId);

            result.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        [InMemoryDbAutoData(nameof(CreateOrderAsync_NullModel_ThrowsArgumentNullException))]
        public static void CreateOrderAsync_NullModel_ThrowsArgumentNullException(
            OrdersController controller)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await controller.CreateOrderAsync(null));
        }

        [Test]
        [InMemoryDbAutoData(nameof(CreateOrderAsync_UnauthorizedUser_ReturnsExpectedResult))]
        public static async Task CreateOrderAsync_UnauthorizedUser_ReturnsExpectedResult(
            Guid orderingPartyId,
            [Frozen] Mock<HttpContext> httpContextMock,
            CreateOrderModel model,
            OrdersController controller)
        {
            var claims = new[] { new Claim("primaryOrganisationId", orderingPartyId.ToString()) };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
            httpContextMock.Setup(c => c.User).Returns(user);

            var result = await controller.CreateOrderAsync(model);

            result.Should().BeOfType<ForbidResult>();
        }

        [Test]
        [InMemoryDbAutoData(nameof(CreateOrderAsync_CreatesOrderingParty))]
        public static async Task CreateOrderAsync_CreatesOrderingParty(
            [Frozen] ApplicationDbContext context,
            [Frozen] Mock<HttpContext> httpContextMock,
            CreateOrderModel model,
            OrdersController controller)
        {
            var claims = new[] { new Claim("primaryOrganisationId", model.OrganisationId.ToString()) };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
            httpContextMock.Setup(c => c.User).Returns(user);

            await controller.CreateOrderAsync(model);

            var orderingParties = context.Set<OrderingParty>();
            orderingParties.Should().HaveCount(1);
            orderingParties.First().Id.Should().Be(model.OrganisationId);
        }

        [Test]
        [InMemoryDbAutoData(nameof(CreateOrderAsync_CreatesOrder))]
        public static async Task CreateOrderAsync_CreatesOrder(
            [Frozen] ApplicationDbContext context,
            [Frozen] Mock<HttpContext> httpContextMock,
            OrderingParty orderingParty,
            string description,
            OrdersController controller)
        {
            context.Add(orderingParty);
            await context.SaveChangesAsync();

            var model = new CreateOrderModel { OrganisationId = orderingParty.Id, Description = description };
            var claims = new[] { new Claim("primaryOrganisationId", model.OrganisationId.ToString()) };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
            httpContextMock.Setup(c => c.User).Returns(user);

            await controller.CreateOrderAsync(model);

            var orders = context.Set<Order>();
            orders.Should().HaveCount(1);
            orders.First().OrderingParty.Should().Be(orderingParty);
            orders.First().Description.Should().Be(description);
        }

        [Test]
        [InMemoryDbAutoData(nameof(CreateOrderAsync_ReturnsExpectedResult))]
        public static async Task CreateOrderAsync_ReturnsExpectedResult(
            [Frozen] Mock<HttpContext> httpContextMock,
            OrderingParty orderingParty,
            OrdersController controller)
        {
            var model = new CreateOrderModel { OrganisationId = orderingParty.Id, Description = "Description" };
            var claims = new[] { new Claim("primaryOrganisationId", model.OrganisationId.ToString()) };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
            httpContextMock.Setup(c => c.User).Returns(user);

            var result = await controller.CreateOrderAsync(model);

            result.Should().BeOfType<CreatedAtActionResult>();
            result.As<CreatedAtActionResult>().ActionName.Should().Be("Get");
            result.As<CreatedAtActionResult>().RouteValues.Should().ContainKey("callOffId");
        }

        [Test]
        [InMemoryDbAutoData(nameof(DeleteOrderAsync_NullOrder_ReturnsNotFound))]
        public static async Task DeleteOrderAsync_NullOrder_ReturnsNotFound(
            OrdersController controller)
        {
            var result = await controller.DeleteOrderAsync(null);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData(nameof(DeleteOrderAsync_OrderIsDeleted_ReturnsNotFound))]
        public static async Task DeleteOrderAsync_OrderIsDeleted_ReturnsNotFound(
            Order order,
            OrdersController controller)
        {
            order.IsDeleted = true;

            var result = await controller.DeleteOrderAsync(order);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData(nameof(DeleteOrderAsync_UpdatesOrder))]
        public static async Task DeleteOrderAsync_UpdatesOrder(
            Order order,
            OrdersController controller)
        {
            order.IsDeleted = false;

            await controller.DeleteOrderAsync(order);

            order.IsDeleted.Should().BeTrue();
        }

        [Test]
        [InMemoryDbAutoData(nameof(DeleteOrderAsync_UpdatesDb))]
        public static async Task DeleteOrderAsync_UpdatesDb(
            [Frozen] ApplicationDbContext context,
            Order order,
            OrdersController controller)
        {
            order.IsDeleted = false;
            context.Order.Add(order);
            await context.SaveChangesAsync();

            await controller.DeleteOrderAsync(order);

            context.Set<Order>().IgnoreQueryFilters().Single(o => o.Equals(order)).IsDeleted.Should().BeTrue();
        }

        [Test]
        [InMemoryDbAutoData(nameof(DeleteOrderAsync_ReturnsExpectedResult))]
        public static async Task DeleteOrderAsync_ReturnsExpectedResult(
            Order order,
            OrdersController controller)
        {
            order.IsDeleted = false;

            var result = await controller.DeleteOrderAsync(order);

            result.Should().BeOfType<NoContentResult>();
        }

        [Test]
        [InMemoryDbAutoData(nameof(UpdateStatusAsync_NullModel_ThrowsException))]
        public static void UpdateStatusAsync_NullModel_ThrowsException(
            OrdersController controller)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await controller.UpdateStatusAsync(default, null));
        }

        [Test]
        [InMemoryDbAutoData(nameof(UpdateStatusAsync_InvalidOrderStatus_ReturnsInvalidOrderStatusError))]
        public static async Task UpdateStatusAsync_InvalidOrderStatus_ReturnsInvalidOrderStatusError(
            StatusModel model,
            OrdersController controller)
        {
            var response = await controller.UpdateStatusAsync(default, model);

            response.Result.Should().BeOfType<BadRequestObjectResult>();
            var expected = new ErrorResponseModel
            {
                Errors = new List<ErrorModel> { ErrorMessages.InvalidOrderStatus() },
            };

            response.Result.As<BadRequestObjectResult>().Value.Should().BeEquivalentTo(expected);
        }

        [Test]
        [InMemoryDbAutoData(nameof(UpdateStatusAsync_IncompleteOrderStatus_ReturnsInvalidOrderStatusError))]
        public static async Task UpdateStatusAsync_IncompleteOrderStatus_ReturnsInvalidOrderStatusError(
            StatusModel model,
            OrdersController controller)
        {
            model.Status = OrderStatus.Incomplete.Name;

            var response = await controller.UpdateStatusAsync(default, model);

            response.Result.Should().BeOfType<BadRequestObjectResult>();
            var expected = new ErrorResponseModel
            {
                Errors = new List<ErrorModel> { ErrorMessages.InvalidOrderStatus() },
            };

            response.Result.As<BadRequestObjectResult>().Value.Should().BeEquivalentTo(expected);
        }

        [Test]
        [InMemoryDbAutoData(nameof(UpdateStatusAsync_OrderDoesNotExist_ReturnsNotFound))]
        public static async Task UpdateStatusAsync_OrderDoesNotExist_ReturnsNotFound(
            StatusModel model,
            OrdersController controller)
        {
            model.Status = OrderStatus.Complete.Name;

            var response = await controller.UpdateStatusAsync(default, model);

            response.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData(nameof(UpdateStatusAsync_OrderStatus_Complete_CompleteOrderServiceCalledOnce))]
        public static async Task UpdateStatusAsync_OrderStatus_Complete_CompleteOrderServiceCalledOnce(
            [Frozen] Mock<ICompleteOrderService> completeOrderServiceMock,
            [Frozen] ApplicationDbContext context,
            [Frozen] CallOffId callOffId,
            OrderItem orderItem,
            Order order,
            StatusModel model,
            OrdersController controller)
        {
            order.AddOrUpdateOrderItem(orderItem);
            context.Order.Add(order);
            await context.SaveChangesAsync();

            Expression<Func<ICompleteOrderService, Task<Result>>> completeAsync = s => s.CompleteAsync(
                It.Is<Order>(o => o.Equals(order)));

            completeOrderServiceMock.Setup(completeAsync).ReturnsAsync(Result.Success);
            model.Status = OrderStatus.Complete.Name;

            await controller.UpdateStatusAsync(callOffId, model);

            completeOrderServiceMock.Verify(completeAsync);
        }

        [Test]
        [InMemoryDbAutoData(nameof(UpdateStatusAsync_CompleteOrderFailed_ReturnsError))]
        public static async Task UpdateStatusAsync_CompleteOrderFailed_ReturnsError(
            [Frozen] Mock<ICompleteOrderService> completeOrderServiceMock,
            [Frozen] ApplicationDbContext context,
            [Frozen] CallOffId callOffId,
            OrderItem orderItem,
            Order order,
            StatusModel model,
            OrdersController controller)
        {
            order.AddOrUpdateOrderItem(orderItem);
            context.Order.Add(order);
            await context.SaveChangesAsync();

            Expression<Func<ICompleteOrderService, Task<Result>>> completeAsync = s => s.CompleteAsync(
                It.Is<Order>(o => o.Equals(order)));

            var expectedErrorDetails = new ErrorDetails("Some error", "Some field name");
            completeOrderServiceMock.Setup(completeAsync).ReturnsAsync(Result.Failure(expectedErrorDetails));
            model.Status = OrderStatus.Complete.Name;

            var response = await controller.UpdateStatusAsync(callOffId, model);

            response.Result.Should().BeOfType<BadRequestObjectResult>();

            var expected = new ErrorResponseModel
            {
                Errors = new List<ErrorModel> { new(expectedErrorDetails.Id, expectedErrorDetails.Field) },
            };

            response.Result.As<BadRequestObjectResult>().Value.Should().BeEquivalentTo(expected);
        }

        [Test]
        [InMemoryDbAutoData(nameof(UpdateStatusAsync_OrderIsComplete_ReturnsNoContent))]
        public static async Task UpdateStatusAsync_OrderIsComplete_ReturnsNoContent(
            [Frozen] Mock<ICompleteOrderService> completeOrderServiceMock,
            [Frozen] ApplicationDbContext context,
            [Frozen] CallOffId callOffId,
            OrderItem orderItem,
            Order order,
            StatusModel model,
            OrdersController controller)
        {
            order.AddOrUpdateOrderItem(orderItem);
            context.Order.Add(order);
            await context.SaveChangesAsync();

            Expression<Func<ICompleteOrderService, Task<Result>>> completeAsync = s => s.CompleteAsync(
                It.Is<Order>(o => o.Equals(order)));

            completeOrderServiceMock.Setup(completeAsync).ReturnsAsync(Result.Success);
            model.Status = OrderStatus.Complete.Name;

            var response = await controller.UpdateStatusAsync(callOffId, model);

            response.Result.Should().BeOfType<NoContentResult>();
        }
    }
}
