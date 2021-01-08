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
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class ServiceRecipientsSectionControllerTests
    {
        private static ServiceRecipientsModel DefaultServiceRecipientsModel
        {
            get
            {
                var service = ServiceRecipientsModelBuilder.Create()
                    .WithServiceRecipientModel(ServiceRecipientModelBuilder.Create().Build())
                    .Build();
                return service;
            }
        }

        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(false, false)]
        public void Ctor_NullRepository_Throws(bool hasOrderRepository, bool hasServiceRepository)
        {
            var context = ServiceRecipientsTestContext.Setup();
            var orderRepository = context.OrderRepositoryMock.Object;
            var serviceRecipientRepository = context.ServiceRecipientRepositoryMock.Object;

            if (!hasOrderRepository)
                orderRepository = null;
            if (!hasServiceRepository)
                serviceRecipientRepository = null;

            Assert.Throws<ArgumentNullException>(() =>
            {
                var _ = new ServiceRecipientsSectionController(orderRepository, serviceRecipientRepository);
            });
        }

        [TestCase(null)]
        [TestCase("INVALID")]
        public async Task GetAllAsync_OrderDoesNotExist_ReturnsNotFound(string orderId)
        {
            var context = ServiceRecipientsTestContext.Setup();
            context.Order = null;

            var response = await context.Controller.GetAllAsync(orderId);
            response.Should().BeEquivalentTo(new ActionResult<ServiceRecipientsModel>(new NotFoundResult()));
        }

        [Test]
        public async Task GetAllAsync_OrganisationIdDoesNotMatch_ReturnsForbidden()
        {
            var context = ServiceRecipientsTestContext.Setup();
            context.Order = OrderBuilder
                .Create()
                .WithOrganisationId(Guid.NewGuid())
                .Build();

            var response = await context.Controller.GetAllAsync("myOrder");
            response.Should().BeEquivalentTo(new ActionResult<ServiceRecipientsModel>(new ForbidResult()));
        }

        [Test]
        public async Task GetAllAsync_NoServiceRecipient_ReturnsEmptyList()
        {
            var context = ServiceRecipientsTestContext.Setup();
            var expected = new ServiceRecipientsModel
            {
                ServiceRecipients = new List<ServiceRecipientModel>(),
            };

            var response = await context.Controller.GetAllAsync("myOrder");
            response.Should().BeEquivalentTo(new ActionResult<ServiceRecipientsModel>(expected));
        }

        [Test]
        public async Task GetAllAsync_SingleServiceRecipient_ReturnsTheRecipient()
        {
            var context = ServiceRecipientsTestContext.Setup();

            const string orderId = "C0000014-01";

            var serviceRecipients = new List<(ServiceRecipient serviceRecipient, ServiceRecipientModel expectedModel)>
            {
                CreateServiceRecipientData("ODS1", orderId, "name"),
            };

            context.ServiceRecipients = serviceRecipients.Select(x => x.serviceRecipient).ToList();

            var expectedList = serviceRecipients.Select(x => x.expectedModel);

            var expected = new ServiceRecipientsModel
            {
                ServiceRecipients = expectedList,
            };

            var response = await context.Controller.GetAllAsync(orderId);
            response.Should().BeEquivalentTo(new ActionResult<ServiceRecipientsModel>(expected));
        }

        [Test]
        public async Task GetAllAsync_MultipleServiceRecipientsMatch_ReturnsAllTheOrdersServicesRecipients()
        {
            var context = ServiceRecipientsTestContext.Setup();

            const string orderId = "C0000014-01";

            context.Order = OrderBuilder.Create().WithOrderId(orderId).WithOrganisationId(context.PrimaryOrganisationId).Build();

            var serviceRecipients = new List<(ServiceRecipient serviceRecipient, ServiceRecipientModel expectedModel)>
            {
                CreateServiceRecipientData("ODS1", orderId, "Test"),
                CreateServiceRecipientData("ODS2", orderId, "Service recipient"),
                CreateServiceRecipientData("ODS3", orderId, "Data"),
            };

            context.ServiceRecipients = serviceRecipients.Select(x => x.serviceRecipient).ToList();
            var expected = new ServiceRecipientsModel();

            var expectedList = serviceRecipients.Select(x => x.expectedModel);

            expected.ServiceRecipients = expectedList.OrderBy(serviceRecipient => serviceRecipient.Name);

            var response = await context.Controller.GetAllAsync(orderId);
            response.Should().BeEquivalentTo(new ActionResult<ServiceRecipientsModel>(expected));
        }

        [Test]
        public async Task GetAllAsync_VerifyRepositoryMethods_CalledOnce()
        {
            var context = ServiceRecipientsTestContext.Setup();

            await context.Controller.GetAllAsync(string.Empty);

            context.OrderRepositoryMock.Verify(x => x.GetOrderByIdAsync(string.Empty), Times.Once);
            context.ServiceRecipientRepositoryMock.Verify(x => x.ListServiceRecipientsByOrderIdAsync(string.Empty),
                Times.Once);
        }

        [TestCase(null)]
        [TestCase("INVALID")]
        public async Task UpdateAsync_OrderDoesNotExist_ReturnsNotFound(string orderId)
        {
            var context = ServiceRecipientsTestContext.Setup();
            context.Order = null;

            var response = await context.Controller.UpdateAsync(orderId, DefaultServiceRecipientsModel);
            response.Should().BeEquivalentTo(new NotFoundResult());
        }

        [Test]
        public async Task UpdateAsync_OrganisationIdDoesNotMatch_ReturnsForbidden()
        {
            var context = ServiceRecipientsTestContext.Setup();
            context.Order = OrderBuilder
                .Create()
                .WithOrganisationId(Guid.NewGuid())
                .Build();

            var response = await context.Controller.UpdateAsync("myOrder", DefaultServiceRecipientsModel);
            response.Should().BeEquivalentTo(new ForbidResult());
        }

        [Test]
        public async Task UpdateAsync_DefaultServiceRecipient_ServiceRecipientViewedIsTrue()
        {
            var context = ServiceRecipientsTestContext.Setup();

            context.Order = OrderBuilder
                .Create()
                .WithOrganisationId(context.PrimaryOrganisationId)
                .WithServiceRecipientsViewed(false)
                .Build();

            context.Order.ServiceRecipientsViewed.Should().BeFalse();

            await context.Controller.UpdateAsync(context.Order.OrderId, DefaultServiceRecipientsModel);

            context.Order.ServiceRecipientsViewed.Should().BeTrue();
        }

        [Test]
        public async Task UpdateAsync_DefaultServiceRecipient_LastUpdatedByChanged()
        {
            var context = ServiceRecipientsTestContext.Setup();

            var lastUpdatedBy = Guid.NewGuid();
            context.Order = OrderBuilder
                .Create()
                .WithLastUpdatedBy(lastUpdatedBy)
                .WithOrganisationId(context.PrimaryOrganisationId)
                .WithServiceRecipientsViewed(false)
                .Build();

            context.Order.LastUpdatedBy.Should().Be(lastUpdatedBy);

            await context.Controller.UpdateAsync(context.Order.OrderId, DefaultServiceRecipientsModel);

            context.Order.LastUpdatedBy.Should().Be(context.UserId);
        }

        [Test]
        public async Task UpdateAsync_DefaultServiceRecipient_LastUpdatedByNameChanged()
        {
            var context = ServiceRecipientsTestContext.Setup();

            var lastUpdatedByName = "Some user";
            context.Order = OrderBuilder
                .Create()
                .WithLastUpdatedByName(lastUpdatedByName)
                .WithOrganisationId(context.PrimaryOrganisationId)
                .WithServiceRecipientsViewed(false)
                .Build();

            context.Order.LastUpdatedByName.Should().Be(lastUpdatedByName);

            await context.Controller.UpdateAsync(context.Order.OrderId, DefaultServiceRecipientsModel);

            context.Order.LastUpdatedByName.Should().Be(context.Username);
        }

        [Test]
        public async Task UpdateAsync_NoServiceRecipients_SetsCatalogueSolutionsViewedFalse()
        {
            var context = ServiceRecipientsTestContext.Setup();
            context.Order = OrderBuilder
                .Create()
                .WithOrganisationId(context.PrimaryOrganisationId)
                .WithCatalogueSolutionsViewed(true)
                .Build();

            var service = ServiceRecipientsModelBuilder.Create().Build();

            string expectedOrderId = context.Order.OrderId;
            await context.Controller.UpdateAsync(expectedOrderId, service);

            context.Order.CatalogueSolutionsViewed.Should().BeFalse();
        }

        [Test]
        public async Task UpdateAsync_OrderRepository_UpdateOrderAsyncCalledOnce()
        {
            var context = ServiceRecipientsTestContext.Setup();

            var expectedOrder = context.Order;
            await context.Controller.UpdateAsync(expectedOrder.OrderId, DefaultServiceRecipientsModel);

            context.OrderRepositoryMock.Verify(x => x.UpdateOrderAsync(expectedOrder), Times.Once);
        }

        [Test]
        public async Task UpdateAsync_OrderRepository_GetOrderByIdAsyncCalledOnce()
        {
            var context = ServiceRecipientsTestContext.Setup();

            string expectedOrderId = context.Order.OrderId;
            await context.Controller.UpdateAsync(expectedOrderId, DefaultServiceRecipientsModel);

            context.OrderRepositoryMock.Verify(x => x.GetOrderByIdAsync(expectedOrderId), Times.Once);
        }

        [Test]
        public async Task UpdateAsync_ServiceRecipientRepository_UpdateAsyncCalledOnce()
        {
            var context = ServiceRecipientsTestContext.Setup();

            string expectedOrderId = context.Order.OrderId;
            await context.Controller.UpdateAsync(expectedOrderId, DefaultServiceRecipientsModel);

            context.ServiceRecipientRepositoryMock.Verify(x => x.UpdateAsync(expectedOrderId, It.IsAny<IEnumerable<ServiceRecipient>>()), Times.Once);
        }

        private static (ServiceRecipient serviceRecipient, ServiceRecipientModel expectedModel)
            CreateServiceRecipientData(string odsCode, string orderId, string name)
        {
            var serviceRecipient = ServiceRecipientBuilder
                .Create()
                .WithOdsCode(odsCode)
                .WithOrderId(orderId)
                .WithName(name)
                .Build();

            return (serviceRecipient,
                new ServiceRecipientModel { OdsCode = serviceRecipient.OdsCode, Name = serviceRecipient.Name });
        }

        private sealed class ServiceRecipientsTestContext
        {
            private ServiceRecipientsTestContext()
            {
                PrimaryOrganisationId = Guid.NewGuid();
                UserId = Guid.NewGuid();
                Username = "Test User";
                Order = OrderBuilder
                    .Create()
                    .WithOrganisationId(PrimaryOrganisationId)
                    .Build();

                OrderRepositoryMock = new Mock<IOrderRepository>();
                OrderRepositoryMock.Setup(x => x.GetOrderByIdAsync(It.IsAny<string>())).ReturnsAsync(() => Order);

                ServiceRecipientRepositoryMock = new Mock<IServiceRecipientRepository>();
                ServiceRecipients = new List<ServiceRecipient>();
                ServiceRecipientRepositoryMock.Setup(x => x.ListServiceRecipientsByOrderIdAsync(It.IsAny<string>()))
                    .ReturnsAsync(() => ServiceRecipients);

                ClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("Ordering", "Manage"),
                    new Claim("primaryOrganisationId", PrimaryOrganisationId.ToString()),
                    new Claim(ClaimTypes.Name, Username),
                    new Claim(ClaimTypes.NameIdentifier, UserId.ToString()),
                },
                "mock"));

                Controller = new ServiceRecipientsSectionController(OrderRepositoryMock.Object, ServiceRecipientRepositoryMock.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext { User = ClaimsPrincipal },
                    },
                };
            }

            internal Guid PrimaryOrganisationId { get; }

            internal Mock<IOrderRepository> OrderRepositoryMock { get; }

            internal Mock<IServiceRecipientRepository> ServiceRecipientRepositoryMock { get; }

            internal Order Order { get; set; }

            internal IEnumerable<ServiceRecipient> ServiceRecipients { get; set; }

            internal ServiceRecipientsSectionController Controller { get; }

            internal Guid UserId { get; }

            internal string Username { get; }

            private ClaimsPrincipal ClaimsPrincipal { get; }

            internal static ServiceRecipientsTestContext Setup() => new();
        }
    }
}
