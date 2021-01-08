using System.Collections.Generic;
using NHSD.BuyingCatalogue.Ordering.Api.Models;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class ServiceRecipientsModelBuilder
    {
        private readonly List<ServiceRecipientModel> ServiceRecipientModelList = new();

        private ServiceRecipientsModelBuilder()
        {
        }

        internal static ServiceRecipientsModelBuilder Create() => new();

        internal ServiceRecipientsModelBuilder WithServiceRecipientModel(ServiceRecipientModel serviceRecipientModel)
        {
            ServiceRecipientModelList.Add(serviceRecipientModel);
            return this;
        }

        internal ServiceRecipientsModel Build()
        {
            return new()
            {
                ServiceRecipients = ServiceRecipientModelList,
            };
        }
    }
}
