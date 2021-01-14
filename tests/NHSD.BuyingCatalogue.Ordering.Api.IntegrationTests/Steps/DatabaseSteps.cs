using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class DatabaseSteps
    {
        private readonly Settings settings;

        public DatabaseSteps(Settings settings)
        {
            this.settings = settings;
        }

        [Given(@"the call to the database will fail")]
        public async Task GivenTheCallToTheDatabaseWillFail()
        {
            await IntegrationDatabase.RemoveReadRoleAsync(settings.OrderingDbAdminConnectionString);
            await IntegrationDatabase.RemoveWriteRoleAsync(settings.OrderingDbAdminConnectionString);
        }

        [Given(@"The Database Server is down")]
        public async Task GivenTheDatabaseServerIsDown()
        {
            await IntegrationDatabase.DenyAccessForNhsdUser(settings.OrderingDbAdminConnectionString);
        }
    }
}
