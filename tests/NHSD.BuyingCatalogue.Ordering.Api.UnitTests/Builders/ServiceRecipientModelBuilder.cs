using NHSD.BuyingCatalogue.Ordering.Api.Models;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class ServiceRecipientModelBuilder
    {
        private string name;
        private string odsCode;

        private ServiceRecipientModelBuilder()
        {
            name = "Some Name";
            odsCode = "Ods1";
        }

        internal static ServiceRecipientModelBuilder Create() => new();

        internal ServiceRecipientModelBuilder WithOdsCode(string code)
        {
            odsCode = code;
            return this;
        }

        internal ServiceRecipientModelBuilder WithName(string recipientName)
        {
            name = recipientName;
            return this;
        }

        internal ServiceRecipientModel Build()
        {
            return new() { Name = name, OdsCode = odsCode };
        }
    }
}
