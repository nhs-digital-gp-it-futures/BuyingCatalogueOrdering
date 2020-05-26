﻿using System;
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
    internal sealed class OrderingPartyControllerTests
    {
        [Test]
        public void Constructor_Null_NullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var _ = new OrderingPartyController(null);
            });
        }

        [Test]
        public async Task GetAsync_OrderIdDoesNotExist_ReturnsNotFound()
        {
            var context = OrderingPartyTestContext.Setup();

            using var controller = context.OrderingPartyController;

            var response = await controller.GetAsync("INVALID");
            response.Should().BeEquivalentTo(new NotFoundResult());
        }

        [Test]
        public async Task GetAsync_WrongOrganisationId_ReturnsForbidden()
        {
            var context = OrderingPartyTestContext.Setup();

            var orderId = "C0000014-01";
            var testData = CreateOrderingPartyTestData(orderId, Guid.NewGuid());

            context.Order = testData.order;

            using var controller = context.OrderingPartyController;

            var result = await controller.GetAsync(orderId);
            result.Should().BeOfType<ForbidResult>();
        }

        [Test]
        public async Task GetAsync_OrderIdExists_ReturnsTheOrderingParty()
        {
            var orderId = "C0000014-01";
            var context = OrderingPartyTestContext.Setup();

            var testData = CreateOrderingPartyTestData(orderId, context.PrimaryOrganisationId);

            context.Order = testData.order;

            using var controller = context.OrderingPartyController;

            var result = await controller.GetAsync(orderId) as OkObjectResult;

            var orderingParty = result.Value as OrderingPartyModel;
            orderingParty.Should().BeEquivalentTo(testData.expectedOrderingParty);
        }

        [Test]
        public async Task GetAsync_GetOrderById_CalledOnce()
        {
            var context = OrderingPartyTestContext.Setup();

            using var controller = context.OrderingPartyController;

            await controller.GetAsync(string.Empty);

            context.OrderRepositoryMock.Verify(x => x.GetOrderByIdAsync(string.Empty), Times.Once);
        }

        private static (Order order, OrderingPartyModel expectedOrderingParty) CreateOrderingPartyTestData(string orderId, Guid organisationId)
        {
            var repositoryOrder = OrderBuilder
                .Create()
                .WithOrderId(orderId)
                .WithOrganisationId(organisationId)
                .Build();

            return (order: repositoryOrder,
                expectedOrderingParty: new OrderingPartyModel
                {
                    Organisation = new OrganisationModel
                    {
                        Name = repositoryOrder.OrganisationName,
                        OdsCode = repositoryOrder.OrganisationOdsCode,
                        Address = new AddressModel
                        {
                            Line1 = repositoryOrder.OrganisationAddress.Line1,
                            Line2 = repositoryOrder.OrganisationAddress.Line2,
                            Line3 = repositoryOrder.OrganisationAddress.Line3,
                            Line4 = repositoryOrder.OrganisationAddress.Line4,
                            Line5 = repositoryOrder.OrganisationAddress.Line5,
                            Town = repositoryOrder.OrganisationAddress.Town,
                            County = repositoryOrder.OrganisationAddress.County,
                            Postcode = repositoryOrder.OrganisationAddress.Postcode,
                            Country = repositoryOrder.OrganisationAddress.Country
                        }
                    },
                    PrimaryContact = new PrimaryContactModel
                    {
                        FirstName = repositoryOrder.OrganisationContact.FirstName,
                        LastName = repositoryOrder.OrganisationContact.LastName,
                        EmailAddress = repositoryOrder.OrganisationContact.Email,
                        TelephoneNumber = repositoryOrder.OrganisationContact.Phone
                    }
                });
        }

        private sealed class OrderingPartyTestContext
        {
            private OrderingPartyTestContext()
            {
                PrimaryOrganisationId = Guid.NewGuid();

                OrderRepositoryMock = new Mock<IOrderRepository>();
                OrderRepositoryMock.Setup(x => x.GetOrderByIdAsync(It.IsAny<string>())).ReturnsAsync(() => Order);
                ClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("Ordering", "Manage"),
                    new Claim("primaryOrganisationId", PrimaryOrganisationId.ToString()),
                    new Claim("name", "Test User")
                }, "mock"));

                OrderingPartyController = new OrderingPartyController(OrderRepositoryMock.Object)
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

            internal Order Order { get; set; }

            internal OrderingPartyController OrderingPartyController { get; }

            internal static OrderingPartyTestContext Setup()
            {
                return new OrderingPartyTestContext();
            }
        }
    }
}
