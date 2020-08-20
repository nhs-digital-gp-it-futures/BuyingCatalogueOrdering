using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrder;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public sealed class CreateOrderServiceTests
    {
        [Test]
        public void Constructor_NullOrderRepository_ThrowsException()
        {
            static void Test()
            {
                var context = CreateOrderServiceTestContext.Setup();
                var sut = new CreateOrderService(null);
            }
            Assert.Throws<ArgumentNullException>(Test);
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

            context.OrderRepositoryMock.Verify(x =>
                x.CreateOrderAsync(It.Is<Order>(
                    actual => actual.OrganisationId==expected.OrganisationId && actual.Description==expected.Description)), Times.Once);
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

            var expected = Result.Failure<string>(new List<ErrorDetails> {new ErrorDetails("OrganisationIdRequired", "OrganisationId") });
            
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

            var expected = Result.Failure<string>(new List<ErrorDetails> { new ErrorDetails("OrderDescriptionRequired", "Description") });

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

            var expected = Result.Failure<string>(new List<ErrorDetails> { new ErrorDetails("OrderDescriptionTooLong", "Description") });

            actual.Should().Be(expected);
        }
    }

    internal sealed class CreateOrderServiceTestContext
    {
        private CreateOrderServiceTestContext()
        {
            OrderRepositoryMock = new Mock<IOrderRepository>();
            OrderRepositoryMock.Setup(x => x.CreateOrderAsync(It.IsAny<Order>())).ReturnsAsync("OrderId");
            CreateOrderService = new CreateOrderService(OrderRepositoryMock.Object);
        }

        internal Result OrderValidationResult { get; set; } = Result.Success();

        internal Mock<IOrderRepository> OrderRepositoryMock { get; set; }

        internal CreateOrderService CreateOrderService { get; }

        public static CreateOrderServiceTestContext Setup()
        {
            return new CreateOrderServiceTestContext();
        }
    }
}
