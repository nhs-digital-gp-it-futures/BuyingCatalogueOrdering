using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.EntityBuilder;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class ServiceRecipientSteps
    {
        private readonly Response response;
        private readonly Request request;
        private readonly Settings settings;
        private readonly OrderContext orderContext;

        private readonly string serviceRecipientUrl;

        public ServiceRecipientSteps(Response response, Request request, Settings settings, OrderContext orderContext)
        {
            this.response = response;
            this.request = request;
            this.settings = settings;
            this.orderContext = orderContext;

            serviceRecipientUrl = settings.OrderingApiBaseUrl + "/api/v1/orders/{0}/sections/service-recipients";
        }

        [Given(@"Service Recipients exist")]
        public async Task GivenServiceRecipientsExist(Table table)
        {
            foreach (var serviceRecipientItem in table.CreateSet<ServiceRecipientTable>())
            {
                var serviceRecipient = ServiceRecipientBuilder
                    .Create()
                    .WithOdsCode(serviceRecipientItem.OdsCode)
                    .WithName(serviceRecipientItem.Name)
                    .WithOrderId(serviceRecipientItem.OrderId)
                    .Build();

                await serviceRecipient.InsertAsync(settings.ConnectionString);

                orderContext.ServiceRecipientReferenceList.Add(serviceRecipient);
            }
        }

        [When(@"the user makes a request to retrieve the service-recipients section with order ID (.*)")]
        public async Task WhenTheUserMakesARequestToRetrieveTheService_RecipientsSectionWithOrderID(string orderId)
        {
            await request.GetAsync(string.Format(CultureInfo.InvariantCulture, serviceRecipientUrl, orderId));
        }

        [When(@"the user makes a request to set the service-recipients section with order ID (.*)")]
        public async Task WhenTheUserMakesARequestToRetrieveTheService_RecipientsSectionWithOrderID(string orderId, Table table)
        {
            var payload = new ServiceRecipientsTable { ServiceRecipients = table.CreateSet<ServiceRecipientTable>() };
            await request.PutJsonAsync(string.Format(CultureInfo.InvariantCulture, serviceRecipientUrl, orderId), payload);
        }

        [Then(@"the service recipients are returned")]
        public async Task ThenTheServiceRecipientsAreReturned(Table table)
        {
            var expected = table.CreateSet<ServiceRecipientTable>();

            var jsonBody = await response.ReadBodyAsJsonAsync();
            var serviceRecipients = jsonBody?.SelectToken("serviceRecipients")?.Select(CreateServiceRecipients);

            expected.Should().BeEquivalentTo(serviceRecipients, conf =>
                conf.Excluding(x => x.OrderId).WithStrictOrdering());
        }

        [Then(@"the persisted service recipients are")]
        public async Task ThenThePersistedServiceRecipientsAreReturned(Table table)
        {
            var expected = table.CreateSet<ServiceRecipientTable>();
            var actual = await ServiceRecipientEntity.FetchAllServiceRecipients(settings.ConnectionString);
            expected.Should().BeEquivalentTo(actual);
        }

        private static ServiceRecipientTable CreateServiceRecipients(JToken token)
        {
            return new()
            {
                Name = token.SelectToken("name").ToString(),
                OdsCode = token.SelectToken("odsCode").ToString(),
            };
        }

        private sealed class ServiceRecipientsTable
        {
            public IEnumerable<ServiceRecipientTable> ServiceRecipients { get; set; }
        }

        private sealed class ServiceRecipientTable
        {
            public string OdsCode { get; set; }

            public string Name { get; set; }

            public string OrderId { get; set; }
        }
    }
}
