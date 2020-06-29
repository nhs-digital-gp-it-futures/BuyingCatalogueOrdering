using System;
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
    public sealed class CatalogueSolutionsControllerTests
    {
        [Test]
        public async Task UpdateAsync_OrderNotFound_ReturnsNotFound()
        {
            var context = CatalogueSolutionsControllerTestContext.Setup();
            context.Order = null;
            var result = await context.Controller.UpdateAsync("myOrder");
            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task UpdateAsync_InvalidPrimaryOrganisationId_ReturnsForbid()
        {
            var context = CatalogueSolutionsControllerTestContext.Setup();
            context.Order.OrganisationId = Guid.NewGuid();
            var result = await context.Controller.UpdateAsync("myOrder");
            result.Should().BeOfType<ForbidResult>();
        }

        [Test]
        public async Task UpdateAsync_ForExistingOrder_UpdatesCatalogueSolutionsViewed()
        {
            var context = CatalogueSolutionsControllerTestContext.Setup();
            var result = await context.Controller.UpdateAsync("myOrder");
            result.Should().BeOfType<NoContentResult>();
            context.Order.CatalogueSolutionsViewed.Should().BeTrue();
        }

        [Test]
        public async Task GetAllAsync_WithDescription_ReturnsOkResult()
        {
            var expectedDescription = "A description";
            var context = CatalogueSolutionsControllerTestContext.Setup();
            context.Order.SetDescription(OrderDescription.Create(expectedDescription).Value);
            var result = await context.Controller.GetAllAsync("myOrder");
            result.Value.Should().BeOfType<CatalogueSolutionsModel>();
            var model = result.Value;
            model.CatalogueSolutions.Should().BeEmpty();
            model.OrderDescription.Should().Be(expectedDescription);
        }

        [Test]
        public async Task GetAllAsync_OrderNotFound_ReturnsNotFound()
        {
            var context = CatalogueSolutionsControllerTestContext.Setup();
            context.Order = null;
            var result = await context.Controller.GetAllAsync("myOrder");

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task GetAllAsync_InvalidPrimaryOrganisationId_ReturnsForbid()
        {
            var context = CatalogueSolutionsControllerTestContext.Setup();
            context.Order.OrganisationId = Guid.NewGuid();
            var result = await context.Controller.GetAllAsync("myOrder");

            result.Result.Should().BeOfType<ForbidResult>();
        }

        [Test]
        public void Ctor_NullRepository_ThrowsException()
        {
            static void Test()
            {
                CatalogueSolutionsControllerBuilder
                    .Create()
                    .WithOrderRepository(null)
                    .WithCreateOrderItemService(Mock.Of<ICreateOrderItemService>())
                    .Build();
            }

            Assert.Throws<ArgumentNullException>(Test);
        }

        [Test]
        public void Ctor_NullService_ThrowsException()
        {
            static void Test()
            {
                CatalogueSolutionsControllerBuilder
                    .Create()
                    .WithOrderRepository(Mock.Of<IOrderRepository>())
                    .WithCreateOrderItemService(null)
                    .Build();
            }

            Assert.Throws<ArgumentNullException>(Test);
        }

        [Test]
        public async Task CreateOrderItemAsync_OrderIdDoesNotExist_ReturnsNotFound()
        {
            var context = CatalogueSolutionsControllerTestContext.Setup();
            context.Order = null;

            var result = await context.Controller.CreateOrderItemAsync("unknown", null);

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task CreateOrderItemAsync_InvalidPrimaryOrganisationId_ReturnsForbid()
        {
            var context = CatalogueSolutionsControllerTestContext.Setup();
            context.Order.OrganisationId = Guid.NewGuid();

            var result = await context.Controller.CreateOrderItemAsync("myOrder", null);

            result.Result.Should().BeOfType<ForbidResult>();
        }

        [Test]
        public async Task CreateOrderItemAsync_OrderExists_ReturnsCreatedAtActionResult()
        {
            const string orderId = "C10000-01";
            const int newOrderItemId = 456;

            var context = CatalogueSolutionsControllerTestContext.Setup();
            context.NewOrderItemId = Result.Success(newOrderItemId);
            context.Order = OrderBuilder
                .Create()
                .WithOrderId(orderId)
                .WithOrganisationId(context.PrimaryOrganisationId)
                .Build();

            var result = await context.Controller.CreateOrderItemAsync(orderId, new CreateOrderItemModel());

            result.Result.As<CreatedAtActionResult>().Should().BeEquivalentTo(new
            {
                ActionName = "GetOrderItem",
                ControllerName = (string)null,
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
            const int newOrderItemId = 456;

            var context = CatalogueSolutionsControllerTestContext.Setup();
            context.NewOrderItemId = Result.Success(newOrderItemId);
            context.Order = OrderBuilder
                .Create()
                .WithOrderId(orderId)
                .WithOrganisationId(context.PrimaryOrganisationId)
                .Build();

            var result = await context.Controller.CreateOrderItemAsync(orderId, new CreateOrderItemModel());

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
            var error = new ErrorDetails("TestError", "TestField");

            var context = CatalogueSolutionsControllerTestContext.Setup();
            context.NewOrderItemId = Result.Failure<int>(error);
            context.Order = OrderBuilder
                .Create()
                .WithOrderId(orderId)
                .WithOrganisationId(context.PrimaryOrganisationId)
                .Build();

            var result = await context.Controller.CreateOrderItemAsync(orderId, new CreateOrderItemModel());

            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task CreateOrderItemAsync_Error_ReturnsErrors()
        {
            const string orderId = "C10000-01";
            var error = new ErrorDetails("TestError", "TestField");

            var context = CatalogueSolutionsControllerTestContext.Setup();
            context.NewOrderItemId = Result.Failure<int>(error);
            context.Order = OrderBuilder
                .Create()
                .WithOrderId(orderId)
                .WithOrganisationId(context.PrimaryOrganisationId)
                .Build();

            var result = await context.Controller.CreateOrderItemAsync(orderId, new CreateOrderItemModel());

            var expected = new CreateOrderItemResponseModel
            {
                Errors = new []
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

            var context = CatalogueSolutionsControllerTestContext.Setup();
            context.Order = OrderBuilder
                .Create()
                .WithOrderId(orderId)
                .WithOrganisationId(context.PrimaryOrganisationId)
                .Build();

            await context.Controller.CreateOrderItemAsync(orderId, new CreateOrderItemModel());

            context.OrderRepositoryMock.Verify(x => x.GetOrderByIdAsync(orderId), Times.Once);
        }

        [Test]
        public async Task CreateOrderItemAsync_CreateOrderItemService_CalledOnce()
        {
            const string orderId = "C10000-01";

            var context = CatalogueSolutionsControllerTestContext.Setup();

            await context.Controller.CreateOrderItemAsync(orderId, new CreateOrderItemModel());

            context.CreateOrderItemService.Verify(x => 
                    x.CreateAsync(It.IsNotNull<CreateOrderItemRequest>()), Times.Once);
        }

        private sealed class CatalogueSolutionsControllerTestContext
        {
            private CatalogueSolutionsControllerTestContext()
            {
                PrimaryOrganisationId = Guid.NewGuid();
                Order = OrderBuilder
                    .Create()
                    .WithOrganisationId(PrimaryOrganisationId)
                    .Build();

                OrderRepositoryMock = new Mock<IOrderRepository>();
                OrderRepositoryMock.Setup(x => x.GetOrderByIdAsync(It.IsAny<string>())).ReturnsAsync(() => Order);
                OrderRepositoryMock.Setup(x => x.UpdateOrderAsync(It.IsAny<Order>())).Callback<Order>(x => Order = x);

                NewOrderItemId = Result.Success(123);

                CreateOrderItemService = new Mock<ICreateOrderItemService>();
                CreateOrderItemService.Setup(x => x.CreateAsync(It.IsNotNull<CreateOrderItemRequest>()))
                    .ReturnsAsync(() => NewOrderItemId);

                ClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("Ordering", "Manage"),
                    new Claim("primaryOrganisationId", PrimaryOrganisationId.ToString()),
                    new Claim(ClaimTypes.Name, "Test User"),
                    new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
                }, "mock"));

                Controller = CatalogueSolutionsControllerBuilder
                    .Create()
                    .WithOrderRepository(OrderRepositoryMock.Object)
                    .WithCreateOrderItemService(CreateOrderItemService.Object)
                    .Build();

                Controller.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = ClaimsPrincipal }
                };
            }

            internal Guid PrimaryOrganisationId { get; }

            internal ClaimsPrincipal ClaimsPrincipal { get; }

            internal Mock<IOrderRepository> OrderRepositoryMock { get; }

            internal Mock<ICreateOrderItemService> CreateOrderItemService { get; }

            internal Order Order { get; set; }

            internal Result<int> NewOrderItemId { get; set; }

            internal CatalogueSolutionsController Controller { get; }

            internal static CatalogueSolutionsControllerTestContext Setup()
            {
                return new CatalogueSolutionsControllerTestContext();
            }
        }
    }
}
