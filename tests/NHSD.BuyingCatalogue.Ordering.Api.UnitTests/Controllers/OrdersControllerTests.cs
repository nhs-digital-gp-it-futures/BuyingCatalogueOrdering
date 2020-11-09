using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Errors;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Summary;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CompleteOrder;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrder;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Common.Extensions;
using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class OrdersControllerTests
    {
        [Test]
        public static void Contructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrdersController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Test]
        public static async Task GetAsync_OrderDoesNotExist_ReturnsNotFound()
        {
            var context = OrdersControllerTestContext.Setup();
            context.Order = null;
            var response = await context.OrdersController.GetAsync("INVALID");
            response.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public static async Task GetAsync_WrongOrganisationId_ReturnsForbidden()
        {
            var context = OrdersControllerTestContext.Setup();

            const string orderId = "C0000014-01";
            context.Order = CreateGetTestData(orderId, Guid.NewGuid(), "ods").order;

            var response = await context.OrdersController.GetAsync(orderId);
            response.Result.Should().BeOfType<ForbidResult>();
        }

        [Test]
        public static async Task GetAsync_OrderExists_ReturnsResult()
        {
            var context = OrdersControllerTestContext.Setup();
            const string orderId = "C0000014-01";

            (Order order, OrderModel expectedOrder) = CreateGetTestData(orderId, context.PrimaryOrganisationId, "ods");

            context.Order = order;

            var response = await context.OrdersController.GetAsync(orderId);
            response.Value.Should().BeEquivalentTo(expectedOrder);
        }

        [Test]
        public static async Task GetAllAsync_NoOrdersExist_ReturnsEmptyResult()
        {
            var context = OrdersControllerTestContext.Setup();

            var controller = context.OrdersController;

            var result = await controller.GetAllAsync(context.PrimaryOrganisationId);
            var orders = result.Value;
            orders.Should().BeEmpty();
        }

        [TestCase(null, "Some Description")]
        [TestCase("C0000014-01", "Some Description")]
        public static async Task GetAllAsync_SingleOrderWithOrganisationIdExists_ReturnsTheOrder(
            string orderId,
            string orderDescription)
        {
            var context = OrdersControllerTestContext.Setup();
            var orders = new List<(Order order, OrderListItemModel expected)>
            {
                CreateOrderTestData(orderId, context.PrimaryOrganisationId, orderDescription)
            };

            context.Orders = orders.Select(x => x.order);

            var controller = context.OrdersController;

            var result = await controller.GetAllAsync(context.PrimaryOrganisationId);
            var ordersResult = result.Value;
            ordersResult.Should().ContainSingle();
            ordersResult.Should().BeEquivalentTo(orders.Select(x => x.expected));
        }

        [Test]
        public static async Task GetAllAsync_SingleOrderWithOtherOrganisationIdExists_ReturnsForbidden()
        {
            var otherOrganisationId = Guid.NewGuid();
            var context = OrdersControllerTestContext.Setup();
            var orders = new List<(Order order, OrderListItemModel expected)>
            {
                CreateOrderTestData("C0000014-01", otherOrganisationId, "A description")
            };

            context.Orders = orders.Select(x => x.order);

            var controller = context.OrdersController;

            var result = await controller.GetAllAsync(otherOrganisationId);
            result.Should().NotBeNull();
            result.Result.Should().BeOfType<ForbidResult>();
        }

        [Test]
        public static async Task GetAllAsync_MultipleOrdersWithOrganisationIdExist_ReturnsAllOrders()
        {
            var context = OrdersControllerTestContext.Setup();

            var orders = new List<(Order order, OrderListItemModel expected)>
            {
                CreateOrderTestData("C0000014-01", context.PrimaryOrganisationId, "Some Description"),
                CreateOrderTestData("C000012-01", context.PrimaryOrganisationId, "Another Description")
            };

            context.Orders = orders.Select(x => x.order);

            var controller = context.OrdersController;

            var result = await controller.GetAllAsync(context.PrimaryOrganisationId);
            var ordersResult = result.Value;
            ordersResult.Count.Should().Be(2);
            ordersResult.Should().BeEquivalentTo(orders.Select(x => x.expected));
        }

        [Test]
        public static async Task GetAll_OrdersByOrganisationId_CalledOnce()
        {
            var context = OrdersControllerTestContext.Setup();

            var controller = context.OrdersController;

            await controller.GetAllAsync(context.PrimaryOrganisationId);

            context.OrderRepositoryMock.Verify(x => x.ListOrdersByOrganisationIdAsync(context.PrimaryOrganisationId),
                Times.Once);
        }

        [Test]
        [OrderingAutoData]
        public static async Task GetOrderSummaryAsync_OrderNotFound_ReturnsNotFound(
            string orderId,
            OrdersController controller)
        {
            var response = await controller.GetOrderSummaryAsync(orderId);

            response.Should().BeOfType<ActionResult<OrderSummaryModel>>();
            response.As<ActionResult<OrderSummaryModel>>().Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public static async Task GetOrderSummaryAsync_IsSummaryComplete_ReturnResult()
        {
            const string orderId = "C0000014-01";
            var context = OrdersControllerTestContext.Setup();

            (Order order, OrderSummaryModel expected) =
                CreateOrderSummaryTestData(orderId, "Some Description", context.PrimaryOrganisationId);

            context.Order = order;

            var controller = context.OrdersController;

            var response = await controller.GetOrderSummaryAsync(orderId);
            response.Should().BeEquivalentTo(new ActionResult<OrderSummaryModel>(new OkObjectResult(expected)));
        }

        [Test]
        public static async Task GetOrderSummaryAsync_OtherOrganisationId_ReturnResult()
        {
            var organisationId = Guid.NewGuid();
            const string orderId = "C0000014-01";
            var context = OrdersControllerTestContext.Setup();

            (Order order, _) = CreateOrderSummaryTestData(orderId, "Some Description", organisationId);

            context.Order = order;

            var controller = context.OrdersController;

            var response = await controller.GetOrderSummaryAsync(orderId);
            response.Should().BeEquivalentTo(new ActionResult<OrderSummaryModel>(new ForbidResult()));
        }

        [TestCaseSource(typeof(SummaryModelSectionTestCaseData), nameof(SummaryModelSectionTestCaseData.OrderDescriptionSectionStatusCases))]
        [TestCaseSource(typeof(SummaryModelSectionTestCaseData), nameof(SummaryModelSectionTestCaseData.OrderingPartySectionStatusCases))]
        [TestCaseSource(typeof(SummaryModelSectionTestCaseData), nameof(SummaryModelSectionTestCaseData.SupplierSectionStatusCases))]
        [TestCaseSource(typeof(SummaryModelSectionTestCaseData), nameof(SummaryModelSectionTestCaseData.CommencementDateSectionStatusCases))]
        [TestCaseSource(typeof(SummaryModelSectionTestCaseData), nameof(SummaryModelSectionTestCaseData.ServiceRecipientsSectionStatusCases))]
        [TestCaseSource(typeof(SummaryModelSectionTestCaseData), nameof(SummaryModelSectionTestCaseData.CatalogueSolutionsSectionStatusCases))]
        [TestCaseSource(typeof(SummaryModelSectionTestCaseData), nameof(SummaryModelSectionTestCaseData.AdditionalServicesSectionStatusCases))]
        [TestCaseSource(typeof(SummaryModelSectionTestCaseData), nameof(SummaryModelSectionTestCaseData.AssociatedServicesSectionStatusCases))]
        [TestCaseSource(typeof(SummaryModelSectionTestCaseData), nameof(SummaryModelSectionTestCaseData.FundingStatusCases))]
        [TestCaseSource(typeof(SummaryModelSectionTestCaseData), nameof(SummaryModelSectionTestCaseData.SectionStatusCases))]
        public static async Task GetOrderSummaryAsync_ChangeOrderData_ReturnsExpectedSummary(Order order,
            OrderSummaryModel expected)
        {
            var context = OrdersControllerTestContext.Setup(order.OrganisationId);
            context.Order = order;
            context.ServiceRecipientListCount = order.ServiceRecipients.Count;
            var controller = context.OrdersController;

            var response = (await controller.GetOrderSummaryAsync(context.Order.OrderId)).Result as OkObjectResult;
            Assert.IsNotNull(response);

            var actual = response.Value.As<OrderSummaryModel>();
            actual.Should().BeEquivalentTo(expected);
        }

        [TestCase]
        public static async Task GetOrderSummaryAsync_ServiceRecipientCount_ReturnsCountOfTwo()
        {
            var order = OrderBuilder.Create().Build();

            var context = OrdersControllerTestContext.Setup(order.OrganisationId);
            context.Order = order;
            context.ServiceRecipientListCount = 2;

            var controller = context.OrdersController;

            string expectedOrderId = context.Order.OrderId;

            var response = (await controller.GetOrderSummaryAsync(expectedOrderId)).Result as OkObjectResult;
            Assert.IsNotNull(response);

            var actual = response.Value.As<OrderSummaryModel>();

            var expected = OrderSummaryModelBuilder
                .Create()
                .WithOrderId(expectedOrderId)
                .WithOrganisationId(order.OrganisationId)
                .WithSections(SectionModelListBuilder
                    .Create()
                    .WithServiceRecipients(
                        SectionModel
                            .ServiceRecipients
                            .WithStatus("incomplete")
                            .WithCount(context.ServiceRecipientListCount))
                    .Build())
                .WithSectionStatus("incomplete")
                .Build();

            actual.Should().BeEquivalentTo(expected);
        }

        [TestCase]
        public static async Task GetOrderSummaryAsync_CatalogueSolutionCount_ReturnsCountOfTwo()
        {
            var order = OrderBuilder.Create()
                .WithOrderItem(OrderItemBuilder.Create()
                    .WithCatalogueItemType(CatalogueItemType.Solution)
                    .Build())
                .WithOrderItem(OrderItemBuilder.Create()
                    .WithCatalogueItemType(CatalogueItemType.Solution)
                    .Build()
                ).Build();

            var context = OrdersControllerTestContext.Setup(order.OrganisationId);
            context.Order = order;

            var response = (await context.OrdersController.GetOrderSummaryAsync(order.OrderId)).Result as OkObjectResult;
            Assert.IsNotNull(response);

            var actual = response.Value.As<OrderSummaryModel>();

            var expected = OrderSummaryModelBuilder
                .Create()
                .WithOrderId(order.OrderId)
                .WithOrganisationId(order.OrganisationId)
                .WithSections(SectionModelListBuilder
                    .Create()
                    .WithServiceRecipients(
                        SectionModel
                            .ServiceRecipients
                            .WithStatus("incomplete")
                            .WithCount(context.ServiceRecipientListCount))
                    .WithCatalogueSolutions(SectionModel.CatalogueSolutions
                        .WithStatus("incomplete")
                        .WithCount(2))
                    .Build())
                .WithSectionStatus("incomplete")
                .Build();

            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public static async Task GetOrderSummaryAsync_AssociatedServiceCount_ReturnsCountOfTwo()
        {
            var order = OrderBuilder.Create()
                .WithOrderItem(OrderItemBuilder.Create()
                    .WithCatalogueItemType(CatalogueItemType.AssociatedService)
                    .Build())
                .WithOrderItem(OrderItemBuilder.Create()
                    .WithCatalogueItemType(CatalogueItemType.AssociatedService)
                    .Build())
                .WithOrderItem(OrderItemBuilder.Create()
                    .WithCatalogueItemType(CatalogueItemType.Solution)
                    .Build())
                .Build();

            var context = OrdersControllerTestContext.Setup(order.OrganisationId);
            context.Order = order;

            var response = (await context.OrdersController.GetOrderSummaryAsync(order.OrderId)).Result as OkObjectResult;
            Assert.IsNotNull(response);

            var actual = response.Value.As<OrderSummaryModel>();

            var expected = OrderSummaryModelBuilder
                .Create()
                .WithOrderId(order.OrderId)
                .WithOrganisationId(order.OrganisationId)
                .WithSections(SectionModelListBuilder
                    .Create()
                    .WithServiceRecipients(
                        SectionModel
                            .ServiceRecipients
                            .WithStatus("incomplete")
                            .WithCount(context.ServiceRecipientListCount))
                    .WithCatalogueSolutions(SectionModel.CatalogueSolutions
                        .WithStatus("incomplete")
                        .WithCount(1))
                    .WithAssociatedServices(SectionModel.AssociatedServices
                        .WithStatus("incomplete")
                        .WithCount(2))
                    .Build())
                .WithSectionStatus("incomplete")
                .Build();

            actual.Should().BeEquivalentTo(expected);
        }

        [TestCase]
        public static async Task GetOrderSummaryAsync_ServiceRecipientRepository_CalledOnce()
        {
            var order = OrderBuilder.Create().Build();

            var context = OrdersControllerTestContext.Setup(order.OrganisationId);
            context.Order = order;
            context.ServiceRecipientListCount = 2;

            var controller = context.OrdersController;

            string expectedOrderId = context.Order.OrderId;

            await controller.GetOrderSummaryAsync(expectedOrderId);

            context.ServiceRecipientRepositoryMock.Verify(x => x.GetCountByOrderIdAsync(expectedOrderId), Times.Once);
        }

        [Test]
        public static async Task CreateOrderAsync_CreateOrderSuccessfulResult_ReturnsOrderId()
        {
            const string newOrderId = "New Test Order Id";

            var context = OrdersControllerTestContext.Setup();
            context.CreateOrderResult = Result.Success(newOrderId);

            var createOrderRequest = new CreateOrderModel
            {
                Description = "Test Order 1",
                OrganisationId = context.PrimaryOrganisationId
            };

            var controller = context.OrdersController;

            var response = await controller.CreateOrderAsync(createOrderRequest);

            var actual = response.Result;

            var expectation = new CreatedAtActionResult(nameof(controller.CreateOrderAsync).TrimAsync(), null,
                new { orderId = newOrderId }, new ErrorResponseModel { OrderId = newOrderId });

            actual.Should().BeEquivalentTo(expectation);
        }

        [Test]
        public static async Task CreateOrderAsync_CreateOrderService_CreateAsync_CalledOnce()
        {
            var context = OrdersControllerTestContext.Setup();

            var controller = context.OrdersController;

            var createOrderModel = new CreateOrderModel
            {
                Description = "Description1",
                OrganisationId = context.PrimaryOrganisationId
            };

            await controller.CreateOrderAsync(createOrderModel);

            context.CreateOrderServiceMock.Verify(x => x.CreateAsync(It.IsAny<CreateOrderRequest>()), Times.Once);
        }

        [Test]
        public static async Task CreateOrderAsync_CreateOrderFailureResult_ReturnsBadRequest()
        {
            var context = OrdersControllerTestContext.Setup();
            var controller = context.OrdersController;

            var errors = new List<ErrorDetails> { new ErrorDetails("TestErrorId", "TestField") };

            var createOrderRequest = new CreateOrderModel
            {
                Description = "Test Order 1",
                OrganisationId = context.PrimaryOrganisationId
            };

            context.CreateOrderResult = Result.Failure<string>(errors);

            var response = await controller.CreateOrderAsync(createOrderRequest);

            response.Should().BeOfType<ActionResult<ErrorResponseModel>>();
            var actual = response.Result;

            var expectedErrors =
                new List<ErrorModel> { new ErrorModel("TestErrorId", "TestField") };
            var expected = new BadRequestObjectResult(new ErrorResponseModel { Errors = expectedErrors });
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public static async Task CreateOrderAsync_IncludesOrganisationAsServiceRecipient()
        {
            const string odsCode = "Ods Code";
            var orgId = Guid.NewGuid();

            var orderItem = OrderItemBuilder.Create()
                .WithOdsCode(odsCode)
                .Build();

            var order = OrderBuilder.Create()
                .WithOrderItem(orderItem)
                .WithOrganisationId(orgId)
                .Build();

            var context = OrdersControllerTestContext.Setup(orgId);
            context.Order = order;

            var controller = context.OrdersController;

            var response = await controller.GetAsync("order-id");
            var serviceRecipients = response.Value.ServiceRecipients.ToList();

            serviceRecipients.Should().HaveCount(1);
            serviceRecipients[0].Name.Should().Be(context.Order.OrganisationName);
            serviceRecipients[0].OdsCode.Should().Be(odsCode);
        }

        [Test]
        public static void CreateOrderAsync_NullApplicationUser_ThrowsException()
        {
            var context = OrdersControllerTestContext.Setup();

            async Task<ActionResult<ErrorResponseModel>> CreateOrder()
            {
                var controller = context.OrdersController;
                return await controller.CreateOrderAsync(null);
            }

            Assert.ThrowsAsync<ArgumentNullException>(CreateOrder);
        }

        [Test]
        public static async Task CreateOrderAsync_InvalidPrimaryOrganisationId_ReturnsForbid()
        {
            var context = OrdersControllerTestContext.Setup();
            context.Order = OrderBuilder
                .Create()
                .WithOrganisationId(Guid.NewGuid())
                .Build();

            var result = await context.OrdersController.CreateOrderAsync(new CreateOrderModel());

            result.Result.Should().BeOfType<ForbidResult>();
        }

        [Test]
        public static async Task DeleteOrderAsync_DeleteOrderSuccessful_ReturnsNoContent()
        {
            var context = OrdersControllerTestContext.Setup();

            var response = await context.OrdersController.DeleteOrderAsync("Order");
            response.Should().BeOfType<NoContentResult>();
        }

        [Test]
        public static async Task DeleteOrderAsync_DeleteOrderSuccessful_DeletesAndUpdatesOrder()
        {
            var context = OrdersControllerTestContext.Setup();

            await context.OrdersController.DeleteOrderAsync("Order");
            context.Order.IsDeleted.Should().BeTrue();
            context.OrderRepositoryMock.Verify(x => x.UpdateOrderAsync(It.Is<Order>(y => y.IsDeleted)));
        }

        [Test]
        public static async Task DeleteOrderAsync_DeleteOrderSuccessful_LastUpdatedIsUpdated()
        {
            var context = OrdersControllerTestContext.Setup();
            await context.OrdersController.DeleteOrderAsync("Order");
            context.Order.LastUpdatedBy.Should().Be(context.NameIdentity);
            context.Order.LastUpdatedByName.Should().Be(context.Name);
            context.Order.LastUpdated.Should().NotBe(DateTime.MinValue);
        }

        [Test]
        public static async Task DeleteOrderAsync_OrderNotFound_ReturnsNotFound()
        {
            var context = OrdersControllerTestContext.Setup();
            context.Order = null;

            var response = await context.OrdersController.DeleteOrderAsync("Order");
            response.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public static async Task DeleteOrderAsync_OrderDeleted_ReturnsNotFound()
        {
            var context = OrdersControllerTestContext.Setup();
            context.Order.IsDeleted = true;

            var response = await context.OrdersController.DeleteOrderAsync("Order");
            response.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public static async Task DeleteOrderAsync_OrderForDifferentOrganisation_ReturnsForbidden()
        {
            var context = OrdersControllerTestContext.Setup();
            context.Order = OrderBuilder.Create()
                .WithOrganisationId(Guid.NewGuid())
                .Build();

            var response = await context.OrdersController.DeleteOrderAsync("Order");
            response.Should().BeOfType<ForbidResult>();
        }

        [Test]
        public static void UpdateStatusAsync_NullModel_ThrowsException()
        {
            var context = OrdersControllerTestContext.Setup();
            Assert.ThrowsAsync<ArgumentNullException>(() => context.OrdersController.UpdateStatusAsync("Order", null));
        }

        [Test]
        public static async Task UpdateStatusAsync_NullOrder_ReturnsNotFound()
        {
            var context = OrdersControllerTestContext.Setup();
            context.Order = null;

            var response = await context.OrdersController.UpdateStatusAsync("Order", context.CompleteOrderStatusModel);
            response.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public static async Task UpdateStatusAsync_OrderForDifferentOrganisation_ReturnsBadRequest()
        {
            var context = OrdersControllerTestContext.Setup();
            context.Order = OrderBuilder.Create()
                .WithOrganisationId(Guid.NewGuid())
                .Build();

            var response = await context.OrdersController.UpdateStatusAsync("Order", context.CompleteOrderStatusModel);
            response.Result.Should().BeOfType<ForbidResult>();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("Incomplete")]
        public static async Task UpdateStatusAsync_InvalidOrderStatus_ReturnsInvalidOrderStatusError(string orderStatusInput)
        {
            var context = OrdersControllerTestContext.Setup();

            var response = await context.OrdersController.UpdateStatusAsync("Order", new StatusModel { Status = orderStatusInput });
            var actual = response.Result.As<BadRequestObjectResult>().Value;

            var expected = new ErrorResponseModel
            {
                Errors = new List<ErrorModel> { ErrorMessages.InvalidOrderStatus() }
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public static async Task UpdateStatusAsync_OrderIsComplete_ReturnsNoContent()
        {
            var context = OrdersControllerTestContext.Setup();
            var orderItem = OrderItemBuilder.Create()
                .WithCatalogueItemType(CatalogueItemType.Solution)
                .Build();

            context.Order = OrderBuilder.Create()
                .WithOrganisationId(context.PrimaryOrganisationId)
                .WithOrderItem(orderItem)
                .WithFundingSourceOnlyGms(true)
                .WithAssociatedServicesViewed(true)
                .WithCatalogueSolutionsViewed(true)
                .Build();

            var response = await context.OrdersController.UpdateStatusAsync("Order", context.CompleteOrderStatusModel);
            response.Result.Should().BeOfType<NoContentResult>();
        }

        [Test]
        public static async Task UpdateStatusAsync_OrderStatus_Complete_CompleteOrderServiceCalledOnce()
        {
            var context = OrdersControllerTestContext.Setup();
            var orderItem = OrderItemBuilder.Create()
                .WithCatalogueItemType(CatalogueItemType.Solution)
                .Build();

            context.Order = OrderBuilder.Create()
                .WithOrganisationId(context.PrimaryOrganisationId)
                .WithOrderItem(orderItem)
                .WithFundingSourceOnlyGms(true)
                .WithAssociatedServicesViewed(true)
                .WithCatalogueSolutionsViewed(true)
                .Build();

            await context.OrdersController.UpdateStatusAsync("Order", context.CompleteOrderStatusModel);

            context.CompleteOrderServiceMock
                .Verify(x => x.CompleteAsync(
                    It.Is<CompleteOrderRequest>(request => request.Order.Equals(context.Order))), Times.Once);
        }

        [Test]
        public static async Task UpdateStatusAsync_CompleteOrderFailed_ReturnsError()
        {
            var context = OrdersControllerTestContext.Setup();
            var orderItem = OrderItemBuilder.Create()
                .WithCatalogueItemType(CatalogueItemType.Solution)
                .Build();

            context.Order = OrderBuilder.Create()
                .WithOrganisationId(context.PrimaryOrganisationId)
                .WithOrderItem(orderItem)
                .WithFundingSourceOnlyGms(true)
                .WithAssociatedServicesViewed(true)
                .WithCatalogueSolutionsViewed(true)
                .Build();

            var expectedErrorDetails = new ErrorDetails("Some error", "Some field name");
            context.CompleteOrderResult = Result.Failure(expectedErrorDetails);

            var response = await context.OrdersController.UpdateStatusAsync("Order", context.CompleteOrderStatusModel);
            var actual = response.Result.As<BadRequestObjectResult>().Value;

            var expected = new ErrorResponseModel
            {
                Errors = new List<ErrorModel> { new ErrorModel(expectedErrorDetails.Id, expectedErrorDetails.Field) }
            };

            actual.Should().BeEquivalentTo(expected);
        }

        private static (Order order, OrderListItemModel expectedOrder) CreateOrderTestData(
            string orderId,
            Guid organisationId,
            string description)
        {
            var repositoryOrder = OrderBuilder
                .Create()
                .WithOrderId(orderId)
                .WithOrganisationId(organisationId)
                .WithDescription(description)
                .Build();

            return (order: repositoryOrder,
                expectedOrder: new OrderListItemModel
                {
                    OrderId = repositoryOrder.OrderId,
                    Description = repositoryOrder.Description.Value,
                    Status = repositoryOrder.OrderStatus.Name,
                    DateCreated = repositoryOrder.Created,
                    LastUpdated = repositoryOrder.LastUpdated,
                    LastUpdatedBy = repositoryOrder.LastUpdatedByName,
                    OnlyGms = repositoryOrder.FundingSourceOnlyGMS
                });
        }

        private static (Order order, OrderSummaryModel expectedSummary) CreateOrderSummaryTestData(string orderId,
            string description, Guid organisationId)
        {
            var repositoryOrder = OrderBuilder
                .Create()
                .WithOrderId(orderId)
                .WithDescription(description)
                .WithOrganisationId(organisationId)
                .Build();

            return (order: repositoryOrder,
                expectedSummary: new OrderSummaryModel
                {
                    OrderId = repositoryOrder.OrderId,
                    OrganisationId = repositoryOrder.OrganisationId,
                    Description = repositoryOrder.Description.Value,
                    Sections = new List<SectionModel>
                    {
                        SectionModel.Description.WithStatus(
                            string.IsNullOrWhiteSpace(repositoryOrder.Description.Value)
                                ? "incomplete"
                                : "complete"),
                        SectionModel.OrderingParty,
                        SectionModel.Supplier,
                        SectionModel.CatalogueSolutions,
                        SectionModel.AssociatedServices,
                        SectionModel.ServiceRecipients,
                        SectionModel.CatalogueSolutions,
                        SectionModel.AdditionalServices,
                        SectionModel.FundingSource
                    }
                });
        }

        private static (Order order, OrderModel expectedOrder) CreateGetTestData(string orderId, Guid organisationId, string odsCode)
        {
            var repositoryOrder = OrderBuilder.Create()
                .WithOrderId(orderId)
                .WithOrganisationId(organisationId)
                .WithCompleted(DateTime.UtcNow)
                .Build();

            var repositoryOrderItem = OrderItemBuilder.Create()
                .WithOdsCode(odsCode)
                .Build();

            var oneOffOrderItem = OrderItemBuilder.Create()
                .WithOdsCode(odsCode)
                .WithCatalogueItemType(CatalogueItemType.AssociatedService)
                .WithProvisioningType(ProvisioningType.Declarative)
                .WithEstimationPeriod(null)
                .WithPriceTimeUnit(null)
                .Build();

            var serviceRecipients = new List<OdsOrganisation>
            {
                new OdsOrganisation(odsCode, "EU test")
            };

            repositoryOrder.AddOrderItem(repositoryOrderItem, Guid.Empty, string.Empty);
            repositoryOrder.AddOrderItem(oneOffOrderItem, Guid.Empty, string.Empty);
            repositoryOrder.SetServiceRecipients(serviceRecipients, Guid.Empty, string.Empty);

            const int monthsPerYear = 12;
            var calculatedCostPerYear = repositoryOrder.CalculateCostPerYear(CostType.Recurring);
            var totalOneOffCost = repositoryOrder.CalculateCostPerYear(CostType.OneOff);

            return (order: repositoryOrder, expectedOrder: new OrderModel
            {
                Description = repositoryOrder.Description.Value,
                OrderParty = new OrderingPartyModel
                {
                    Name = repositoryOrder.OrganisationName,
                    OdsCode = repositoryOrder.OrganisationOdsCode,
                    Address = repositoryOrder.OrganisationAddress.ToModel(),
                    PrimaryContact = repositoryOrder.OrganisationContact.ToModel()
                },
                CommencementDate = repositoryOrder.CommencementDate,
                Supplier = new SupplierModel
                {
                    Name = repositoryOrder.SupplierName,
                    Address = repositoryOrder.SupplierAddress.ToModel(),
                    PrimaryContact = repositoryOrder.SupplierContact.ToModel()
                },
                TotalOneOffCost = totalOneOffCost,
                TotalRecurringCostPerMonth = calculatedCostPerYear / monthsPerYear,
                TotalRecurringCostPerYear = calculatedCostPerYear,
                TotalOwnershipCost = repositoryOrder.CalculateTotalOwnershipCost(),
                Status = repositoryOrder.OrderStatus.Name,
                DateCompleted = repositoryOrder.Completed,
                ServiceRecipients = repositoryOrder.ServiceRecipients.Select(serviceRecipient =>
                    new ServiceRecipientModel
                    {
                        Name = serviceRecipient.Name,
                        OdsCode = serviceRecipient.OdsCode
                    }),
                OrderItems = repositoryOrder.OrderItems.Select(orderItem =>
                    new OrderItemModel
                    {
                        ItemId = $"{repositoryOrder.OrderId}-{orderItem.OdsCode}-{orderItem.OrderItemId}",
                        ServiceRecipientsOdsCode = orderItem.OdsCode,
                        CataloguePriceType = orderItem.CataloguePriceType.ToString(),
                        CatalogueItemType = orderItem.CatalogueItemType.ToString(),
                        CatalogueItemName = orderItem.CatalogueItemName,
                        ProvisioningType = orderItem.ProvisioningType.ToString(),
                        ItemUnitDescription = orderItem.CataloguePriceUnit.Description,
                        TimeUnitDescription = orderItem.PriceTimeUnit?.Description(),
                        QuantityPeriodDescription = orderItem.EstimationPeriod?.Description(),
                        Price = orderItem.Price,
                        Quantity = orderItem.Quantity,
                        CostPerYear = orderItem.CalculateTotalCostPerYear(),
                        DeliveryDate = orderItem.DeliveryDate
                    }),
            });
        }

        internal sealed class OrdersControllerTestContext
        {
            private OrdersControllerTestContext(Guid primaryOrganisationId)
            {
                Name = "Test User";
                NameIdentity = Guid.NewGuid();
                PrimaryOrganisationId = primaryOrganisationId;

                Order = OrderBuilder.Create()
                    .WithOrganisationId(PrimaryOrganisationId)
                    .WithLastUpdated(DateTime.MinValue)
                    .Build();

                OrderRepositoryMock = new Mock<IOrderRepository>();
                OrderRepositoryMock.Setup(x => x.GetOrderByIdAsync(It.IsAny<string>())).ReturnsAsync(() => Order);

                CreateOrderServiceMock = new Mock<ICreateOrderService>();
                CreateOrderServiceMock.Setup(x => x.CreateAsync(It.IsAny<CreateOrderRequest>()))
                    .ReturnsAsync(() => CreateOrderResult);

                Orders = new List<Order>();
                OrderRepositoryMock.Setup(x => x.ListOrdersByOrganisationIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(() => Orders);

                OrderRepositoryMock.Setup(x => x.GetOrderByIdAsync(It.IsAny<string>())).ReturnsAsync(() => Order);

                ServiceRecipientRepositoryMock = new Mock<IServiceRecipientRepository>();
                ServiceRecipientRepositoryMock
                    .Setup(x => x.GetCountByOrderIdAsync(It.IsNotNull<string>()))
                    .ReturnsAsync(() => ServiceRecipientListCount);

                CompleteOrderServiceMock = new Mock<ICompleteOrderService>();
                CompleteOrderServiceMock
                    .Setup(x => x.CompleteAsync(It.IsNotNull<CompleteOrderRequest>()))
                    .ReturnsAsync(() => CompleteOrderResult);

                ClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(
                    new[]
                    {
                        new Claim("Ordering", "Manage"),
                        new Claim("primaryOrganisationId", PrimaryOrganisationId.ToString()),
                        new Claim(ClaimTypes.Name, Name),
                        new Claim(ClaimTypes.NameIdentifier, NameIdentity.ToString())
                    }, "mock"));

                OrdersController = OrdersControllerBuilder
                    .Create()
                    .WithOrderRepository(OrderRepositoryMock.Object)
                    .WithServiceRecipientRepository(ServiceRecipientRepositoryMock.Object)
                    .WithCreateOrderService(CreateOrderServiceMock.Object)
                    .WithCompleteOrderService(CompleteOrderServiceMock.Object)
                    .Build();

                OrdersController.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = ClaimsPrincipal }
                };
            }

            internal StatusModel IncompleteOrderStatusModel { get; } = new StatusModel { Status = "incomplete" };

            internal StatusModel CompleteOrderStatusModel { get; } = new StatusModel { Status = "complete" };

            internal string Name { get; }

            internal Guid NameIdentity { get; }

            internal Guid PrimaryOrganisationId { get; }

            internal int ServiceRecipientListCount { get; set; }

            internal Mock<IOrderRepository> OrderRepositoryMock { get; }

            internal Mock<ICreateOrderService> CreateOrderServiceMock { get; }

            internal Mock<IServiceRecipientRepository> ServiceRecipientRepositoryMock { get; }

            internal Mock<ICompleteOrderService> CompleteOrderServiceMock { get; }

            internal Result<string> CreateOrderResult { get; set; } = Result.Success("NewOrderId");

            internal Result CompleteOrderResult { get; set; } = Result.Success();

            internal IEnumerable<Order> Orders { get; set; }

            internal Order Order { get; set; }

            internal OrdersController OrdersController { get; }

            private ClaimsPrincipal ClaimsPrincipal { get; }

            internal static OrdersControllerTestContext Setup()
            {
                return new OrdersControllerTestContext(Guid.NewGuid());
            }

            internal static OrdersControllerTestContext Setup(Guid primaryOrganisationId)
            {
                return new OrdersControllerTestContext(primaryOrganisationId);
            }
        }

        private class SummaryModelSectionTestCaseData
        {
            internal static IEnumerable<TestCaseData> OrderDescriptionSectionStatusCases
            {
                get
                {
                    var organisationId = Guid.NewGuid();

                    yield return new TestCaseData(
                        OrderBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithSupplierContact(null)
                            .WithOrganisationContact(null)
                            .WithCommencementDate(null)
                            .Build(),
                        OrderSummaryModelBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithSections(
                                SectionModelListBuilder
                                    .Create()
                                    .WithDescription(SectionModel.Description.WithStatus("complete"))
                                    .Build())
                            .Build());
                }
            }

            internal static IEnumerable<TestCaseData> OrderingPartySectionStatusCases
            {
                get
                {
                    var organisationId = Guid.NewGuid();

                    yield return new TestCaseData(
                        OrderBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithOrganisationContact(ContactBuilder.Create().Build())
                            .Build(),
                        OrderSummaryModelBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithSections(SectionModelListBuilder.Create()
                                .WithOrderingParty(SectionModel.OrderingParty.WithStatus("complete"))
                                .Build())
                            .Build());
                }
            }

            internal static IEnumerable<TestCaseData> SupplierSectionStatusCases
            {
                get
                {
                    var organisationId = Guid.NewGuid();

                    yield return new TestCaseData(
                        OrderBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithSupplierContact(ContactBuilder.Create().Build())
                            .Build(),
                        OrderSummaryModelBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithSections(SectionModelListBuilder.Create()
                                .WithSupplier(SectionModel.Supplier.WithStatus("complete"))
                                .Build())
                            .Build());
                }
            }

            internal static IEnumerable<TestCaseData> CommencementDateSectionStatusCases
            {
                get
                {
                    var organisationId = Guid.NewGuid();

                    yield return new TestCaseData(
                        OrderBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithCommencementDate(DateTime.UtcNow)
                            .Build(),
                        OrderSummaryModelBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithSections(SectionModelListBuilder.Create()
                                .WithCommencementDate(SectionModel.CommencementDate.WithStatus("complete"))
                                .Build())
                            .Build());
                }
            }

            internal static IEnumerable<TestCaseData> ServiceRecipientsSectionStatusCases
            {
                get
                {
                    var organisationId = Guid.NewGuid();

                    yield return new TestCaseData(
                        OrderBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithServiceRecipientsViewed(true)
                            .Build(),
                        OrderSummaryModelBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithSections(SectionModelListBuilder.Create()
                                .WithServiceRecipients(
                                    SectionModel
                                        .ServiceRecipients
                                        .WithStatus("complete")
                                        .WithCount(0))
                                .Build())
                            .Build());

                    yield return new TestCaseData(
                        OrderBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithServiceRecipientsViewed(false)
                            .Build(),
                        OrderSummaryModelBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithSections(SectionModelListBuilder
                                .Create()
                                .WithServiceRecipients(
                                    SectionModel
                                        .ServiceRecipients
                                        .WithStatus("incomplete")
                                        .WithCount(0))
                                .Build())
                            .Build());
                }
            }

            internal static IEnumerable<TestCaseData> AdditionalServicesSectionStatusCases
            {
                get
                {
                    var organisationId = Guid.NewGuid();

                    yield return new TestCaseData(
                        OrderBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithAdditionalServicesViewed(true)
                            .Build(),
                        OrderSummaryModelBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithSections(SectionModelListBuilder.Create()
                                .WithAdditionalServices(SectionModel.AdditionalServices
                                    .WithStatus("complete")
                                    .WithCount(0))
                                .Build())
                            .Build());

                    yield return new TestCaseData(
                        OrderBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithAdditionalServicesViewed(false)
                            .Build(),
                        OrderSummaryModelBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithSections(SectionModelListBuilder.Create()
                                .WithAdditionalServices(SectionModel.AdditionalServices
                                    .WithStatus("incomplete")
                                    .WithCount(0))
                                .Build())
                            .Build());

                    yield return new TestCaseData(
                        OrderBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithAdditionalServicesViewed(true)
                            .WithOrderItem(OrderItemBuilder.Create()
                                .WithCatalogueItemType(CatalogueItemType.AdditionalService)
                                .Build())
                            .Build(),
                        OrderSummaryModelBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithSections(SectionModelListBuilder.Create()
                                .WithAdditionalServices(SectionModel.AdditionalServices
                                    .WithStatus("complete")
                                    .WithCount(1))
                                .Build())
                            .Build());
                }
            }

            internal static IEnumerable<TestCaseData> CatalogueSolutionsSectionStatusCases
            {
                get
                {
                    var organisationId = Guid.NewGuid();

                    yield return new TestCaseData(
                        OrderBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithCatalogueSolutionsViewed(true)
                            .Build(),
                        OrderSummaryModelBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithSections(SectionModelListBuilder.Create()
                                .WithCatalogueSolutions(SectionModel.CatalogueSolutions
                                    .WithStatus("complete")
                                    .WithCount(0))
                                .Build())
                            .Build());

                    yield return new TestCaseData(
                        OrderBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithCatalogueSolutionsViewed(false)
                            .Build(),
                        OrderSummaryModelBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithSections(SectionModelListBuilder.Create()
                                .WithCatalogueSolutions(SectionModel.CatalogueSolutions
                                    .WithStatus("incomplete")
                                    .WithCount(0))
                                .Build())
                            .Build());

                    yield return new TestCaseData(
                        OrderBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithCatalogueSolutionsViewed(true)
                            .WithOrderItem(OrderItemBuilder.Create()
                                .WithCatalogueItemType(CatalogueItemType.Solution)
                                .Build())
                            .Build(),
                        OrderSummaryModelBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithSections(SectionModelListBuilder.Create()
                                .WithCatalogueSolutions(SectionModel.CatalogueSolutions
                                    .WithStatus("complete")
                                    .WithCount(1))
                                .Build())
                            .Build());
                }
            }

            internal static IEnumerable<TestCaseData> AssociatedServicesSectionStatusCases
            {
                get
                {
                    var organisationId = Guid.NewGuid();

                    yield return new TestCaseData(
                        OrderBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithAssociatedServicesViewed(true)
                            .Build(),
                        OrderSummaryModelBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithSections(SectionModelListBuilder.Create()
                                .WithAssociatedServices(SectionModel.AssociatedServices
                                    .WithStatus("complete")
                                    .WithCount(0))
                                .Build())
                            .Build());

                    yield return new TestCaseData(
                        OrderBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithAssociatedServicesViewed(false)
                            .Build(),
                        OrderSummaryModelBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithSections(SectionModelListBuilder.Create()
                                .WithAssociatedServices(SectionModel.AssociatedServices
                                    .WithStatus("incomplete")
                                    .WithCount(0))
                                .Build())
                            .Build());

                    yield return new TestCaseData(
                        OrderBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithAssociatedServicesViewed(true)
                            .WithOrderItem(OrderItemBuilder.Create()
                                .WithCatalogueItemType(CatalogueItemType.AssociatedService)
                                .Build())
                            .Build(),
                        OrderSummaryModelBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithSections(SectionModelListBuilder.Create()
                                .WithAssociatedServices(SectionModel.AssociatedServices
                                    .WithStatus("complete")
                                    .WithCount(1))
                                .Build())
                            .Build());
                }
            }

            internal static IEnumerable<TestCaseData> FundingStatusCases
            {
                get
                {
                    var organisationId = Guid.NewGuid();

                    yield return new TestCaseData(
                        OrderBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithFundingSourceOnlyGms(true)
                            .Build(),
                        OrderSummaryModelBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithSections(SectionModelListBuilder.Create()
                                .WithFundingSource(SectionModel.FundingSource
                                    .WithStatus("complete"))
                                .Build())
                            .Build());

                    yield return new TestCaseData(
                        OrderBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithFundingSourceOnlyGms(false)
                            .Build(),
                        OrderSummaryModelBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithSections(SectionModelListBuilder.Create()
                                .WithFundingSource(SectionModel.FundingSource
                                    .WithStatus("complete"))
                                .Build())
                            .Build());

                    yield return new TestCaseData(
                        OrderBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithFundingSourceOnlyGms(null)
                            .Build(),
                        OrderSummaryModelBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithSections(SectionModelListBuilder.Create()
                                .WithFundingSource(SectionModel.FundingSource
                                    .WithStatus("incomplete"))
                                .Build())
                            .Build());
                }
            }

            internal static IEnumerable<TestCaseData> SectionStatusCases
            {
                get
                {
                    var organisationId = Guid.NewGuid();

                    yield return new TestCaseData(
                        OrderBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithCatalogueSolutionsViewed(false)
                            .WithServiceRecipientsViewed(false)
                            .WithAssociatedServicesViewed(false)
                            .WithFundingSourceOnlyGms(null)
                            .Build(),
                        OrderSummaryModelBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithSections(SectionModelListBuilder.Create()
                                .WithCatalogueSolutions(SectionModel.CatalogueSolutions
                                    .WithStatus("incomplete")
                                    .WithCount(0))
                                .WithFundingSource(SectionModel.FundingSource.WithStatus("incomplete"))
                                .Build())
                            .WithSectionStatus("incomplete")
                            .Build());

                    yield return new TestCaseData(
                        OrderBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithCatalogueSolutionsViewed(true)
                            .WithOrderItem(OrderItemBuilder.Create().WithCatalogueItemType(CatalogueItemType.Solution).Build())
                            .WithServiceRecipientsViewed(true)
                            .WithAssociatedServicesViewed(true)
                            .WithFundingSourceOnlyGms(true)
                            .Build(),
                        OrderSummaryModelBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithSections(SectionModelListBuilder.Create()
                                .WithCatalogueSolutions(SectionModel.CatalogueSolutions.WithStatus("complete").WithCount(1))
                                .WithServiceRecipients(SectionModel.ServiceRecipients.WithStatus("complete").WithCount(0))
                                .WithAssociatedServices(SectionModel.AssociatedServices.WithStatus("complete").WithCount(0))
                                .WithFundingSource(SectionModel.FundingSource.WithStatus("complete"))
                                .Build())
                            .WithSectionStatus("complete")
                            .Build());

                    yield return new TestCaseData(
                        OrderBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithCatalogueSolutionsViewed(true)
                            .WithOrderItem(OrderItemBuilder.Create().WithCatalogueItemType(CatalogueItemType.Solution).Build())
                            .WithServiceRecipientsViewed(true)
                            .WithAssociatedServicesViewed(true)
                            .WithOrderItem(OrderItemBuilder.Create().WithCatalogueItemType(CatalogueItemType.AssociatedService).Build())
                            .WithFundingSourceOnlyGms(true)
                            .Build(),
                        OrderSummaryModelBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithSections(SectionModelListBuilder.Create()
                                .WithCatalogueSolutions(SectionModel.CatalogueSolutions
                                    .WithStatus("complete")
                                    .WithCount(1))
                                .WithServiceRecipients(SectionModel.ServiceRecipients.WithStatus("complete").WithCount(0))
                                .WithAssociatedServices(SectionModel.AssociatedServices.WithStatus("complete").WithCount(1))
                                .WithFundingSource(SectionModel.FundingSource.WithStatus("complete"))
                                .Build())
                            .WithSectionStatus("complete")
                            .Build());

                    yield return new TestCaseData(
                        OrderBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithCatalogueSolutionsViewed(true)
                            .WithServiceRecipientsViewed(true)
                            .WithServiceRecipient("ODS1", "Recip1")
                            .WithAssociatedServicesViewed(true)
                            .WithOrderItem(OrderItemBuilder.Create().WithCatalogueItemType(CatalogueItemType.AssociatedService).Build())
                            .WithFundingSourceOnlyGms(true)
                            .Build(),
                        OrderSummaryModelBuilder
                            .Create()
                            .WithOrganisationId(organisationId)
                            .WithSections(SectionModelListBuilder.Create()
                                .WithCatalogueSolutions(SectionModel.CatalogueSolutions
                                    .WithStatus("complete")
                                    .WithCount(0))
                                .WithServiceRecipients(SectionModel.ServiceRecipients.WithStatus("complete").WithCount(1))
                                .WithAssociatedServices(SectionModel.AssociatedServices.WithStatus("complete").WithCount(1))
                                .WithFundingSource(SectionModel.FundingSource.WithStatus("complete"))
                                .Build())
                            .WithSectionStatus("complete")
                            .Build());
                }
            }
        }
    }
}
