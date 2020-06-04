using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
        private readonly Settings _settings;

        public ServiceRecipientSteps(Response response, Settings settings)
        {
            _response = response;
            _settings = settings;
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

        private sealed class ServiceRecipientTable
        {
            public string OdsCode { get; set; }
            public string Name { get; set; }
            public string OrderId { get; set; }
        }
    }
}
