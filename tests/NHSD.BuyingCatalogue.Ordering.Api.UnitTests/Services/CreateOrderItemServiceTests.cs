using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Api.Validation;
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
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class CreateOrderItemServiceTests
    {
        [Test]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(CreateOrderItemService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Test]
        [OrderingAutoData]
        public static void CreateAsync_NullCreateOrderItemRequest_ThrowsArgumentNullException(
            CreateOrderItemService service)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.CreateAsync(null));
        }

        [Test]
        [OrderingAutoData]
        public static async Task CreateAsync_ValidatorReturnsErrors_ReturnsFailure(
            CreateOrderItemSolutionRequest createOrderItemRequest,
            [Frozen] Mock<ICreateOrderItemValidator> validator,
            CreateOrderItemService service)
        {
            var expectedErrors = new List<ErrorDetails>
            {
                new("Error1", "ErrorField"),
                new("Error2", "ErrorField"),
            };

            validator
                .Setup(x => x.Validate(createOrderItemRequest))
                .Returns(new ValidationResult(expectedErrors));

            var actual = await service.CreateAsync(createOrderItemRequest);
            actual.Should().Be(Result.Failure<int>(expectedErrors));
        }

        [Test]
        [OrderingAutoData]
        public static async Task CreateAsync_CreateOrderItemRequest_ReturnsSuccess(
            [Frozen] Order order,
            CreateOrderItemSolutionRequest createOrderItemRequest,
            CreateOrderItemService service)
        {
            var actual = await service.CreateAsync(createOrderItemRequest);

            var expected = order.OrderItems.First().OrderItemId;
            actual.Should().Be(Result.Success(expected));
        }

        [Test]
        [OrderingInlineAutoData(ProvisioningType.OnDemand, TimeUnit.PerYear)]
        [OrderingInlineAutoData(ProvisioningType.Patient, TimeUnit.PerMonth)]
        [OrderingInlineAutoData(ProvisioningType.Declarative, TimeUnit.PerYear)]
        public static async Task CreateAsync_AdditionalService_MapCreateOrderItemRequestToOrderItem_AreEqual(
            ProvisioningType provisioningType,
            TimeUnit? expectedEstimationPeriodNameInput,
            Order order,
            CreateOrderItemModel model,
            CreateOrderItemService service)
        {
            model.ProvisioningType = provisioningType.ToString();
            var createOrderItemRequest = new CreateOrderItemAdditionalServiceRequest(order, model);

            await service.CreateAsync(createOrderItemRequest);

            var actual = order.OrderItems.First();

            actual.Should().BeEquivalentTo(new
            {
                createOrderItemRequest.OdsCode,
                createOrderItemRequest.CatalogueItemId,
                createOrderItemRequest.CatalogueItemName,
                ParentCatalogueItemId = createOrderItemRequest.CatalogueSolutionId,
                createOrderItemRequest.ProvisioningType,
                createOrderItemRequest.CataloguePriceType,
                CataloguePriceUnit = CataloguePriceUnit.Create(
                    createOrderItemRequest.CataloguePriceUnitTierName,
                    createOrderItemRequest.CataloguePriceUnitDescription),
                createOrderItemRequest.PriceTimeUnit,
                createOrderItemRequest.CurrencyCode,
                createOrderItemRequest.Quantity,
                EstimationPeriod = expectedEstimationPeriodNameInput,
                createOrderItemRequest.DeliveryDate,
                createOrderItemRequest.Price,
            });
        }

        [Test]
        [OrderingInlineAutoData(ProvisioningType.OnDemand, TimeUnit.PerYear)]
        [OrderingInlineAutoData(ProvisioningType.Declarative, null)]
        public static async Task CreateAsync_AssociatedService_MapCreateOrderItemRequestToOrderItem_AreEqual(
            ProvisioningType provisioningType,
            TimeUnit? expectedEstimationPeriodNameInput,
            Order order,
            CreateOrderItemModel model,
            CreateOrderItemService service)
        {
            model.ProvisioningType = provisioningType.ToString();
            var createOrderItemRequest = new CreateOrderItemAssociatedServiceRequest(order, model);

            await service.CreateAsync(createOrderItemRequest);

            var actual = order.OrderItems.First();

            actual.Should().BeEquivalentTo(new
            {
                createOrderItemRequest.OdsCode,
                createOrderItemRequest.CatalogueItemId,
                createOrderItemRequest.CatalogueItemName,
                ParentCatalogueItemId = createOrderItemRequest.CatalogueSolutionId,
                createOrderItemRequest.ProvisioningType,
                createOrderItemRequest.CataloguePriceType,
                CataloguePriceUnit = CataloguePriceUnit.Create(
                    createOrderItemRequest.CataloguePriceUnitTierName,
                    createOrderItemRequest.CataloguePriceUnitDescription),
                createOrderItemRequest.PriceTimeUnit,
                createOrderItemRequest.CurrencyCode,
                createOrderItemRequest.Quantity,
                EstimationPeriod = expectedEstimationPeriodNameInput,
                createOrderItemRequest.DeliveryDate,
                createOrderItemRequest.Price,
            });
        }

        [Test]
        [OrderingInlineAutoData(ProvisioningType.OnDemand, TimeUnit.PerYear)]
        [OrderingInlineAutoData(ProvisioningType.Patient, TimeUnit.PerMonth)]
        [OrderingInlineAutoData(ProvisioningType.Declarative, TimeUnit.PerYear)]
        public static async Task CreateAsync_Solution_MapCreateOrderItemRequestToOrderItem_AreEqual(
            ProvisioningType provisioningType,
            TimeUnit? expectedEstimationPeriodNameInput,
            Order order,
            CreateOrderItemModel model,
            CreateOrderItemService service)
        {
            model.ProvisioningType = provisioningType.ToString();
            var createOrderItemRequest = new CreateOrderItemSolutionRequest(order, model);

            await service.CreateAsync(createOrderItemRequest);

            var actual = order.OrderItems.First();

            actual.Should().BeEquivalentTo(new
            {
                createOrderItemRequest.OdsCode,
                createOrderItemRequest.CatalogueItemId,
                createOrderItemRequest.CatalogueItemName,
                ParentCatalogueItemId = createOrderItemRequest.CatalogueSolutionId,
                createOrderItemRequest.ProvisioningType,
                createOrderItemRequest.CataloguePriceType,
                CataloguePriceUnit = CataloguePriceUnit.Create(
                    createOrderItemRequest.CataloguePriceUnitTierName,
                    createOrderItemRequest.CataloguePriceUnitDescription),
                createOrderItemRequest.PriceTimeUnit,
                createOrderItemRequest.CurrencyCode,
                createOrderItemRequest.Quantity,
                EstimationPeriod = expectedEstimationPeriodNameInput,
                createOrderItemRequest.DeliveryDate,
                createOrderItemRequest.Price,
            });
        }

        [Test]
        [OrderingAutoData]
        public static async Task CreateAsync_OrderRepository_CalledOnce(
            CreateOrderItemSolutionRequest createOrderItemRequest,
            [Frozen] Mock<IOrderRepository> repository,
            CreateOrderItemService service)
        {
            await service.CreateAsync(createOrderItemRequest);

            repository.Verify(r => r.UpdateOrderAsync(createOrderItemRequest.Order));
        }

        [Test]
        [OrderingAutoData]
        public static async Task CreateAsync_OrderItemValidator_CalledOnce(
            CreateOrderItemSolutionRequest createOrderItemRequest,
            [Frozen] Mock<ICreateOrderItemValidator> validator,
            CreateOrderItemService service)
        {
            validator.Setup(v => v.Validate(createOrderItemRequest))
                .Returns(new ValidationResult(Array.Empty<ErrorDetails>()));

            await service.CreateAsync(createOrderItemRequest);

            validator.Verify(v => v.Validate(createOrderItemRequest));
        }

        [Test]
        [OrderingAutoData]
        public static async Task CreateAsync_IdentityService_CalledTwice(
            CreateOrderItemSolutionRequest createOrderItemRequest,
            [Frozen] Mock<IIdentityService> identityService,
            CreateOrderItemService service)
        {
            await service.CreateAsync(createOrderItemRequest);

            identityService.Verify(i => i.GetUserName());
            identityService.Verify(i => i.GetUserIdentity());
        }

        [Test]
        [OrderingInlineAutoData(ProvisioningType.OnDemand, null, null)]
        [OrderingInlineAutoData(ProvisioningType.OnDemand, "month", TimeUnit.PerMonth)]
        [OrderingInlineAutoData(ProvisioningType.OnDemand, "year", TimeUnit.PerYear)]
        [OrderingInlineAutoData(ProvisioningType.Patient, null, TimeUnit.PerMonth)]
        [OrderingInlineAutoData(ProvisioningType.Declarative, null, TimeUnit.PerYear)]
        public static async Task CreateAsync_AdditionalService_ProvisioningType_OrderItemAddedWithExpectedEstimationPeriod(
            ProvisioningType provisioningType,
            string inputEstimationPeriod,
            TimeUnit? expectedEstimationPeriod,
            Order order,
            CreateOrderItemModel model,
            CreateOrderItemService service)
        {
            model.EstimationPeriod = inputEstimationPeriod;
            model.ProvisioningType = provisioningType.ToString();

            var createOrderItemRequest = new CreateOrderItemAdditionalServiceRequest(order, model);

            await service.CreateAsync(createOrderItemRequest);

            var orderItem = order.OrderItems.First();

            orderItem.EstimationPeriod.Should().Be(expectedEstimationPeriod);
        }

        [Test]
        [OrderingInlineAutoData(ProvisioningType.OnDemand, null, null)]
        [OrderingInlineAutoData(ProvisioningType.OnDemand, "month", TimeUnit.PerMonth)]
        [OrderingInlineAutoData(ProvisioningType.OnDemand, "year", TimeUnit.PerYear)]
        [OrderingInlineAutoData(ProvisioningType.Patient, null, null)]
        [OrderingInlineAutoData(ProvisioningType.Declarative, null, null)]
        public static async Task CreateAsync_AssociatedService_ProvisioningType_OrderItemAddedWithExpectedEstimationPeriod(
            ProvisioningType provisioningType,
            string inputEstimationPeriod,
            TimeUnit? expectedEstimationPeriod,
            Order order,
            CreateOrderItemModel model,
            CreateOrderItemService service)
        {
            model.EstimationPeriod = inputEstimationPeriod;
            model.ProvisioningType = provisioningType.ToString();

            var createOrderItemRequest = new CreateOrderItemAssociatedServiceRequest(order, model);

            await service.CreateAsync(createOrderItemRequest);

            var orderItem = order.OrderItems.First();

            orderItem.EstimationPeriod.Should().Be(expectedEstimationPeriod);
        }

        [Test]
        [OrderingInlineAutoData(ProvisioningType.OnDemand, null, null)]
        [OrderingInlineAutoData(ProvisioningType.OnDemand, "month", TimeUnit.PerMonth)]
        [OrderingInlineAutoData(ProvisioningType.OnDemand, "year", TimeUnit.PerYear)]
        [OrderingInlineAutoData(ProvisioningType.Patient, null, TimeUnit.PerMonth)]
        [OrderingInlineAutoData(ProvisioningType.Declarative, null, TimeUnit.PerYear)]
        public static async Task CreateAsync_Solution_ProvisioningType_OrderItemAddedWithExpectedEstimationPeriod(
            ProvisioningType provisioningType,
            string inputEstimationPeriod,
            TimeUnit? expectedEstimationPeriod,
            Order order,
            CreateOrderItemModel model,
            CreateOrderItemService service)
        {
            model.EstimationPeriod = inputEstimationPeriod;
            model.ProvisioningType = provisioningType.ToString();

            var createOrderItemRequest = new CreateOrderItemSolutionRequest(order, model);

            await service.CreateAsync(createOrderItemRequest);

            var orderItem = order.OrderItems.First();

            orderItem.EstimationPeriod.Should().Be(expectedEstimationPeriod);
        }

        [Test]
        [OrderingAutoData]
        public static void CreateAsync_IEnumerable_CreateOrderItemRequest_NullOrder_ThrowsException(
            IReadOnlyList<CreateOrderItemRequest> requests,
            CreateOrderItemService service)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => service.CreateAsync(null, requests));
        }

        [Test]
        [OrderingAutoData]
        public static void CreateAsync_IEnumerable_CreateOrderItemRequest_NullRequests_ThrowsException(
            Order order,
            CreateOrderItemService service)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => service.CreateAsync(order, null));
        }

        [Test]
        [OrderingAutoData]
        public static async Task CreateAsync_Order_IEnumerable_CreateOrderItemRequest_AddsExpectedOrderItemToOrder(
            [Frozen] Mock<IOrderItemFactory> orderItemFactory,
            Order order,
            CreateOrderItemRequest request,
            CreateOrderItemService service)
        {
            var item = OrderItemBuilder.Create().Build();
            orderItemFactory.Setup(f => f.Create(request)).Returns(item);

            await service.CreateAsync(order, new[] { request });

            order.OrderItems.Should().HaveCount(1);
            order.OrderItems.Should().Contain(item);
        }

        [Test]
        [OrderingAutoData]
        public static async Task CreateAsync_Order_IEnumerable_CreateOrderItemRequest_UsesExpectedUserId(
            [Frozen] Mock<IOrderItemFactory> orderItemFactory,
            [Frozen] Mock<IIdentityService> identityService,
            Guid userId,
            Order order,
            CreateOrderItemRequest request,
            CreateOrderItemService service)
        {
            var item = OrderItemBuilder.Create().Build();
            identityService.Setup(i => i.GetUserInfo()).Returns(new IdentityUser(userId, "Name"));
            orderItemFactory.Setup(f => f.Create(request)).Returns(item);

            await service.CreateAsync(order, new[] { request });

            order.LastUpdatedBy.Should().Be(userId);
        }

        [Test]
        [OrderingAutoData]
        public static async Task CreateAsync_Order_IEnumerable_CreateOrderItemRequest_UsesExpectedUserName(
            [Frozen] Mock<IOrderItemFactory> orderItemFactory,
            [Frozen] Mock<IIdentityService> identityService,
            string userName,
            Order order,
            CreateOrderItemRequest request,
            CreateOrderItemService service)
        {
            var item = OrderItemBuilder.Create().Build();
            identityService.Setup(i => i.GetUserInfo()).Returns(new IdentityUser(Guid.Empty, userName));
            orderItemFactory.Setup(f => f.Create(request)).Returns(item);

            await service.CreateAsync(order, new[] { request });

            order.LastUpdatedByName.Should().Be(userName);
        }

        [Test]
        [OrderingAutoData]
        public static async Task CreateAsync_Order_IEnumerable_CreateOrderItemRequest_FailedValidation_DoesNotAddOrderItem(
            [Frozen] Mock<ICreateOrderItemValidator> orderItemValidator,
            [Frozen] Mock<IOrderItemFactory> orderItemFactory,
            [Frozen] Mock<IIdentityService> identityService,
            ValidationResult validationResult,
            string userName,
            Order order,
            OrderItem item,
            CreateOrderItemRequest request,
            CreateOrderItemService service)
        {
            orderItemValidator.Setup(v => v.Validate(request)).Returns(validationResult);
            identityService.Setup(i => i.GetUserName()).Returns(userName);
            orderItemFactory.Setup(f => f.Create(request)).Returns(item);

            await service.CreateAsync(order, new[] { request });

            order.OrderItems.Should().BeEmpty();
        }

        [Test]
        [OrderingAutoData]
        public static async Task CreateAsync_Order_IEnumerable_CreateOrderItemRequest_FailedValidation_ReturnsExpectedResult(
            [Frozen] Mock<ICreateOrderItemValidator> orderItemValidator,
            [Frozen] Mock<IOrderItemFactory> orderItemFactory,
            [Frozen] Mock<IIdentityService> identityService,
            AggregateValidationResult aggregateResult,
            string userName,
            Order order,
            OrderItem item,
            CreateOrderItemRequest request,
            CreateOrderItemService service)
        {
            aggregateResult.AddValidationResult(new ValidationResult(new ErrorDetails("Id", "Field")), 0);
            orderItemValidator
                .Setup(v => v.Validate(It.IsNotNull<IReadOnlyList<CreateOrderItemRequest>>(), It.IsNotNull<IEnumerable<OrderItem>>()))
                .Returns(aggregateResult);

            identityService.Setup(i => i.GetUserName()).Returns(userName);
            orderItemFactory.Setup(f => f.Create(request)).Returns(item);

            var result = await service.CreateAsync(order, new[] { request });

            result.Should().Be(aggregateResult);
            result.Success.Should().BeFalse();
        }

        [Test]
        [OrderingAutoData]
        public static async Task CreateAsync_Order_IEnumerable_CreateOrderItemRequest_DoesNotAddNullServiceRecipient(
            [Frozen] Mock<IOrderItemFactory> orderItemFactory,
            [Frozen] Mock<IServiceRecipientRepository> repository,
            Order order,
            OrderItem item,
            CreateOrderItemModel model,
            CreateOrderItemService service)
        {
            static void ServiceRecipientsIsEmpty(string _, IEnumerable<ServiceRecipient> serviceRecipients)
            {
                serviceRecipients.Should().BeEmpty();
            }

            model.ServiceRecipient = null;
            var request = model.ToRequest(order);

            orderItemFactory.Setup(f => f.Create(request)).Returns(item);
            repository.Setup(r => r.UpdateWithoutSavingAsync(It.IsAny<string>(), It.IsNotNull<IEnumerable<ServiceRecipient>>()))
                .Callback<string, IEnumerable<ServiceRecipient>>(ServiceRecipientsIsEmpty);

            await service.CreateAsync(order, new[] { request });
        }

        [Test]
        [OrderingAutoData]
        public static async Task CreateAsync_Order_IEnumerable_CreateOrderItemRequest_AddsServiceRecipient(
            [Frozen] Mock<IOrderItemFactory> orderItemFactory,
            [Frozen] Mock<IServiceRecipientRepository> repository,
            Order order,
            OrderItem item,
            CreateOrderItemSolutionRequest request,
            CreateOrderItemService service)
        {
            void ServiceRecipientsIsEmpty(string _, IEnumerable<ServiceRecipient> serviceRecipients)
            {
                serviceRecipients.Should().Contain(request.ServiceRecipient);
            }

            orderItemFactory.Setup(f => f.Create(request)).Returns(item);
            repository.Setup(r => r.UpdateWithoutSavingAsync(It.IsAny<string>(), It.IsNotNull<IEnumerable<ServiceRecipient>>()))
                .Callback<string, IEnumerable<ServiceRecipient>>(ServiceRecipientsIsEmpty);

            await service.CreateAsync(order, new[] { request });
        }

        [Test]
        [OrderingAutoData]
        public static async Task CreateAsync_Order_IEnumerable_CreateOrderItemRequest_FailedValidation_DoesNotInvokeUpdateWithoutSavingAsync(
            [Frozen] Mock<ICreateOrderItemValidator> orderItemValidator,
            [Frozen] Mock<IOrderItemFactory> orderItemFactory,
            AggregateValidationResult aggregateResult,
            Order order,
            OrderItem item,
            [Frozen] Mock<IServiceRecipientRepository> repository,
            CreateOrderItemRequest request,
            CreateOrderItemService service)
        {
            aggregateResult.AddValidationResult(new ValidationResult(new ErrorDetails("Id", "Field")), 0);
            orderItemValidator
                .Setup(v => v.Validate(It.IsNotNull<IReadOnlyList<CreateOrderItemRequest>>(), It.IsNotNull<IEnumerable<OrderItem>>()))
                .Returns(aggregateResult);

            orderItemFactory.Setup(f => f.Create(request)).Returns(item);

            await service.CreateAsync(order, new[] { request });

            repository.Verify(
                s => s.UpdateWithoutSavingAsync(It.Is<string>(o => o == order.OrderId), It.IsNotNull<IEnumerable<ServiceRecipient>>()),
                Times.Never());
        }

        [Test]
        [OrderingAutoData]
        public static async Task CreateAsync_Order_IEnumerable_CreateOrderItemRequest_InvokesServiceUpdateWithoutSavingAsync(
            [Frozen] Mock<IOrderItemFactory> orderItemFactory,
            Order order,
            OrderItem item,
            [Frozen] Mock<IServiceRecipientRepository> repository,
            CreateOrderItemRequest request,
            CreateOrderItemService service)
        {
            orderItemFactory.Setup(f => f.Create(request)).Returns(item);

            await service.CreateAsync(order, new[] { request });

            repository.Verify(s => s.UpdateWithoutSavingAsync(
                It.Is<string>(o => o == order.OrderId),
                It.IsNotNull<IEnumerable<ServiceRecipient>>()));
        }

        [Test]
        [OrderingAutoData]
        public static async Task CreateAsync_Order_IEnumerable_CreateOrderItemRequest_FailedValidation_DoesNotInvokeUpdateOrderAsync(
            [Frozen] Mock<ICreateOrderItemValidator> orderItemValidator,
            [Frozen] Mock<IOrderItemFactory> orderItemFactory,
            [Frozen] Mock<IOrderRepository> repository,
            AggregateValidationResult aggregateResult,
            Order order,
            OrderItem item,
            CreateOrderItemRequest request,
            CreateOrderItemService service)
        {
            aggregateResult.AddValidationResult(new ValidationResult(new ErrorDetails("Id", "Field")), 0);
            orderItemValidator
                .Setup(v => v.Validate(It.IsNotNull<IReadOnlyList<CreateOrderItemRequest>>(), It.IsNotNull<IEnumerable<OrderItem>>()))
                .Returns(aggregateResult);

            orderItemFactory.Setup(f => f.Create(request)).Returns(item);

            await service.CreateAsync(order, new[] { request });

            repository.Verify(r => r.UpdateOrderAsync(order), Times.Never());
        }

        [Test]
        [OrderingAutoData]
        public static async Task CreateAsync_Order_IEnumerable_CreateOrderItemRequest_FailedValidation_AddsValidationResultsWithExpectedKey(
            [Frozen] Mock<ICreateOrderItemValidator> orderItemValidator,
            [Frozen] Mock<IOrderItemFactory> orderItemFactory,
            ErrorDetails errorDetails,
            Order order,
            OrderItem item1,
            OrderItem item2,
            CreateOrderItemRequest request1,
            CreateOrderItemRequest request2,
            CreateOrderItemService service)
        {
            var expectedKeys = new[]
            {
                $"[0].{errorDetails.Field}",
                $"[1].{errorDetails.Field}",
            };

            var validationResult = new ValidationResult(new[] { errorDetails });
            var aggregateResult = new AggregateValidationResult();
            aggregateResult.AddValidationResult(validationResult, 0);
            aggregateResult.AddValidationResult(validationResult, 1);

            orderItemValidator
                .Setup(v => v.Validate(It.IsNotNull<IReadOnlyList<CreateOrderItemRequest>>(), It.IsNotNull<IEnumerable<OrderItem>>()))
                .Returns(aggregateResult);

            orderItemFactory
                .SetupSequence(f => f.Create(It.IsNotNull<CreateOrderItemRequest>()))
                .Returns(item1)
                .Returns(item2);

            var requests = new[] { request1, request2 };

            var result = await service.CreateAsync(order, requests);

            var errors = result.ToModelErrors();
            errors.Select(e => e.Key).Should().BeEquivalentTo(expectedKeys);
        }

        [Test]
        [OrderingAutoData]
        public static void CreateAsync_Order_IEnumerable_CreateOrderItemRequest_DuplicateOrderItemId_ThrowsException(
            [Frozen] Mock<ICreateOrderItemValidator> orderItemValidator,
            [Frozen] Mock<IOrderItemFactory> orderItemFactory,
            AggregateValidationResult aggregateResult,
            Order order,
            OrderItem item,
            CreateOrderItemRequest request,
            CreateOrderItemService service)
        {
            orderItemValidator
                .Setup(v => v.Validate(It.IsNotNull<IReadOnlyList<CreateOrderItemRequest>>(), It.IsNotNull<IEnumerable<OrderItem>>()))
                .Returns(aggregateResult);

            orderItemFactory
                .Setup(f => f.Create(It.IsNotNull<CreateOrderItemRequest>()))
                .Returns(item);

            var requests = new[] { request, request };

            Assert.ThrowsAsync<InvalidOperationException>(async () => await service.CreateAsync(order, requests));
        }

        [Test]
        [OrderingAutoData]
        public static async Task CreateAsync_Order_IEnumerable_CreateOrderItemRequest_InvokesUpdateOrderAsync(
            [Frozen] Mock<IOrderItemFactory> orderItemFactory,
            [Frozen] Mock<IOrderRepository> repository,
            Order order,
            OrderItem item,
            CreateOrderItemRequest request,
            CreateOrderItemService service)
        {
            orderItemFactory.Setup(f => f.Create(request)).Returns(item);

            await service.CreateAsync(order, new[] { request });

            repository.Verify(r => r.UpdateOrderAsync(order));
        }

        [Test]
        [OrderingAutoData]
        public static async Task CreateAsync_Order_IEnumerable_CreateOrderItemRequest_ReturnsExpectedResult(
            [Frozen] Mock<IOrderItemFactory> orderItemFactory,
            [Frozen] Mock<ICreateOrderItemValidator> orderItemValidator,
            AggregateValidationResult aggregateResult,
            Order order,
            OrderItem item,
            CreateOrderItemRequest request,
            CreateOrderItemService service)
        {
            orderItemFactory.Setup(f => f.Create(request)).Returns(item);
            orderItemValidator
                .Setup(v => v.Validate(It.IsNotNull<IReadOnlyList<CreateOrderItemRequest>>(), It.IsNotNull<IEnumerable<OrderItem>>()))
                .Returns(aggregateResult);

            var result = await service.CreateAsync(order, new[] { request });

            result.Should().Be(aggregateResult);
            result.Success.Should().BeTrue();
        }
    }
}
