using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Errors;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Api.Services.UpdateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Api.Validation;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;
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
        public static async Task ListAsync_OrderDoesNotExist_ReturnsNotFound()
        {
            var context = OrderItemsControllerTestContext.Setup();
            context.Order = null;

            var response = await context.OrderItemsController.ListAsync("INVALID", null);
            response.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [OrderingAutoData]
        public static async Task ListAsync_CatalogueItemTypeIsInvalid_ReturnsEmptyList(
            [Frozen] Order order,
            [Frozen] Mock<IOrderRepository> repository,
            OrderItemsController controller)
        {
            const string orderId = "myOrder";

            repository.Setup(r => r.GetOrderByIdAsync(orderId, It.IsAny<Action<IOrderQuery>>()))
                .ReturnsAsync(order);

            var response = await controller.ListAsync(orderId, "INVALID");
            response.Value.Should().BeEmpty();
        }

        [Test]
        [OrderingInlineAutoData(null)]
        [OrderingInlineAutoData("Solution")]
        [OrderingInlineAutoData("AdditionalService")]
        [OrderingInlineAutoData("AssociatedService")] // ReSharper disable StringLiteralTypo
        [OrderingInlineAutoData("SOLutiON")]
        [OrderingInlineAutoData("ADDitIONalServiCE")]
        [OrderingInlineAutoData("associatedSERVICe")] // ReSharper restore StringLiteralTypo
        public static async Task ListAsync_OrderExistsWithFilter_ReturnsListOfOrderItems(
            string catalogueItemTypeFilter,
            [Frozen] Order order,
            [Frozen] Mock<IOrderRepository> repository,
            OrderItemsController controller)
        {
            var serviceRecipients = new List<OdsOrganisation>
            {
                new("eu", "EU test"),
                new("auz", null),
            };

            order.SetServiceRecipients(serviceRecipients, Guid.Empty, string.Empty);

            var solutionOrderItem1 = CreateOrderItem(CatalogueItemType.Solution, serviceRecipients[0].Code, 1, new DateTime(2020, 04, 13));
            var solutionOrderItem2 = CreateOrderItem(CatalogueItemType.Solution, serviceRecipients[1].Code, 2, new DateTime(2020, 04, 12));
            var additionalServiceOrderItem1 = CreateOrderItem(CatalogueItemType.AdditionalService, serviceRecipients[1].Code, 3, new DateTime(2020, 05, 13));
            var associatedServiceOrderItem1 = CreateOrderItem(CatalogueItemType.AssociatedService, serviceRecipients[0].Code, 4, new DateTime(2020, 05, 11));

            order.AddOrderItem(solutionOrderItem1, Guid.Empty, string.Empty);
            order.AddOrderItem(solutionOrderItem2, Guid.Empty, string.Empty);
            order.AddOrderItem(additionalServiceOrderItem1, Guid.Empty, string.Empty);
            order.AddOrderItem(associatedServiceOrderItem1, Guid.Empty, string.Empty);

            const string orderId = "myOrder";

            repository.Setup(r => r.GetOrderByIdAsync(orderId, It.IsAny<Action<IOrderQuery>>()))
                .ReturnsAsync(order);

            var result = await controller.ListAsync(orderId, catalogueItemTypeFilter);

            result.Value.Should().BeOfType<List<GetOrderItemModel>>();

            var expectedOrderItems = CreateOrderItemModels(
                    new List<OrderItem>
                    {
                        solutionOrderItem1,
                        solutionOrderItem2,
                        additionalServiceOrderItem1,
                        associatedServiceOrderItem1,
                    },
                    catalogueItemTypeFilter,
                    order.ServiceRecipients);

            result.Value.Should().BeEquivalentTo(expectedOrderItems, options => options.WithStrictOrdering());
        }

        [Test]
        public static async Task GetAsync_NonExistentOrderID_OrderItemId_ReturnsNotFound()
        {
            var context = OrderItemsControllerTestContext.Setup();
            context.Order = null;

            var result = await context.OrderItemsController.GetAsync(string.Empty, 1);

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public static async Task GetAsync_OrderId_NonExistentOrderItemId_ReturnsNotFound()
        {
            var serviceRecipient = ServiceRecipientBuilder
                .Create()
                .WithOdsCode("ODS1")
                .Build();

            var orderItem = CreateOrderItem(CatalogueItemType.Solution, serviceRecipient.OdsCode, 1, DateTime.UtcNow);

            var context = OrderItemsControllerTestContext.Setup();
            context.Order = OrderBuilder
                .Create()
                .WithOrganisationId(context.PrimaryOrganisationId)
                .WithOrderItem(orderItem)
                .WithServiceRecipient(serviceRecipient.OdsCode, serviceRecipient.Name)
                .Build();

            const int nonExistentOrderItemId = 2;
            var result = await context.OrderItemsController.GetAsync(context.Order.OrderId, nonExistentOrderItemId);

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public static async Task GetAsync_EmptyOrderItems_ReturnsNotFound()
        {
            var context = OrderItemsControllerTestContext.Setup();
            context.Order = OrderBuilder
                .Create()
                .WithOrganisationId(context.PrimaryOrganisationId)
                .Build();

            const int orderItemId = 1;
            var result = await context.OrderItemsController.GetAsync(context.Order.OrderId, orderItemId);

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public static async Task GetAsync_UnmatchedServiceRecipient_ReturnsExpected()
        {
            var serviceRecipient = ServiceRecipientBuilder
                .Create()
                .WithOdsCode("ODS2")
                .Build();

            var orderItem = CreateOrderItem(CatalogueItemType.Solution, serviceRecipient.OdsCode, 1, DateTime.UtcNow);

            var context = OrderItemsControllerTestContext.Setup();
            context.Order = OrderBuilder
                .Create()
                .WithOrganisationId(context.PrimaryOrganisationId)
                .WithOrderItem(orderItem)
                .WithServiceRecipient("UNMATCHED ODS1", serviceRecipient.Name)
                .Build();

            var result = await context.OrderItemsController.GetAsync(context.Order.OrderId, orderItem.OrderItemId);

            result.Value.Should().BeEquivalentTo(new GetOrderItemModel(orderItem, null));
        }

        [Test]
        public static async Task GetAsync_OrderRepository_CalledOnce()
        {
            var serviceRecipient = ServiceRecipientBuilder
                .Create()
                .WithOdsCode("ODS2")
                .Build();

            var orderItem = CreateOrderItem(CatalogueItemType.Solution, serviceRecipient.OdsCode, 1, DateTime.UtcNow);

            var context = OrderItemsControllerTestContext.Setup();
            context.Order = OrderBuilder
                .Create()
                .WithOrganisationId(context.PrimaryOrganisationId)
                .WithOrderItem(orderItem)
                .WithServiceRecipient(serviceRecipient.OdsCode, serviceRecipient.Name)
                .Build();

            string orderId = context.Order.OrderId;
            await context.OrderItemsController.GetAsync(orderId, orderItem.OrderItemId);

            context.OrderRepositoryMock.Verify(r => r.GetOrderByIdAsync(orderId));
        }

        [Test]
        public static async Task GetAsync_OrderId_OrderItemId_ReturnsExpected()
        {
            var serviceRecipient = ServiceRecipientBuilder
                .Create()
                .WithOdsCode("ODS1")
                .Build();

            var orderItem = CreateOrderItem(CatalogueItemType.Solution, serviceRecipient.OdsCode, 1, DateTime.UtcNow);

            var context = OrderItemsControllerTestContext.Setup();
            context.Order = OrderBuilder
                .Create()
                .WithOrganisationId(context.PrimaryOrganisationId)
                .WithOrderItem(orderItem)
                .WithServiceRecipient(serviceRecipient.OdsCode, serviceRecipient.Name)
                .Build();

            var result = await context.OrderItemsController.GetAsync(context.Order.OrderId, orderItem.OrderItemId);

            result.Value.Should().BeEquivalentTo(new GetOrderItemModel(orderItem, serviceRecipient));
        }

        [Test]
        [OrderingAutoData]
        public static void CreateOrderItemAsync_NullModel_ThrowsException(
            string orderId,
            OrderItemsController controller)
        {
            Assert.ThrowsAsync<ArgumentNullException>(
                async () => await controller.CreateOrderItemAsync(orderId, null));
        }

        [Test]
        [OrderingAutoData]
        public static async Task CreateOrderItemAsync_OrderIdDoesNotExist_ReturnsNotFound(CreateOrderItemModel createModel)
        {
            var context = OrderItemsControllerTestContext.Setup();
            context.Order = null;

            var result = await context.OrderItemsController.CreateOrderItemAsync("unknown", createModel);

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [OrderingAutoData]
        public static async Task CreateOrderItemAsync_OrderExists_ReturnsCreatedAtActionResult(CreateOrderItemModel createModel)
        {
            const string orderId = "C10000-01";
            const int newOrderItemId = 456;

            var context = OrderItemsControllerTestContext.Setup();
            context.NewOrderItemId = Result.Success(newOrderItemId);
            context.Order = OrderBuilder
                .Create()
                .WithOrderId(orderId)
                .WithOrganisationId(context.PrimaryOrganisationId)
                .Build();

            var result = await context.OrderItemsController.CreateOrderItemAsync(orderId, createModel);

            result.Result.As<CreatedAtActionResult>().Should().BeEquivalentTo(new
            {
                ActionName = "Get",
                RouteValues = new RouteValueDictionary
                {
                    { "orderId", orderId },
                    { "orderItemId", newOrderItemId },
                },
            });
        }

        [Test]
        [OrderingAutoData]
        public static async Task CreateOrderItemAsync_OrderExists_ReturnsNewOrderItemId(CreateOrderItemModel createModel)
        {
            const string orderId = "C10000-01";
            const int newOrderItemId = 456;

            var context = OrderItemsControllerTestContext.Setup();
            context.NewOrderItemId = Result.Success(newOrderItemId);
            context.Order = OrderBuilder
                .Create()
                .WithOrderId(orderId)
                .WithOrganisationId(context.PrimaryOrganisationId)
                .Build();

            var result = await context.OrderItemsController.CreateOrderItemAsync(orderId, createModel);

            var expected = new CreateOrderItemResponseModel
            {
                OrderItemId = newOrderItemId,
            };

            result.Result.As<ObjectResult>().Value.Should().BeEquivalentTo(expected);
        }

        [Test]
        [OrderingAutoData]
        public static async Task CreateOrderItemAsync_Error_ReturnsBadRequest(CreateOrderItemModel createModel)
        {
            const string orderId = "C10000-01";
            var error = new ErrorDetails("TestError", "TestField");

            var context = OrderItemsControllerTestContext.Setup();
            context.NewOrderItemId = Result.Failure<int>(error);
            context.Order = OrderBuilder
                .Create()
                .WithOrderId(orderId)
                .WithOrganisationId(context.PrimaryOrganisationId)
                .Build();

            var result = await context.OrderItemsController.CreateOrderItemAsync(orderId, createModel);

            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        [OrderingAutoData]
        public static async Task CreateOrderItemAsync_Error_ReturnsErrors(CreateOrderItemModel createModel)
        {
            const string orderId = "C10000-01";

            var error = new ErrorDetails("TestError", "TestField");

            var context = OrderItemsControllerTestContext.Setup();
            context.NewOrderItemId = Result.Failure<int>(error);
            context.Order = OrderBuilder
                .Create()
                .WithOrderId(orderId)
                .WithOrganisationId(context.PrimaryOrganisationId)
                .Build();

            var result = await context.OrderItemsController.CreateOrderItemAsync(orderId, createModel);

            var expected = new CreateOrderItemResponseModel
            {
                Errors = new[]
                {
                    new ErrorModel(error.Id, error.Field),
                },
            };

            result.Result.As<ObjectResult>().Value.Should().BeEquivalentTo(expected);
        }

        [Test]
        [OrderingAutoData]
        public static async Task CreateOrderItemAsync_OrderRepository_CalledOnce(CreateOrderItemModel createModel)
        {
            const string orderId = "C10000-01";

            var context = OrderItemsControllerTestContext.Setup();
            context.Order = OrderBuilder
                .Create()
                .WithOrderId(orderId)
                .WithOrganisationId(context.PrimaryOrganisationId)
                .Build();

            await context.OrderItemsController.CreateOrderItemAsync(orderId, createModel);

            context.OrderRepositoryMock.Verify(r => r.GetOrderByIdAsync(orderId));
        }

        [Test]
        [OrderingAutoData]
        public static async Task CreateOrderItemAsync_CreateOrderItemService_CalledOnce(CreateOrderItemModel createModel)
        {
            const string orderId = "C10000-01";

            var context = OrderItemsControllerTestContext.Setup();

            await context.OrderItemsController.CreateOrderItemAsync(orderId, createModel);

            context.CreateOrderItemServiceMock.Verify(s => s.CreateAsync(It.IsNotNull<CreateOrderItemRequest>()));
        }

        [Test]
        [OrderingAutoData]
        public static void CreateOrderItemsAsync_NullModel_ThrowsException(
            string orderId,
            OrderItemsController controller)
        {
            Assert.ThrowsAsync<ArgumentNullException>(
                async () => await controller.CreateOrderItemsAsync(orderId, null));
        }

        [Test]
        [OrderingAutoData]
        public static async Task CreateOrderItemsAsync_OrderNotFound_ReturnsNotFound(
            string orderId,
            IEnumerable<CreateOrderItemModel> model,
            OrderItemsController controller)
        {
            var response = await controller.CreateOrderItemsAsync(orderId, model);

            response.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [OrderingAutoData]
        public static async Task CreateOrderItemsAsync_ConvertsModelToRequest(
            string orderId,
            CreateOrderItemModel model,
            Order order,
            [Frozen] Mock<ICreateOrderItemService> service,
            [Frozen] Mock<IOrderRepository> repository,
            OrderItemsController controller)
        {
            repository.Setup(r => r.GetOrderByIdAsync(orderId)).ReturnsAsync(order);
            var models = new[] { model };

            IReadOnlyList<CreateOrderItemRequest> requests = null;

            service.Setup(s => s.CreateAsync(order, It.IsNotNull<IReadOnlyList<CreateOrderItemRequest>>()))
                .Callback<Order, IEnumerable<CreateOrderItemRequest>>((_, r) => requests = r.ToList())
                .ReturnsAsync(new AggregateValidationResult());

            await controller.CreateOrderItemsAsync(orderId, models);

            requests.Should().NotBeNull();
            requests.Should().HaveCount(1);
            requests[0].Should().BeEquivalentTo(model.ToRequest(order));
        }

        [Test]
        [OrderingAutoData]
        public static async Task CreateOrderItemsAsync_InvokesCreateAsync(
            string orderId,
            IEnumerable<CreateOrderItemModel> model,
            Order order,
            [Frozen] Mock<ICreateOrderItemService> service,
            [Frozen] Mock<IOrderRepository> repository,
            OrderItemsController controller)
        {
            service.Setup(s => s.CreateAsync(order, It.IsNotNull<IReadOnlyList<CreateOrderItemRequest>>()))
                .ReturnsAsync(new AggregateValidationResult());

            repository.Setup(r => r.GetOrderByIdAsync(orderId)).ReturnsAsync(order);
            await controller.CreateOrderItemsAsync(orderId, model);

            service.Verify(s => s.CreateAsync(
                It.Is<Order>(o => o == order),
                It.IsNotNull<IReadOnlyList<CreateOrderItemRequest>>()));
        }

        [Test]
        [OrderingAutoData]
        public static async Task CreateOrderItemsAsync_ReturnsExpectedResult(
            string orderId,
            IEnumerable<CreateOrderItemModel> model,
            Order order,
            [Frozen] Mock<ICreateOrderItemService> service,
            [Frozen] Mock<IOrderRepository> repository,
            OrderItemsController controller)
        {
            service.Setup(s => s.CreateAsync(order, It.IsNotNull<IReadOnlyList<CreateOrderItemRequest>>()))
                .ReturnsAsync(new AggregateValidationResult());

            repository.Setup(r => r.GetOrderByIdAsync(orderId)).ReturnsAsync(order);
            var response = await controller.CreateOrderItemsAsync(orderId, model);

            response.Should().BeOfType<NoContentResult>();
        }

        [Test]
        [OrderingAutoData]
        public static async Task CreateOrderItemsAsync_ValidationFailure_AddsModelErrors(
            string orderId,
            IEnumerable<CreateOrderItemModel> model,
            Order order,
            [Frozen] IReadOnlyList<ErrorDetails> errorDetails,
            AggregateValidationResult aggregateValidationResult,
            [Frozen] Mock<ICreateOrderItemService> service,
            [Frozen] Mock<IOrderRepository> repository,
            OrderItemsController controller)
        {
            controller.ProblemDetailsFactory = new TestProblemDetailsFactory();
            aggregateValidationResult.AddValidationResult(new ValidationResult(errorDetails), 0);

            service.Setup(s => s.CreateAsync(order, It.IsNotNull<IReadOnlyList<CreateOrderItemRequest>>()))
                .ReturnsAsync(aggregateValidationResult);

            repository.Setup(r => r.GetOrderByIdAsync(orderId)).ReturnsAsync(order);
            await controller.CreateOrderItemsAsync(orderId, model);

            var modelState = controller.ModelState;
            modelState.ErrorCount.Should().Be(errorDetails.Count);
            modelState.Keys.Should().BeEquivalentTo(errorDetails.Select(e => "[0]." + e.Field));

            var modelStateErrors = modelState.Values.Select(v => v.Errors[0].ErrorMessage);
            modelStateErrors.Should().BeEquivalentTo(errorDetails.Select(e => e.Id));
        }

        [Test]
        [OrderingAutoData]
        public static async Task CreateOrderItemsAsync_ValidationFailure_ReturnsExpectedResponse(
            string orderId,
            IEnumerable<CreateOrderItemModel> model,
            Order order,
            ErrorDetails errorDetails,
            AggregateValidationResult aggregateValidationResult,
            [Frozen] Mock<ICreateOrderItemService> service,
            [Frozen] Mock<IOrderRepository> repository,
            OrderItemsController controller)
        {
            controller.ProblemDetailsFactory = new TestProblemDetailsFactory();
            aggregateValidationResult.AddValidationResult(new ValidationResult(errorDetails), 0);

            service.Setup(s => s.CreateAsync(order, It.IsNotNull<IReadOnlyList<CreateOrderItemRequest>>()))
                .ReturnsAsync(aggregateValidationResult);

            repository.Setup(r => r.GetOrderByIdAsync(orderId)).ReturnsAsync(order);
            var response = await controller.CreateOrderItemsAsync(orderId, model);

            response.Should().BeOfType<BadRequestObjectResult>();
            response.As<BadRequestObjectResult>().Value.Should().BeOfType<ValidationProblemDetails>();
        }

        [Test]
        public static void UpdateOrderItemAsync_NullModel_ThrowsArgumentNullException()
        {
            const string orderId = "C10000-01";
            const int orderItemId = 1;

            var context = OrderItemsControllerTestContext.Setup();
            context.Order = null;

            Assert.ThrowsAsync<ArgumentNullException>(async () => await context.OrderItemsController.UpdateOrderItemAsync(orderId, orderItemId, null));
        }

        [Test]
        public static async Task UpdateOrderItemAsync_UnknownOrder_ReturnsNotFound()
        {
            const string orderId = "C10000-01";
            const int orderItemId = 1;
            var updateModel = UpdateOrderItemModelBuilder.Create().BuildSolution();

            var context = OrderItemsControllerTestContext.Setup();
            context.Order = null;

            var result = await context.OrderItemsController.UpdateOrderItemAsync(orderId, orderItemId, updateModel);

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public static async Task UpdateOrderItemAsync_OrderRepository_CalledOnce()
        {
            const string orderId = "C10000-01";
            const int orderItemId = 1;
            var updateModel = UpdateOrderItemModelBuilder.Create().BuildSolution();

            var context = OrderItemsControllerTestContext.Setup();
            context.Order = OrderBuilder
                .Create()
                .WithOrderId(orderId)
                .WithOrganisationId(context.PrimaryOrganisationId)
                .WithOrderItem(OrderItemBuilder
                    .Create()
                    .WithOrderItemId(orderItemId)
                    .WithCatalogueItemType(CatalogueItemType.Solution)
                    .Build())
                .Build();

            await context.OrderItemsController.UpdateOrderItemAsync(orderId, orderItemId, updateModel);

            context.OrderRepositoryMock.Verify(r => r.GetOrderByIdAsync(orderId));
        }

        [Test]
        public static async Task UpdateOrderItemAsync_EmptyOrderItems_ReturnsNotFound()
        {
            const string orderId = "C10000-01";
            const int orderItemId = 1;
            var updateModel = UpdateOrderItemModelBuilder.Create().BuildSolution();

            var context = OrderItemsControllerTestContext.Setup();

            var result = await context.OrderItemsController.UpdateOrderItemAsync(orderId, orderItemId, updateModel);

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public static async Task UpdateOrderItemAsync_UpdateOrderItemModel_ReturnsNotContent()
        {
            const string orderId = "C10000-01";
            const int orderItemId = 1;
            var updateModel = UpdateOrderItemModelBuilder.Create().BuildSolution();

            var context = OrderItemsControllerTestContext.Setup();
            context.Order = OrderBuilder
                .Create()
                .WithOrderId(orderId)
                .WithOrganisationId(context.PrimaryOrganisationId)
                .WithOrderItem(OrderItemBuilder
                    .Create()
                    .WithOrderItemId(orderItemId)
                    .WithCatalogueItemType(CatalogueItemType.Solution)
                    .Build())
                .Build();

            var result = await context.OrderItemsController.UpdateOrderItemAsync(orderId, orderItemId, updateModel);

            result.Result.Should().BeOfType<NoContentResult>();
        }

        [Test]
        public static async Task UpdateOrderItemAsync_UpdateOrderItemService_CalledOnce()
        {
            const string orderId = "C10000-01";
            const int orderItemId = 1;
            var updateModel = UpdateOrderItemModelBuilder.Create().BuildSolution();

            var context = OrderItemsControllerTestContext.Setup();
            context.Order = OrderBuilder
                .Create()
                .WithOrderId(orderId)
                .WithOrganisationId(context.PrimaryOrganisationId)
                .WithOrderItem(OrderItemBuilder
                    .Create()
                    .WithOrderItemId(orderItemId)
                    .WithCatalogueItemType(CatalogueItemType.Solution)
                    .Build())
                .Build();

            await context.OrderItemsController.UpdateOrderItemAsync(orderId, orderItemId, updateModel);

            context.UpdateOrderItemServiceMock.Verify(s => s.UpdateAsync(
                It.Is<UpdateOrderItemRequest>(r => orderId.Equals(r.Order.OrderId, StringComparison.Ordinal) && r.OrderItemId == orderItemId),
                It.IsAny<CatalogueItemType>(),
                It.IsAny<ProvisioningType>()));
        }

        [Test]
        public static async Task UpdateOrderItemAsync_Error_ReturnsBadRequest()
        {
            const string orderId = "C10000-01";
            const int orderItemId = 1;
            var updateModel = UpdateOrderItemModelBuilder.Create().BuildSolution();
            var error = new ErrorDetails("TestError", "TestField");

            var context = OrderItemsControllerTestContext.Setup();
            context.UpdateOrderItemResult = Result.Failure(error);
            context.Order = OrderBuilder
                .Create()
                .WithOrderId(orderId)
                .WithOrganisationId(context.PrimaryOrganisationId)
                .WithOrderItem(OrderItemBuilder
                    .Create()
                    .WithOrderItemId(orderItemId)
                    .WithCatalogueItemType(CatalogueItemType.Solution)
                    .Build())
                .Build();

            var result = await context.OrderItemsController.UpdateOrderItemAsync(orderId, orderItemId, updateModel);

            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public static async Task UpdateOrderItemAsync_Error_ReturnsError()
        {
            const string orderId = "C10000-01";
            const int orderItemId = 1;
            var updateModel = UpdateOrderItemModelBuilder.Create().BuildSolution();
            var error = new ErrorDetails("TestError", "TestField");

            var context = OrderItemsControllerTestContext.Setup();
            context.UpdateOrderItemResult = Result.Failure(error);
            context.Order = OrderBuilder
                .Create()
                .WithOrderId(orderId)
                .WithOrganisationId(context.PrimaryOrganisationId)
                .WithOrderItem(OrderItemBuilder
                    .Create()
                    .WithOrderItemId(orderItemId)
                    .WithCatalogueItemType(CatalogueItemType.Solution)
                    .Build())
                .Build();

            var result = await context.OrderItemsController.UpdateOrderItemAsync(orderId, orderItemId, updateModel);

            var expected = new UpdateOrderItemResponseModel
            {
                Errors = new[]
                {
                    new ErrorModel(error.Id, error.Field),
                },
            };

            result.Result.As<ObjectResult>().Value.Should().BeEquivalentTo(expected);
        }

        private static OrderItem CreateOrderItem(
            CatalogueItemType catalogueItemType, string odsCode, int orderItemId, DateTime created)
        {
            return OrderItemBuilder.Create()
                .WithCatalogueItemType(catalogueItemType)
                .WithOrderItemId(orderItemId)
                .WithOdsCode(odsCode)
                .WithCreated(created)
                .Build();
        }

        private static IEnumerable<GetOrderItemModel> CreateOrderItemModels(
            IEnumerable<OrderItem> orderItems,
            string catalogueItemTypeFilter,
            IEnumerable<ServiceRecipient> serviceRecipients)
        {
            var serviceRecipientDictionary = serviceRecipients.ToDictionary(r => r.OdsCode);

            var items = orderItems
                .OrderBy(orderItem => orderItem.Created)
                .Select(orderItem =>
                    new GetOrderItemModel(
                        orderItem,
                        serviceRecipientDictionary[orderItem.OdsCode]));

            if (catalogueItemTypeFilter is not null)
            {
                items = items.Where(m =>
                    m.CatalogueItemType.Equals(catalogueItemTypeFilter, StringComparison.OrdinalIgnoreCase));
            }

            return items;
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

        private sealed class OrderItemsControllerTestContext
        {
            private OrderItemsControllerTestContext(Guid primaryOrganisationId)
            {
                PrimaryOrganisationId = primaryOrganisationId;
                Order = OrderBuilder
                    .Create()
                    .WithOrganisationId(PrimaryOrganisationId)
                    .Build();

                OrderRepositoryMock = new Mock<IOrderRepository>();

                OrderRepositoryMock.Setup(r => r.GetOrderByIdAsync(It.IsAny<string>())).ReturnsAsync(() => Order);
                OrderRepositoryMock.Setup(r => r.UpdateOrderAsync(It.IsAny<Order>())).Callback<Order>(o => Order = o);

                UpdateOrderItemResult = Result.Success();

                UpdateOrderItemServiceMock = new Mock<IUpdateOrderItemService>();
                UpdateOrderItemServiceMock
                    .Setup(s => s.UpdateAsync(It.IsNotNull<UpdateOrderItemRequest>(), It.IsAny<CatalogueItemType>(), It.IsAny<ProvisioningType>()))
                    .ReturnsAsync(() => UpdateOrderItemResult);

                NewOrderItemId = Result.Success(123);

                CreateOrderItemServiceMock = new Mock<ICreateOrderItemService>();
                CreateOrderItemServiceMock
                    .Setup(s => s.CreateAsync(It.IsNotNull<CreateOrderItemRequest>()))
                    .ReturnsAsync(() => NewOrderItemId);

                ClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(
                    new[]
                    {
                    new Claim("Ordering", "Manage"),
                    new Claim("primaryOrganisationId", PrimaryOrganisationId.ToString()),
                    new Claim(ClaimTypes.Name, "Test User"),
                    new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                    },
                    "mock"));

                OrderItemsController = OrderItemsControllerBuilder.Create()
                    .WithOrderRepository(OrderRepositoryMock.Object)
                    .WithUpdateOrderItemService(UpdateOrderItemServiceMock.Object)
                    .WithCreateOrderItemService(CreateOrderItemServiceMock.Object)
                    .Build();

                OrderItemsController.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = ClaimsPrincipal },
                };
            }

            internal Guid PrimaryOrganisationId { get; }

            internal Mock<IOrderRepository> OrderRepositoryMock { get; }

            internal Mock<ICreateOrderItemService> CreateOrderItemServiceMock { get; }

            internal Result<int> NewOrderItemId { get; set; }

            internal Order Order { get; set; }

            internal OrderItemsController OrderItemsController { get; }

            internal Mock<IUpdateOrderItemService> UpdateOrderItemServiceMock { get; }

            internal Result UpdateOrderItemResult { get; set; }

            private ClaimsPrincipal ClaimsPrincipal { get; }

            internal static OrderItemsControllerTestContext Setup()
            {
                return new(Guid.NewGuid());
            }
        }
    }
}
