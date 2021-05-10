using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Equivalency;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Data;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class OrderItemPutSteps
    {
        private readonly Request request;
        private readonly Response response;
        private readonly Settings settings;

        private CreateOrderItemRequest createOrderItemRequest;

        public OrderItemPutSteps(
            Request request,
            Response response,
            Settings settings)
        {
            this.request = request;
            this.response = response;
            this.settings = settings;
        }

        [Given(@"the user creates a request to add the catalogue item with ID ([a-zA-Z0-9-]{1,14}) and the following details to the order with ID (\d{1,6})")]
        public void GivenTheUserCreatesARequestToAddACatalogueItemToTheOrderWithId(string catalogueItemId, int orderId, Table requestInfoTable)
        {
            requestInfoTable.RowCount.Should().Be(1);

            createOrderItemRequest = new CreateOrderItemRequest(
                orderId,
                catalogueItemId,
                requestInfoTable.CreateInstance<RequestInfo>(),
                settings);
        }

        [Given(@"for the following recipients")]
        public void GivenForTheFollowingRecipients(Table table)
        {
            createOrderItemRequest.SetRecipients(table.CreateSet<RecipientInfo>());
        }

        [When(@"the user sends the create order items request")]
        [When(@"the user sends the edit order items request")]
        public async Task WhenTheUserSendsTheCreateOrderItemsRequest()
        {
            await createOrderItemRequest.SendRequest(request);
        }

        [Then(@"the expected order item is created")]
        public async Task ThenTheExpectedOrderItemIsCreated()
        {
            await createOrderItemRequest.ThenTheExpectedOrderItemIsCreated();
        }

        [Then(@"the order item has the expected recipients")]
        public async Task ThenTheOrderItemHasTheExpectedRecipients()
        {
            await createOrderItemRequest.ThenTheExpectedOrderItemRecipientsAreCreated();
        }

        [Then(@"the response contains the following error details")]
        public async Task ThenTheResponseContainsTheFollowingErrorDetails(Table table)
        {
            IReadOnlyList<dynamic> expectedErrorDetails = table.CreateDynamicSet().ToList();

            var expectedErrors = new Dictionary<string, string[]>();
            for (var i = 0; i < expectedErrorDetails.Count; i++)
            {
                var expectedErrorDetail = expectedErrorDetails[i];
                if (string.IsNullOrWhiteSpace(expectedErrorDetail.Field))
                    continue;

                expectedErrors.Add($"{expectedErrorDetail.Field}", new string[] { expectedErrorDetail.Id });
            }

            var responseBody = await response.ReadBodyAsStringAsync();
            var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseBody);

            problemDetails?.Errors.Should().BeEquivalentTo(expectedErrors);
        }

        [Then(@"the catalogue solution order section is marked as complete")]
        public async Task ThenTheCatalogueSolutionOrderSectionIsMarkedAsComplete()
        {
            var orderProgress = await OrderProgressEntity.FetchOrderByOrderId(settings.ConnectionString, createOrderItemRequest.OrderId);

            orderProgress.CatalogueSolutionsViewed.Should().BeTrue();
        }

        [Then(@"the additional service order section is marked as complete")]
        public async Task ThenTheAdditionalServicesOrderSectionIsMarkedAsComplete()
        {
            var orderProgress = await OrderProgressEntity.FetchOrderByOrderId(settings.ConnectionString, createOrderItemRequest.OrderId);

            orderProgress.AdditionalServicesViewed.Should().BeTrue();
        }

        [Then(@"the associated service order section is marked as complete")]
        public async Task ThenTheAssociatedServicesOrderSectionIsMarkedAsComplete()
        {
            var orderProgress = await OrderProgressEntity.FetchOrderByOrderId(settings.ConnectionString, createOrderItemRequest.OrderId);

            orderProgress.AssociatedServicesViewed.Should().BeTrue();
        }

        [Then(@"the order item estimation period is set to '(.*)'")]
        public async Task ThenTheOrderItemEstimationPeriodIsSetTo(string estimationPeriodName)
        {
            await createOrderItemRequest.ThenTheOrderItemEstimationPeriodIsSetTo(estimationPeriodName);
        }

        private sealed class CreateOrderItemRequest
        {
            private readonly string catalogueItemId;
            private readonly RequestInfo requestInfo;
            private readonly Settings settings;
            private readonly Uri url;

            internal CreateOrderItemRequest(
                int orderId,
                string catalogueItemId,
                RequestInfo requestInfo,
                Settings settings)
            {
                OrderId = orderId;
                this.catalogueItemId = catalogueItemId;
                this.requestInfo = requestInfo;
                this.settings = settings;

                url = new Uri($"{settings.OrderingApiBaseUrl}/api/v1/orders/C{orderId}-01/order-items/{catalogueItemId}");
            }

            internal int OrderId { get; }

            internal void SetRecipients(IEnumerable<RecipientInfo> recipients) => requestInfo.AddRecipients(recipients);

            internal async Task SendRequest(Request request)
            {
                await request.PutJsonAsync(url.ToString(), requestInfo.ToModel());
            }

            internal async Task ThenTheExpectedOrderItemIsCreated()
            {
                var orderItem = await OrderItemEntity.FetchByKey(
                    settings.ConnectionString,
                    OrderId,
                    catalogueItemId);

                var expectedOrderItemEntity = requestInfo.GetExpectedOrderItemEntity(OrderId, catalogueItemId);

                EquivalencyAssertionOptions<OrderItemEntity> AssertionOptions(EquivalencyAssertionOptions<OrderItemEntity> o)
                {
                    var options = o.Excluding(i => i.Created).Excluding(i => i.LastUpdated);

                    // Estimation period is tested specifically in its own scenario
                    return requestInfo.CatalogueItemType == CatalogueItemType.AssociatedService
                        ? options.Excluding(i => i.EstimationPeriod)
                        : options;
                }

                orderItem.Should().BeEquivalentTo(expectedOrderItemEntity, AssertionOptions);
            }

            internal async Task ThenTheExpectedOrderItemRecipientsAreCreated()
            {
                var orderItemRecipients = await OrderItemRecipientEntity.FetchByOrderId(
                    settings.ConnectionString,
                    OrderId);

                orderItemRecipients.Should().BeEquivalentTo(
                    requestInfo.GetExpectedOrderItemRecipients(OrderId, catalogueItemId));
            }

            internal async Task ThenTheOrderItemEstimationPeriodIsSetTo(string estimationPeriodName)
            {
                TimeUnit? expectedEstimationPeriod = null;
                if (!string.IsNullOrWhiteSpace(estimationPeriodName))
                    expectedEstimationPeriod = Enum.Parse<TimeUnit>(estimationPeriodName, true);

                var orderItem = await OrderItemEntity.FetchByKey(settings.ConnectionString, OrderId, catalogueItemId);
                orderItem.EstimationPeriod.Should().Be(expectedEstimationPeriod);
            }
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class RequestInfo
        {
            private readonly List<RecipientInfo> serviceRecipients = new();

            public string CatalogueItemName { get; init; }

            public CatalogueItemType? CatalogueItemType { get; init; }

            public string CatalogueSolutionId { get; init; }

            public string CurrencyCode { get; init; } = "GBP";

            public string EstimationPeriod { get; init; } = "month";

            public string ItemUnitName { get; init; } = "patient";

            public string ItemUnitDescription { get; init; } = "per patient";

            public int? PriceId { get; init; } = 1;

            public decimal? Price { get; init; } = 10.50m;

            public ProvisioningType? ProvisioningType { get; init; } = Testing.Data.Data.ProvisioningType.Patient;

            public IList<RecipientInfo> ServiceRecipients => serviceRecipients;

            public string TimeUnitName { get; init; } = "month";

            public string TimeUnitDescription { get; init; } = "per month";

            public string Type { get; init; } = "flat";

            internal void AddRecipients(IEnumerable<RecipientInfo> recipients) => serviceRecipients.AddRange(recipients);

            internal object ToModel()
            {
                return new
                {
                    CatalogueItemName,
                    CatalogueItemType = CatalogueItemType.ToString(),
                    CatalogueSolutionId,
                    CurrencyCode,
                    EstimationPeriod,
                    ItemUnit = ItemUnitName is null & ItemUnitDescription is null ? null : new
                    {
                        Name = ItemUnitName,
                        Description = ItemUnitDescription,
                    },
                    PriceId,
                    Price,
                    ProvisioningType = ProvisioningType.ToString(),
                    ServiceRecipients,
                    TimeUnit = TimeUnitName is null & TimeUnitDescription is null ? null : new
                    {
                        Name = TimeUnitName,
                        Description = TimeUnitDescription,
                    },
                    Type,
                };
            }

            internal OrderItemEntity GetExpectedOrderItemEntity(int orderId, string catalogueItemId)
            {
                return new()
                {
                    CatalogueItemId = catalogueItemId,
                    CataloguePriceType = Enum.Parse<CataloguePriceType>(Type, true),
                    CurrencyCode = CurrencyCode,
                    EstimationPeriod = Enum.Parse<TimeUnit>(EstimationPeriod, true),
                    OrderId = orderId,
                    PriceId = PriceId,
                    Price = Price,
                    PricingUnitName = ItemUnitName,
                    ProvisioningType = ProvisioningType.GetValueOrDefault(),
                    TimeUnit = Enum.Parse<TimeUnit>(TimeUnitName, true),
                };
            }

            internal IEnumerable<OrderItemRecipientEntity> GetExpectedOrderItemRecipients(int orderId, string catalogueItemId)
            {
                return ServiceRecipients.Select(r => new OrderItemRecipientEntity
                {
                    CatalogueItemId = catalogueItemId,
                    DeliveryDate = r.DeliveryDate.GetValueOrDefault(),
                    OdsCode = r.OdsCode,
                    OrderId = orderId,
                    Quantity = r.Quantity.GetValueOrDefault(),
                });
            }
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class RecipientInfo
        {
            public DateTime? DeliveryDate { get; init; } = DateTime.UtcNow.Date;

            public string Name { get; init; }

            public string OdsCode { get; init; }

            public int? Quantity { get; init; } = 1;
        }
    }
}
