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
    internal sealed class ContactSteps
    {
        private readonly OrderContext orderContext;
        private readonly Response response;
        private readonly Settings settings;

        public ContactSteps(OrderContext orderContext, Response response, Settings settings)
        {
            this.orderContext = orderContext;
            this.response = response;
            this.settings = settings;
        }

        [Given(@"contacts exist")]
        public async Task GivenContactsExist(Table table)
        {
            foreach (var entity in table.CreateSet<ContactEntity>())
            {
                await entity.InsertAsync(settings.OrderingDbAdminConnectionString);
                orderContext.ContactReferenceList.Add(entity.Id, entity);
            }
        }

        [Then(@"the contact section (.*) is returned")]
        public async Task ThenTheContactPrimaryContactIsReturned(string section, Table table)
        {
            var expected = table.CreateSet<ContactTable>().FirstOrDefault();

            var contact = (await response.ReadBodyAsJsonAsync()).SelectToken(section);

            Assert.NotNull(contact);

            var actual = new
            {
                FirstName = contact.Value<string>("firstName"),
                LastName = contact.Value<string>("lastName"),
                EmailAddress = contact.Value<string>("emailAddress"),
                TelephoneNumber = contact.Value<string>("telephoneNumber"),
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class ContactTable
        {
            public string FirstName { get; init; }

            public string LastName { get; init; }

            public string EmailAddress { get; init; }

            public string TelephoneNumber { get; init; }
        }
    }
}
