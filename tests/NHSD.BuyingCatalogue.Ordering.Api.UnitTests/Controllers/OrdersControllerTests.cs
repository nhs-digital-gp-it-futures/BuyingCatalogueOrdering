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
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Errors;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Summary;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CompleteOrder;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Contracts;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
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
        [InMemoryDbAutoData]
        public static async Task GetAsync_OrderDoesNotExist_ReturnsNotFound(
            CallOffId callOffId,
            OrdersController controller)
        {
            var response = await controller.GetAsync(callOffId);

            response.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetAsync_OrderExists_ReturnsExpectedResult(
            [Frozen] Mock<IOrderService> service,
            [Frozen] CallOffId callOffId,
            OrderItem orderItem,
            Order order,
            OrdersController controller)
        {
            order.AddOrUpdateOrderItem(orderItem);
            service.Setup(o => o.GetOrder(callOffId)).ReturnsAsync(order);

            var expectedResult = OrderModel.Create(order);

            var response = await controller.GetAsync(callOffId);

            response.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetAsync_InvokesGetOrder(
            [Frozen] Mock<IOrderingPartyService> service,
            [Frozen] CallOffId callOffId,
            Order order,
            OrderingPartyController controller)
        {
            service.Setup(o => o.GetOrder(callOffId)).ReturnsAsync(order);
            await controller.GetAsync(callOffId);

            service.Verify(o => o.GetOrder(callOffId), Times.Once);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetAllAsync_NoOrdersExist_ReturnsEmptyResult(
            Guid organisationId,
            OrdersController controller)
        {
            var result = await controller.GetAllAsync(organisationId);

            result.Value.Should().BeEmpty();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetAllAsync_OrdersExist_ReturnsExpectedResult(
            [Frozen] Mock<IOrderService> service,
            [Frozen] OrderingParty orderingParty,
            IList<Order> orders,
            OrdersController controller)
        {
            service.Setup(o => o.GetOrders(orderingParty.Id)).ReturnsAsync(orders);

            var expectedResult = orders.Select(o => new OrderListItemModel(o));

            var result = await controller.GetAllAsync(orderingParty.Id);

            result.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetAsync_InvokesGetOrderList(
            [Frozen] Mock<IOrderService> service,
            [Frozen] OrderingParty orderingParty,
            IList<Order> orders,
            OrdersController controller)
        {
            service.Setup(o => o.GetOrders(orderingParty.Id)).ReturnsAsync(orders);

            await controller.GetAllAsync(orderingParty.Id);

            service.Verify(o => o.GetOrders(orderingParty.Id), Times.Once);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetOrderSummaryAsync_OrderNotFound_ReturnsNotFound(
            CallOffId callOffId,
            OrdersController controller)
        {
            var response = await controller.GetOrderSummaryAsync(callOffId);

            response.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetOrderSummaryAsync_ReturnsExpectedResult(
            [Frozen] Mock<IOrderService> service,
            [Frozen] CallOffId callOffId,
            IReadOnlyList<OrderItem> orderItems,
            Order order,
            OrdersController controller)
        {
            foreach (var orderItem in orderItems)
                order.AddOrUpdateOrderItem(orderItem);

            service.Setup(o => o.GetOrderSummary(callOffId)).ReturnsAsync(order);

            var expectedResult = OrderSummaryModel.Create(order);

            var result = await controller.GetOrderSummaryAsync(callOffId);

            result.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetOrderSummaryAsync_InvokesGetOrderSummary(
            [Frozen] Mock<IOrderService> service,
            [Frozen] CallOffId callOffId,
            IReadOnlyList<OrderItem> orderItems,
            Order order,
            OrdersController controller)
        {
            foreach (var orderItem in orderItems)
                order.AddOrUpdateOrderItem(orderItem);

            service.Setup(o => o.GetOrderSummary(callOffId)).ReturnsAsync(order);

            await controller.GetOrderSummaryAsync(callOffId);

            service.Verify(o => o.GetOrderSummary(callOffId), Times.Once());
        }

        [Test]
        [InMemoryDbAutoData]
        public static void CreateOrderAsync_NullModel_ThrowsArgumentNullException(
            OrdersController controller)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await controller.CreateOrderAsync(null));
        }

        [Test]
        [InMemoryDbAutoData]
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
        [InMemoryDbAutoData]
        public static async Task CreateOrderAsync_CreatesOrderingParty(
            [Frozen] Mock<IOrderService> service,
            [Frozen] Mock<HttpContext> httpContextMock,
            Order order,
            CreateOrderModel model,
            OrdersController controller)
        {
            var claims = new[] { new Claim("primaryOrganisationId", model.OrganisationId.ToString()) };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
            httpContextMock.Setup(c => c.User).Returns(user);
            service.Setup(o => o.CreateOrder(model.Description, model.OrganisationId!.Value)).Callback(() =>
            {
                order = new Order
                {
                    CallOffId = order.CallOffId,
                    OrderingParty = new OrderingParty
                    {
                        Id = model.OrganisationId!.Value,
                    },
                };
            }).ReturnsAsync(order);

            await controller.CreateOrderAsync(model);

            order.OrderingParty.Id.Should().Be(model.OrganisationId!.Value);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task CreateOrderAsync_CreatesOrder(
            [Frozen] Mock<IOrderService> service,
            [Frozen] Mock<HttpContext> httpContextMock,
            Order order,
            OrderingParty orderingParty,
            string description,
            OrdersController controller)
        {
            var model = new CreateOrderModel { OrganisationId = orderingParty.Id, Description = description };
            var claims = new[] { new Claim("primaryOrganisationId", model.OrganisationId.ToString()) };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
            httpContextMock.Setup(c => c.User).Returns(user);
            service.Setup(o => o.CreateOrder(model.Description, model.OrganisationId!.Value)).Callback(() =>
            {
                order = new Order
                {
                    CallOffId = order.CallOffId,
                    Description = model.Description,
                    OrderingParty = orderingParty,
                };
            }).ReturnsAsync(order);

            await controller.CreateOrderAsync(model);

            order.OrderingParty.Should().Be(orderingParty);
            order.Description.Should().Be(description);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task CreateOrderAsync_UserFromRelatedOrganisation_CreatesOrder(
            [Frozen] Mock<IOrderService> service,
            [Frozen] Mock<HttpContext> httpContextMock,
            Order order,
            OrderingParty orderingParty,
            string description,
            OrdersController controller)
        {
            var model = new CreateOrderModel { OrganisationId = orderingParty.Id, Description = description };
            var claims = new[] { new Claim("relatedOrganisationId", model.OrganisationId.ToString()) };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
            httpContextMock.Setup(c => c.User).Returns(user);
            service.Setup(o => o.CreateOrder(model.Description, model.OrganisationId!.Value)).Callback(() =>
            {
                order = new Order
                {
                    CallOffId = order.CallOffId,
                    Description = model.Description,
                    OrderingParty = orderingParty,
                };
            }).ReturnsAsync(order);

            await controller.CreateOrderAsync(model);

            order.OrderingParty.Should().Be(orderingParty);
            order.Description.Should().Be(description);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task CreateOrderAsync_ReturnsExpectedResult(
            [Frozen] Mock<IOrderService> service,
            [Frozen] Mock<HttpContext> httpContextMock,
            Order order,
            OrderingParty orderingParty,
            OrdersController controller)
        {
            var model = new CreateOrderModel { OrganisationId = orderingParty.Id, Description = "Description" };
            var claims = new[] { new Claim("primaryOrganisationId", model.OrganisationId.ToString()) };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
            httpContextMock.Setup(c => c.User).Returns(user);
            service.Setup(o => o.CreateOrder(model.Description, model.OrganisationId!.Value)).Callback(() =>
            {
                order = new Order
                {
                    CallOffId = order.CallOffId,
                    OrderingParty = new OrderingParty
                    {
                        Id = model.OrganisationId!.Value,
                    },
                };
            }).ReturnsAsync(order);

            var result = await controller.CreateOrderAsync(model);

            result.Should().BeOfType<CreatedAtActionResult>();
            result.As<CreatedAtActionResult>().ActionName.Should().Be("Get");
            result.As<CreatedAtActionResult>().RouteValues.Should().ContainKey("callOffId");
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task CreateOrderAsync_InvokesCreatesOrder(
            [Frozen] Mock<IOrderService> service,
            [Frozen] Mock<HttpContext> httpContextMock,
            Order order,
            OrderingParty orderingParty,
            string description,
            OrdersController controller)
        {
            var model = new CreateOrderModel { OrganisationId = orderingParty.Id, Description = description };
            var claims = new[] { new Claim("primaryOrganisationId", model.OrganisationId.ToString()) };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
            httpContextMock.Setup(c => c.User).Returns(user);
            service.Setup(o => o.CreateOrder(model.Description, model.OrganisationId!.Value)).Callback(() =>
            {
                order = new Order
                {
                    CallOffId = order.CallOffId,
                    OrderingParty = new OrderingParty
                    {
                        Id = model.OrganisationId!.Value,
                    },
                };
            }).ReturnsAsync(order);

            await controller.CreateOrderAsync(model);

            service.Verify(o => o.CreateOrder(model.Description, model.OrganisationId!.Value), Times.Once);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task DeleteOrderAsync_NullOrder_ReturnsNotFound(
            OrdersController controller)
        {
            var result = await controller.DeleteOrderAsync(null);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task DeleteOrderAsync_OrderIsDeleted_ReturnsNotFound(
            Order order,
            OrdersController controller)
        {
            order.IsDeleted = true;

            var result = await controller.DeleteOrderAsync(order);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task DeleteOrderAsync_UpdatesOrder(
            [Frozen] Mock<IOrderService> service,
            Order order,
            OrdersController controller)
        {
            order.IsDeleted = false;
            service.Setup(o => o.DeleteOrder(order)).Callback(() =>
            {
                order.IsDeleted = true;
            });

            await controller.DeleteOrderAsync(order);

            order.IsDeleted.Should().BeTrue();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task DeleteOrderAsync_ReturnsExpectedResult(
            [Frozen] Mock<IOrderService> service,
            Order order,
            OrdersController controller)
        {
            order.IsDeleted = false;
            service.Setup(o => o.DeleteOrder(order)).Callback(() =>
            {
                order.IsDeleted = true;
            });

            var result = await controller.DeleteOrderAsync(order);

            result.Should().BeOfType<NoContentResult>();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task DeleteOrderAsync_InvokesDeleteOrder(
            [Frozen] Mock<IOrderService> service,
            Order order,
            OrdersController controller)
        {
            service.Setup(o => o.DeleteOrder(order));

            await controller.DeleteOrderAsync(order);

            service.Verify(o => o.DeleteOrder(order), Times.Once());
        }

        [Test]
        [InMemoryDbAutoData]
        public static void UpdateStatusAsync_NullModel_ThrowsException(
            OrdersController controller)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await controller.UpdateStatusAsync(default, null));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateStatusAsync_InvalidOrderStatus_ReturnsInvalidOrderStatusError(
            [Frozen] Mock<IOrderService> service,
            Order order,
            StatusModel model,
            OrdersController controller)
        {
            service.Setup(o => o.GetOrderForStatusUpdate(It.IsAny<CallOffId>())).ReturnsAsync(order);
            var response = await controller.UpdateStatusAsync(default, model);

            response.Result.Should().BeOfType<BadRequestObjectResult>();
            var expected = new ErrorResponseModel
            {
                Errors = new List<ErrorModel> { ErrorMessages.InvalidOrderStatus() },
            };

            response.Result.As<BadRequestObjectResult>().Value.Should().BeEquivalentTo(expected);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateStatusAsync_IncompleteOrderStatus_ReturnsInvalidOrderStatusError(
            [Frozen] Mock<IOrderService> service,
            Order order,
            StatusModel model,
            OrdersController controller)
        {
            model.Status = OrderStatus.Incomplete.Name;
            service.Setup(o => o.GetOrderForStatusUpdate(It.IsAny<CallOffId>())).ReturnsAsync(order);

            var response = await controller.UpdateStatusAsync(default, model);

            response.Result.Should().BeOfType<BadRequestObjectResult>();
            var expected = new ErrorResponseModel
            {
                Errors = new List<ErrorModel> { ErrorMessages.InvalidOrderStatus() },
            };

            response.Result.As<BadRequestObjectResult>().Value.Should().BeEquivalentTo(expected);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateStatusAsync_OrderDoesNotExist_ReturnsNotFound(
            StatusModel model,
            OrdersController controller)
        {
            model.Status = OrderStatus.Complete.Name;

            var response = await controller.UpdateStatusAsync(default, model);

            response.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateStatusAsync_OrderStatus_Complete_CompleteOrderServiceCalledOnce(
            [Frozen] Mock<ICompleteOrderService> completeOrderServiceMock,
            [Frozen] Mock<IOrderService> service,
            [Frozen] CallOffId callOffId,
            Order order,
            StatusModel model,
            OrdersController controller)
        {
            service.Setup(o => o.GetOrderForStatusUpdate(It.IsAny<CallOffId>())).ReturnsAsync(order);

            Expression<Func<ICompleteOrderService, Task<Result>>> completeAsync = s => s.CompleteAsync(
                It.Is<Order>(o => o.Equals(order)));

            completeOrderServiceMock.Setup(completeAsync).ReturnsAsync(Result.Success);
            model.Status = OrderStatus.Complete.Name;

            await controller.UpdateStatusAsync(callOffId, model);

            completeOrderServiceMock.Verify(completeAsync);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateStatusAsync_CompleteOrderFailed_ReturnsError(
            [Frozen] Mock<ICompleteOrderService> completeOrderServiceMock,
            [Frozen] Mock<IOrderService> service,
            [Frozen] CallOffId callOffId,
            Order order,
            StatusModel model,
            OrdersController controller)
        {
            service.Setup(o => o.GetOrderForStatusUpdate(It.IsAny<CallOffId>())).ReturnsAsync(order);
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
        [InMemoryDbAutoData]
        public static async Task UpdateStatusAsync_OrderIsComplete_ReturnsNoContent(
            [Frozen] Mock<ICompleteOrderService> completeOrderServiceMock,
            [Frozen] Mock<IOrderService> service,
            [Frozen] CallOffId callOffId,
            Order order,
            StatusModel model,
            OrdersController controller)
        {
            service.Setup(o => o.GetOrderForStatusUpdate(It.IsAny<CallOffId>())).ReturnsAsync(order);
            Expression<Func<ICompleteOrderService, Task<Result>>> completeAsync = s => s.CompleteAsync(
                It.Is<Order>(o => o.Equals(order)));

            completeOrderServiceMock.Setup(completeAsync).ReturnsAsync(Result.Success);
            model.Status = OrderStatus.Complete.Name;

            var response = await controller.UpdateStatusAsync(callOffId, model);

            response.Result.Should().BeOfType<NoContentResult>();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateStatusAsync_OrderIsComplete_InvokesGetOrderCompletedStatus(
            [Frozen] Mock<IOrderService> service,
            [Frozen] CallOffId callOffId,
            Order order,
            StatusModel model,
            OrdersController controller)
        {
            service.Setup(o => o.GetOrderForStatusUpdate(It.IsAny<CallOffId>())).ReturnsAsync(order);

            await controller.UpdateStatusAsync(callOffId, model);

            service.Verify(o => o.GetOrderForStatusUpdate(It.IsAny<CallOffId>()), Times.Once());
        }
    }
}
