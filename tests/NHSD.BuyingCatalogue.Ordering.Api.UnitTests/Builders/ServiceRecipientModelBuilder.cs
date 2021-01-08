using NHSD.BuyingCatalogue.Ordering.Api.Models;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class ServiceRecipientModelBuilder
    {
        private string _name;
        private string _odsCode;

        private ServiceRecipientModelBuilder()
        {
            _name = "Some Name";
            _odsCode = "Ods1";
        }

        internal static ServiceRecipientModelBuilder Create() => new();

        internal ServiceRecipientModelBuilder WithOdsCode(string odsCode)
        {
            _odsCode = odsCode;
            return this;
        }

        internal ServiceRecipientModelBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        internal ServiceRecipientModel Build()
        {
            return new() { Name = _name, OdsCode = _odsCode };
        }
    }
}
