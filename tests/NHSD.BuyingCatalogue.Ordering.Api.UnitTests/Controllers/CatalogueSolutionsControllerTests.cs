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
using NHSD.BuyingCatalogue.Ordering.Api.Services.UpdateOrderItem;
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
        public async Task GetAllAsync_WithDescriptionAndNoSolution_ReturnsOkResult()
        {
            const string expectedDescription = "A description";
            var context = CatalogueSolutionsControllerTestContext.Setup();
            context.Order.SetDescription(OrderDescription.Create(expectedDescription).Value);

            var result = await context.Controller.GetAllAsync("myOrder");
            result.Value.Should().BeOfType<CatalogueSolutionsModel>();
            var model = result.Value;
            model.CatalogueSolutions.Should().BeEmpty();
            model.OrderDescription.Should().Be(expectedDescription);
        }

        [Test]
        public async Task GetAllAsync_WithSolution_ReturnsOkayResult()
        {
            const string expectedDescription = "A description";

            var context = CatalogueSolutionsControllerTestContext.Setup();

            context.Order.SetDescription(OrderDescription.Create(expectedDescription).Value);

            var serviceRecipients = new List<(string Ods, string Name)>
            {
                ("eu", "EU test")
            };
            context.Order.SetServiceRecipient(serviceRecipients, Guid.Empty, string.Empty);

            var orderItem = OrderItemBuilder.Create().WithOdsCode(serviceRecipients[0].Ods).Build();
            context.Order.AddOrderItem(orderItem, Guid.Empty, string.Empty);

            var result = await context.Controller.GetAllAsync("myOrder");
            result.Value.Should().BeOfType<CatalogueSolutionsModel>();
            var model = result.Value;
            model.CatalogueSolutions.Count().Should().Be(1);

            var expectedCatalogueSolutionList = new List<CatalogueSolutionModel>
            {
                CreateCatalogueSolutionModel(orderItem, serviceRecipients[0])
            };

            model.OrderDescription.Should().Be(expectedDescription);
            model.CatalogueSolutions.Should().BeEquivalentTo(expectedCatalogueSolutionList);
        }

        [Test]
        public async Task GetAllAsync_MultipleSolutions_ReturnsOkayResult()
        {
            var context = CatalogueSolutionsControllerTestContext.Setup();

            const string expectedDescription = "A description";
            context.Order.SetDescription(OrderDescription.Create(expectedDescription).Value);

            var serviceRecipients = new List<(string Ods, string Name)>
            {
                ("eu", "EU test"),
                ("auz", null)
            };

            context.Order.SetServiceRecipient(serviceRecipients, Guid.Empty, string.Empty);

            var orderItem1 = OrderItemBuilder.Create().WithOdsCode(serviceRecipients[0].Ods).Build();
            var orderItem2 = OrderItemBuilder.Create().WithOdsCode(serviceRecipients[1].Ods).Build();
            context.Order.AddOrderItem(orderItem1, Guid.Empty, string.Empty);
            context.Order.AddOrderItem(orderItem2, Guid.Empty, string.Empty);

            var result = await context.Controller.GetAllAsync("myOrder");
            result.Value.Should().BeOfType<CatalogueSolutionsModel>();
            var model = result.Value;

            model.OrderDescription.Should().Be(expectedDescription);
            model.CatalogueSolutions.Count().Should().Be(2);
            var expectedCatalogueSolutionList = new List<CatalogueSolutionModel>
            {
                CreateCatalogueSolutionModel(orderItem1, serviceRecipients[0]),
                CreateCatalogueSolutionModel(orderItem2, serviceRecipients[1])
            };

            model.CatalogueSolutions.Should().BeEquivalentTo(expectedCatalogueSolutionList);
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

            var result = await context.Controller.CreateOrderItemAsync(orderId, new CreateOrderItemSolutionModel { Quantity = 1 });

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

            var result = await context.Controller.CreateOrderItemAsync(orderId, new CreateOrderItemSolutionModel { Quantity = 1 });

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

            var result = await context.Controller.CreateOrderItemAsync(orderId, new CreateOrderItemSolutionModel { Quantity = 1 });

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

            var result = await context.Controller.CreateOrderItemAsync(orderId, new CreateOrderItemSolutionModel { Quantity = 1 });

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

            await context.Controller.CreateOrderItemAsync(orderId, new CreateOrderItemSolutionModel { Quantity = 1 });

            context.OrderRepositoryMock.Verify(x => x.GetOrderByIdAsync(orderId), Times.Once);
        }

        [Test]
        public async Task CreateOrderItemAsync_CreateOrderItemService_CalledOnce()
        {
            const string orderId = "C10000-01";

            var context = CatalogueSolutionsControllerTestContext.Setup();

            await context.Controller.CreateOrderItemAsync(orderId, new CreateOrderItemSolutionModel {Quantity = 1});

            context.CreateOrderItemService.Verify(x =>
                    x.CreateAsync(It.IsNotNull<CreateOrderItemRequest>()), Times.Once);
        }

        [Test]
        public void UpdateOrderItemAsync_NullModel_ThrowsArgumentNullException()
        {
            static async Task TestAsync()
            {
                const string orderId = "C10000-01";
                const int orderItemId = 1;

                var context = CatalogueSolutionsControllerTestContext.Setup();
                context.Order = null;

                await context.Controller.UpdateOrderItemAsync(orderId, orderItemId, null);
            }

            Assert.ThrowsAsync<ArgumentNullException>(TestAsync);
        }

        [Test]
        public async Task UpdateOrderItemAsync_UnknownOrder_ReturnsNotFound()
        {
            const string orderId = "C10000-01";
            const int orderItemId = 1;

            var context = CatalogueSolutionsControllerTestContext.Setup();
            context.Order = null;

            var result = await context.Controller.UpdateOrderItemAsync(orderId, orderItemId, new UpdateOrderItemSolutionModel { Quantity = 1 });

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task UpdateOrderItemAsync_OrderRepository_CalledOnce()
        {
            const string orderId = "C10000-01";
            const int orderItemId = 1;

            var context = CatalogueSolutionsControllerTestContext.Setup();
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

            await context.Controller.UpdateOrderItemAsync(orderId, orderItemId, new UpdateOrderItemSolutionModel { Quantity = 1 });

            context.OrderRepositoryMock.Verify(x => x.GetOrderByIdAsync(orderId), Times.Once);
        }

        [Test]
        public async Task UpdateOrderItemAsync_InvalidPrimaryOrganisationId_ReturnsForbid()
        {
            const string orderId = "C10000-01";
            const int orderItemId = 1;

            var context = CatalogueSolutionsControllerTestContext.Setup();
            context.Order.OrganisationId = Guid.NewGuid();

            var result = await context.Controller.UpdateOrderItemAsync(orderId, orderItemId, new UpdateOrderItemSolutionModel { Quantity = 1 });

            result.Result.Should().BeOfType<ForbidResult>();
        }

        [Test]
        public async Task UpdateOrderItemAsync_EmptyOrderItems_ReturnsNotFound()
        {
            const string orderId = "C10000-01";
            const int orderItemId = 1;

            var context = CatalogueSolutionsControllerTestContext.Setup();

            var result = await context.Controller.UpdateOrderItemAsync(orderId, orderItemId, new UpdateOrderItemSolutionModel { Quantity = 1 });

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task UpdateOrderItemAsync_AdditionalServiceOrderItem_ReturnsNotFound()
        {
            const string orderId = "C10000-01";
            const int orderItemId = 1;

            var context = CatalogueSolutionsControllerTestContext.Setup();
            context.Order = OrderBuilder
                .Create()
                .WithOrderId(orderId)
                .WithOrganisationId(context.PrimaryOrganisationId)
                .WithOrderItem(OrderItemBuilder
                    .Create()
                    .WithOrderItemId(orderItemId)
                    .WithCatalogueItemType(CatalogueItemType.AdditionalService)
                    .Build())
                .Build();

            var result = await context.Controller.UpdateOrderItemAsync(orderId, orderItemId, new UpdateOrderItemSolutionModel { Quantity = 1 });

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task UpdateOrderItemAsync_UpdateOrderItemModel_ReturnsNotContent()
        {
            const string orderId = "C10000-01";
            const int orderItemId = 1;

            var context = CatalogueSolutionsControllerTestContext.Setup();
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

            var result = await context.Controller.UpdateOrderItemAsync(orderId, orderItemId, new UpdateOrderItemSolutionModel { Quantity = 1 });

            result.Result.Should().BeOfType<NoContentResult>();
        }

        [Test]
        public async Task UpdateOrderItemAsync_UpdateOrderItemService_CalledOnce()
        {
            const string orderId = "C10000-01";
            const int orderItemId = 1;

            var context = CatalogueSolutionsControllerTestContext.Setup();
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

            await context.Controller.UpdateOrderItemAsync(orderId, orderItemId, new UpdateOrderItemSolutionModel { Quantity = 1 });

            context.UpdateOrderItemService.Verify(x => 
                x.UpdateAsync(It.Is<UpdateOrderItemRequest>(r => 
                    orderId.Equals(r.Order.OrderId) 
                    && orderItemId.Equals(r.OrderItemId)), 
                    It.IsAny<CatalogueItemType>(),
                    It.IsAny<ProvisioningType>()), Times.Once);
        }

        [Test]
        public async Task UpdateOrderItemAsync_Error_ReturnsBadRequest()
        {
            const string orderId = "C10000-01";
            const int orderItemId = 1;
            var error = new ErrorDetails("TestError", "TestField");

            var context = CatalogueSolutionsControllerTestContext.Setup();
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

            var result = await context.Controller.UpdateOrderItemAsync(orderId, orderItemId, new UpdateOrderItemSolutionModel { Quantity = 1 });

            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task UpdateOrderItemAsync_Error_ReturnsError()
        {
            const string orderId = "C10000-01";
            const int orderItemId = 1;
            var error = new ErrorDetails("TestError", "TestField");

            var context = CatalogueSolutionsControllerTestContext.Setup();
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

            var result = await context.Controller.UpdateOrderItemAsync(orderId, orderItemId, new UpdateOrderItemSolutionModel { Quantity = 1 });

            var expected = new UpdateOrderItemResponseModel
            {
                Errors = new []
                {
                    new ErrorModel(error.Id, error.Field) 
                }
            };

            result.Result.As<ObjectResult>().Value.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task GetOrderItemAsync_OrderIdIsInvalid_ReturnsNotFound()
        {
            var context = CatalogueSolutionsControllerTestContext.Setup();
            context.Order = null;

            var result = await context.Controller.GetOrderItemAsync("INVALID", 1);
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task GetOrderItemAsync_OrderItemIdIsInvalid_ReturnsNotFound()
        {
            var context = CatalogueSolutionsControllerTestContext.Setup();

            var result = await context.Controller.GetOrderItemAsync("myOrder", -1);
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task GetOrderItemAsync_OrderItemExists_ReturnsResult()
        {
            var context = CatalogueSolutionsControllerTestContext.Setup();

            var serviceRecipients = new List<(string Ods, string Name)>
            {
                ("eu", "EU test")
            };

            context.Order.SetServiceRecipient(serviceRecipients, Guid.Empty, string.Empty);

            var orderItem = OrderItemBuilder.Create().WithOdsCode(serviceRecipients[0].Ods).Build();
            context.Order.AddOrderItem(orderItem, Guid.Empty, string.Empty);

            var result = await context.Controller.GetOrderItemAsync("myOrder", orderItem.OrderItemId);
            result.Value.Should().BeOfType<GetCatalogueSolutionOrderItemModel>();

            var expected = new GetCatalogueSolutionOrderItemModel
            {
                ServiceRecipient = new ServiceRecipientModel
                {
                    OdsCode = orderItem.OdsCode
                },
                CatalogueSolutionId = orderItem.CatalogueItemId,
                CatalogueSolutionName = orderItem.CatalogueItemName,
                CurrencyCode = orderItem.CurrencyCode,
                DeliveryDate = orderItem.DeliveryDate,
                EstimationPeriod = orderItem.EstimationPeriod.Name,
                ItemUnit = new ItemUnitModel { Description = orderItem.CataloguePriceUnit.Description, Name = orderItem.CataloguePriceUnit.Name },
                Price = orderItem.Price,
                ProvisioningType = orderItem.ProvisioningType.Name,
                Quantity = orderItem.Quantity,
                Type = orderItem.CataloguePriceType.Name
            };

            result.Value.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task GetOrderItemAsync_DoesNotHavePrimaryOrganisationId_ReturnsForbid()
        {
            var context = CatalogueSolutionsControllerTestContext.Setup();
            context.Order.OrganisationId = Guid.NewGuid();
            var result = await context.Controller.GetOrderItemAsync("myOrder", -1);

            result.Result.Should().BeOfType<ForbidResult>();
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

                UpdateOrderItemResult = Result.Success();

                UpdateOrderItemService = new Mock<IUpdateOrderItemService>();
                UpdateOrderItemService.Setup(x => x.UpdateAsync(It.IsNotNull<UpdateOrderItemRequest>(), It.IsAny<CatalogueItemType>(), It.IsAny<ProvisioningType>()))
                    .ReturnsAsync(() => UpdateOrderItemResult);

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
                    .WithUpdateOrderItemService(UpdateOrderItemService.Object)
                    .Build();

                Controller.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = ClaimsPrincipal }
                };
            }

            internal Guid PrimaryOrganisationId { get; }

            private ClaimsPrincipal ClaimsPrincipal { get; }

            internal Mock<IOrderRepository> OrderRepositoryMock { get; }

            internal Mock<ICreateOrderItemService> CreateOrderItemService { get; }

            internal Mock<IUpdateOrderItemService> UpdateOrderItemService { get; }

            internal Order Order { get; set; }

            internal Result<int> NewOrderItemId { get; set; }

            internal Result UpdateOrderItemResult { get; set; }

            internal CatalogueSolutionsController Controller { get; }

            internal static CatalogueSolutionsControllerTestContext Setup()
            {
                return new CatalogueSolutionsControllerTestContext();
            }
        }
        private static CatalogueSolutionModel CreateCatalogueSolutionModel(OrderItem orderItem, (string Ods, string Name) serviceRecipient)
        {
            return new CatalogueSolutionModel
            {
                OrderItemId = orderItem.OrderItemId,
                SolutionName = orderItem.CatalogueItemName,
                ServiceRecipient = new GetServiceRecipientModel
                {
                    OdsCode = serviceRecipient.Ods,
                    Name = serviceRecipient.Name
                }
            };
        }
    }
}
