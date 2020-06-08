using System;
using System.Collections.Generic;
using System.Text;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NUnit.Framework.Internal;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Controllers
{
    internal sealed class ServiceRecipientsModelBuilder
    {
        private readonly ServiceRecipientsModel _serviceRecipientsModel;
        private readonly List<ServiceRecipientModel> ServiceRecipientModelList = new List<ServiceRecipientModel>();
        private ServiceRecipientsModelBuilder()
        {
            _serviceRecipientsModel = new ServiceRecipientsModel()
            {
                ServiceRecipients = new List<ServiceRecipientModel>()
            };
        }

        internal static ServiceRecipientsModelBuilder Create() => new ServiceRecipientsModelBuilder();

        internal ServiceRecipientsModelBuilder WithServiceRecipientModel(ServiceRecipientModel serviceRecipientModel)
        {
            ServiceRecipientModelList.Add(serviceRecipientModel);
            return this;
        }

        internal ServiceRecipientsModel Build()
        {
            _serviceRecipientsModel.ServiceRecipients = ServiceRecipientModelList;
            return _serviceRecipientsModel;
        }

    }
}
