using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;
using NUnit.Framework;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class AddressSteps
    {
        private readonly Response response;
        private readonly Settings settings;
        private readonly OrderContext orderContext;

        public AddressSteps(Response response, Settings settings, OrderContext orderContext)
        {
            this.response = response;
            this.settings = settings;
            this.orderContext = orderContext;
        }

        [Given(@"addresses exist")]
        public async Task GivenAddressesExist(Table table)
        {
            foreach (var entity in table.CreateSet<AddressEntity>())
            {
                await entity.InsertAsync(settings.OrderingDbAdminConnectionString);

                orderContext.AddressReferenceList.Add(entity.Id, entity);
            }
        }

        [Then(@"the address is returned")]
        public async Task ThenTheAddressSectionIsReturned(Table table)
        {
            var expected = table.CreateSet<AddressTable>().FirstOrDefault();
            var address = (await response.ReadBodyAsJsonAsync()).SelectToken("address");

            Assert.NotNull(address);

            var actual = new AddressEntity
            {
                Line1 = address.Value<string>("line1"),
                Line2 = address.Value<string>("line2"),
                Line3 = address.Value<string>("line3"),
                Line4 = address.Value<string>("line4"),
                Line5 = address.Value<string>("line5"),
                Town = address.Value<string>("town"),
                County = address.Value<string>("county"),
                Postcode = address.Value<string>("postcode"),
                Country = address.Value<string>("country"),
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class AddressTable
        {
            public string Line1 { get; init; }

            public string Line2 { get; init; }

            public string Line3 { get; init; }

            public string Line4 { get; init; }

            public string Line5 { get; init; }

            public string Town { get; init; }

            public string County { get; init; }

            public string Postcode { get; init; }

            public string Country { get; init; }
        }
    }
}
