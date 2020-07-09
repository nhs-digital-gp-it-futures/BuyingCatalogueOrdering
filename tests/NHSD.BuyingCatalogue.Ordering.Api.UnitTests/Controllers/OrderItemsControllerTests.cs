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
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class OrderItemsControllerTests
    {
        [Test]
        public void Constructor_Null_NullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var _ = new OrderItemsController(null);
            });
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
        public async Task GetAllAsync_OrderExistsWithFilter_ReturnsListOfOrderItems(string catalogueItemTypeFilter)
        {
            var context = OrderItemsControllerTestContext.Setup();

            var serviceRecipients = new List<(string Ods, string Name)>
            {
                ("eu", "EU test"),
                ("auz", null)
            };

            context.Order.SetServiceRecipient(serviceRecipients, Guid.Empty, String.Empty);

            var solutionOrderItem1 = CreateOrderItem(CatalogueItemType.Solution, serviceRecipients[0].Ods, 1);
            var solutionOrderItem2 = CreateOrderItem(CatalogueItemType.Solution, serviceRecipients[1].Ods, 2);
            var additionalServiceOrderItem1 = CreateOrderItem(CatalogueItemType.AdditionalService, serviceRecipients[1].Ods, 3);
            var associatedServiceOrderItem1 = CreateOrderItem(CatalogueItemType.AssociatedService, serviceRecipients[0].Ods, 4);

            context.Order.AddOrderItem(solutionOrderItem1, Guid.Empty, string.Empty);
            context.Order.AddOrderItem(solutionOrderItem2, Guid.Empty, string.Empty);
            context.Order.AddOrderItem(additionalServiceOrderItem1, Guid.Empty, string.Empty);
            context.Order.AddOrderItem(associatedServiceOrderItem1, Guid.Empty, string.Empty);

            const string orderId = "myOrder";
            var result = await context.OrderItemsController.GetAllAsync(orderId, catalogueItemTypeFilter);
            result.Value.Should().BeOfType<List<GetOrderItemModel>>();

            var expectedOrderItems =
                CreateOrderItemModel(
                    new List<OrderItem>
                    {
                        solutionOrderItem1,
                        solutionOrderItem2,
                        additionalServiceOrderItem1,
                        associatedServiceOrderItem1
                    }, orderId,
                    catalogueItemTypeFilter,
                    context.Order.ServiceRecipients);

            result.Value.Should().BeEquivalentTo(expectedOrderItems);
        }

        private OrderItem CreateOrderItem(CatalogueItemType catalogueItemType, string odsCode, int orderItemId)
        {
            return OrderItemBuilder.Create()
                .WithCatalogueItemType(catalogueItemType)
                .WithOrderItemId(orderItemId)
                .WithOdsCode(odsCode)
                .Build();
        }

        private static IEnumerable<GetOrderItemModel> CreateOrderItemModel(
            IEnumerable<OrderItem> orderItems,
            string orderId,
            string catalogueItemTypeFilter,
            IReadOnlyList<ServiceRecipient> serviceRecipients)
        {
            var items = orderItems.Select(x => new GetOrderItemModel
            {
                ItemId = $"{orderId}{x.OdsCode}{x.OrderItemId}",
                ServiceRecipient = new ServiceRecipientModel
                {
                    Name = serviceRecipients.FirstOrDefault(serviceRecipient => string.Equals(x.OdsCode,
                           serviceRecipient.OdsCode, StringComparison.InvariantCulture))?.Name,
                    OdsCode = x.OdsCode
                },
                CataloguePriceType = x.CataloguePriceType.Name,
                CatalogueItemType = x.CatalogueItemType.Name,
                CatalogueItemName = x.CatalogueItemName,
                CatalogueItemId = x.CatalogueItemId,
            });

            if (catalogueItemTypeFilter != null)
            {
                items = items.Where(x => x.CatalogueItemType == catalogueItemTypeFilter);
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
                .Build();

            OrderItemsController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = ClaimsPrincipal }
            };
        }

        internal Guid PrimaryOrganisationId { get; }
        private ClaimsPrincipal ClaimsPrincipal { get; }
        internal Mock<IOrderRepository> OrderRepositoryMock { get; }
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
