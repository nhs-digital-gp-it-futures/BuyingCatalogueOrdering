using System;
using System.Collections.Generic;
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
    internal static class ServiceRecipientServiceTests
    {
        [Test]
        public static void Constructor_NullAccessor_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new ServiceRecipientService(null));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetAllOrderItemRecipient_ReturnsNull(
            CallOffId callOffId,
            ServiceRecipientService service)
        {
            var result = await service.GetAllOrderItemRecipients(callOffId);

            result.Should().BeNull();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetAllOrderItemRecipient_ReturnsEmptyList(
            [Frozen] ApplicationDbContext context,
            Order order,
            ServiceRecipientService service)
        {
            context.Order.Add(order);
            await context.SaveChangesAsync();

            var result = await service.GetAllOrderItemRecipients(order.CallOffId);

            result.Should().BeEmpty();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task GetAllOrderItemRecipient_ReturnsOrderItemRecipients(
            [Frozen] ApplicationDbContext context,
            [Frozen] Order order,
            OrderItem orderItem,
            List<OrderItemRecipient> orderItemRecipients,
            ServiceRecipientService service)
        {
            orderItem.SetRecipients(orderItemRecipients);
            order.AddOrUpdateOrderItem(orderItem);
            context.Order.Add(order);
            await context.SaveChangesAsync();

            var expectedServiceRecipients =
                orderItemRecipients.Select(o => new ServiceRecipient(o.Recipient.OdsCode, o.Recipient.Name));

            var result = await service.GetAllOrderItemRecipients(order.CallOffId);

            result.Should().BeEquivalentTo(expectedServiceRecipients);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task AddOrUpdateServiceRecipients_AddServiceRecipients(
            List<ServiceRecipient> serviceRecipients,
            ServiceRecipientService service)
        {
            var result = await service.AddOrUpdateServiceRecipients(serviceRecipients);

            result.Should().Contain(serviceRecipients.ToDictionary(r => r.OdsCode));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task AddOrUpdateServiceRecipients_UpdateServiceRecipients(
            [Frozen] ApplicationDbContext context,
            List<ServiceRecipient> serviceRecipients,
            ServiceRecipientService service)
        {
            context.ServiceRecipient.AddRange(serviceRecipients);
            await context.SaveChangesAsync();

            serviceRecipients.OrderBy(s => s.OdsCode).First().Name = "Recipient A";

            await service.AddOrUpdateServiceRecipients(serviceRecipients);

            var resultServiceRecipients = context.Set<ServiceRecipient>().OrderBy(s => s.OdsCode).First();

            serviceRecipients.OrderBy(s => s.OdsCode).First().Name.Should().BeEquivalentTo(resultServiceRecipients.Name);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task AddOrUpdateServiceRecipients_ReturnsExpectedServiceRecipients(
            [Frozen] ApplicationDbContext context,
            List<ServiceRecipient> serviceRecipients,
            ServiceRecipientService service)
        {
            context.ServiceRecipient.AddRange(serviceRecipients);
            await context.SaveChangesAsync();

            var result = await service.AddOrUpdateServiceRecipients(serviceRecipients);

            result.Values.Should().BeEquivalentTo(serviceRecipients);
        }
    }
}
