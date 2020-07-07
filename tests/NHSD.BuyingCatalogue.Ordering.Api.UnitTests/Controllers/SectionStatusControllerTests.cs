using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Summary;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrder;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class SectionStatusControllerTests
    {
        [Test]
        public void Constructor_NullParameter_ThrowsArgumentNullException()
        {
            var sectonControllerBuilder = SectionStatusControllerBuilder
                .Create()
                .WithOrderRepository(null);
            Assert.Throws<ArgumentNullException>(() => sectonControllerBuilder.Build());
        }

        [Test]
        public async Task UpdateStatusAsync_OrderDoesNotExist_ReturnsNotFound()
        {
            var context = SectionStatusControllerTestContext.Setup();

            var response =
                await context.SectionStatusController.UpdateStatusAsync("INVALID", SectionModel.AdditionalServices.Id,new SectionStatusRequestModel{Status = "complete"});
            response.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task UpdateStatusAsync_WrongOrganisationId_ReturnsForbidden()
        {
            var context = SectionStatusControllerTestContext.Setup();

            const string orderId = "C0000014-01";
            context.Order = CreateGetTestData(orderId, Guid.NewGuid(), "ods");

            var response = await context.SectionStatusController.UpdateStatusAsync(orderId, SectionModel.AdditionalServices.Id, new SectionStatusRequestModel { Status = "complete" }); 
            response.Should().BeOfType<ForbidResult>();
        }

        [Test]
        public async Task UpdateStatusAsync_WrongSectionId_ReturnsForbidden()
        {
            var context = SectionStatusControllerTestContext.Setup();

            const string orderId = "C0000014-01";
            context.Order = CreateGetTestData(orderId, context.PrimaryOrganisationId, "ods");

            var response = await context.SectionStatusController.UpdateStatusAsync(orderId, "unknown-section", new SectionStatusRequestModel { Status = "complete" });
            response.Should().BeOfType<ForbidResult>();
        }

        [TestCase("additional-services")]
        [TestCase("catalogue-solutions")]
        public async Task UpdateStatusAsync_CorrectSectionId_ReturnsNoContentResult(string sectionName)
        {
            var context = SectionStatusControllerTestContext.Setup();

            const string orderId = "C0000014-01";
            context.Order = CreateGetTestData(orderId, context.PrimaryOrganisationId, "ods");

            var response = await context.SectionStatusController.UpdateStatusAsync(orderId, sectionName, new SectionStatusRequestModel { Status = "complete" });
            response.Should().BeOfType<NoContentResult>();
        }

        [Test]
        public async Task UpdateStatusAsync_WithAdditionalServicesSectionId_UpdatesAdditionalServicesViewedField()
        {
            var context = SectionStatusControllerTestContext.Setup();

            const string orderId = "C0000014-01";
            context.Order = CreateGetTestData(orderId, context.PrimaryOrganisationId, "ods");

            var response = await context.SectionStatusController.UpdateStatusAsync(orderId, "additional-services", new SectionStatusRequestModel { Status = "complete" });
            response.Should().BeOfType<NoContentResult>();
            context.OrderRepositoryMock.Verify(u=>u.UpdateOrderAsync(It.Is<Order>(o=>o.AdditionalServicesViewed==true)));
            context.OrderRepositoryMock.Verify(u => u.UpdateOrderAsync(It.Is<Order>(o => o.CatalogueSolutionsViewed == false)));
            context.OrderRepositoryMock.Verify(u => u.UpdateOrderAsync(It.Is<Order>(o => o.ServiceRecipientsViewed == false)));
        }

        [Test]
        public async Task UpdateStatusAsync_WithCatalogueSolutionsSectionId_UpdatesCatalogueSolutionsViewedField()
        {
            var context = SectionStatusControllerTestContext.Setup();

            const string orderId = "C0000014-01";
            context.Order = CreateGetTestData(orderId, context.PrimaryOrganisationId, "ods");

            var response = await context.SectionStatusController.UpdateStatusAsync(orderId, "catalogue-solutions", new SectionStatusRequestModel { Status = "complete" });
            response.Should().BeOfType<NoContentResult>();
            context.OrderRepositoryMock.Verify(u => u.UpdateOrderAsync(It.Is<Order>(o => o.AdditionalServicesViewed == false)));
            context.OrderRepositoryMock.Verify(u => u.UpdateOrderAsync(It.Is<Order>(o => o.CatalogueSolutionsViewed == true)));
            context.OrderRepositoryMock.Verify(u => u.UpdateOrderAsync(It.Is<Order>(o => o.ServiceRecipientsViewed == false)));
        }

        private static Order CreateGetTestData(string orderId, Guid organisationId, string odsCode)
        {
            var repositoryOrder = OrderBuilder.Create()
                .WithOrderId(orderId)
                .WithOrganisationId(organisationId)
                .Build();

            var repositoryOrderItem = OrderItemBuilder.Create()
                .WithOdsCode(odsCode)
                .Build();

            var serviceRecipients = new List<(string Ods, string Name)>
            {
                (odsCode, "EU test")
            };

            repositoryOrder.AddOrderItem(repositoryOrderItem, Guid.Empty, string.Empty);
            repositoryOrder.SetServiceRecipient(serviceRecipients, Guid.Empty, string.Empty);

            return repositoryOrder;
        }



        internal sealed class SectionStatusControllerTestContext
        {
            private SectionStatusControllerTestContext(Guid primaryOrganisationId)
            {
                Name = "Test User";
                NameIdentity = Guid.NewGuid();
                PrimaryOrganisationId = primaryOrganisationId;
                OrderRepositoryMock = new Mock<IOrderRepository>();

                Orders = new List<Order>();
                OrderRepositoryMock.Setup(x => x.ListOrdersByOrganisationIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(() => Orders);

                OrderRepositoryMock.Setup(x => x.GetOrderByIdAsync(It.IsAny<string>())).ReturnsAsync(() => Order);

                OrderRepositoryMock.Setup(x => x.UpdateOrderAsync(It.IsAny<Order>()));

                ClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(
                    new[]
                    {
                        new Claim("Ordering", "Manage"),
                        new Claim("primaryOrganisationId", PrimaryOrganisationId.ToString()),
                        new Claim(ClaimTypes.Name, Name),
                        new Claim(ClaimTypes.NameIdentifier, NameIdentity.ToString())
                    }, "mock"));

                SectionStatusController = SectionStatusControllerBuilder
                    .Create()
                    .WithOrderRepository(OrderRepositoryMock.Object)
                    .Build();

                SectionStatusController.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = ClaimsPrincipal }
                };
            }

            internal string Name { get; }

            internal Guid NameIdentity { get; }

            internal Guid PrimaryOrganisationId { get; }

            private ClaimsPrincipal ClaimsPrincipal { get; }

            internal Mock<IOrderRepository> OrderRepositoryMock { get; }

            internal IEnumerable<Order> Orders { get; set; }

            internal Order Order { get; set; }

            internal SectionStatusController SectionStatusController { get; }

            internal static SectionStatusControllerTestContext Setup()
            {
                return new SectionStatusControllerTestContext(Guid.NewGuid());
            }

            internal static SectionStatusControllerTestContext Setup(Guid primaryOrganisationId)
            {
                return new SectionStatusControllerTestContext(primaryOrganisationId);
            }
        }
    }
}
