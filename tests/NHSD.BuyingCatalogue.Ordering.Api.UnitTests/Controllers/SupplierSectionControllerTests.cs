using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.Children)]
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
        [InMemoryDbAutoData(nameof(GetAsync_OrderDoesNotExist_ReturnsNotFound))]
        public static async Task GetAsync_OrderDoesNotExist_ReturnsNotFound(
            CallOffId callOffId,
            SupplierSectionController controller)
        {
            var response = await controller.GetAsync(callOffId);

            response.Should().NotBeNull();
            response.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData(nameof(GetAsync_OrderIdExists_SupplierSectionDetailsReturned))]
        public static async Task GetAsync_OrderIdExists_SupplierSectionDetailsReturned(
            [Frozen] ApplicationDbContext context,
            [Frozen] CallOffId callOffId,
            Order order,
            SupplierSectionController controller)
        {
            order.Supplier.Should().NotBeNull();
            order.SupplierContact.Should().NotBeNull();

            context.Add(order);
            await context.SaveChangesAsync();

            var expectedValue = new SupplierModel(order.Supplier, order.SupplierContact);

            var response = await controller.GetAsync(callOffId);

            response.Value.Should().BeEquivalentTo(expectedValue);
        }

        [Test]
        [InMemoryDbAutoData(nameof(UpdateAsync_ModelIsNull_ThrowsArgumentNullException))]
        public static void UpdateAsync_ModelIsNull_ThrowsArgumentNullException(
            CallOffId callOffId,
            SupplierSectionController controller)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await controller.UpdateAsync(callOffId, null));
        }

        [Test]
        [InMemoryDbAutoData(nameof(UpdateAsync_OrderDoesNotExist_ReturnsNotFound))]
        public static async Task UpdateAsync_OrderDoesNotExist_ReturnsNotFound(
            CallOffId callOffId,
            SupplierModel model,
            SupplierSectionController controller)
        {
            var response = await controller.UpdateAsync(callOffId, model);

            response.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        [InMemoryDbAutoData(nameof(UpdateAsync_UpdatesOrderingParty))]
        public static async Task UpdateAsync_UpdatesOrderingParty(
            [Frozen] Mock<IContactDetailsService> contactDetailsService,
            [Frozen] ApplicationDbContext context,
            [Frozen] CallOffId callOffId,
            Order order,
            SupplierModel model,
            SupplierSectionController controller)
        {
            order.Supplier.Should().NotBeEquivalentTo(model);

            context.Order.Add(order);
            await context.SaveChangesAsync();

            contactDetailsService
                .Setup(s => s.AddOrUpdateAddress(It.IsNotNull<Address>(), It.IsNotNull<AddressModel>()))
                .Returns(order.Supplier.Address);

            await controller.UpdateAsync(callOffId, model);

            order.Supplier.Should().BeEquivalentTo(model, o => o.Including(p => p.Name));
        }

        [Test]
        [InMemoryDbAutoData(nameof(UpdateAsync_InvokesAddOrUpdateAddress))]
        public static async Task UpdateAsync_InvokesAddOrUpdateAddress(
            [Frozen] Mock<IContactDetailsService> contactDetailsService,
            [Frozen] ApplicationDbContext context,
            [Frozen] CallOffId callOffId,
            Order order,
            SupplierModel model,
            SupplierSectionController controller)
        {
            context.Order.Add(order);
            await context.SaveChangesAsync();

            var originalAddress = order.Supplier.Address;

            Expression<Func<IContactDetailsService, Address>> addOrUpdateAddress = s => s.AddOrUpdateAddress(
                It.Is<Address>(a => a == originalAddress),
                It.Is<AddressModel>(a => a == model.Address));

            contactDetailsService.Setup(addOrUpdateAddress).Returns(originalAddress);

            await controller.UpdateAsync(callOffId, model);

            contactDetailsService.Verify(addOrUpdateAddress);
        }

        [Test]
        [InMemoryDbAutoData(nameof(UpdateAsync_InvokesAddOrUpdatePrimaryContact))]
        public static async Task UpdateAsync_InvokesAddOrUpdatePrimaryContact(
            [Frozen] Mock<IContactDetailsService> contactDetailsService,
            [Frozen] ApplicationDbContext context,
            [Frozen] CallOffId callOffId,
            Order order,
            SupplierModel model,
            SupplierSectionController controller)
        {
            context.Order.Add(order);
            await context.SaveChangesAsync();

            contactDetailsService
                .Setup(s => s.AddOrUpdateAddress(It.IsNotNull<Address>(), It.IsNotNull<AddressModel>()))
                .Returns(order.Supplier.Address);

            var originalContact = order.SupplierContact;

            await controller.UpdateAsync(callOffId, model);

            contactDetailsService.Verify(s => s.AddOrUpdatePrimaryContact(
                It.Is<Contact>(c => c == originalContact),
                It.Is<PrimaryContactModel>(c => c == model.PrimaryContact)));
        }

        [Test]
        [InMemoryDbAutoData(nameof(UpdateAsync_SavesToDb))]
        public static async Task UpdateAsync_SavesToDb(
            [Frozen] Mock<IContactDetailsService> contactDetailsService,
            [Frozen] ApplicationDbContext context,
            [Frozen] CallOffId callOffId,
            Order order,
            SupplierModel model,
            SupplierSectionController controller)
        {
            context.Order.Add(order);
            await context.SaveChangesAsync();

            contactDetailsService
                .Setup(s => s.AddOrUpdateAddress(It.IsNotNull<Address>(), It.IsNotNull<AddressModel>()))
                .Returns(order.Supplier.Address);

            await controller.UpdateAsync(callOffId, model);

            context.Set<Order>().First(o => o.Equals(order)).Supplier.Should().BeEquivalentTo(
                model,
                o => o.Including(p => p.Name));
        }

        [Test]
        [InMemoryDbAutoData(nameof(UpdateAsync_SuccessfulUpdate_ReturnsNoContentResult))]
        public static async Task UpdateAsync_SuccessfulUpdate_ReturnsNoContentResult(
            [Frozen] Mock<IContactDetailsService> contactDetailsService,
            [Frozen] ApplicationDbContext context,
            [Frozen] CallOffId callOffId,
            Order order,
            SupplierModel model,
            SupplierSectionController controller)
        {
            context.Order.Add(order);
            await context.SaveChangesAsync();

            contactDetailsService
                .Setup(s => s.AddOrUpdateAddress(It.IsNotNull<Address>(), It.IsNotNull<AddressModel>()))
                .Returns(order.Supplier.Address);

            var result = await controller.UpdateAsync(callOffId, model);

            result.Should().BeOfType<NoContentResult>();
        }
    }
}
