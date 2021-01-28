using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Data;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.EntityBuilder;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class OrderSteps
    {
        private readonly Request request;
        private readonly Response response;
        private readonly Settings settings;
        private readonly OrderContext orderContext;

        private readonly string orderOrganisationsUrl;

        public OrderSteps(
            Request request,
            Response response,
            Settings settings,
            OrderContext orderContext)
        {
            this.request = request;
            this.response = response;
            this.settings = settings;
            this.orderContext = orderContext;

            orderOrganisationsUrl = this.settings.OrderingApiBaseUrl + "/api/v1/organisations/{0}/orders";
        }

        [Given(@"the order with orderId (.*) has a primary contact")]
        public async Task ThenTheOrderHasAPrimaryContact(string orderId)
        {
            var order = await OrderEntity.FetchOrderByOrderId(settings.ConnectionString, orderId);
            order.OrganisationContactId.Should().NotBeNull();
        }

        [Given(@"the order with orderId (.*) does not have a primary contact")]
        public async Task ThenTheOrderDoesNotHaveAPrimaryContact(string orderId)
        {
            var order = await OrderEntity.FetchOrderByOrderId(settings.ConnectionString, orderId);
            order.OrganisationContactId.Should().BeNull();
        }

        [Given(@"Orders exist")]
        public async Task GivenOrdersExist(Table table)
        {
            foreach (var ordersTableItem in table.CreateSet<OrdersTable>())
            {
                var addressReferenceList = orderContext.AddressReferenceList;
                var contactReferenceList = orderContext.ContactReferenceList;

                int? organisationAddressId = addressReferenceList.GetByPostcode(ordersTableItem.OrganisationAddressPostcode)?.AddressId;
                int? organisationContactId = contactReferenceList.GetByEmail(ordersTableItem.OrganisationContactEmail)?.ContactId;

                int? supplierAddressId = addressReferenceList.GetByPostcode(ordersTableItem.SupplierAddressPostcode)?.AddressId;
                int? supplierContactId = contactReferenceList.GetByEmail(ordersTableItem.SupplierContactEmail)?.ContactId;

                DateTime? commencementDate = null;
                if (ordersTableItem.CommencementDate != DateTime.MinValue)
                {
                    commencementDate = ordersTableItem.CommencementDate;
                }

                var orderId = ordersTableItem.OrderId;

                var order = OrderEntityBuilder
                    .Create()
                    .WithOrderId(orderId)
                    .WithDescription(ordersTableItem.Description)
                    .WithOrganisationId(ordersTableItem.OrganisationId)
                    .WithOrganisationName(ordersTableItem.OrganisationName)
                    .WithOrganisationOdsCode(ordersTableItem.OrganisationOdsCode)
                    .WithOrganisationAddressId(organisationAddressId)
                    .WithOrganisationBillingAddressId(ordersTableItem.OrganisationBillingAddressId)
                    .WithOrganisationContactId(organisationContactId)
                    .WithOrderStatus(ordersTableItem.OrderStatus)
                    .WithDateCreated(ordersTableItem.Created != DateTime.MinValue ? ordersTableItem.Created : DateTime.UtcNow)
                    .WithLastUpdatedBy(ordersTableItem.LastUpdatedBy)
                    .WithLastUpdatedName(ordersTableItem.LastUpdatedByName)
                    .WithLastUpdated(ordersTableItem.LastUpdated != DateTime.MinValue ? ordersTableItem.LastUpdated : DateTime.UtcNow)
                    .WithServiceRecipientsViewed(ordersTableItem.ServiceRecipientsViewed)
                    .WithCatalogueSolutionsViewed(ordersTableItem.CatalogueSolutionsViewed)
                    .WithAdditionalServicesViewed(ordersTableItem.AdditionalServicesViewed)
                    .WithAssociatedServicesViewed(ordersTableItem.AssociatedServicesViewed)
                    .WithFundingSourceOnlyGms(ordersTableItem.FundingSourceOnlyGms)
                    .WithSupplierId(ordersTableItem.SupplierId)
                    .WithSupplierName(ordersTableItem.SupplierName)
                    .WithSupplierAddressId(supplierAddressId)
                    .WithSupplierContactId(supplierContactId)
                    .WithCommencementDate(commencementDate)
                    .WithIsDeleted(ordersTableItem.IsDeleted)
                    .WithDateCompleted(ordersTableItem.Completed)
                    .Build();

                await order.InsertAsync(settings.ConnectionString);

                orderContext.OrderReferenceList.Add(orderId, order);
            }
        }

        [When(@"a GET request is made for a list of orders with organisationId (.*)")]
        public async Task WhenAGetRequestIsMadeForOrders(Guid organisationId)
        {
            await request.GetAsync(string.Format(CultureInfo.InvariantCulture, orderOrganisationsUrl, organisationId));
        }

        [Then(@"the orders list is returned with the following values")]
        public async Task ThenTheOrdersListIsReturnedWithTheFollowingValues(Table table)
        {
            var expectedOrders = table.CreateSet<GetOrdersTable>();

            var orders = (await response.ReadBodyAsJsonAsync()).Select(CreateOrders);

            orders.Should().BeEquivalentTo(expectedOrders);
        }

        [Then(@"an empty list is returned")]
        public async Task AnEmptyListIsReturned()
        {
            var orders = (await response.ReadBodyAsJsonAsync()).Select(CreateOrders);
            orders.Count().Should().Be(0);
        }

        [Then(@"the order with orderId (.*) is updated in the database with data")]
        public async Task ThenTheOrderIsUpdatedInTheDatabase(string orderId, Table table)
        {
            var actual = await OrderEntity.FetchOrderByOrderId(settings.ConnectionString, orderId);
            table.CompareToInstance(actual);
        }

        [Then(@"the order is created in the database with orderId (.*) and data")]
        public async Task ThenTheOrderIsCreatedInTheDatabase(string orderId, Table table)
        {
            var actual = await OrderEntity.FetchOrderByOrderId(settings.ConnectionString, orderId);
            table.CompareToInstance(actual);
        }

        [Then(@"the order with orderId (.*) is updated and has a primary contact with data")]
        public async Task ThenTheOrderWithOrderIdHasContactData(string orderId, Table table)
        {
            var expected = table.CreateInstance<ContactEntity>();

            var order = await OrderEntity.FetchOrderByOrderId(settings.ConnectionString, orderId);
            var actual = await ContactEntity.FetchContactById(settings.ConnectionString, order.OrganisationContactId);

            actual.Should().BeEquivalentTo(expected, options => options.Excluding(c => c.ContactId));
        }

        [Then(@"the order with orderId (.*) is updated and has a Organisation Address with data")]
        public async Task ThenTheOrderWithOrderIdHasOrganisationAddressData(string orderId, Table table)
        {
            var order = await OrderEntity.FetchOrderByOrderId(settings.ConnectionString, orderId);
            var actual = await AddressEntity.FetchAddressById(settings.ConnectionString, order.OrganisationAddressId);
            table.CompareToInstance(actual);
        }

        [Then(@"the order with orderId (.*) has LastUpdated time present and it is the current time")]
        public async Task ThenOrderOrderIdHasLastUpdatedAtCurrentTime(string orderId)
        {
            var actual = await OrderEntity.FetchOrderByOrderId(settings.ConnectionString, orderId);
            actual.LastUpdated.Should().BeWithin(TimeSpan.FromSeconds(3)).Before(DateTime.UtcNow);
        }

        [Then(@"the order with orderId (.*) has Created time present and it is the current time")]
        public async Task ThenOrderOrderIdHasCreatedAtCurrentTime(string orderId)
        {
            var actual = await OrderEntity.FetchOrderByOrderId(settings.ConnectionString, orderId);
            actual.Created.Should().BeWithin(TimeSpan.FromSeconds(3)).Before(DateTime.UtcNow);
        }

        private static object CreateOrders(JToken token)
        {
            return new
            {
                OrderId = token.Value<string>("orderId"),
                Description = token.Value<string>("description"),
                Status = token.Value<string>("status"),
                LastUpdated = token.Value<DateTime>("lastUpdated"),
                LastUpdatedByName = token.Value<string>("lastUpdatedBy"),
                Created = token.Value<DateTime>("dateCreated"),
                Completed = token.Value<DateTime>("dateCompleted"),
                FundingSourceOnlyGms = token.Value<bool?>("onlyGMS"),
            };
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class GetOrdersTable
        {
            public string OrderId { get; init; }

            public string Description { get; init; }

            public string Status { get; init; }

            public DateTime Created { get; init; }

            public DateTime LastUpdated { get; init; }

            public string LastUpdatedByName { get; init; }

            public bool? FundingSourceOnlyGms { get; init; }
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class OrdersTable
        {
            public string OrderId { get; init; }

            public string Description { get; init; }

            public Guid OrganisationId { get; init; }

            public string OrganisationName { get; init; }

            public string OrganisationOdsCode { get; init; } = "test";

            public string OrganisationAddressPostcode { get; init; }

            public string OrganisationContactEmail { get; init; }

            public int? OrganisationBillingAddressId { get; init; }

            public OrderStatus OrderStatus { get; init; } = OrderStatus.Complete;

            public DateTime Created { get; init; }

            public Guid LastUpdatedBy { get; init; }

            public string LastUpdatedByName { get; init; }

            public DateTime LastUpdated { get; init; }

            public string SupplierId { get; init; }

            public string SupplierName { get; init; }

            public string SupplierAddressPostcode { get; init; }

            public string SupplierContactEmail { get; init; }

            public DateTime? CommencementDate { get; init; }

            public bool ServiceRecipientsViewed { get; init; }

            public bool CatalogueSolutionsViewed { get; init; }

            public bool AdditionalServicesViewed { get; init; }

            public bool AssociatedServicesViewed { get; init; }

            public bool? FundingSourceOnlyGms { get; init; }

            public bool IsDeleted { get; init; }

            public DateTime? Completed { get; init; }
        }
    }
}
