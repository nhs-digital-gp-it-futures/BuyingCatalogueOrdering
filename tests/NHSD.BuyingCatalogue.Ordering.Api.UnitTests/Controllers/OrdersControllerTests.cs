using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Summary;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrder;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Common.Extensions;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class OrdersControllerTests
    {
        [Test]
        public void Constructor_NullRepository_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var _ = new OrdersController(null , null);
            });
        }

        [Test]
        public async Task GetAllAsync_NoOrdersExist_ReturnsEmptyResult()
        {
            var context = OrdersControllerTestContext.Setup();

            using var controller = context.OrdersController;

            var result = await controller.GetAllAsync(context.PrimaryOrganisationId) as OkObjectResult;
            var orders = result.Value as List<OrderModel>;
            orders.Should().BeEmpty();
        }

        [TestCase(null, "Some Description")]
        [TestCase("C0000014-01", "Some Description")]
        public async Task GetAllAsync_SingleOrderWithOrganisationIdExists_ReturnsTheOrder(string orderId, string orderDescription)
        {
            var context = OrdersControllerTestContext.Setup();
            var orders = new List<(Order order, OrderModel expected)>
            {
                CreateOrderTestData(orderId, context.PrimaryOrganisationId, orderDescription)
            };

            context.Orders = orders.Select(x => x.order);

            using var controller = context.OrdersController;

            var result = await controller.GetAllAsync(context.PrimaryOrganisationId) as OkObjectResult;
            var ordersResult = result.Value as List<OrderModel>;
            ordersResult.Should().ContainSingle();
            ordersResult.Should().BeEquivalentTo(orders.Select(x => x.expected));
        }

        [Test]
        public async Task GetAllAsync_SingleOrderWithOtherOrganisationIdExists_ReturnsForbidden()
        {
            var otherOrganisationId = Guid.NewGuid();
            var context = OrdersControllerTestContext.Setup();
            var orders = new List<(Order order, OrderModel expected)>
            {
                CreateOrderTestData("C0000014-01", otherOrganisationId, "A description")
            };

            context.Orders = orders.Select(x => x.order);

            using var controller = context.OrdersController;

            var result = await controller.GetAllAsync(otherOrganisationId);
            result.Should().BeOfType<ForbidResult>();
        }

        [Test]
        public async Task GetAllAsync_MultipleOrdersWithOrganisationIdExist_ReturnsAllOrders()
        {
            var context = OrdersControllerTestContext.Setup();

            var orders = new List<(Order order, OrderModel expected)>
            {
                CreateOrderTestData("C0000014-01", context.PrimaryOrganisationId, "Some Description"),
                CreateOrderTestData("C000012-01", context.PrimaryOrganisationId, "Another Description")
            };

            context.Orders = orders.Select(x => x.order);

            using var controller = context.OrdersController;

            var result = await controller.GetAllAsync(context.PrimaryOrganisationId) as OkObjectResult;
            var ordersResult = result.Value as List<OrderModel>;
            ordersResult.Count.Should().Be(2);
            ordersResult.Should().BeEquivalentTo(orders.Select(x => x.expected));
        }

        [Test]
        public async Task GetAll_OrdersByOrganisationId_CalledOnce()
        {
            var context = OrdersControllerTestContext.Setup();

            using var controller = context.OrdersController;

            await controller.GetAllAsync(context.PrimaryOrganisationId);

            context.OrderRepositoryMock.Verify(x => x.ListOrdersByOrganisationIdAsync(context.PrimaryOrganisationId), Times.Once);
        }

        [Test]
        public async Task GetOrderSummaryAsync_OrderIdDoesNotExist_ReturnsNotFound()
        {
            var context = OrdersControllerTestContext.Setup();

            using var controller = context.OrdersController;

            var response = await controller.GetOrderSummaryAsync("INVALID");
            response.Should().BeEquivalentTo(new NotFoundResult());
        }

        [Test]
        public async Task GetOrderSummaryAsync_IsSummaryComplete_ReturnResult()
        {
            const string orderId = "C0000014-01";
            var context = OrdersControllerTestContext.Setup();

            (Order order, OrderSummaryModel expected) = CreateOrderSummaryTestData(orderId, "Some Description", context.PrimaryOrganisationId);

            context.Order = order;

            using var controller = context.OrdersController;

            var result = await controller.GetOrderSummaryAsync(orderId) as OkObjectResult;
            var orderSummary = result.Value as OrderSummaryModel;
            orderSummary.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task GetOrderSummaryAsync_OrderPartyOrganisationNameMissing_ReturnsOrderPartyIncomplete()
        {
            const string orderId = "C0000014-01";
            var context = OrdersControllerTestContext.Setup();

            (Order order, OrderSummaryModel expected) = CreateOrderSummaryTestData(orderId, "Some Description", context.PrimaryOrganisationId);

            order.OrganisationName = null;
            expected.Sections.Single(s => s.Id == "ordering-party").Status = "incomplete";

            context.Order = order;

            using var controller = context.OrdersController;

            var result = await controller.GetOrderSummaryAsync(orderId) as OkObjectResult;
            var orderSummary = result.Value as OrderSummaryModel;
            orderSummary.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task GetOrderSummaryAsync_OrderPartyOrganisationOdsMissing_ReturnsOrderPartyIncomplete()
        {
            const string orderId = "C0000014-01";
            var context = OrdersControllerTestContext.Setup();

            (Order order, OrderSummaryModel expected) = CreateOrderSummaryTestData(orderId, "Some Description", context.PrimaryOrganisationId);

            order.OrganisationOdsCode = null;
            expected.Sections.Single(s => s.Id == "ordering-party").Status = "incomplete";

            context.Order = order;

            using var controller = context.OrdersController;

            var result = await controller.GetOrderSummaryAsync(orderId) as OkObjectResult;
            var orderSummary = result.Value as OrderSummaryModel;
            orderSummary.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task GetOrderSummaryAsync_OrderPartyOrganisationAddressMissing_ReturnsOrderPartyIncomplete()
        {
            const string orderId = "C0000014-01";
            var context = OrdersControllerTestContext.Setup();

            (Order order, OrderSummaryModel expected) = CreateOrderSummaryTestData(orderId, "Some Description", context.PrimaryOrganisationId);

            order.OrganisationAddress =null;
            expected.Sections.Single(s => s.Id == "ordering-party").Status = "incomplete";

            context.Order = order;

            using var controller = context.OrdersController;

            var result = await controller.GetOrderSummaryAsync(orderId) as OkObjectResult;
            var orderSummary = result.Value as OrderSummaryModel;
            orderSummary.Should().BeEquivalentTo(expected);
        }


        [Test]
        public async Task GetOrderSummaryAsync_OrderPartyOrganisationContactMissing_ReturnsOrderPartyIncomplete()
        {
            const string orderId = "C0000014-01";
            var context = OrdersControllerTestContext.Setup();

            (Order order, OrderSummaryModel expected) = CreateOrderSummaryTestData(orderId, "Some Description", context.PrimaryOrganisationId);

            order.OrganisationContact = null;
            expected.Sections.Single(s => s.Id == "ordering-party").Status = "incomplete";

            context.Order = order;

            using var controller = context.OrdersController;

            var result = await controller.GetOrderSummaryAsync(orderId) as OkObjectResult;
            var orderSummary = result.Value as OrderSummaryModel;
            orderSummary.Should().BeEquivalentTo(expected);
        }

        [TestCase(null, "incomplete")]
        [TestCase("", "incomplete")]
        [TestCase("777 Street Lain", "complete")]
        public async Task GetOrderSummaryAsync_OrderPartyOrganisationAddressline1Missing_ReturnsOrderPartyIncomplete(string line1Value,string expectedStatus)
        {
            const string orderId = "C0000014-01";
            var context = OrdersControllerTestContext.Setup();

            (Order order, OrderSummaryModel expected) = CreateOrderSummaryTestData(orderId, "Some Description", context.PrimaryOrganisationId);

            order.OrganisationAddress.Line1 = line1Value;
            expected.Sections.Single(s => s.Id == "ordering-party").Status = expectedStatus;

            context.Order = order;

            using var controller = context.OrdersController;

            var result = await controller.GetOrderSummaryAsync(orderId) as OkObjectResult;
            var orderSummary = result.Value as OrderSummaryModel;
            orderSummary.Should().BeEquivalentTo(expected);
        }

        [TestCase(null, "incomplete")]
        [TestCase("", "incomplete")]
        [TestCase("LS7 1NS", "complete")]
        public async Task GetOrderSummaryAsync_OrderPartyOrganisationAddressPostCodeMissing_ReturnsOrderPartyIncomplete(string postcodeValue,string expectedStatus)
        {
            const string orderId = "C0000014-01";
            var context = OrdersControllerTestContext.Setup();

            (Order order, OrderSummaryModel expected) = CreateOrderSummaryTestData(orderId, "Some Description", context.PrimaryOrganisationId);

            order.OrganisationAddress.Postcode = postcodeValue;
            expected.Sections.Single(s => s.Id == "ordering-party").Status = expectedStatus;

            context.Order = order;

            using var controller = context.OrdersController;

            var result = await controller.GetOrderSummaryAsync(orderId) as OkObjectResult;
            var orderSummary = result.Value as OrderSummaryModel;
            orderSummary.Should().BeEquivalentTo(expected);
        }

        [TestCase(null, "incomplete")]
        [TestCase("", "incomplete")]
        [TestCase("Leeds", "complete")]
        public async Task GetOrderSummaryAsync_OrderPartyOrganisationAddressTownMissing_ReturnsOrderPartyIncomplete(string townValue,string expectedStatus)
        {
            const string orderId = "C0000014-01";
            var context = OrdersControllerTestContext.Setup();

            (Order order, OrderSummaryModel expected) = CreateOrderSummaryTestData(orderId, "Some Description", context.PrimaryOrganisationId);

            order.OrganisationAddress.Town = townValue;
            expected.Sections.Single(s => s.Id == "ordering-party").Status = expectedStatus;

            context.Order = order;

            using var controller = context.OrdersController;

            var result = await controller.GetOrderSummaryAsync(orderId) as OkObjectResult;
            var orderSummary = result.Value as OrderSummaryModel;
            orderSummary.Should().BeEquivalentTo(expected);
        }

        [TestCase(null, "incomplete")]
        [TestCase("", "incomplete")]
        [TestCase("Rupert", "complete")]
        public async Task GetOrderSummaryAsync_OrderPartyOrganisationContactFirstNameMissing_ReturnsOrderPartyIncomplete(string firstNameValue, string expectedStatus)
        {
            const string orderId = "C0000014-01";
            var context = OrdersControllerTestContext.Setup();

            (Order order, OrderSummaryModel expected) = CreateOrderSummaryTestData(orderId, "Some Description", context.PrimaryOrganisationId);

            order.OrganisationContact.FirstName = firstNameValue;
            expected.Sections.Single(s => s.Id == "ordering-party").Status = expectedStatus;

            context.Order = order;

            using var controller = context.OrdersController;

            var result = await controller.GetOrderSummaryAsync(orderId) as OkObjectResult;
            var orderSummary = result.Value as OrderSummaryModel;
            orderSummary.Should().BeEquivalentTo(expected);
        }

        [TestCase(null, "incomplete")]
        [TestCase("", "incomplete")]
        [TestCase("Fortesque", "complete")]
        public async Task GetOrderSummaryAsync_OrderPartyOrganisationContactLastNameMissing_ReturnsOrderPartyIncomplete(string  lastNameValue, string expectedStatus)
        {
            const string orderId = "C0000014-01";
            var context = OrdersControllerTestContext.Setup();

            (Order order, OrderSummaryModel expected) = CreateOrderSummaryTestData(orderId, "Some Description", context.PrimaryOrganisationId);

            order.OrganisationContact.LastName = lastNameValue;
            expected.Sections.Single(s => s.Id == "ordering-party").Status = expectedStatus;

            context.Order = order;

            using var controller = context.OrdersController;

            var result = await controller.GetOrderSummaryAsync(orderId) as OkObjectResult;
            var orderSummary = result.Value as OrderSummaryModel;
            orderSummary.Should().BeEquivalentTo(expected);
        }

        [TestCase(null, "incomplete")]
        [TestCase("", "incomplete")]
        [TestCase("Fortesque", "complete")]
        public async Task GetOrderSummaryAsync_OrderPartyOrganisationContactEmailMissing_ReturnsOrderPartyIncomplete(string emailValue, string expectedStatus)
        {
            const string orderId = "C0000014-01";
            var context = OrdersControllerTestContext.Setup();

            (Order order, OrderSummaryModel expected) = CreateOrderSummaryTestData(orderId, "Some Description", context.PrimaryOrganisationId);

            order.OrganisationContact.Email = emailValue;
            expected.Sections.Single(s => s.Id == "ordering-party").Status = expectedStatus;

            context.Order = order;

            using var controller = context.OrdersController;

            var result = await controller.GetOrderSummaryAsync(orderId) as OkObjectResult;
            var orderSummary = result.Value as OrderSummaryModel;
            orderSummary.Should().BeEquivalentTo(expected);
        }

        [TestCase(null, "incomplete")]
        [TestCase("", "incomplete")]
        [TestCase("123456789", "complete")]
        public async Task GetOrderSummaryAsync_OrderPartyOrganisationContactPhoneMissing_ReturnsOrderPartyIncomplete(string phoneValue, string expectedStatus)
        {
            const string orderId = "C0000014-01";
            var context = OrdersControllerTestContext.Setup();

            (Order order, OrderSummaryModel expected) = CreateOrderSummaryTestData(orderId, "Some Description", context.PrimaryOrganisationId);

            order.OrganisationContact.Phone = phoneValue;
            expected.Sections.Single(s => s.Id == "ordering-party").Status = expectedStatus;

            context.Order = order;

            using var controller = context.OrdersController;

            var result = await controller.GetOrderSummaryAsync(orderId) as OkObjectResult;
            var orderSummary = result.Value as OrderSummaryModel;
            orderSummary.Should().BeEquivalentTo(expected);
        }


        [Test]
        public async Task GetOrderSummaryAsync_OtherOrganisationId_ReturnResult()
        {
            var organisationId = Guid.NewGuid();
            const string orderId = "C0000014-01";
            var context = OrdersControllerTestContext.Setup();

            (Order order, OrderSummaryModel expected) = CreateOrderSummaryTestData(orderId, "Some Description", organisationId);

            context.Order = order;

            using var controller = context.OrdersController;

            var result = await controller.GetOrderSummaryAsync(orderId);
            result.Should().BeOfType<ForbidResult>();
        }

        [Test]
        public async Task CreateOrderAsync_CreateOrderSuccessfullResult_ReturnsOrderId()
        {
            const string newOrderId = "New Test Order Id";

            var context = OrdersControllerTestContext.Setup();
            context.CreateOrderResult = Result.Success(newOrderId);

            var createOrderRequest = new CreateOrderModel { Description = "Test Order 1", OrganisationId= context.PrimaryOrganisationId };

            using var controller = context.OrdersController;

            var response = await controller.CreateOrderAsync(createOrderRequest);

            var actual = response.Result;

            var expectation = new CreatedAtActionResult(nameof(controller.CreateOrderAsync).TrimAsync(),null, new { orderId = newOrderId }, new CreateOrderResponseModel { OrderId= newOrderId});

            actual.Should().BeEquivalentTo(expectation);
        }

        [Test]
        public async Task CreateOrderAsync_CreateOrderService_CreateAsync_CalledOnce()
        {
            var context = OrdersControllerTestContext.Setup();

            using var controller = context.OrdersController;

            var createOrderModel = new CreateOrderModel
            {
                Description = "Description1",
                OrganisationId = context.PrimaryOrganisationId
            };

            var response = await controller.CreateOrderAsync(createOrderModel);

            context.CreateOrderServiceMock.Verify(x => x.CreateAsync(It.IsAny<CreateOrderRequest>()), Times.Once);
        }

        [Test]
        public async Task CreateOrderAsync_CreateOrderFailureResult_ReturnsBadRequest()
        {
            var context = OrdersControllerTestContext.Setup();
            using var controller = context.OrdersController;

            var errors = new List<ErrorDetails> { new ErrorDetails("TestErrorId", "TestField") };

            var createOrderRequest = new CreateOrderModel { Description = "Test Order 1", OrganisationId = context.PrimaryOrganisationId };
                        
            context.CreateOrderResult = Result.Failure<string>(errors);
            
            var response = await controller.CreateOrderAsync(createOrderRequest);

            response.Should().BeOfType<ActionResult<CreateOrderResponseModel>>();
            var actual = response.Result;

            var expectedErrors =
                new List<ErrorModel> { new ErrorModel("TestErrorId", "TestField") };
            var expected = new BadRequestObjectResult(new CreateOrderResponseModel { Errors = expectedErrors });
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void CreateOrderAsync_NullApplicationUser_ThrowsException()
        {
            var context = OrdersControllerTestContext.Setup();

            async Task<ActionResult<CreateOrderResponseModel>> CreateOrder()
            {
                using var controller = context.OrdersController;
                return await controller.CreateOrderAsync(null);
            }

            Assert.ThrowsAsync<ArgumentNullException>(CreateOrder);
        }

        private static (Order order, OrderModel expectedOrder) CreateOrderTestData(string orderId, Guid organisationId, string description)
        {
            var repositoryOrder = OrderBuilder
                .Create()
                .WithOrderId(orderId)
                .WithOrganisationId(organisationId)
                .WithDescription(description)
                .Build();

            return (order: repositoryOrder,
                expectedOrder: new OrderModel
                {
                    OrderId = repositoryOrder.OrderId,
                    Description = repositoryOrder.Description.Value,
                    Status = repositoryOrder.OrderStatus.Name,
                    DateCreated = repositoryOrder.Created,
                    LastUpdated = repositoryOrder.LastUpdated,
                    LastUpdatedBy = repositoryOrder.LastUpdatedByName
                });
        }

        private static (Order order, OrderSummaryModel expectedSummary) CreateOrderSummaryTestData(string orderId, string description, Guid organisationId)
        {
            var repositoryOrder = OrderBuilder
                .Create()
                .WithOrderId(orderId)
                .WithDescription(description)
                .WithOrganisationId(organisationId)
                .Build();

            return (order: repositoryOrder,
                expectedSummary: new OrderSummaryModel
                {
                    OrderId = repositoryOrder.OrderId,
                    OrganisationId = repositoryOrder.OrganisationId,
                    Description = repositoryOrder.Description.Value,
                    Sections = new List<SectionModel>
                    {
                        new SectionModel
                        {
                            Id = "description",
                            Status = string.IsNullOrWhiteSpace(repositoryOrder.Description.Value) ? "incomplete" : "complete"
                        },
                        new SectionModel {Id = "ordering-party",Status = "complete"},
                        new SectionModel {Id = "supplier", Status = "incomplete"},
                        new SectionModel {Id = "commencement-date", Status = "incomplete"},
                        new SectionModel {Id = "associated-services", Status = "incomplete"},
                        new SectionModel {Id = "service-recipients", Status = "incomplete"},
                        new SectionModel {Id = "catalogue-solutions", Status = "incomplete"},
                        new SectionModel {Id = "additional-services", Status = "incomplete"},
                        new SectionModel {Id = "funding-source", Status = "incomplete"}
                    }
                });
        }

        internal sealed class OrdersControllerTestContext
        {
            private OrdersControllerTestContext()
            {
                Name = "Test User";
                NameIdentity = Guid.NewGuid();
                PrimaryOrganisationId = Guid.NewGuid();
                OrderRepositoryMock = new Mock<IOrderRepository>();

                CreateOrderServiceMock = new Mock<ICreateOrderService>();
                CreateOrderServiceMock.Setup(x => x.CreateAsync(It.IsAny<CreateOrderRequest>()))
                    .ReturnsAsync(() => CreateOrderResult);


                Orders = new List<Order>();
                OrderRepositoryMock.Setup(x => x.ListOrdersByOrganisationIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(() => Orders);

                OrderRepositoryMock.Setup(x => x.GetOrderByIdAsync(It.IsAny<string>())).ReturnsAsync(() => Order);

                ClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("Ordering", "Manage"),
                    new Claim("primaryOrganisationId", PrimaryOrganisationId.ToString()),
                    new Claim(ClaimTypes.Name,Name),
                    new Claim(ClaimTypes.NameIdentifier,NameIdentity.ToString())
                }, "mock")) ;              

                OrdersController = new OrdersController(OrderRepositoryMock.Object, CreateOrderServiceMock.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext { User = ClaimsPrincipal }
                    }
                };
            }

            internal string Name { get; }

            internal Guid NameIdentity { get; }

            internal Guid PrimaryOrganisationId { get; }

            internal ClaimsPrincipal ClaimsPrincipal { get; }

            internal Mock<IOrderRepository> OrderRepositoryMock { get; }

            internal Mock<ICreateOrderService> CreateOrderServiceMock { get; }

            internal Result<string> CreateOrderResult { get; set; } = Result.Success("NewOrderId");

            internal IEnumerable<Order> Orders { get; set; }

            internal Order Order { get; set; }

            internal OrdersController OrdersController { get; }

            internal static OrdersControllerTestContext Setup()
            {
                return new OrdersControllerTestContext();
            }
        }
    }
}
