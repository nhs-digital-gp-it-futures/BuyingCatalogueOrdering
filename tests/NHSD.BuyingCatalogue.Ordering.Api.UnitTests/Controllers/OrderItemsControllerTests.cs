using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Api.Validation;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests;
using NHSD.BuyingCatalogue.Ordering.Contracts;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class OrderItemsControllerTests
    {
        [Test]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrderItemsController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task ListAsync_OrderDoesNotExist_ReturnsNotFound(
            CallOffId callOffId,
            OrderItemsController controller)
        {
            var response = await controller.ListAsync(callOffId, null);

            response.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task ListAsync_WithoutFilter_ReturnsExpectedResult(
            [Frozen] Mock<IOrderItemService> service,
            [Frozen] CallOffId callOffId,
            List<OrderItem> orderItems,
            Order order,
            OrderItemsController controller)
        {
            foreach (var orderItem in orderItems)
                order.AddOrUpdateOrderItem(orderItem);

            service.Setup(o => o.GetOrder(callOffId)).ReturnsAsync(order);
            service.Setup(o => o.GetOrderItems(callOffId, null)).ReturnsAsync(orderItems);

            var expectedResult = orderItems.Select(i => new GetOrderItemModel(i));

            var response = await controller.ListAsync(callOffId, null);

            response.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task ListAsync_CatalogueItemTypeIsInvalid_ReturnsEmptyList(
            [Frozen] Mock<IOrderItemService> service,
            [Frozen] CallOffId callOffId,
            List<OrderItem> orderItems,
            Order order,
            OrderItemsController controller)
        {
            foreach (var orderItem in orderItems)
                order.AddOrUpdateOrderItem(orderItem);

            service.Setup(o => o.GetOrder(callOffId)).ReturnsAsync(order);
            service.Setup(o => o.GetOrderItems(callOffId, null)).ReturnsAsync(new List<OrderItem>());

            var response = await controller.ListAsync(callOffId, (CatalogueItemType)100);

            response.Value.Should().BeEmpty();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task ListAsync_WithFilter_ReturnsExpectedResult(
            [Frozen] Mock<IOrderItemService> service,
            [Frozen] CallOffId callOffId,
            List<OrderItem> orderItems,
            Order order,
            OrderItemsController controller)
        {
            foreach (var orderItem in orderItems)
                order.AddOrUpdateOrderItem(orderItem);

            const CatalogueItemType catalogueItemType = CatalogueItemType.AdditionalService;

            service.Setup(o => o.GetOrder(callOffId)).ReturnsAsync(order);
            service.Setup(o => o.GetOrderItems(callOffId, catalogueItemType)).ReturnsAsync(
            orderItems.Where(i => i.CatalogueItem.CatalogueItemType == catalogueItemType).Select(i => i).ToList());

            var expectedResult = orderItems
                .Where(i => i.CatalogueItem.CatalogueItemType == catalogueItemType)
                .Select(i => new GetOrderItemModel(i));

            var response = await controller.ListAsync(callOffId, catalogueItemType);

            response.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetAsync_OrderItemDoesNotExist_ReturnsNotFound(
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            OrderItemsController controller)
        {
            var response = await controller.GetAsync(callOffId, catalogueItemId);

            response.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetAsync_OrderItemExists_ReturnsExpected(
            [Frozen] Mock<IOrderItemService> service,
            [Frozen] CallOffId callOffId,
            [Frozen] CatalogueItemId catalogueItemId,
            OrderItem orderItem,
            Order order,
            OrderItemsController controller)
        {
            order.AddOrUpdateOrderItem(orderItem);

            service.Setup(o => o.GetOrderItem(callOffId, catalogueItemId)).ReturnsAsync(orderItem);

            var expectedValue = new GetOrderItemModel(orderItem);

            var response = await controller.GetAsync(callOffId, catalogueItemId);

            response.Value.Should().BeEquivalentTo(expectedValue);
        }

        [Test]
        [InMemoryDbAutoData]
        public static void CreateOrderItemAsync_NullModel_ThrowsException(
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            OrderItemsController controller)
        {
            Assert.ThrowsAsync<ArgumentNullException>(
                async () => await controller.CreateOrderItemAsync(callOffId, catalogueItemId, null));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task CreateOrderItemAsync_OrderDoesNotExist_ReturnsNotFound(
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            CreateOrderItemModel model,
            OrderItemsController controller)
        {
            var result = await controller.CreateOrderItemAsync(callOffId, catalogueItemId, model);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task CreateOrderItemAsync_InvokesCreateOrderItemServiceCreateAsync(
            [Frozen] Mock<ICreateOrderItemService> createOrderItemService,
            [Frozen] Mock<IOrderItemService> orderItemService,
            [Frozen] CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            Order order,
            CreateOrderItemModel model,
            OrderItemsController controller)
        {
            Expression<Func<ICreateOrderItemService, Task<AggregateValidationResult>>> createAsync = s => s.CreateAsync(
                It.Is<Order>(o => o.Equals(order)),
                It.Is<CatalogueItemId>(i => i == catalogueItemId),
                It.Is<CreateOrderItemModel>(m => m == model));

            orderItemService.Setup(o => o.GetOrder(callOffId)).ReturnsAsync(order);
            createOrderItemService.Setup(createAsync).ReturnsAsync(new AggregateValidationResult());

            await controller.CreateOrderItemAsync(callOffId, catalogueItemId, model);

            createOrderItemService.Verify(createAsync);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task CreateOrderItemAsync_OrderExists_ReturnsCreatedAtActionResult(
            [Frozen] Mock<ICreateOrderItemService> createOrderItemService,
            [Frozen] Mock<IOrderItemService> orderItemService,
            [Frozen] CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            Order order,
            CreateOrderItemModel model,
            OrderItemsController controller)
        {
            Expression<Func<ICreateOrderItemService, Task<AggregateValidationResult>>> createAsync = s => s.CreateAsync(
                It.Is<Order>(o => o.Equals(order)),
                It.Is<CatalogueItemId>(i => i == catalogueItemId),
                It.Is<CreateOrderItemModel>(m => m == model));

            orderItemService.Setup(o => o.GetOrder(callOffId)).ReturnsAsync(order);
            createOrderItemService.Setup(createAsync).ReturnsAsync(new AggregateValidationResult());

            var result = await controller.CreateOrderItemAsync(callOffId, catalogueItemId, model);

            result.Should().BeOfType<CreatedAtActionResult>();
            result.As<CreatedAtActionResult>().Should().BeEquivalentTo(new
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
        public static async Task CreateOrderItemAsync_ValidationFailure_AddsModelErrors(
            [Frozen] IReadOnlyList<ErrorDetails> errorDetails,
            AggregateValidationResult aggregateValidationResult,
            [Frozen] Mock<ICreateOrderItemService> createOrderItemService,
            [Frozen] Mock<IOrderItemService> orderItemService,
            [Frozen] CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            Order order,
            CreateOrderItemModel model,
            OrderItemsController controller)
        {
            controller.ProblemDetailsFactory = new TestProblemDetailsFactory();
            aggregateValidationResult.AddValidationResult(new ValidationResult(errorDetails), 0);

            Expression<Func<ICreateOrderItemService, Task<AggregateValidationResult>>> createAsync = s => s.CreateAsync(
                It.Is<Order>(o => o.Equals(order)),
                It.Is<CatalogueItemId>(i => i == catalogueItemId),
                It.Is<CreateOrderItemModel>(m => m == model));

            orderItemService.Setup(o => o.GetOrder(callOffId)).ReturnsAsync(order);
            createOrderItemService.Setup(createAsync).ReturnsAsync(aggregateValidationResult);

            await controller.CreateOrderItemAsync(callOffId, catalogueItemId, model);

            var modelState = controller.ModelState;
            modelState.ErrorCount.Should().Be(errorDetails.Count);
            modelState.Keys.Should().BeEquivalentTo(errorDetails.Select(e => e.ParentName + "[0]." + e.Field));

            var modelStateErrors = modelState.Values.Select(v => v.Errors[0].ErrorMessage);
            modelStateErrors.Should().BeEquivalentTo(errorDetails.Select(e => e.Id));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task CreateOrderItemsAsync_ValidationFailure_ReturnsExpectedResponse(
            ErrorDetails errorDetails,
            AggregateValidationResult aggregateValidationResult,
            [Frozen] Mock<ICreateOrderItemService> createOrderItemService,
            [Frozen] Mock<IOrderItemService> orderItemService,
            [Frozen] CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            Order order,
            CreateOrderItemModel model,
            OrderItemsController controller)
        {
            controller.ProblemDetailsFactory = new TestProblemDetailsFactory();
            aggregateValidationResult.AddValidationResult(new ValidationResult(errorDetails), 0);

            Expression<Func<ICreateOrderItemService, Task<AggregateValidationResult>>> createAsync = s => s.CreateAsync(
                It.Is<Order>(o => o.Equals(order)),
                It.Is<CatalogueItemId>(i => i == catalogueItemId),
                It.Is<CreateOrderItemModel>(m => m == model));

            orderItemService.Setup(o => o.GetOrder(callOffId)).ReturnsAsync(order);
            createOrderItemService.Setup(createAsync).ReturnsAsync(aggregateValidationResult);

            var response = await controller.CreateOrderItemAsync(callOffId, catalogueItemId, model);

            response.Should().BeOfType<BadRequestObjectResult>();
            response.As<BadRequestObjectResult>().Value.Should().BeOfType<ValidationProblemDetails>();
        }

        [Test]
        public static void DeleteOrderItemAsync_HttpDeleteAttribute_HasExpectedTemplate()
        {
            typeof(OrderItemsController)
                .GetMethod(nameof(OrderItemsController.DeleteOrderItemAsync))
                .GetCustomAttribute<HttpDeleteAttribute>()
                .Template.Should().Be("{catalogueItemId}");
        }

        [Test]
        public static void DeleteOrderItemAsync_AuthorizeAttribute_HasExpectedPolicy()
        {
            typeof(OrderItemsController)
                .GetMethod(nameof(OrderItemsController.DeleteOrderItemAsync))
                .GetCustomAttribute<AuthorizeAttribute>()
                .Policy.Should().Be(PolicyName.CanManageOrders);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task DeleteOrderItemAsync_OrderItemExistsInOrder_DeletesOrderItem(
            [Frozen] Mock<IOrderItemService> service,
            [Frozen] CallOffId callOffId,
            Order order,
            List<OrderItem> orderItems,
            OrderItemsController controller)
        {
            orderItems[2].CatalogueItem.ParentCatalogueItemId = orderItems[1].CatalogueItem.Id;
            orderItems.ForEach(o => order.AddOrUpdateOrderItem(o));

            service.Setup(o => o.GetOrderWithCatalogueItem(callOffId, orderItems[1].CatalogueItem.Id))
                .ReturnsAsync(order);
            service.Setup(o => o.DeleteOrderItem(order, orderItems[1].CatalogueItem.Id)).Callback(() =>
            {
                order.DeleteOrderItemAndUpdateProgress(orderItems[1].CatalogueItem.Id);
            });

            await controller.DeleteOrderItemAsync(callOffId, orderItems[1].CatalogueItem.Id);

            order.OrderItems.Contains(orderItems[1]).Should().BeFalse();
            order.OrderItems.Contains(orderItems[2]).Should().BeFalse();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task DeleteOrderItemAsync_OrderItemExistsInOrder_UpdatesProgress(
            [Frozen] Mock<IOrderItemService> service,
            [Frozen] CallOffId callOffId,
            Order order,
            List<OrderItem> orderItems,
            OrderItemsController controller)
        {
            order.RemoveOrderItems();
            var itemsToAdd = orderItems.Take(2).ToList();
            itemsToAdd[1].CatalogueItem.ParentCatalogueItemId = itemsToAdd[0].CatalogueItem.Id;
            itemsToAdd.ForEach(o => order.AddOrUpdateOrderItem(o));
            order.Progress.AdditionalServicesViewed = true;

            service.Setup(o => o.GetOrderWithCatalogueItem(callOffId, itemsToAdd[0].CatalogueItem.Id)).ReturnsAsync(order);
            service.Setup(o => o.DeleteOrderItem(order, itemsToAdd[0].CatalogueItem.Id)).Callback(() =>
            {
                order.DeleteOrderItemAndUpdateProgress(itemsToAdd[0].CatalogueItem.Id);
            });
            await controller.DeleteOrderItemAsync(callOffId, itemsToAdd[0].CatalogueItem.Id);

            order.Progress.AdditionalServicesViewed.Should().BeFalse();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task DeleteOrderItemAsync_OrderItemExistsInOrder_ReturnsNoContentResult(
            [Frozen] Mock<IOrderItemService> service,
            Order order,
            List<OrderItem> orderItems,
            OrderItemsController controller)
        {
            order.RemoveOrderItems();
            orderItems.ForEach(o => order.AddOrUpdateOrderItem(o));
            service.Setup(o => o.GetOrderWithCatalogueItem(order.CallOffId, order.OrderItems[1].CatalogueItem.Id)).ReturnsAsync(order);
            service.Setup(o => o.DeleteOrderItem(order, It.IsAny<CatalogueItemId>())).Callback(() =>
            {
                order.DeleteOrderItemAndUpdateProgress(order.OrderItems[1].CatalogueItem.Id);
            }).ReturnsAsync(1);

            var response = await controller.DeleteOrderItemAsync(order.CallOffId, orderItems[1].CatalogueItem.Id);

            response.Should().BeOfType<NoContentResult>();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task DeleteOrderItemAsync_OrderItemDoesNotExistInOrder_ReturnsNotFoundResult(
            [Frozen] CallOffId callOffId,
            [Frozen] Mock<IOrderItemService> service,
            Order order,
            OrderItem orderItem,
            OrderItemsController controller)
        {
            order.RemoveOrderItems().OrderItems.Count.Should().Be(0);

            service.Setup(o => o.GetOrderWithCatalogueItem(callOffId, orderItem.CatalogueItem.Id))
                .ReturnsAsync(order);

            var response = await controller.DeleteOrderItemAsync(callOffId, new CatalogueItemId(42, "111"));

            response.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task DeleteOrderItemAsync_OrderDeleted_ReturnsNotFoundResult(
            [Frozen] Mock<IOrderItemService> service,
            [Frozen] CallOffId callOffId,
            Order order,
            OrderItem orderItem,
            OrderItemsController controller)
        {
            order.AddOrUpdateOrderItem(orderItem);
            order.IsDeleted = false;

            service.Setup(o => o.GetOrderWithCatalogueItem(callOffId, orderItem.CatalogueItem.Id))
                .ReturnsAsync(order);

            var response = await controller.DeleteOrderItemAsync(callOffId, orderItem.CatalogueItem.Id);

            response.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task DeleteOrderItemAsync_OrderDoesNotExist_ReturnsNotFoundResult(
            [Frozen] Mock<IOrderItemService> service,
            [Frozen] CallOffId callOffId,
            OrderItemsController controller)
        {
            service.Setup(o => o.GetOrderWithCatalogueItem(callOffId, new CatalogueItemId(42, "111")))
                .ReturnsAsync((Order)null);

            var response = await controller.DeleteOrderItemAsync(callOffId, new CatalogueItemId(42, "111"));

            response.Should().BeOfType<NotFoundResult>();
        }

        private sealed class TestProblemDetailsFactory : ProblemDetailsFactory
        {
            public override ProblemDetails CreateProblemDetails(
                HttpContext httpContext,
                int? statusCode = null,
                string title = null,
                string type = null,
                string detail = null,
                string instance = null)
            {
                throw new InvalidOperationException($"{nameof(CreateProblemDetails)} should not be invoked.");
            }

            public override ValidationProblemDetails CreateValidationProblemDetails(
                HttpContext httpContext,
                ModelStateDictionary modelStateDictionary,
                int? statusCode = null,
                string title = null,
                string type = null,
                string detail = null,
                string instance = null)
            {
                return new(modelStateDictionary) { Status = 400 };
            }
        }
    }
}
