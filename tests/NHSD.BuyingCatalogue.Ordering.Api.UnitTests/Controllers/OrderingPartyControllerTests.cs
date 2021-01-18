using System;
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
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class OrderingPartyControllerTests
    {
        [Test]
        public static void Constructor_Null_NullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new OrderingPartyController(null));
        }

        [Test]
        public static async Task GetAsync_OrderIdDoesNotExist_ReturnsNotFound()
        {
            var context = OrderingPartyTestContext.Setup();

            var controller = context.OrderingPartyController;

            var response = await controller.GetAsync("INVALID");

            response.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public static async Task GetAsync_WrongOrganisationId_ReturnsForbidden()
        {
            var context = OrderingPartyTestContext.Setup();

            const string orderId = "C0000014-01";
            (Order order, _) = CreateOrderingPartyTestData(orderId, Guid.NewGuid());

            context.Order = order;

            var controller = context.OrderingPartyController;

            var result = await controller.GetAsync(orderId);

            result.Result.Should().BeOfType<ForbidResult>();
        }

        [Test]
        public static async Task GetAsync_EmptyOrderingParty_ReturnsEmptyResult()
        {
            const string orderId = "C0000014-01";
            var context = OrderingPartyTestContext.Setup();

            (Order order, OrderingPartyModel _) = CreateOrderingPartyTestData(orderId, context.PrimaryOrganisationId, false);

            context.Order = order;

            var controller = context.OrderingPartyController;

            var result = await controller.GetAsync(orderId);

            result.Result.Should().BeOfType<OkResult>();
        }

        [Test]
        public static async Task GetAsync_OrderIdExists_ReturnsTheOrderingParty()
        {
            const string orderId = "C0000014-01";
            var context = OrderingPartyTestContext.Setup();

            (Order order, OrderingPartyModel expectedOrderingParty) = CreateOrderingPartyTestData(orderId, context.PrimaryOrganisationId);

            context.Order = order;

            var controller = context.OrderingPartyController;

            var result = await controller.GetAsync(orderId);

            result.Should()
                .BeEquivalentTo(new ActionResult<OrderingPartyModel>(new OkObjectResult(expectedOrderingParty)));
        }

        [Test]
        public static async Task GetAsync_GetOrderById_CalledOnce()
        {
            var context = OrderingPartyTestContext.Setup();

            var controller = context.OrderingPartyController;

            await controller.GetAsync(string.Empty);

            context.OrderRepositoryMock.Verify(x => x.GetOrderByIdAsync(string.Empty), Times.Once);
        }

        [TestCase(null)]
        [TestCase("INVALID")]
        public static async Task UpdateAsync_OrderIdDoesNotExist_ReturnNotFound(string orderId)
        {
            var context = OrderingPartyTestContext.Setup();

            var controller = context.OrderingPartyController;

            var response =
                await controller.UpdateAsync(orderId, new OrderingPartyModel { PrimaryContact = new PrimaryContactModel() });

            response.Should().BeEquivalentTo(new NotFoundResult());
        }

        [Test]
        public static void UpdateAsync_ModelIsNull_ThrowsNullArgumentException()
        {
            static async Task GetOrderingPartyWithModelModel()
            {
                var context = OrderingPartyTestContext.Setup();

                var controller = context.OrderingPartyController;
                await controller.UpdateAsync("OrderId", null);
            }

            Assert.ThrowsAsync<ArgumentNullException>(GetOrderingPartyWithModelModel);
        }

        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(false, false)]
        public static void UpdateAsync_NullAddressOrContact_ThrowsNullArgumentException(bool hasPrimaryContact, bool hasAddress)
        {
            const string orderId = "C0000014-01";
            var context = OrderingPartyTestContext.Setup();

            (Order order, OrderingPartyModel _) = CreateOrderingPartyTestData(orderId, context.PrimaryOrganisationId);

            context.Order = order;

            var controller = context.OrderingPartyController;

            Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                _ = await controller.UpdateAsync(
                    orderId,
                    new OrderingPartyModel
                    {
                        Name = "New Description",
                        OdsCode = "NewODS",
                        PrimaryContact = hasPrimaryContact ? new PrimaryContactModel() : null,
                        Address = hasAddress ? new AddressModel() : null,
                    });
            });
        }

        [Test]
        public static async Task UpdateAsync_UpdateIsValid_ReturnsNoContent()
        {
            const string orderId = "C0000014-01";
            var context = OrderingPartyTestContext.Setup();

            (Order order, OrderingPartyModel _) = CreateOrderingPartyTestData(orderId, context.PrimaryOrganisationId);

            context.Order = order;

            var controller = context.OrderingPartyController;

            var response = await controller.UpdateAsync(
                orderId,
                new OrderingPartyModel
                {
                    Name = "New Description",
                    OdsCode = "New",
                    PrimaryContact = new PrimaryContactModel(),
                    Address = new AddressModel(),
                });

            response.Should().BeOfType<NoContentResult>();
        }

        private static (Order Order, OrderingPartyModel ExpectedOrderingParty) CreateOrderingPartyTestData(
            string orderId,
            Guid organisationId,
            bool hasOrganisationContact = true)
        {
            var repositoryOrder = OrderBuilder
                .Create()
                .WithOrderId(orderId)
                .WithOrganisationId(organisationId)
                .WithOrganisationContact(hasOrganisationContact ? ContactBuilder.Create().Build() : null)
                .Build();

            var orderingPartyAddress = repositoryOrder.OrganisationAddress;

            return (repositoryOrder, new OrderingPartyModel
            {
                Name = repositoryOrder.OrganisationName,
                OdsCode = repositoryOrder.OrganisationOdsCode,
                Address = new AddressModel
                {
                    Line1 = orderingPartyAddress.Line1,
                    Line2 = orderingPartyAddress.Line2,
                    Line3 = orderingPartyAddress.Line3,
                    Line4 = orderingPartyAddress.Line4,
                    Line5 = orderingPartyAddress.Line5,
                    Town = orderingPartyAddress.Town,
                    County = orderingPartyAddress.County,
                    Postcode = orderingPartyAddress.Postcode,
                    Country = orderingPartyAddress.Country,
                },
                PrimaryContact = !hasOrganisationContact ? null : new PrimaryContactModel
                {
                    FirstName = repositoryOrder.OrganisationContact.FirstName,
                    LastName = repositoryOrder.OrganisationContact.LastName,
                    EmailAddress = repositoryOrder.OrganisationContact.Email,
                    TelephoneNumber = repositoryOrder.OrganisationContact.Phone,
                },
            });
        }

        private sealed class OrderingPartyTestContext
        {
            private OrderingPartyTestContext()
            {
                PrimaryOrganisationId = Guid.NewGuid();

                OrderRepositoryMock = new Mock<IOrderRepository>();
                OrderRepositoryMock.Setup(x => x.GetOrderByIdAsync(It.IsAny<string>())).ReturnsAsync(() => Order);
                ClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(
                    new[]
                    {
                        new Claim("Ordering", "Manage"),
                        new Claim("primaryOrganisationId", PrimaryOrganisationId.ToString()),
                        new Claim(ClaimTypes.Name, "Test User"),
                        new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                    },
                    "mock"));

                OrderingPartyController = new OrderingPartyController(OrderRepositoryMock.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext { User = ClaimsPrincipal },
                    },
                };
            }

            internal Guid PrimaryOrganisationId { get; }

            internal Mock<IOrderRepository> OrderRepositoryMock { get; }

            internal Order Order { get; set; }

            internal OrderingPartyController OrderingPartyController { get; }

            private ClaimsPrincipal ClaimsPrincipal { get; }

            internal static OrderingPartyTestContext Setup() => new();
        }
    }
}
