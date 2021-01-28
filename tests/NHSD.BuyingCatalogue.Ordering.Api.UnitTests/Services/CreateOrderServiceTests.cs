using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrder;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class CreateOrderServiceTests
    {
        [Test]
        public void Constructor_NullOrderRepository_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new CreateOrderService(null));
        }

        [Test]
        public void CreateAsync_NullCreateOrderRequest_ThrowsException()
        {
            static async Task TestAsync()
            {
                var context = CreateOrderServiceTestContext.Setup();
                var sut = context.CreateOrderService;

                _ = await sut.CreateAsync(null);
            }

            Assert.ThrowsAsync<ArgumentNullException>(TestAsync);
        }

        [Test]
        public async Task CreateAsync_SuccessfulOrderValidation_ReturnsSuccess()
        {
            var context = CreateOrderServiceTestContext.Setup();
            var sut = context.CreateOrderService;

            var request = CreateOrderRequestBuilder.Create().Build();

            var actual = await sut.CreateAsync(request);

            actual.IsSuccess.Should().BeTrue();
            actual.Value.Should().NotBeNull();
        }

        [Test]
        public async Task CreateAsync_SuccessfulOrderValidation_OrderRepository_CalledOnce()
        {
            var context = CreateOrderServiceTestContext.Setup();
            var sut = context.CreateOrderService;

            var request = CreateOrderRequestBuilder.Create().Build();

            await sut.CreateAsync(request);

            var expected = OrderBuilder
                .Create()
                .WithDescription(request.Description)
                .WithOrganisationId(request.OrganisationId)
                .Build();

            context.OrderRepositoryMock.Verify(r => r.CreateOrderAsync(It.Is<Order>(
                actual => actual.OrganisationId == expected.OrganisationId && actual.Description == expected.Description)));
        }

        [Test]
        public async Task CreateAsync_OrderValidationFailsWithEmptyOrganisationId_ReturnFailureResult()
        {
            var context = CreateOrderServiceTestContext.Setup();

            var sut = context.CreateOrderService;

            var request = CreateOrderRequestBuilder
                .Create()
                .WithOrganisationId(Guid.Empty)
                .Build();

            var actual = await sut.CreateAsync(request);

            var expected = Result.Failure<string>(new List<ErrorDetails> { new("OrganisationIdRequired", "OrganisationId") });

            actual.Should().Be(expected);
        }

        [Test]
        public async Task CreateAsync_OrderValidationFailsWithEmptyDescription_ReturnFailureResult()
        {
            var context = CreateOrderServiceTestContext.Setup();

            var sut = context.CreateOrderService;

            var request = CreateOrderRequestBuilder
                .Create()
                .WithDescription(null)
                .Build();

            var actual = await sut.CreateAsync(request);

            var expected = Result.Failure<string>(new List<ErrorDetails> { new("OrderDescriptionRequired", "Description") });

            actual.Should().Be(expected);
        }

        [Test]
        public async Task CreateAsync_OrderValidationFailsWithLongDescription_ReturnFailureResult()
        {
            var context = CreateOrderServiceTestContext.Setup();

            var sut = context.CreateOrderService;

            var request = CreateOrderRequestBuilder
                .Create()
                .WithDescription("This description is too long to be processed, 1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890")
                .Build();

            var actual = await sut.CreateAsync(request);

            var expected = Result.Failure<string>(new List<ErrorDetails> { new("OrderDescriptionTooLong", "Description") });

            actual.Should().Be(expected);
        }

        private sealed class CreateOrderServiceTestContext
        {
            private CreateOrderServiceTestContext()
            {
                OrderRepositoryMock = new Mock<IOrderRepository>();
                OrderRepositoryMock.Setup(r => r.CreateOrderAsync(It.IsAny<Order>())).ReturnsAsync("OrderId");
                CreateOrderService = new CreateOrderService(OrderRepositoryMock.Object);
            }

            internal Mock<IOrderRepository> OrderRepositoryMock { get; }

            internal CreateOrderService CreateOrderService { get; }

            public static CreateOrderServiceTestContext Setup() => new();
        }
    }
}
