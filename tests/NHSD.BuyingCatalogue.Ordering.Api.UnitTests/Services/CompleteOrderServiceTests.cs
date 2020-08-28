using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NHSD.BuyingCatalogue.EmailClient;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CompleteOrder;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreatePurchasingDocument;
using NHSD.BuyingCatalogue.Ordering.Api.Settings;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders.Services;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Application.Services;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class CompleteOrderServiceTests
    {
        [TestCase(false, true, true, true)]
        [TestCase(true, false, true, true)]
        [TestCase(true, true, false, true)]
        [TestCase(true, true, true, false)]
        public void Constructor_NullParameter_ThrowsArgumentNullException(
            bool hasIdentityService,
            bool hasOrderRepository,
            bool hasEmailService,
            bool hasPurchasingSettings)
        {
            var builder = CompleteOrderServiceBuilder
                .Create()
                .WithIdentityService(hasIdentityService ? Mock.Of<IIdentityService>() : null)
                .WithOrderRepository(hasOrderRepository ? Mock.Of<IOrderRepository>() : null)
                .WithEmailService(hasEmailService ? Mock.Of<IEmailService>() : null)
                .WithPurchasingSettings(hasPurchasingSettings ? Mock.Of<PurchasingSettings>() : null);

            Assert.Throws<ArgumentNullException>(() => builder.Build());
        }

        [Test]
        public void CompleteAsync_NullRequest_ThrowsArgumentNullException()
        {
            var context = CompleteOrderServiceTestContext.Setup();

            Assert.ThrowsAsync<ArgumentNullException>(() => context.CompleteOrderService.CompleteAsync(null));
        }

        [Test]
        public async Task CompleteAsync_CompleteOrderRequest_ReturnsSuccessfulResult()
        {
            var context = CompleteOrderServiceTestContext.Setup();

            var actual = await context.CompleteOrderService.CompleteAsync(context.CompleteOrderRequest);

            actual.Should().Be(Result.Success());
        }

        [Test]
        public async Task CompleteAsync_IncompleteOrderRequest_ReturnsFailureResult()
        {
            var context = CompleteOrderServiceTestContext.Setup();

            var actual = await context.CompleteOrderService.CompleteAsync(
                new CompleteOrderRequest(OrderBuilder.Create().Build()));

            actual.Should().Be(Result.Failure(new ErrorDetails("OrderNotComplete")));
        }

        [Test]
        public async Task CompleteAsync_CompleteOrder_LastUpdatedByIsChanged()
        {
            var context = CompleteOrderServiceTestContext.Setup();

            await context.CompleteOrderService.CompleteAsync(context.CompleteOrderRequest);

            context.UpdateOrder.LastUpdatedBy.Should().Be(context.UserId);
        }

        [Test]
        public async Task CompleteAsync_CompleteOrder_LastUpdatedByNameIsChanged()
        {
            var context = CompleteOrderServiceTestContext.Setup();

            await context.CompleteOrderService.CompleteAsync(context.CompleteOrderRequest);

            context.UpdateOrder.LastUpdatedByName.Should().Be(context.UserName);
        }

        [Test]
        public async Task CompleteAsync_IdentityService_CalledTwice()
        {
            var context = CompleteOrderServiceTestContext.Setup();

            await context.CompleteOrderService.CompleteAsync(context.CompleteOrderRequest);

            context.IdentityServiceMock.Verify(identityService =>
                identityService.GetUserName(), Times.Once);

            context.IdentityServiceMock.Verify(identityService =>
                identityService.GetUserIdentity(), Times.Once);
        }

        [Test]
        public async Task CompleteAsync_OrderRepository_CalledOnce()
        {
            var context = CompleteOrderServiceTestContext.Setup();

            await context.CompleteOrderService.CompleteAsync(context.CompleteOrderRequest);

            context.OrderRepositoryMock.Verify(orderRepository =>
                orderRepository.UpdateOrderAsync(It.Is<Order>(order => order.Equals(context.CompleteOrderRequest.Order))), Times.Once);
        }

        [Test]
        public async Task CompleteAsync_OrderRepository_OrderStatusIsComplete()
        {
            var context = CompleteOrderServiceTestContext.Setup();

            await context.CompleteOrderService.CompleteAsync(context.CompleteOrderRequest);

            context.UpdateOrder.OrderStatus.Should().Be(OrderStatus.Complete);
        }

        [Test]
        public async Task CompleteAsync_EmailService_CalledOnce()
        {
            var context = CompleteOrderServiceTestContext.Setup();

            await context.CompleteOrderService.CompleteAsync(context.CompleteOrderRequest);

            context.EmailServiceMock.Verify(emailService =>
                emailService.SendEmailAsync(context.PurchasingSettings.EmailMessage), Times.Once);
        }

        [Test]
        public async Task CompleteAsync_PurchasingDocumentServiceFundingSourceTrue_CalledOnce()
        {
            var context = CompleteOrderServiceTestContext.Setup();

            await context.CompleteOrderService.CompleteAsync(context.CompleteOrderRequest);

            context.CreatePurchaseDocumentServiceMock.Verify(purchasingDocumentService =>
                purchasingDocumentService.CreatePatientNumbersCsvAsync(It.IsAny<Stream>(),
                    It.Is<Order>(order => order.Equals(context.CompleteOrderRequest.Order))), Times.Once);
        }

        [Test]
        public async Task CompleteAsync_PurchasingDocumentServiceFundingSourceFalse_NotCalled()
        {
            var context = CompleteOrderServiceTestContext.Setup(false);

            await context.CompleteOrderService.CompleteAsync(context.CompleteOrderRequest);

            context.CreatePurchaseDocumentServiceMock.Verify(purchasingDocumentService =>
                purchasingDocumentService.CreatePatientNumbersCsvAsync(It.IsAny<Stream>(),
                    It.Is<Order>(order => order.Equals(context.CompleteOrderRequest.Order))), Times.Never);
        }

        internal sealed class CompleteOrderServiceTestContext
        {
            private CompleteOrderServiceTestContext(bool fundingSource)
            {
                UserId = Guid.NewGuid();
                UserName = $"Username {Guid.NewGuid()}";
                PurchasingSettings = new PurchasingSettings { EmailMessage = new EmailMessage() };
                CompleteOrderRequest = new CompleteOrderRequest(
                    OrderBuilder
                        .Create()
                        .WithOrderId(Guid.NewGuid().ToString())
                        .WithServiceRecipientsViewed(true)
                        .WithAssociatedServicesViewed(true)
                        .WithOrderItem(
                            OrderItemBuilder
                                .Create()
                                .WithCatalogueItemType(CatalogueItemType.AssociatedService)
                                .Build())
                        .WithFundingSourceOnlyGms(fundingSource)
                        .Build());

                OrderRepositoryMock = new Mock<IOrderRepository>();
                OrderRepositoryMock.Setup(x => x.UpdateOrderAsync(It.IsAny<Order>()))
                    .Callback<Order>(x => UpdateOrder = x);

                IdentityServiceMock = new Mock<IIdentityService>();
                IdentityServiceMock.Setup(identityService => identityService.GetUserIdentity()).Returns(() => UserId);
                IdentityServiceMock.Setup(identityService => identityService.GetUserName()).Returns(() => UserName);

                EmailServiceMock = new Mock<IEmailService>();
                EmailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<EmailMessage>()));
                CreatePurchaseDocumentServiceMock = new Mock<ICreatePurchasingDocumentService>();
                CreatePurchaseDocumentServiceMock.Setup(x => x.CreatePatientNumbersCsvAsync(It.IsAny<Stream>(), It.IsAny<Order>()));

                CompleteOrderService = CompleteOrderServiceBuilder
                    .Create()
                    .WithOrderRepository(OrderRepositoryMock.Object)
                    .WithIdentityService(IdentityServiceMock.Object)
                    .WithEmailService(EmailServiceMock.Object)
                    .WithCreatePurchasingDocumentService(CreatePurchaseDocumentServiceMock.Object)
                    .WithPurchasingSettings(PurchasingSettings)
                    .Build();
            }

            internal Mock<IIdentityService> IdentityServiceMock { get; }

            internal Mock<IOrderRepository> OrderRepositoryMock { get; }

            internal Mock<IEmailService> EmailServiceMock { get; }

            internal Mock<ICreatePurchasingDocumentService> CreatePurchaseDocumentServiceMock { get; }

            internal PurchasingSettings PurchasingSettings { get; }

            internal CompleteOrderService CompleteOrderService { get; }

            internal Order UpdateOrder { get; private set; }

            internal string UserName { get; }

            internal Guid UserId { get; }

            internal CompleteOrderRequest CompleteOrderRequest { get; }

            public static CompleteOrderServiceTestContext Setup(bool fundingSource = true)
            {
                return new CompleteOrderServiceTestContext(fundingSource);
            }
        }
    }
}
