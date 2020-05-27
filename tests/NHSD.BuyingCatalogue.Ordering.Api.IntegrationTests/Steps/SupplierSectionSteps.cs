using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NUnit.Framework;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class SupplierSectionSteps
    {
        private readonly Request _request;
        private readonly Response _response;
        private readonly Settings _settings;

        private readonly string _orderSupplierSectionUrl;

        public SupplierSectionSteps(
            Request request, 
            Response response, 
            Settings settings)
        {
            _request = request;
            _response = response;
            _settings = settings;

            _orderSupplierSectionUrl = settings.OrderingApiBaseUrl + "/api/v1/orders/{0}/sections/supplier";
        }

        [When(@"the user makes a request to retrieve the order supplier section with the ID (.*)")]
        public async Task WhenTheUserMakesARequestToRetrieveTheOrderSupplierSectionWithId(string orderId)
        {
            await _request.GetAsync(string.Format(_orderSupplierSectionUrl, orderId));
        }

        [Then(@"the response contains the following supplier details")]
        public async Task ThenTheResponseContainsTheFollowingSupplierDetails(Table table)
        {
            var response = await _response.ReadBodyAsJsonAsync();

            var actual = new SupplierSectionTable
            {
                SupplierId = response.Value<string>("supplierId"),
                SupplierName = response.Value<string>("name")
            };

            var expected = table.CreateInstance<SupplierSectionTable>();
            actual.Should().BeEquivalentTo(expected);
        }

        [Then(@"the response contains the following primary supplier contact details")]
        public async Task ThenTheResponseContainsTheFollowingSupplierContactDetails(Table table)
        {
            var response = await _response.ReadBodyAsJsonAsync();

            var primaryContactResponse = response.SelectToken("primaryContact");
            Assert.IsNotNull(primaryContactResponse);

            var actual = new SupplierContactTable
            {
                FirstName = primaryContactResponse.Value<string>("firstName"),
                LastName = primaryContactResponse.Value<string>("lastName"),
                Email = primaryContactResponse.Value<string>("emailAddress"),
                PhoneNumber = primaryContactResponse.Value<string>("telephoneNumber")
            };

            var expected = table.CreateInstance<SupplierContactTable>();
            actual.Should().BeEquivalentTo(expected);
        }

        private sealed class SupplierSectionTable
        {
            public string SupplierId { get; set; }

            public string SupplierName { get; set; }
        }

        private sealed class SupplierContactTable
        {
            public string FirstName { get; set; }

            public string LastName { get; set; }

            public string Email { get; set; }

            public string PhoneNumber { get; set; }
        }
    }
}
