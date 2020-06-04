using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.EntityBuilder;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class ServiceRecipientSteps
    {
        private readonly Response _response;
        private readonly Request _request;
        private readonly Settings _settings;

        private readonly string _serviceRecipientUrl;

        public ServiceRecipientSteps(Response response, Request request, Settings settings)
        {
            _response = response;
            _request = request;
            _settings = settings;

            _serviceRecipientUrl = settings.OrderingApiBaseUrl + "/api/v1/orders/{0}/sections/service-recipients";
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

                await serviceRecipient.InsertAsync(_settings.ConnectionString);
            }
        }

        [When(@"the user makes a request to retrieve the service-recipients section with order ID (.*)")]
        public async Task WhenTheUserMakesARequestToRetrieveTheService_RecipientsSectionWithOrderID(string orderId)
        {
            await _request.GetAsync(string.Format(_serviceRecipientUrl, orderId));
        }

        [Then(@"the service recipients are returned")]
        public async Task ThenTheServiceRecipientsAreReturned(Table table)
        {
            var expected = table.CreateSet<ServiceRecipientTable>();

            var serviceRecipients = (await _response.ReadBodyAsJsonAsync()).SelectToken("serviceRecipients").Select(CreateServiceRecipients);

            expected.Should().BeEquivalentTo(serviceRecipients, conf => conf.Excluding(x => x.OrderId));
        }

        private static ServiceRecipientTable CreateServiceRecipients(JToken token)
        {
            return new ServiceRecipientTable
            {
                Name = token.SelectToken("name").ToString(),
                OdsCode = token.SelectToken("odsCode").ToString()
            };
        }

        private sealed class ServiceRecipientTable
        {
            public string OdsCode { get; set; }
            public string Name { get; set; }
            public string OrderId { get; set; }
        }
    }
}
