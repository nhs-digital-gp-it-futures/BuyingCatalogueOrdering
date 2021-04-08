using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class GetOrderRequestSteps
    {
        private readonly Request request;
        private readonly Settings settings;
        private readonly OrderContext orderContext;
        private GetOrderRequest getOrderRequest;
        private GetOrderResponse getOrderResponse;

        public GetOrderRequestSteps(
            Request request,
            Settings settings,
            OrderContext orderContext)
        {
            this.request = request ?? throw new ArgumentNullException(nameof(request));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.orderContext = orderContext ?? throw new ArgumentNullException(nameof(orderContext));
        }

        [Given(@"the user creates a request to retrieve the details of an order by ID (\d{1,6})")]
        public void GivenTheUserCreatesARequestToRetrieveTheDetailsOfAnOrderById(int orderId)
        {
            getOrderRequest = new GetOrderRequest(
                request,
                settings.OrderingApiBaseUrl,
                orderId);
        }

        [When(@"the user sends the get order request")]
        public async Task WhenTheUserSendsTheGetOrderRequest()
        {
            getOrderResponse = await getOrderRequest.ExecuteAsync();
        }

        [Then(@"the expected service instance ID is included for each order item as follows")]
        public void ThenTheExpectedServiceInstanceIdIsIncludedForEachOrderItemAsFollows(Table table)
        {
            var expected = table.CreateSet<GetOrderServiceInstanceItem>();
            getOrderResponse.AssertServiceInstanceId(expected);
        }

        [Then(@"the get order response displays the expected order")]
        public void ThenTheGetOrderResponseDisplaysTheExpectedOrder()
        {
            var order = orderContext.OrderReferenceList[getOrderRequest.OrderId];

            var orderingParty = orderContext.OrderingPartyReferenceList[order.OrderingPartyId];
            var orderingPartyAddress = orderContext.AddressReferenceList[orderingParty.AddressId.GetValueOrDefault()];
            var orderingPartyContact = orderContext.ContactReferenceList[order.OrderingPartyContactId.GetValueOrDefault()];

            var supplier = orderContext.SupplierReferenceList[order.SupplierId];
            var supplierAddress = orderContext.AddressReferenceList[supplier.AddressId];
            var supplierContact = orderContext.ContactReferenceList[order.SupplierContactId.GetValueOrDefault()];

            var orderItems = orderContext.OrderItemReferenceList[getOrderRequest.OrderId].Values;

            getOrderResponse.AssertOrder(
                order,
                orderingParty,
                supplier,
                orderingPartyAddress,
                orderingPartyContact,
                supplierAddress,
                supplierContact,
                orderItems,
                orderContext.OrderItemRecipientsReferenceList,
                orderContext.PricingUnitReferenceList,
                orderContext.CatalogueItemReferenceList);
        }

        [Then(@"the get order response contains a yearly value of (.*)")]
        public void ThenTheGetOrderResponseContainsYearlyValue(decimal amount)
        {
            getOrderResponse.AssertOrderItemCost(amount);
        }

        [Then(@"the get order response contains recipient with (.*) a yearly value of (.*)")]
        public void ThenTheGetOrderResponseContainsRecipientWithAYearlyValueOf(string odsCode, decimal amount)
        {
            getOrderResponse.AssertOrderItemRecipientCost(odsCode, amount);
        }

        [Then(@"the get order response contains a (.*) of (.*) for the order")]
        public void ThenTheGetOrderResponseContainsARecurringCostForTheOrder(string item, decimal amount)
        {
            getOrderResponse.AssertRecurringCost(item, amount);
        }

        [Then(@"each order item in the order has the expected service instance ID as follows")]
        public void ThenEachOrderItemHasTheExpectedServiceInstanceIdAsFollows(Table table)
        {
            var expected = table.CreateSet<ServiceInstanceItem>();
            getOrderResponse.AssertServiceInstanceIdAsync(expected);
        }

        private sealed class GetOrderRequest
        {
            private readonly Request request;
            private readonly string getOrderUrl;

            public GetOrderRequest(
                Request request,
                string orderingApiBaseAddress,
                int orderId)
            {
                this.request = request ?? throw new ArgumentNullException(nameof(request));
                OrderId = orderId;

                getOrderUrl = $"{orderingApiBaseAddress}/api/v1/orders/C{orderId}-01";
            }

            public int OrderId { get; }

            public async Task<GetOrderResponse> ExecuteAsync()
            {
                var response = await request.GetAsync(getOrderUrl);
                return await GetOrderResponse.CreateAsync(response);
            }
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class GetOrderServiceInstanceItem
        {
            public string ItemId { get; init; }

            public string ServiceInstanceId { get; init; }
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class ServiceInstanceItem
        {
            public string CatalogueItemId { get; init; }

            public string OdsCode { get; init; }

            public string ServiceInstanceId { get; init; }
        }
    }
}
