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
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class OrderItemsControllerTests
    {
        [Test]
        public void Constructor_NullOrderRepository_NullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var _ = new OrderItemsController(null, Mock.Of<ICreateOrderItemService>());
            });
        }

        [Test]
        public void Constructor_NullCreateOrderItemService_NullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var _ = new OrderItemsController(Mock.Of<IOrderRepository>(), null);
            });
        }

        [Test]
        public async Task CreateOrderItemAsync_OrderIdDoesNotExist_ReturnsNotFound()
        {
            var createModel = CreateOrderItemModelBuilder.Create().BuildSolution();

            var context = OrderItemsControllerTestContext.Setup();
            context.Order = null;

            var result = await context.OrderItemsController.CreateOrderItemAsync("unknown", createModel);

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task CreateOrderItemAsync_InvalidPrimaryOrganisationId_ReturnsForbid()
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
        public async Task CreateOrderItemAsync_OrderExists_ReturnsCreatedAtActionResult()
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
                ActionName = "GetOrderItem",
                ControllerName = "CatalogueSolutions",
                RouteValues = new RouteValueDictionary
                {
                    { "orderId", orderId },
                    { "orderItemId", newOrderItemId }
                }
            });
        }

        [Test]
        public async Task CreateOrderItemAsync_OrderExists_ReturnsNewOrderItemId()
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
        public async Task CreateOrderItemAsync_Error_ReturnsBadRequest()
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
        public async Task CreateOrderItemAsync_Error_ReturnsErrors()
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
        public async Task CreateOrderItemAsync_OrderRepository_CalledOnce()
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
        public async Task CreateOrderItemAsync_CreateOrderItemService_CalledOnce()
        {
            const string orderId = "C10000-01";
            var createModel = CreateOrderItemModelBuilder.Create().BuildSolution();

            var context = OrderItemsControllerTestContext.Setup();

            await context.OrderItemsController.CreateOrderItemAsync(orderId, createModel);

            context.CreateOrderItemServiceMock.Verify(x =>
                    x.CreateAsync(It.IsNotNull<CreateOrderItemRequest>()), Times.Once);
        }

        [Test]
        public async Task GetAllAsync_OrderDoesNotExist_ReturnsNotFound()
        {
            var context = OrderItemsControllerTestContext.Setup();
            context.Order = null;

            var response = await context.OrderItemsController.GetAllAsync("INVALID", null);
            response.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task GetAllAsync_CatalogueItemTypeIsInvalid_ReturnsEmptyList()
        {
            var context = OrderItemsControllerTestContext.Setup();

            var response = await context.OrderItemsController.GetAllAsync("myOrder", "INVALID");
            response.Value.Should().BeEmpty();
        }

        [Test]
        public async Task GetAllAsync_InvalidPrimaryOrganisationId_ReturnsForbid()
        {
            var context = OrderItemsControllerTestContext.Setup();
            context.Order = OrderBuilder
                .Create()
                .WithOrganisationId(Guid.NewGuid())
                .Build();

            var result = await context.OrderItemsController.GetAllAsync("myOrder", null);

            result.Result.Should().BeOfType<ForbidResult>();
        }

        [TestCase(null)]
        [TestCase("Solution")]
        [TestCase("AdditionalService")]
        [TestCase("AssociatedService")]
        [TestCase("SOLutiON")]
        [TestCase("ADDitIONalServiCE")]
        [TestCase("associatedSERVICe")]
        public async Task GetAllAsync_OrderExistsWithFilter_ReturnsListOfOrderItems(string catalogueItemTypeFilter)
        {
            var context = OrderItemsControllerTestContext.Setup();

            var serviceRecipients = new List<(string Ods, string Name)>
            {
                ("eu", "EU test"),
                ("auz", null)
            };

            context.Order.SetServiceRecipient(serviceRecipients, Guid.Empty, String.Empty);

            var solutionOrderItem1 = CreateOrderItem(CatalogueItemType.Solution, serviceRecipients[0].Ods, 1, new DateTime(2020, 04, 13));
            var solutionOrderItem2 = CreateOrderItem(CatalogueItemType.Solution, serviceRecipients[1].Ods, 2, new DateTime(2020, 04, 12));
            var additionalServiceOrderItem1 = CreateOrderItem(CatalogueItemType.AdditionalService, serviceRecipients[1].Ods, 3, new DateTime(2020, 05, 13));
            var associatedServiceOrderItem1 = CreateOrderItem(CatalogueItemType.AssociatedService, serviceRecipients[0].Ods, 4, new DateTime(2020, 05, 11));

            context.Order.AddOrderItem(solutionOrderItem1, Guid.Empty, string.Empty);
            context.Order.AddOrderItem(solutionOrderItem2, Guid.Empty, string.Empty);
            context.Order.AddOrderItem(additionalServiceOrderItem1, Guid.Empty, string.Empty);
            context.Order.AddOrderItem(associatedServiceOrderItem1, Guid.Empty, string.Empty);

            const string orderId = "myOrder";
            var result = await context.OrderItemsController.GetAllAsync(orderId, catalogueItemTypeFilter);
            result.Value.Should().BeOfType<List<GetOrderItemModel>>();

            var expectedOrderItems =
                CreateOrderItemModels(
                    new List<OrderItem>
                    {
                        solutionOrderItem1,
                        solutionOrderItem2,
                        additionalServiceOrderItem1,
                        associatedServiceOrderItem1
                    }, orderId,
                    catalogueItemTypeFilter,
                    context.Order.ServiceRecipients);

            result.Value.Should().BeEquivalentTo(expectedOrderItems, options => options.WithStrictOrdering());
        }

        private OrderItem CreateOrderItem(CatalogueItemType catalogueItemType, string odsCode, int orderItemId, DateTime created)
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
            string orderId,
            string catalogueItemTypeFilter,
            IReadOnlyList<ServiceRecipient> serviceRecipients)
        {
            var items = orderItems
                .OrderBy(orderItem => orderItem.Created)
                .Select(orderItem => new GetOrderItemModel
            {
                ItemId = $"{orderId}-{orderItem.OdsCode}-{orderItem.OrderItemId}",
                ServiceRecipient = new ServiceRecipientModel
                {
                    Name = serviceRecipients.FirstOrDefault(serviceRecipient => string.Equals(orderItem.OdsCode,
                           serviceRecipient.OdsCode, StringComparison.OrdinalIgnoreCase))?.Name,
                    OdsCode = orderItem.OdsCode
                },
                CataloguePriceType = orderItem.CataloguePriceType.Name,
                CatalogueItemType = orderItem.CatalogueItemType.Name,
                CatalogueItemName = orderItem.CatalogueItemName,
                CatalogueItemId = orderItem.CatalogueItemId,
            });

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
                .WithCreateOrderItemService(CreateOrderItemServiceMock.Object)
                .Build();

            OrderItemsController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = ClaimsPrincipal }
            };
        }

        internal Guid PrimaryOrganisationId { get; }
        private ClaimsPrincipal ClaimsPrincipal { get; }
        internal Mock<IOrderRepository> OrderRepositoryMock { get; }
        internal Mock<ICreateOrderItemService> CreateOrderItemServiceMock { get; }
        internal Result<int> NewOrderItemId { get; set; }
        internal Order Order { get; set; }
        internal OrderItemsController OrderItemsController { get; set; }

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
