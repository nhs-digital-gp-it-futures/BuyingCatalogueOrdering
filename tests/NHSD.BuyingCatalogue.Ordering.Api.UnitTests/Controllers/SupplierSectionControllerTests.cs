using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Services;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Contracts;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class SupplierSectionControllerTests
    {
        [Test]
        public static void Constructor_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(SupplierSectionController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetAsync_OrderDoesNotExist_ReturnsNotFound(
            [Frozen] Mock<ISupplierSectionService> service,
            CallOffId callOffId,
            SupplierSectionController controller)
        {
            service.Setup(o => o.GetOrder(callOffId)).ReturnsAsync((Order)null);
            var response = await controller.GetAsync(callOffId);

            response.Should().NotBeNull();
            response.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetAsync_OrderIdExists_SupplierSectionDetailsReturned(
            [Frozen] Mock<ISupplierSectionService> service,
            [Frozen] CallOffId callOffId,
            Order order,
            SupplierSectionController controller)
        {
            order.Supplier.Should().NotBeNull();
            order.SupplierContact.Should().NotBeNull();
            service.Setup(o => o.GetOrder(callOffId)).ReturnsAsync(order);

            var expectedValue = new SupplierModel(order.Supplier, order.SupplierContact);

            var response = await controller.GetAsync(callOffId);

            response.Value.Should().BeEquivalentTo(expectedValue);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetAsync_InvokesGetOrder(
            [Frozen] Mock<ISupplierSectionService> service,
            [Frozen] CallOffId callOffId,
            Order order,
            SupplierSectionController controller)
        {
            order.Supplier.Should().NotBeNull();
            service.Setup(o => o.GetOrder(callOffId)).ReturnsAsync(order);

            await controller.GetAsync(callOffId);

            service.Verify(o => o.GetOrder(callOffId), () => Times.AtMost(1));
        }

        [Test]
        [InMemoryDbAutoData]
        public static void UpdateAsync_ModelIsNull_ThrowsArgumentNullException(
            CallOffId callOffId,
            SupplierSectionController controller)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await controller.UpdateAsync(callOffId, null));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateAsync_OrderDoesNotExist_ReturnsNotFound(
            [Frozen] Mock<ISupplierSectionService> service,
            CallOffId callOffId,
            SupplierModel model,
            SupplierSectionController controller)
        {
            service.Setup(o => o.GetOrder(callOffId)).ReturnsAsync((Order)null);
            var response = await controller.UpdateAsync(callOffId, model);

            response.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateAsync_UpdatesSupplierDetails(
            [Frozen] Mock<IContactDetailsService> contactDetailsService,
            [Frozen] Mock<ISupplierSectionService> supplierSectionService,
            [Frozen] CallOffId callOffId,
            Order order,
            SupplierModel model,
            SupplierSectionController controller)
        {
            order.Supplier.Should().NotBeEquivalentTo(model);
            supplierSectionService.Setup(o => o.GetOrder(callOffId)).ReturnsAsync(order);
            supplierSectionService.Setup(o => o.SetSupplierSection(order, It.IsAny<Supplier>(), It.IsAny<Contact>())).Callback(() =>
            {
                order.Supplier = new Supplier { Name = model.Name };
            });

            contactDetailsService
                .Setup(s => s.AddOrUpdateAddress(It.IsNotNull<Address>(), It.IsNotNull<AddressModel>()))
                .Returns(order.Supplier.Address);

            await controller.UpdateAsync(callOffId, model);

            order.Supplier.Should().BeEquivalentTo(model, o => o.Including(s => s.Name));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateAsync_UpdatesSupplierContact(
            [Frozen] Mock<IContactDetailsService> contactDetailsService,
            [Frozen] Mock<ISupplierSectionService> supplierSectionService,
            [Frozen] CallOffId callOffId,
            Order order,
            SupplierModel model,
            SupplierSectionController controller)
        {
            var expectedContact = order.SupplierContact;

            order.Supplier.Should().NotBeEquivalentTo(model);
            supplierSectionService.Setup(o => o.GetOrder(callOffId)).ReturnsAsync(order);

            contactDetailsService
                .Setup(s => s.AddOrUpdatePrimaryContact(It.IsNotNull<Contact>(), It.IsNotNull<PrimaryContactModel>()))
                .Returns(order.SupplierContact);

            await controller.UpdateAsync(callOffId, model);

            expectedContact.Should().BeEquivalentTo(order.SupplierContact);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateAsync_InvokesAddOrUpdateAddress(
            [Frozen] Mock<IContactDetailsService> contactDetailsService,
            [Frozen] Mock<ISupplierSectionService> supplierSectionService,
            [Frozen] CallOffId callOffId,
            Order order,
            SupplierModel model,
            SupplierSectionController controller)
        {
            var originalAddress = order.Supplier.Address;

            Expression<Func<IContactDetailsService, Address>> addOrUpdateAddress = s => s.AddOrUpdateAddress(
                It.Is<Address>(a => a == originalAddress),
                It.Is<AddressModel>(a => a == model.Address));

            supplierSectionService.Setup(o => o.GetOrder(callOffId)).ReturnsAsync(order);
            supplierSectionService.Setup(o => o.SetSupplierSection(order, It.IsAny<Supplier>(), It.IsAny<Contact>())).Callback(() =>
            {
                order.Supplier.Address = originalAddress;
            });
            contactDetailsService.Setup(addOrUpdateAddress).Returns(originalAddress);

            await controller.UpdateAsync(callOffId, model);
            contactDetailsService.Verify(addOrUpdateAddress);

            order.Supplier.Address.Should().BeEquivalentTo(originalAddress);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateAsync_InvokesAddOrUpdatePrimaryContact(
            [Frozen] Mock<IContactDetailsService> contactDetailsService,
            [Frozen] Mock<ISupplierSectionService> supplierSectionService,
            [Frozen] CallOffId callOffId,
            Order order,
            SupplierModel model,
            SupplierSectionController controller)
        {
            var originalContact = order.SupplierContact;

            supplierSectionService.Setup(o => o.GetOrder(callOffId)).ReturnsAsync(order);
            contactDetailsService
                .Setup(s => s.AddOrUpdatePrimaryContact(It.IsNotNull<Contact>(), It.IsNotNull<PrimaryContactModel>()))
                .Returns(order.SupplierContact);

            await controller.UpdateAsync(callOffId, model);

            contactDetailsService.Verify(s => s.AddOrUpdatePrimaryContact(
                It.Is<Contact>(c => c == originalContact),
                It.Is<PrimaryContactModel>(c => c == model.PrimaryContact)));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task UpdateAsync_SuccessfulUpdate_ReturnsNoContentResult(
            [Frozen] Mock<IContactDetailsService> contactDetailsService,
            [Frozen] Mock<ISupplierSectionService> supplierSectionService,
            [Frozen] CallOffId callOffId,
            Order order,
            SupplierModel model,
            SupplierSectionController controller)
        {
            supplierSectionService.Setup(o => o.GetOrder(callOffId)).ReturnsAsync(order);
            contactDetailsService
                .Setup(s => s.AddOrUpdateAddress(It.IsNotNull<Address>(), It.IsNotNull<AddressModel>()))
                .Returns(order.Supplier.Address);

            var result = await controller.UpdateAsync(callOffId, model);

            result.Should().BeOfType<NoContentResult>();
        }
    }
}
