﻿using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class DatabaseSteps
    {
        private readonly Settings _settings;

        public DatabaseSteps(Settings settings)
        {
            _settings = settings;
        }

        [Given(@"the call to the database will fail")]
        public async Task GivenTheCallToTheDatabaseWillFail()
        {
            await IntegrationDatabase.RemoveReadRoleAsync(_settings.AdminConnectionString);
            await IntegrationDatabase.RemoveWriteRoleAsync(_settings.AdminConnectionString);
        }
    }
}
