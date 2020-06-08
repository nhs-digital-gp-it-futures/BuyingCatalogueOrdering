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
    internal sealed class ServiceRecipientsSectionControllerTests
    {
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
            context.Order.OrganisationId = Guid.NewGuid();

            var response = await context.Controller.GetAllAsync("myOrder");
            response.Should().BeEquivalentTo(new ActionResult<ServiceRecipientsModel>(new ForbidResult()));
        }

        [Test]
        public async Task GetAllAsync_NoServiceRecipient_ReturnsEmptyList()
        {
            var context = ServiceRecipientsTestContext.Setup();
            var expected = new ServiceRecipientsModel
            {
                ServiceRecipients = new List<ServiceRecipientModel>()
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
                CreateServiceRecipientData("ODS1", orderId)
            };

            context.ServiceRecipients = serviceRecipients.Select(x => x.serviceRecipient).ToList();

        
            var expectedList = serviceRecipients.Select(x => x.expectedModel);

            var expected = new ServiceRecipientsModel
            {
                ServiceRecipients = expectedList
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
                CreateServiceRecipientData("ODS1", orderId),
                CreateServiceRecipientData("ODS2", orderId),
                CreateServiceRecipientData("ODS3", orderId)
            };

            context.ServiceRecipients = serviceRecipients.Select(x => x.serviceRecipient).ToList();
            var expected = new ServiceRecipientsModel();

            var expectedList = serviceRecipients.Select(x => x.expectedModel);

            expected.ServiceRecipients = expectedList;

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
            context.Order.OrganisationId = Guid.NewGuid();

            var response = await context.Controller.UpdateAsync("myOrder", DefaultServiceRecipientsModel);
            response.Should().BeEquivalentTo(new ForbidResult());
        }

        [Test]
        public async Task UpdateAsync_VerifyRepositoryMethods_CalledOnce()
        {
            var context = ServiceRecipientsTestContext.Setup();

            await context.Controller.UpdateAsync(context.Order.OrderId,DefaultServiceRecipientsModel);

            context.OrderRepositoryMock.Verify(x => x.GetOrderByIdAsync(context.Order.OrderId), Times.Once);
            context.ServiceRecipientRepositoryMock.Verify(x => x.UpdateServiceRecipientsAsync(context.Order,It.IsAny<IEnumerable<ServiceRecipient>>()), Times.Once);
        }

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

        private static (ServiceRecipient serviceRecipient, ServiceRecipientModel expectedModel)
            CreateServiceRecipientData(string odsCode, string orderId)
        {
            var serviceRecipient = ServiceRecipientBuilder
                .Create()
                .WithOdsCode(odsCode)
                .WithOrderId(orderId)
                .Build();

            return (serviceRecipient,
                new ServiceRecipientModel { OdsCode = serviceRecipient.OdsCode, Name = serviceRecipient.Name });
        }

        private sealed class ServiceRecipientsTestContext
        {
            private ServiceRecipientsTestContext()
            {
                PrimaryOrganisationId = Guid.NewGuid();
                Order = new Order { OrganisationId = PrimaryOrganisationId };

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
                    new Claim(ClaimTypes.Name, "Test User"),
                    new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
                },
                "mock"));

                Controller = new ServiceRecipientsSectionController(OrderRepositoryMock.Object, ServiceRecipientRepositoryMock.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext { User = ClaimsPrincipal }
                    }
                };
            }

            internal Guid PrimaryOrganisationId { get; }

            internal Mock<IOrderRepository> OrderRepositoryMock { get; }

            internal Mock<IServiceRecipientRepository> ServiceRecipientRepositoryMock { get; }

            internal Order Order { get; set; }

            internal IEnumerable<ServiceRecipient> ServiceRecipients { get; set; }

            internal ServiceRecipientsSectionController Controller { get; }

            private ClaimsPrincipal ClaimsPrincipal { get; }

            internal static ServiceRecipientsTestContext Setup()
            {
                return new ServiceRecipientsTestContext();
            }
        }
    }
}
