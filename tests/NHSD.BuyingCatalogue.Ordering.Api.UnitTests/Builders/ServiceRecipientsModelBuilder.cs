using System.Collections.Generic;
using NHSD.BuyingCatalogue.Ordering.Api.Models;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class ServiceRecipientsModelBuilder
    {
        private readonly List<ServiceRecipientModel> ServiceRecipientModelList = new List<ServiceRecipientModel>();

        private ServiceRecipientsModelBuilder()
        {
        }

        internal static ServiceRecipientsModelBuilder Create() => new ServiceRecipientsModelBuilder();

        internal ServiceRecipientsModelBuilder WithServiceRecipientModel(ServiceRecipientModel serviceRecipientModel)
        {
            ServiceRecipientModelList.Add(serviceRecipientModel);
            return this;
        }

        internal ServiceRecipientsModel Build()
        {
            return new ServiceRecipientsModel()
            {
                ServiceRecipients = ServiceRecipientModelList
            };
        }
    }
}
