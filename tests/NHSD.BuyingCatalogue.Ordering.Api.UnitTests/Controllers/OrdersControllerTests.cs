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
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Application.Services.CreateOrder;
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
        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        [TestCase(false, false, true)]
        public void Constructor_NullParameter_ThrowsArgumentNullException(
            bool isOrderRepositoryNull,
            bool isCreateOrderServiceNull,
            bool isServiceRecipientRepositoryNull)
        {
            void Test()
            {
                var _ = OrdersControllerBuilder
                    .Create()
                    .WithOrderRepository(isOrderRepositoryNull ? null : Mock.Of<IOrderRepository>())
                    .WithCreateOrderService(isCreateOrderServiceNull ? null : Mock.Of<ICreateOrderService>())
                    .WithServiceRecipientRepository(isServiceRecipientRepositoryNull ? null : Mock.Of<IServiceRecipientRepository>())
                    .Build();
            }

            Assert.Throws<ArgumentNullException>(Test);
        }

        [Test]
        public async Task GetAllAsync_NoOrdersExist_ReturnsEmptyResult()
        {
            var context = OrdersControllerTestContext.Setup();

            var controller = context.OrdersController;

            var result = await controller.GetAllAsync(context.PrimaryOrganisationId) as OkObjectResult;
            var orders = result.Value as List<OrderListItemModel>;
            orders.Should().BeEmpty();
        }

        [TestCase(null, "Some Description")]
        [TestCase("C0000014-01", "Some Description")]
        public async Task GetAllAsync_SingleOrderWithOrganisationIdExists_ReturnsTheOrder(string orderId,
            string orderDescription)
        {
            var context = OrdersControllerTestContext.Setup();
            var orders = new List<(Order order, OrderListItemModel expected)>
            {
                CreateOrderTestData(orderId, context.PrimaryOrganisationId, orderDescription)
            };

            context.Orders = orders.Select(x => x.order);

            var controller = context.OrdersController;

            var result = await controller.GetAllAsync(context.PrimaryOrganisationId) as OkObjectResult;
            var ordersResult = result.Value as List<OrderListItemModel>;
            ordersResult.Should().ContainSingle();
            ordersResult.Should().BeEquivalentTo(orders.Select(x => x.expected));
        }

        [Test]
        public async Task GetAllAsync_SingleOrderWithOtherOrganisationIdExists_ReturnsForbidden()
        {
            var otherOrganisationId = Guid.NewGuid();
            var context = OrdersControllerTestContext.Setup();
            var orders = new List<(Order order, OrderListItemModel expected)>
            {
                CreateOrderTestData("C0000014-01", otherOrganisationId, "A description")
            };

            context.Orders = orders.Select(x => x.order);

            var controller = context.OrdersController;

            var result = await controller.GetAllAsync(otherOrganisationId);
            result.Should().BeOfType<ForbidResult>();
        }

        [Test]
        public async Task GetAllAsync_MultipleOrdersWithOrganisationIdExist_ReturnsAllOrders()
        {
            var context = OrdersControllerTestContext.Setup();

            var orders = new List<(Order order, OrderListItemModel expected)>
            {
                CreateOrderTestData("C0000014-01", context.PrimaryOrganisationId, "Some Description"),
                CreateOrderTestData("C000012-01", context.PrimaryOrganisationId, "Another Description")
            };

            context.Orders = orders.Select(x => x.order);

            var controller = context.OrdersController;

            var result = await controller.GetAllAsync(context.PrimaryOrganisationId) as OkObjectResult;
            var ordersResult = result.Value as List<OrderListItemModel>;
            ordersResult.Count.Should().Be(2);
            ordersResult.Should().BeEquivalentTo(orders.Select(x => x.expected));
        }

        [Test]
        public async Task GetAll_OrdersByOrganisationId_CalledOnce()
        {
            var context = OrdersControllerTestContext.Setup();

            var controller = context.OrdersController;

            await controller.GetAllAsync(context.PrimaryOrganisationId);

            context.OrderRepositoryMock.Verify(x => x.ListOrdersByOrganisationIdAsync(context.PrimaryOrganisationId),
                Times.Once);
        }

        [Test]
        public async Task GetOrderSummaryAsync_OrderIdDoesNotExist_ReturnsNotFound()
        {
            var context = OrdersControllerTestContext.Setup();

            var controller = context.OrdersController;

            var response = await controller.GetOrderSummaryAsync("INVALID");
            response.Should().BeEquivalentTo(new ActionResult<OrderSummaryModel>(new NotFoundResult()));
        }

        [Test]
        public async Task GetOrderSummaryAsync_IsSummaryComplete_ReturnResult()
        {
            const string orderId = "C0000014-01";
            var context = OrdersControllerTestContext.Setup();

            (Order order, OrderSummaryModel expected) =
                CreateOrderSummaryTestData(orderId, "Some Description", context.PrimaryOrganisationId);

            context.Order = order;

            var controller = context.OrdersController;

            var response = await controller.GetOrderSummaryAsync(orderId);
            response.Should().BeEquivalentTo(new ActionResult<OrderSummaryModel>(new OkObjectResult(expected)));
        }

        [Test]
        public async Task GetOrderSummaryAsync_OtherOrganisationId_ReturnResult()
        {
            var organisationId = Guid.NewGuid();
            const string orderId = "C0000014-01";
            var context = OrdersControllerTestContext.Setup();

            (Order order, _) = CreateOrderSummaryTestData(orderId, "Some Description", organisationId);

            context.Order = order;

            var controller = context.OrdersController;

            var response = await controller.GetOrderSummaryAsync(orderId);
            response.Should().BeEquivalentTo(new ActionResult<OrderSummaryModel>(new ForbidResult()));
        }

        [TestCaseSource(typeof(SummaryModelSectionTestCaseData), nameof(SummaryModelSectionTestCaseData.OrderDescriptionSectionStatusCases))]
        [TestCaseSource(typeof(SummaryModelSectionTestCaseData), nameof(SummaryModelSectionTestCaseData.OrderingPartySectionStatusCases))]
        [TestCaseSource(typeof(SummaryModelSectionTestCaseData), nameof(SummaryModelSectionTestCaseData.SupplierSectionStatusCases))]
        [TestCaseSource(typeof(SummaryModelSectionTestCaseData), nameof(SummaryModelSectionTestCaseData.CommencementDateSectionStatusCases))]
        [TestCaseSource(typeof(SummaryModelSectionTestCaseData), nameof(SummaryModelSectionTestCaseData.ServiceRecipientsSectionStatusCases))]
        [TestCaseSource(typeof(SummaryModelSectionTestCaseData), nameof(SummaryModelSectionTestCaseData.CatalogueSolutionsSectionStatusCases))]
        public async Task GetOrderSummaryAsync_ChangeOrderData_ReturnsExpectedSummary(Order order,
            OrderSummaryModel expected)
        {
            var context = OrdersControllerTestContext.Setup(order.OrganisationId);
            context.Order = order;

            var controller = context.OrdersController;

            var response = (await controller.GetOrderSummaryAsync(context.Order.OrderId)).Result as OkObjectResult;
            Assert.IsNotNull(response);

            var actual = response.Value.As<OrderSummaryModel>();
            actual.Should().BeEquivalentTo(expected);
        }

        [TestCase]
        public async Task GetOrderSummaryAsync_ServiceRecipientCount_ReturnsCountOfTwo()
        {
            var order = OrderBuilder.Create().Build();

            var context = OrdersControllerTestContext.Setup(order.OrganisationId);
            context.Order = order;
            context.ServiceRecipientListCount = 2;

            var controller = context.OrdersController;

            string expectedOrderId = context.Order.OrderId;

            var response = (await controller.GetOrderSummaryAsync(expectedOrderId)).Result as OkObjectResult;
            Assert.IsNotNull(response);

            var actual = response.Value.As<OrderSummaryModel>();

            var expected = OrderSummaryModelBuilder
                .Create()
                .WithOrderId(expectedOrderId)
                .WithOrganisationId(order.OrganisationId)
                .WithSections(SectionModelListBuilder
                    .Create()
                    .WithServiceRecipients(
                        SectionModel
                            .ServiceRecipients
                            .WithStatus("incomplete")
                            .WithCount(context.ServiceRecipientListCount))
                    .Build())
                .Build();

            actual.Should().BeEquivalentTo(expected);
        }

        [TestCase]
        public async Task GetOrderSummaryAsync_ServiceRecipientRepository_CalledOnce()
        {
            var order = OrderBuilder.Create().Build();

            var context = OrdersControllerTestContext.Setup(order.OrganisationId);
            context.Order = order;
            context.ServiceRecipientListCount = 2;

            var controller = context.OrdersController;

            string expectedOrderId = context.Order.OrderId;

            await controller.GetOrderSummaryAsync(expectedOrderId);

            context.ServiceRecipientRepositoryMock.Verify(x => x.GetCountByOrderIdAsync(expectedOrderId), Times.Once);
        }

        [Test]
        public async Task CreateOrderAsync_CreateOrderSuccessfulResult_ReturnsOrderId()
        {
            const string newOrderId = "New Test Order Id";

            var context = OrdersControllerTestContext.Setup();
            context.CreateOrderResult = Result.Success(newOrderId);

            var createOrderRequest = new CreateOrderModel
            {
                Description = "Test Order 1",
                OrganisationId = context.PrimaryOrganisationId
            };

            var controller = context.OrdersController;

            var response = await controller.CreateOrderAsync(createOrderRequest);

            var actual = response.Result;

            var expectation = new CreatedAtActionResult(nameof(controller.CreateOrderAsync).TrimAsync(), null,
                new { orderId = newOrderId }, new CreateOrderResponseModel { OrderId = newOrderId });

            actual.Should().BeEquivalentTo(expectation);
        }

        [Test]
        public async Task CreateOrderAsync_CreateOrderService_CreateAsync_CalledOnce()
        {
            var context = OrdersControllerTestContext.Setup();

            var controller = context.OrdersController;

            var createOrderModel = new CreateOrderModel
            {
                Description = "Description1",
                OrganisationId = context.PrimaryOrganisationId
            };

            await controller.CreateOrderAsync(createOrderModel);

            context.CreateOrderServiceMock.Verify(x => x.CreateAsync(It.IsAny<CreateOrderRequest>()), Times.Once);
        }

        [Test]
        public async Task CreateOrderAsync_CreateOrderFailureResult_ReturnsBadRequest()
        {
            var context = OrdersControllerTestContext.Setup();
            var controller = context.OrdersController;

            var errors = new List<ErrorDetails> { new ErrorDetails("TestErrorId", "TestField") };

            var createOrderRequest = new CreateOrderModel
            {
                Description = "Test Order 1",
                OrganisationId = context.PrimaryOrganisationId
            };

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
                var controller = context.OrdersController;
                return await controller.CreateOrderAsync(null);
            }

            Assert.ThrowsAsync<ArgumentNullException>(CreateOrder);
        }

        private static (Order order, OrderListItemModel expectedOrder) CreateOrderTestData(
            string orderId,
            Guid organisationId,
            string description)
        {
            var repositoryOrder = OrderBuilder
                .Create()
                .WithOrderId(orderId)
                .WithOrganisationId(organisationId)
                .WithDescription(description)
                .Build();

            return (order: repositoryOrder,
                expectedOrder: new OrderListItemModel
                {
                    OrderId = repositoryOrder.OrderId,
                    Description = repositoryOrder.Description.Value,
                    Status = repositoryOrder.OrderStatus.Name,
                    DateCreated = repositoryOrder.Created,
                    LastUpdated = repositoryOrder.LastUpdated,
                    LastUpdatedBy = repositoryOrder.LastUpdatedByName
                });
        }

        private static (Order order, OrderSummaryModel expectedSummary) CreateOrderSummaryTestData(string orderId,
            string description, Guid organisationId)
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
                        SectionModel.Description.WithStatus(
                            string.IsNullOrWhiteSpace(repositoryOrder.Description.Value)
                                ? "incomplete"
                                : "complete"),
                        SectionModel.OrderingParty,
                        SectionModel.Supplier,
                        SectionModel.CatalogueSolutions,
                        SectionModel.AssociatedServices,
                        SectionModel.ServiceRecipients,
                        SectionModel.CatalogueSolutions,
                        SectionModel.AdditionalServices,
                        SectionModel.FundingSource
                    }
                });
        }

        internal sealed class OrdersControllerTestContext
        {
            private OrdersControllerTestContext(Guid primaryOrganisationId)
            {
                Name = "Test User";
                NameIdentity = Guid.NewGuid();
                PrimaryOrganisationId = primaryOrganisationId;
                OrderRepositoryMock = new Mock<IOrderRepository>();

                CreateOrderServiceMock = new Mock<ICreateOrderService>();
                CreateOrderServiceMock.Setup(x => x.CreateAsync(It.IsAny<CreateOrderRequest>()))
                    .ReturnsAsync(() => CreateOrderResult);

                Orders = new List<Order>();
                OrderRepositoryMock.Setup(x => x.ListOrdersByOrganisationIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(() => Orders);

                OrderRepositoryMock.Setup(x => x.GetOrderByIdAsync(It.IsAny<string>())).ReturnsAsync(() => Order);

                ServiceRecipientRepositoryMock = new Mock<IServiceRecipientRepository>();
                ServiceRecipientRepositoryMock
                    .Setup(x => x.GetCountByOrderIdAsync(It.IsNotNull<string>()))
                    .ReturnsAsync(() => ServiceRecipientListCount);

                ClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(
                    new[]
                    {
                        new Claim("Ordering", "Manage"),
                        new Claim("primaryOrganisationId", PrimaryOrganisationId.ToString()),
                        new Claim(ClaimTypes.Name, Name),
                        new Claim(ClaimTypes.NameIdentifier, NameIdentity.ToString())
                    }, "mock"));

                OrdersController = OrdersControllerBuilder
                    .Create()
                    .WithOrderRepository(OrderRepositoryMock.Object)
                    .WithServiceRecipientRepository(ServiceRecipientRepositoryMock.Object)
                    .WithCreateOrderService(CreateOrderServiceMock.Object)
                    .Build();

                OrdersController.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = ClaimsPrincipal }
                };
            }

            internal string Name { get; }

            internal Guid NameIdentity { get; }

            internal Guid PrimaryOrganisationId { get; }

            internal int ServiceRecipientListCount { get; set; }

            private ClaimsPrincipal ClaimsPrincipal { get; }

            internal Mock<IOrderRepository> OrderRepositoryMock { get; }

            internal Mock<ICreateOrderService> CreateOrderServiceMock { get; }

            internal Mock<IServiceRecipientRepository> ServiceRecipientRepositoryMock { get; }

            internal Result<string> CreateOrderResult { get; set; } = Result.Success("NewOrderId");

            internal IEnumerable<Order> Orders { get; set; }

            internal Order Order { get; set; }

            internal OrdersController OrdersController { get; }

            internal static OrdersControllerTestContext Setup()
            {
                return new OrdersControllerTestContext(Guid.NewGuid());
            }

            internal static OrdersControllerTestContext Setup(Guid primaryOrganisationId)
            {
                return new OrdersControllerTestContext(primaryOrganisationId);
            }
        }

        private class SummaryModelSectionTestCaseData
        {
            internal static IEnumerable<TestCaseData> OrderDescriptionSectionStatusCases
            {
                get
                {
                    var organisationId = Guid.NewGuid();

                    yield return new TestCaseData(
                        OrderBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithSupplierContact(null)
                            .WithOrganisationContact(null)
                            .WithCommencementDate(null)
                            .Build(),
                        OrderSummaryModelBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithSections(
                                SectionModelListBuilder
                                    .Create()
                                    .WithDescription(SectionModel.Description.WithStatus("complete"))
                                    .Build())
                            .Build());
                }
            }

            internal static IEnumerable<TestCaseData> OrderingPartySectionStatusCases
            {
                get
                {
                    var organisationId = Guid.NewGuid();

                    yield return new TestCaseData(
                        OrderBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithOrganisationContact(ContactBuilder.Create().Build())
                            .Build(),
                        OrderSummaryModelBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithSections(SectionModelListBuilder.Create()
                                .WithOrderingParty(SectionModel.OrderingParty.WithStatus("complete"))
                                .Build())
                            .Build());
                }
            }

            internal static IEnumerable<TestCaseData> SupplierSectionStatusCases
            {
                get
                {
                    var organisationId = Guid.NewGuid();

                    yield return new TestCaseData(
                        OrderBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithSupplierContact(ContactBuilder.Create().Build())
                            .Build(),
                        OrderSummaryModelBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithSections(SectionModelListBuilder.Create()
                                .WithSupplier(SectionModel.Supplier.WithStatus("complete"))
                                .Build())
                            .Build());
                }
            }

            internal static IEnumerable<TestCaseData> CommencementDateSectionStatusCases
            {
                get
                {
                    var organisationId = Guid.NewGuid();

                    yield return new TestCaseData(
                        OrderBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithCommencementDate(DateTime.UtcNow)
                            .Build(),
                        OrderSummaryModelBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithSections(SectionModelListBuilder.Create()
                                .WithCommencementDate(SectionModel.CommencementDate.WithStatus("complete"))
                                .Build())
                            .Build());
                }
            }

            internal static IEnumerable<TestCaseData> ServiceRecipientsSectionStatusCases
            {
                get
                {
                    var organisationId = Guid.NewGuid();

                    yield return new TestCaseData(
                        OrderBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithServiceRecipientsViewed(true)
                            .Build(),
                        OrderSummaryModelBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithSections(SectionModelListBuilder.Create()
                                .WithServiceRecipients(
                                    SectionModel
                                        .ServiceRecipients
                                        .WithStatus("complete")
                                        .WithCount(0))
                                .Build())
                            .Build());

                    yield return new TestCaseData(
                        OrderBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithServiceRecipientsViewed(false)
                            .Build(),
                        OrderSummaryModelBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithSections(SectionModelListBuilder
                                .Create()
                                .WithServiceRecipients(
                                    SectionModel
                                        .ServiceRecipients
                                        .WithStatus("incomplete")
                                        .WithCount(0))
                                .Build())
                            .Build());
                }
            }

            internal static IEnumerable<TestCaseData> CatalogueSolutionsSectionStatusCases
            {
                get
                {
                    var organisationId = Guid.NewGuid();

                    yield return new TestCaseData(
                        OrderBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithCatalogueSolutionsViewed(true)
                            .Build(),
                        OrderSummaryModelBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithSections(SectionModelListBuilder.Create()
                                .WithCatalogueSolutions(SectionModel.CatalogueSolutions.WithStatus("complete"))
                                .Build())
                            .Build());

                    yield return new TestCaseData(
                        OrderBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithCatalogueSolutionsViewed(false)
                            .Build(),
                        OrderSummaryModelBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithSections(SectionModelListBuilder.Create()
                                .WithCatalogueSolutions(SectionModel.CatalogueSolutions.WithStatus("incomplete"))
                                .Build())
                            .Build());
                }
            }

        }
    }
}
