using System.Collections.Generic;
using NHSD.BuyingCatalogue.Ordering.Api.Models;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class ServiceRecipientsModelBuilder
    {
        private readonly List<ServiceRecipientModel> serviceRecipientModelList = new();

        private ServiceRecipientsModelBuilder()
        {
        }

        internal static ServiceRecipientsModelBuilder Create() => new();

        internal ServiceRecipientsModelBuilder WithServiceRecipientModel(ServiceRecipientModel serviceRecipientModel)
        {
            serviceRecipientModelList.Add(serviceRecipientModel);
            return this;
        }

        internal ServiceRecipientsModel Build() => new() { ServiceRecipients = serviceRecipientModelList };
    }
}
