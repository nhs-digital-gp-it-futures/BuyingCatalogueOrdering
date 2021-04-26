using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Services.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class SupplierSectionServiceTests
    {
        [Test]
        public static void Constructor_NullAccessor_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new SupplierSectionService(null));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetOrder_ReturnsNull(
            CallOffId callOffId,
            SupplierSectionService service)
        {
            var result = await service.GetOrder(callOffId);

            result.Should().BeNull();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetOrder_ReturnsOrder(
            [Frozen] ApplicationDbContext context,
            Order order,
            SupplierSectionService service)
        {
            context.Order.Add(order);
            await context.SaveChangesAsync();

            var result = await service.GetOrder(order.CallOffId);

            result.Should().BeEquivalentTo(order);
        }

        [Test]
        [InMemoryDbAutoData]
        public static void SetSupplierSection_NullOrder_ThrowsException(
            Supplier supplier,
            Contact contact,
            SupplierSectionService service)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.SetSupplierSection(null, supplier, contact));
        }

        [Test]
        [InMemoryDbAutoData]
        public static void SetSupplierSection_NullSupplier_ThrowsException(
            Order order,
            Contact contact,
            SupplierSectionService service)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.SetSupplierSection(order, null, contact));
        }

        [Test]
        [InMemoryDbAutoData]
        public static void SetSupplierSection_NullContact_ThrowsException(
            Order order,
            Supplier supplier,
            SupplierSectionService service)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.SetSupplierSection(order, supplier, null));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task SetSupplierSection_UpdatesSupplierDetails(
            [Frozen] ApplicationDbContext context,
            [Frozen] Contact contact,
            Order order,
            Supplier supplier,
            SupplierSectionService service)
        {
            context.Order.Add(order);
            context.Supplier.Add(supplier);
            await context.SaveChangesAsync();

            var expected = order.Supplier;
            await service.SetSupplierSection(order, supplier, contact);

            expected.Name.Should().Be(supplier.Name);
            expected.Address.Should().Be(supplier.Address);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task SetSupplierSection_UpdatesSupplierContact(
            [Frozen] ApplicationDbContext context,
            [Frozen] Contact contact,
            Order order,
            Supplier supplier,
            SupplierSectionService service)
        {
            context.Order.Add(order);
            context.Supplier.Add(supplier);
            await context.SaveChangesAsync();

            var expected = order.SupplierContact;
            await service.SetSupplierSection(order, supplier, contact);

            expected.Should().Be(contact);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task SetSupplierSection_SavesToDb(
            [Frozen] ApplicationDbContext context,
            [Frozen] Contact contact,
            Order order,
            Supplier supplier,
            SupplierSectionService service)
        {
            context.Order.Add(order);
            context.Supplier.Add(supplier);
            await context.SaveChangesAsync();

            await service.SetSupplierSection(order, supplier, contact);

            var expectedOrder = context.Set<Order>().First(o => o.Equals(order));

            expectedOrder.Supplier.Name.Should().Be(supplier.Name);
            expectedOrder.SupplierContact.Should().Be(contact);
        }
    }
}
