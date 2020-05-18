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
using NHSD.BuyingCatalogue.Ordering.Domain;
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
                var _ = new OrdersController(null);
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
                    LastUpdated = repositoryOrder.LastUpdated,
                    DateCreated = repositoryOrder.Created,
                    LastUpdatedBy = repositoryOrder.LastUpdatedBy
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
                            Id = "ordering-description",
                            Status = string.IsNullOrWhiteSpace(repositoryOrder.Description.Value) ? "incomplete" : "complete"
                        },
                        new SectionModel {Id = "ordering-party", Status = "incomplete"},
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

        private sealed class OrdersControllerTestContext
        {
            private OrdersControllerTestContext()
            {
                PrimaryOrganisationId = Guid.NewGuid();
                OrderRepositoryMock = new Mock<IOrderRepository>();

                Orders = new List<Order>();
                OrderRepositoryMock.Setup(x => x.ListOrdersByOrganisationIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(() => Orders);

                OrderRepositoryMock.Setup(x => x.GetOrderByIdAsync(It.IsAny<string>())).ReturnsAsync(() => Order);

                ClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("Ordering", "Manage"),
                    new Claim("primaryOrganisationId", PrimaryOrganisationId.ToString())
                }, "mock"));

                OrdersController = new OrdersController(OrderRepositoryMock.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext { User = ClaimsPrincipal }
                    }
                };
            }

            internal Guid PrimaryOrganisationId { get; }

            internal ClaimsPrincipal ClaimsPrincipal { get; }

            internal Mock<IOrderRepository> OrderRepositoryMock { get; }

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
