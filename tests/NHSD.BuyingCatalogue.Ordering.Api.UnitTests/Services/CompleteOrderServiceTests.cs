using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NHSD.BuyingCatalogue.EmailClient;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CompleteOrder;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreatePurchasingDocument;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class CompleteOrderServiceTests
    {
        [Test]
        public static void Constructor_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(CompleteOrderService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Test]
        [CommonAutoData]
        public static async Task CompleteAsync_OrderCompleteFails_ReturnsFailureResult(
            Order order,
            CompleteOrderService service)
        {
            var result = await service.CompleteAsync(order);

            result.Should().Be(Result.Failure(new ErrorDetails("OrderNotComplete")));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task CompleteAsync_UpdatesDb(
            [Frozen] ApplicationDbContext context,
            OrderItem orderItem,
            Order order,
            CompleteOrderService service)
        {
            order.FundingSourceOnlyGms = false;
            order.OrderStatus = OrderStatus.Incomplete;
            order.AddOrUpdateOrderItem(orderItem);
            context.Order.Add(order);
            await context.SaveChangesAsync();

            await service.CompleteAsync(order);

            context.Set<Order>().First().OrderStatus.Should().Be(OrderStatus.Complete);
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task CompleteAsync_NotGms_ReturnsExpectedResult(
            OrderItem orderItem,
            Order order,
            CompleteOrderService service)
        {
            order.FundingSourceOnlyGms = false;
            order.OrderStatus = OrderStatus.Incomplete;
            order.AddOrUpdateOrderItem(orderItem);

            var result = await service.CompleteAsync(order);

            result.Should().Be(Result.Success());
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task CompleteAsync_NotGms_DoesNotCreateCsv(
            [Frozen] Mock<ICreatePurchasingDocumentService> createPurchasingDocumentServiceMock,
            OrderItem orderItem,
            Order order,
            CompleteOrderService service)
        {
            order.FundingSourceOnlyGms = false;
            order.OrderStatus = OrderStatus.Incomplete;
            order.AddOrUpdateOrderItem(orderItem);

            await service.CompleteAsync(order);

            createPurchasingDocumentServiceMock.VerifyNoOtherCalls();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task CompleteAsync_NotGms_DoesNotSendEmail(
            [Frozen] Mock<IEmailService> emailServiceMock,
            OrderItem orderItem,
            Order order,
            CompleteOrderService service)
        {
            order.FundingSourceOnlyGms = false;
            order.OrderStatus = OrderStatus.Incomplete;
            order.AddOrUpdateOrderItem(orderItem);

            await service.CompleteAsync(order);

            emailServiceMock.VerifyNoOtherCalls();
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task CompleteAsync_Gms_PurchasingDocumentService_CreateCsvAsync_CalledOnce(
            [Frozen] Mock<ICreatePurchasingDocumentService> createPurchasingDocumentServiceMock,
            OrderItem orderItem,
            Order order,
            CompleteOrderService service)
        {
            order.RemoveOrderItems();
            order.FundingSourceOnlyGms = true;
            order.OrderStatus = OrderStatus.Incomplete;
            order.AddOrUpdateOrderItem(orderItem);

            await service.CompleteAsync(order);

            createPurchasingDocumentServiceMock.Verify(s => s.CreatePatientNumbersCsvAsync(
                It.IsAny<Stream>(),
                It.Is<Order>(o => o.Equals(order))));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task CompleteAsync_EmailServiceSubjectIsSet_SubjectIsChanged(
            [Frozen] Mock<IEmailService> emailServiceMock,
            OrderItem orderItem,
            Order order,
            CompleteOrderService service)
        {
            order.FundingSourceOnlyGms = true;
            order.OrderStatus = OrderStatus.Incomplete;
            order.AddOrUpdateOrderItem(orderItem);

            EmailMessage sentMessage = null;
            emailServiceMock
                .Setup(s => s.SendEmailAsync(It.IsAny<EmailMessage>()))
                .Callback<EmailMessage>(m => sentMessage = m);

            await service.CompleteAsync(order);

            sentMessage.Subject.Should().BeEquivalentTo($"New Order {order.CallOffId}_{order.OrderingParty.OdsCode}");
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task CompleteAsync_EmailService_CalledOnce(
            [Frozen] Mock<IEmailService> emailServiceMock,
            OrderItem orderItem,
            Order order,
            CompleteOrderService service)
        {
            order.FundingSourceOnlyGms = true;
            order.OrderStatus = OrderStatus.Incomplete;
            order.AddOrUpdateOrderItem(orderItem);

            await service.CompleteAsync(order);

            emailServiceMock.Verify(emailService => emailService.SendEmailAsync(It.IsNotNull<EmailMessage>()));
        }

        [Test]
        [InMemoryDbAutoData]
        public static async Task CompleteAsync_CompleteOrderRequest_ReturnsSuccessfulResult(
            OrderItem orderItem,
            Order order,
            CompleteOrderService service)
        {
            order.FundingSourceOnlyGms = true;
            order.OrderStatus = OrderStatus.Incomplete;
            order.AddOrUpdateOrderItem(orderItem);

            var result = await service.CompleteAsync(order);

            result.Should().Be(Result.Success());
        }
    }
}
