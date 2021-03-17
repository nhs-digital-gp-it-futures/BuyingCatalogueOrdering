using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NUnit.Framework;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class OrderSummarySteps
    {
        private readonly Response response;
        private readonly Request request;

        private readonly string orderSummaryUrl;

        public OrderSummarySteps(Response response, Request request, Settings settings)
        {
            this.response = response;
            this.request = request;
            orderSummaryUrl = settings.OrderingApiBaseUrl + "/api/v1/orders/C{0}-01/summary";
        }

        [When(@"the user makes a request to retrieve the order summary with the ID (.*)")]
        public async Task WhenTheUserMakesARequestToRetrieveTheOrderSummaryWithTheId(int orderId)
        {
            await request.GetAsync(string.Format(CultureInfo.InvariantCulture, orderSummaryUrl, orderId));
        }

        [Then(@"the order summary is returned with the following values")]
        public async Task ThenTheOrderSummaryIsReturnedWithTheFollowingValues(Table table)
        {
            var expected = table.CreateSet<OrderSummaryTable>().FirstOrDefault();

            var jsonResponse = await response.ReadBodyAsJsonAsync();

            var actual = new OrderSummaryTable
            {
                OrderId = jsonResponse.Value<string>("orderId"),
                OrganisationId = jsonResponse.SelectToken("organisationId")?.ToObject<Guid>(),
                Description = jsonResponse.Value<string>("description"),
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Then(@"the order Summary Sections have the following values")]
        public async Task ThenTheOrderSummarySectionsHaveTheFollowingValues(Table table)
        {
            var expected = table.CreateSet<SectionTable>();

            var jsonResponse = await response.ReadBodyAsJsonAsync();

            var sections = jsonResponse.SelectToken("sections");
            Assert.IsNotNull(sections);

            var actual = new SectionsTable
            {
                Sections = sections.ToObject<IEnumerable<SectionTable>>(),
            };

            actual.Sections.Should().BeEquivalentTo(expected);
        }

        [Then(@"the order Section Status is (.*)")]
        public async Task ThenTheOrderSectionStatusIs(string expectedStatus)
        {
            var jsonResponse = await response.ReadBodyAsJsonAsync();

            var actualSectionStatus = jsonResponse.SelectToken("sectionStatus")?.ToString();

            Assert.IsNotNull(actualSectionStatus);
            actualSectionStatus.Should().BeEquivalentTo(expectedStatus);
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class OrderSummaryTable
        {
            public string OrderId { get; init; }

            public Guid? OrganisationId { get; init; }

            public string Description { get; init; }
        }

        private sealed class SectionsTable
        {
            public IEnumerable<SectionTable> Sections { get; init; }
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class SectionTable
        {
            public string Id { get; init; }

            public string Status { get; init; }

            public int? Count { get; init; }
        }
    }
}
