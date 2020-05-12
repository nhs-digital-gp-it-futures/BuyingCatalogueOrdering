using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
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

            var result = await controller.GetAllAsync(Guid.Empty) as OkObjectResult;
            var orders = result.Value as List<OrderModel>;
            orders.Count.Should().Be(0);
        }

        [TestCase(null, null)]
        [TestCase("C0000014-01", "Some Description")]
        [TestCase("C0000014-02", null)]
        public async Task GetAllAsync_SingleOrderWithOrganisationIdExists_ReturnsTheOrder(string orderId, string orderDescription)
        {
            var organisationId = Guid.NewGuid();

            var orders = new List<(Order order, OrderModel expected)>
            {
                CreateOrderTestData(orderId, organisationId, orderDescription)
            };

            var context = OrdersControllerTestContext.Setup();
            context.Orders = orders.Select(x => x.order);

            using var controller = context.OrdersController;

            var result = await controller.GetAllAsync(organisationId) as OkObjectResult;
            var ordersResult = result.Value as List<OrderModel>;
            ordersResult.Count.Should().Be(1);
            ordersResult.Should().BeEquivalentTo(orders.Select(x => x.expected));
        }

        [Test]
        public async Task GetAllAsync_MultipleOrdersWithOrganisationIdExist_ReturnsAllOrders()
        {
            var organisationId = Guid.NewGuid();

            var orders = new List<(Order order, OrderModel expected)>
            {
                CreateOrderTestData("C0000014-01", organisationId, "Some Description"),
                CreateOrderTestData("C000012-01", organisationId, "Another Description")
            };

            var context = OrdersControllerTestContext.Setup();
            context.Orders = orders.Select(x => x.order);

            using var controller = context.OrdersController;

            var result = await controller.GetAllAsync(organisationId) as OkObjectResult;
            var ordersResult = result.Value as List<OrderModel>;
            ordersResult.Count.Should().Be(2);
            ordersResult.Should().BeEquivalentTo(orders.Select(x => x.expected));
        }

        private static (Order order, OrderModel expectedOrder) CreateOrderTestData(string orderId, Guid organisationId, string description)
        {
            var repositoryOrder = OrdersBuilder
                .Create()
                .WithOrderId(orderId)
                .WithOrganisationId(organisationId)
                .WithDescription(description)
                .Build();

            return (order: repositoryOrder,
                expectedOrder: new OrderModel
                {
                    OrderId = repositoryOrder.OrderId,
                    Description = repositoryOrder.Description,
                    Status = repositoryOrder.OrderStatus.Name,
                    LastUpdated = repositoryOrder.LastUpdated,
                    DateCreated = repositoryOrder.Created,
                    LastUpdatedBy = repositoryOrder.LastUpdatedBy
                });
        }

        private sealed class OrdersControllerTestContext
        {
            private OrdersControllerTestContext()
            {
                OrderRepositoryMock = new Mock<IOrderRepository>();

                Orders = new List<Order>();
                OrderRepositoryMock.Setup(x => x.ListOrdersByOrganisationIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(() => Orders);

                OrdersController = new OrdersController(OrderRepositoryMock.Object);

                OrderRepositoryMock.Setup(x => x.ListOrdersByOrganisationIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(() => Orders);
            }

            internal Mock<IOrderRepository> OrderRepositoryMock { get; }

            internal IEnumerable<Order> Orders { get; set; }

            internal OrdersController OrdersController { get; }

            internal static OrdersControllerTestContext Setup()
            {
                return new OrdersControllerTestContext();
            }
        }
    }
}
