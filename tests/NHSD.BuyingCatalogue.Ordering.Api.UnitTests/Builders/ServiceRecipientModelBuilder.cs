using NHSD.BuyingCatalogue.Ordering.Api.Models;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class ServiceRecipientModelBuilder
    {
        private readonly ServiceRecipientModel _serviceRecipientModel;

        private ServiceRecipientModelBuilder()
        {
            _serviceRecipientModel = new ServiceRecipientModel
            {
                Name = "Some name",
                OdsCode =  "Ods1"
            };
        }

        internal static ServiceRecipientModelBuilder Create() => new ServiceRecipientModelBuilder();

        internal ServiceRecipientModelBuilder WithOdsCode(string odsCode)
        {
            _serviceRecipientModel.OdsCode = odsCode;
            return this;
        }

        internal ServiceRecipientModelBuilder WithName(string name)
        {
            _serviceRecipientModel.Name = name;
            return this;
        }

        internal ServiceRecipientModel Build() => _serviceRecipientModel;
    }
}
