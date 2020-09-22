using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Errors;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Api.Services.UpdateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class OrderItemsControllerTests
    {
        [Test]
        public static void Constructor_NullOrderRepository_NullException()
        {
            var builder = OrderItemsControllerBuilder.Create()
                .WithOrderRepository(null)
                .WithCreateOrderItemService(Mock.Of<ICreateOrderItemService>())
                .WithUpdateOrderItemService(Mock.Of<IUpdateOrderItemService>());

            Assert.Throws<ArgumentNullException>(() => builder.Build());
        }

        [Test]
        public static void Constructor_UpdateOrderItemServiceNull_NullException()
        {
            var builder = OrderItemsControllerBuilder.Create()
                .WithOrderRepository(Mock.Of<IOrderRepository>())
                .WithCreateOrderItemService(Mock.Of<ICreateOrderItemService>())
                .WithUpdateOrderItemService(null);

            Assert.Throws<ArgumentNullException>(() => builder.Build());
        }

        [Test]
        public static void Constructor_NullCreateOrderItemService_NullException()
        {
            var builder = OrderItemsControllerBuilder.Create()
                .WithOrderRepository(Mock.Of<IOrderRepository>())
                .WithCreateOrderItemService(null)
                .WithUpdateOrderItemService(Mock.Of<IUpdateOrderItemService>());

            Assert.Throws<ArgumentNullException>(() => builder.Build());
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
        public static async Task ListAsync_CatalogueItemTypeIsInvalid_ReturnsEmptyList()
        {
            var context = OrderItemsControllerTestContext.Setup();

            var response = await context.OrderItemsController.ListAsync("myOrder", "INVALID");
            response.Value.Should().BeEmpty();
        }

        [Test]
        public static async Task ListAsync_InvalidPrimaryOrganisationId_ReturnsForbid()
        {
            var context = OrderItemsControllerTestContext.Setup();
            context.Order = OrderBuilder
                .Create()
                .WithOrganisationId(Guid.NewGuid())
                .Build();

            var result = await context.OrderItemsController.ListAsync("myOrder", null);

            result.Result.Should().BeOfType<ForbidResult>();
        }

        [TestCase(null)]
        [TestCase("Solution")]
        [TestCase("AdditionalService")]
        [TestCase("AssociatedService")]
        [TestCase("SOLutiON")]
        [TestCase("ADDitIONalServiCE")]
        [TestCase("associatedSERVICe")]
        public static async Task ListAsync_OrderExistsWithFilter_ReturnsListOfOrderItems(string catalogueItemTypeFilter)
        {
            var context = OrderItemsControllerTestContext.Setup();

            var serviceRecipients = new List<OdsOrganisation>
            {
                new OdsOrganisation("eu", "EU test"),
                new OdsOrganisation("auz", null)
            };

            context.Order.SetServiceRecipients(serviceRecipients, Guid.Empty, String.Empty);

            var solutionOrderItem1 = CreateOrderItem(CatalogueItemType.Solution, serviceRecipients[0].Code, 1, new DateTime(2020, 04, 13));
            var solutionOrderItem2 = CreateOrderItem(CatalogueItemType.Solution, serviceRecipients[1].Code, 2, new DateTime(2020, 04, 12));
            var additionalServiceOrderItem1 = CreateOrderItem(CatalogueItemType.AdditionalService, serviceRecipients[1].Code, 3, new DateTime(2020, 05, 13));
            var associatedServiceOrderItem1 = CreateOrderItem(CatalogueItemType.AssociatedService, serviceRecipients[0].Code, 4, new DateTime(2020, 05, 11));

            context.Order.AddOrderItem(solutionOrderItem1, Guid.Empty, string.Empty);
            context.Order.AddOrderItem(solutionOrderItem2, Guid.Empty, string.Empty);
            context.Order.AddOrderItem(additionalServiceOrderItem1, Guid.Empty, string.Empty);
            context.Order.AddOrderItem(associatedServiceOrderItem1, Guid.Empty, string.Empty);

            const string orderId = "myOrder";
            var result = await context.OrderItemsController.ListAsync(orderId, catalogueItemTypeFilter);
            result.Value.Should().BeOfType<List<GetOrderItemModel>>();

            var expectedOrderItems =
                CreateOrderItemModels(
                    new List<OrderItem>
                    {
                        solutionOrderItem1,
                        solutionOrderItem2,
                        additionalServiceOrderItem1,
                        associatedServiceOrderItem1
                    },
                    catalogueItemTypeFilter,
                    context.Order.ServiceRecipients);

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
        public static async Task GetAsync_InvalidPrimaryOrganisationId_ReturnsForbid()
        {
            var context = OrderItemsControllerTestContext.Setup();
            context.Order = OrderBuilder
                .Create()
                .Build();

            var result = await context.OrderItemsController.GetAsync(context.Order.OrderId, 1);

            result.Result.Should().BeOfType<ForbidResult>();
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

            context.OrderRepositoryMock.Verify(x => x.GetOrderByIdAsync(orderId), Times.Once);
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
        public static async Task CreateOrderItemAsync_OrderIdDoesNotExist_ReturnsNotFound()
        {
            var createModel = CreateOrderItemModelBuilder.Create().BuildSolution();

            var context = OrderItemsControllerTestContext.Setup();
            context.Order = null;

            var result = await context.OrderItemsController.CreateOrderItemAsync("unknown", createModel);

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public static async Task CreateOrderItemAsync_InvalidPrimaryOrganisationId_ReturnsForbid()
        {
            var createModel = CreateOrderItemModelBuilder.Create().BuildSolution();

            var context = OrderItemsControllerTestContext.Setup();
            context.Order = OrderBuilder
                .Create()
                .WithOrganisationId(Guid.NewGuid())
                .Build();

            var result = await context.OrderItemsController.CreateOrderItemAsync("myOrder", createModel);

            result.Result.Should().BeOfType<ForbidResult>();
        }

        [Test]
        public static async Task CreateOrderItemAsync_OrderExists_ReturnsCreatedAtActionResult()
        {
            const string orderId = "C10000-01";
            var createModel = CreateOrderItemModelBuilder.Create().BuildSolution();
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
                    { "orderItemId", newOrderItemId }
                }
            });
        }

        [Test]
        public static async Task CreateOrderItemAsync_OrderExists_ReturnsNewOrderItemId()
        {
            const string orderId = "C10000-01";
            var createModel = CreateOrderItemModelBuilder.Create().BuildSolution();
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
                OrderItemId = newOrderItemId
            };

            result.Result.As<ObjectResult>().Value.Should().BeEquivalentTo(expected);
        }

        [Test]
        public static async Task CreateOrderItemAsync_Error_ReturnsBadRequest()
        {
            const string orderId = "C10000-01";
            var createModel = CreateOrderItemModelBuilder.Create().BuildSolution();
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
        public static async Task CreateOrderItemAsync_Error_ReturnsErrors()
        {
            const string orderId = "C10000-01";
            var createModel = CreateOrderItemModelBuilder.Create().BuildSolution();

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
                    new ErrorModel(error.Id, error.Field)
                }
            };

            result.Result.As<ObjectResult>().Value.Should().BeEquivalentTo(expected);
        }

        [Test]
        public static async Task CreateOrderItemAsync_OrderRepository_CalledOnce()
        {
            const string orderId = "C10000-01";
            var createModel = CreateOrderItemModelBuilder.Create().BuildSolution();

            var context = OrderItemsControllerTestContext.Setup();
            context.Order = OrderBuilder
                .Create()
                .WithOrderId(orderId)
                .WithOrganisationId(context.PrimaryOrganisationId)
                .Build();

            await context.OrderItemsController.CreateOrderItemAsync(orderId, createModel);

            context.OrderRepositoryMock.Verify(x => x.GetOrderByIdAsync(orderId), Times.Once);
        }

        [Test]
        public static async Task CreateOrderItemAsync_CreateOrderItemService_CalledOnce()
        {
            const string orderId = "C10000-01";
            var createModel = CreateOrderItemModelBuilder.Create().BuildSolution();

            var context = OrderItemsControllerTestContext.Setup();

            await context.OrderItemsController.CreateOrderItemAsync(orderId, createModel);

            context.CreateOrderItemServiceMock.Verify(x =>
                    x.CreateAsync(It.IsNotNull<CreateOrderItemRequest>()), Times.Once);
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

            context.OrderRepositoryMock.Verify(x => x.GetOrderByIdAsync(orderId), Times.Once);
        }

        [Test]
        public static async Task UpdateOrderItemAsync_InvalidPrimaryOrganisationId_ReturnsForbid()
        {
            const string orderId = "C10000-01";
            const int orderItemId = 1;
            var updateModel = UpdateOrderItemModelBuilder.Create().BuildSolution();

            var context = OrderItemsControllerTestContext.Setup();
            context.Order = OrderBuilder
                .Create()
                .WithOrganisationId(Guid.NewGuid())
                .Build();

            var result = await context.OrderItemsController.UpdateOrderItemAsync(orderId, orderItemId, updateModel);

            result.Result.Should().BeOfType<ForbidResult>();
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

            context.UpdateOrderItemServiceMock.Verify(x =>
                x.UpdateAsync(It.Is<UpdateOrderItemRequest>(r =>
                        orderId.Equals(r.Order.OrderId)
                        && orderItemId.Equals(r.OrderItemId)),
                    It.IsAny<CatalogueItemType>(),
                    It.IsAny<ProvisioningType>()), Times.Once);
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
                    new ErrorModel(error.Id, error.Field)
                }
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
            var serviceRecipientDictionary = serviceRecipients.ToDictionary(x => x.OdsCode);

            var items = orderItems
                .OrderBy(orderItem => orderItem.Created)
                .Select(orderItem =>
                    new GetOrderItemModel(
                        orderItem,
                        serviceRecipientDictionary[orderItem.OdsCode]));

            if (catalogueItemTypeFilter != null)
            {
                items = items.Where(x =>
                    x.CatalogueItemType.Equals(catalogueItemTypeFilter, StringComparison.OrdinalIgnoreCase));
            }

            return items;
        }
    }

    internal sealed class OrderItemsControllerTestContext
    {
        private OrderItemsControllerTestContext(Guid primaryOrganisationId)
        {
            PrimaryOrganisationId = primaryOrganisationId;
            Order = OrderBuilder
                .Create()
                .WithOrganisationId(PrimaryOrganisationId)
                .Build();

            OrderRepositoryMock = new Mock<IOrderRepository>();

            OrderRepositoryMock.Setup(x => x.GetOrderByIdAsync(It.IsAny<string>())).ReturnsAsync(() => Order);
            OrderRepositoryMock.Setup(x => x.UpdateOrderAsync(It.IsAny<Order>())).Callback<Order>(x => Order = x);

            UpdateOrderItemResult = Result.Success();

            UpdateOrderItemServiceMock = new Mock<IUpdateOrderItemService>();
            UpdateOrderItemServiceMock.Setup(x => x.UpdateAsync(It.IsNotNull<UpdateOrderItemRequest>(), It.IsAny<CatalogueItemType>(), It.IsAny<ProvisioningType>()))
                .ReturnsAsync(() => UpdateOrderItemResult);
            NewOrderItemId = Result.Success(123);

            CreateOrderItemServiceMock = new Mock<ICreateOrderItemService>();
            CreateOrderItemServiceMock.Setup(x => x.CreateAsync(It.IsNotNull<CreateOrderItemRequest>()))
                .ReturnsAsync(() => NewOrderItemId);

            ClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(
                new[]
                {
                    new Claim("Ordering", "Manage"),
                    new Claim("primaryOrganisationId", PrimaryOrganisationId.ToString()),
                    new Claim(ClaimTypes.Name, "Test User"),
                    new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
                }, "mock"));

            OrderItemsController = OrderItemsControllerBuilder.Create()
                .WithOrderRepository(OrderRepositoryMock.Object)
                .WithUpdateOrderItemService(UpdateOrderItemServiceMock.Object)
                .WithCreateOrderItemService(CreateOrderItemServiceMock.Object)
                .Build();

            OrderItemsController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = ClaimsPrincipal }
            };
        }

        internal Guid PrimaryOrganisationId { get; }

        internal Mock<IOrderRepository> OrderRepositoryMock { get; }

        internal Mock<ICreateOrderItemService> CreateOrderItemServiceMock { get; }

        internal Result<int> NewOrderItemId { get; set; }

        internal Order Order { get; set; }

        internal OrderItemsController OrderItemsController { get; set; }

        internal Mock<IUpdateOrderItemService> UpdateOrderItemServiceMock { get; }

        internal Result UpdateOrderItemResult { get; set; }

        private ClaimsPrincipal ClaimsPrincipal { get; }

        internal static OrderItemsControllerTestContext Setup()
        {
            return new OrderItemsControllerTestContext(Guid.NewGuid());
        }

        internal static OrderItemsControllerTestContext Setup(Guid primaryOrganisationId)
        {
            return new OrderItemsControllerTestContext(primaryOrganisationId);
        }
    }
}
