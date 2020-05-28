using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.EntityBuilder;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class ContactSteps
    {
        private readonly ScenarioContext _context;
        private readonly Response _response;
        private readonly Settings _settings;

        public ContactSteps(ScenarioContext context, Response response, Settings settings)
        {
            _context = context;
            _response = response;
            _settings = settings;
        }

        [Given(@"Contacts exist")]
        public async Task GivenContactsExist(Table table)
        {
            IDictionary<string, int> contactDictionary = new Dictionary<string, int>();

            foreach (var contactItem in table.CreateSet<ContactTable>())
            {
                var contact = ContactEntityBuilder
                    .Create()
                    .WithFirstName(contactItem.FirstName)
                    .WithLastName(contactItem.LastName)
                    .WithEmail(contactItem.EmailAddress)
                    .WithPhone(contactItem.TelephoneNumber)
                    .Build();

                var contactId = await contact.InsertAsync<int>(_settings.ConnectionString);
                contactDictionary.Add(contact.Email, contactId);
            }

            _context[ScenarioContextKeys.ContactMapDictionary] = contactDictionary;
        }

        [Then(@"the Contact section (.*) is returned")]
        public async Task ThenTheContactPrimaryContactIsReturned(string section, Table table)
        {
            var expected = table.CreateSet<ContactTable>().FirstOrDefault();

            var response = (await _response.ReadBodyAsJsonAsync()).SelectToken(section);

            var actual = new
            {
                FirstName = response.Value<string>("firstName"),
                LastName = response.Value<string>("lastName"),
                EmailAddress = response.Value<string>("emailAddress"),
                TelephoneNumber = response.Value<string>("telephoneNumber")
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Given(@"the user wants to update the (.*) section for the contact")]
        public void WhenTheUserWantsToUpdateTheSupplierContactSectionForTheContact(string section, Table table)
        {
            var contact = table.CreateSet<ContactTable>().FirstOrDefault();
            _context[section] = contact;
        }

        private sealed class ContactTable
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string EmailAddress { get; set; }
            public string TelephoneNumber { get; set; }
        }
    }
}
