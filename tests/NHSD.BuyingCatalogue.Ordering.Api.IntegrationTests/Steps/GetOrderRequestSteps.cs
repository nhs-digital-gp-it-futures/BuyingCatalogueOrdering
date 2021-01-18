using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests;
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

        [Given(@"the user creates a request to retrieve the details of an order by ID '(.*)'")]
        public void GivenTheUserCreatesARequestToRetrieveTheDetailsOfAnOrderById(string orderId)
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
            var expected = table.CreateSet<ServiceInstanceItem>();
            getOrderResponse.AssertServiceInstanceId(expected);
        }

        [Then(@"the get order response displays the expected order")]
        public void ThenTheGetOrderResponseDisplaysTheExpectedOrder()
        {
            var order = orderContext.OrderReferenceList.GetByOrderId(getOrderRequest.OrderId);
            var orderingPartyAddress = orderContext.AddressReferenceList.GetByAddressId(order.OrganisationAddressId);
            var orderingPartyContact = orderContext.ContactReferenceList.GetByContactId(order.OrganisationContactId);
            var supplierAddress = orderContext.AddressReferenceList.GetByAddressId(order.SupplierAddressId);
            var supplierContact = orderContext.ContactReferenceList.GetByContactId(order.SupplierContactId);
            var orderItems = orderContext.OrderItemReferenceList.FindByOrderId(getOrderRequest.OrderId);
            var serviceRecipients = orderContext.ServiceRecipientReferenceList.FindByOrderId(getOrderRequest.OrderId);

            getOrderResponse.AssertOrder(
                order,
                orderingPartyAddress,
                orderingPartyContact,
                supplierAddress,
                supplierContact,
                orderItems,
                serviceRecipients);
        }

        [Then(@"the get order response contains a yearly value of (.*)")]
        public void ThenTheGetOrderResponseContainsYearlyValue(decimal amount)
        {
            getOrderResponse.AssertOrderItemCost(amount);
        }

        [Then(@"the get order response contains a (.*) of (.*) for the order")]
        public void ThenTheGetOrderResponseContainsARecurringCostForTheOrder(string item, decimal amount)
        {
            getOrderResponse.AssertRecurringCost(item, amount);
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class ServiceInstanceItem
        {
            public string ItemId { get; init; }

            public string ServiceInstanceId { get; init; }
        }
    }
}
