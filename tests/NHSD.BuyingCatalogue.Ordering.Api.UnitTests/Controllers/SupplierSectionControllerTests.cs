using System;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Controllers
{
    [TestFixture]

    [Parallelizable(ParallelScope.All)]
    internal sealed class SupplierSectionControllerTests
    {
        [Test]
        public void Constructor_NullRepository_Throws()
        {
            static void Test()
            {
                var _ = new SupplierSectionController(null);
            }

            Assert.Throws<ArgumentNullException>(Test);
        }

        [Test]
        public async Task GetAsync_OrderIdDoesNotExists_NotFoundReturned()
        {
            const string orderId = "C0000014-01";

            var context = SupplierSectionControllerTestContext.Setup();

            var controller = context.SupplierSectionController;
            var response = await controller.GetAsync(orderId);

            response.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task GetAsync_OrderIdExists_SupplierSectionDetailsReturned()
        {
            const string orderId = "C0000014-01";
            const string supplierId = "1234";
            const string supplierName = "NHS Supplier";

            var supplierAddress = AddressBuilder
                .Create()
                .Build();

            var supplierContact = ContactBuilder
                .Create()
                .Build();

            var context = SupplierSectionControllerTestContext.Setup();
            context.Order = OrderBuilder
                .Create()
                .WithOrderId(orderId)
                .WithOrganisationId(context.PrimaryOrganisationId)
                .WithSupplierId(supplierId)
                .WithSupplierName(supplierName)
                .WithSupplierAddress(supplierAddress)
                .WithSupplierContact(supplierContact)
                .Build();

            var controller = context.SupplierSectionController;
            var response = await controller.GetAsync(orderId);

            var expected = new SupplierModel
            {
                SupplierId = supplierId,
                Name = supplierName,
                Address = supplierAddress.ToModel(),
                PrimaryContact = supplierContact.ToModel()
            };

            response.Should().BeEquivalentTo(new ActionResult<SupplierModel>(new OkObjectResult(expected)));
        }

        [Test]
        public async Task GetAsync_DifferentOrganisationId_ForbiddenReturned()
        {
            var orderId = "C0000014-01";
            var context = SupplierSectionControllerTestContext.Setup();

            context.PrimaryOrganisationId = Guid.NewGuid();
            context.Order = OrderBuilder
                .Create()
                .WithOrganisationId(Guid.NewGuid())
                .Build();

            var controller = context.SupplierSectionController;
            var actual = await controller.GetAsync(orderId);

            var expected = new ActionResult<SupplierModel>(new ForbidResult());
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task GetAsync_GetOrderByIdAsync_CalledOnce()
        {
            var context = SupplierSectionControllerTestContext.Setup();

            var controller = context.SupplierSectionController;

            const string orderId = "C123";
            await controller.GetAsync(orderId);

            context.OrderRepositoryMock.Verify(x => x.GetOrderByIdAsync(orderId), Times.Once);
        }

        [TestCase(null)]
        [TestCase("INVALID")]
        public async Task UpdateAsync_OrderIdDoesNotExist_ReturnNotFound(string orderId)
        {
            var context = SupplierSectionControllerTestContext.Setup();

            var controller = context.SupplierSectionController;

            var response =
                await controller.UpdateAsync(orderId, new SupplierModel { PrimaryContact = new PrimaryContactModel() });

            response.Should().BeEquivalentTo(new NotFoundResult());
        }

        [Test]
        public void UpdateAsync_ModelIsNull_ThrowsNullArgumentException()
        {
            static async Task GetSupplierSectionWithNullModel()
            {
                var context = SupplierSectionControllerTestContext.Setup();

                var controller = context.SupplierSectionController;
                await controller.UpdateAsync("OrderId", null);
            }

            Assert.ThrowsAsync<ArgumentNullException>(GetSupplierSectionWithNullModel);
        }

        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(false, false)]
        public void UpdateAsync_NullAddressOrContact_ThrowsNullArgumentException(bool hasPrimaryContact, bool hasAddress)
        {
            const string orderId = "C0000014-01";
            const string supplierId = "1234";
            const string supplierName = "NHS Supplier";

            var supplierAddress = AddressBuilder
                .Create()
                .Build();

            var supplierContact = ContactBuilder
                .Create()
                .Build();
            var context = SupplierSectionControllerTestContext.Setup();

            context.Order = OrderBuilder
                .Create()
                .WithOrderId(orderId)
                .WithOrganisationId(context.PrimaryOrganisationId)
                .WithSupplierId(supplierId)
                .WithSupplierName(supplierName)
                .WithSupplierAddress(supplierAddress)
                .WithSupplierContact(supplierContact)
                .Build(); ;

            var controller = context.SupplierSectionController;

            Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                var _ = await controller.UpdateAsync(orderId,
                    new SupplierModel()
                    {
                        Name = "New Description",
                        SupplierId = "New",
                        PrimaryContact = hasPrimaryContact ? new PrimaryContactModel() : null,
                        Address = hasAddress ? new AddressModel() : null
                    });
            });
        }

        [Test]
        public async Task UpdateAsync_UpdateIsValid_ReturnsNoContent()
        {
            const string orderId = "C0000014-01";
            const string supplierId = "1234";
            const string supplierName = "NHS Supplier";

            var supplierAddress = AddressBuilder
                .Create()
                .Build();

            var supplierContact = ContactBuilder
                .Create()
                .Build();
            var context = SupplierSectionControllerTestContext.Setup();

            context.Order = OrderBuilder
                .Create()
                .WithOrderId(orderId)
                .WithOrganisationId(context.PrimaryOrganisationId)
                .WithSupplierId(supplierId)
                .WithSupplierName(supplierName)
                .WithSupplierAddress(supplierAddress)
                .WithSupplierContact(supplierContact)
                .Build(); ;

            var controller = context.SupplierSectionController;

            var response =
                await controller.UpdateAsync(orderId,
                    new SupplierModel() { Name = "New Description",SupplierId = "New", PrimaryContact = new PrimaryContactModel(), Address = new AddressModel()});

            response.Should().BeOfType<NoContentResult>();
        }

        private sealed class SupplierSectionControllerTestContext
        {
            private SupplierSectionControllerTestContext()
            {
                PrimaryOrganisationId = Guid.NewGuid();

                OrderRepositoryMock = new Mock<IOrderRepository>();
                OrderRepositoryMock.Setup(x => x.GetOrderByIdAsync(It.IsAny<string>()))
                    .ReturnsAsync(() => Order);

                ClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("primaryOrganisationId", PrimaryOrganisationId.ToString()),
                    new Claim(ClaimTypes.Name, "Test User"),
                    new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
                }, "mock"));

                SupplierSectionController = new SupplierSectionController(OrderRepositoryMock.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext { User = ClaimsPrincipal }
                    }
                };
            }

            internal Guid PrimaryOrganisationId { get; set; }

            private ClaimsPrincipal ClaimsPrincipal { get; }

            internal Mock<IOrderRepository> OrderRepositoryMock { get; }

            internal Order Order { get; set; }

            internal SupplierSectionController SupplierSectionController { get; }

            internal static SupplierSectionControllerTestContext Setup()
            {
                return new SupplierSectionControllerTestContext();
            }
        }
    }
}
