using NHSD.BuyingCatalogue.Ordering.Common.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class ServiceRecipientBuilder
    {
        private string odsCode;
        private string name;
        private string orderId;

        private ServiceRecipientBuilder()
        {
            name = "Some name";
        }

        internal static ServiceRecipientBuilder Create() => new();

        internal ServiceRecipientBuilder WithOdsCode(string code)
        {
            odsCode = code;
            return this;
        }

        internal ServiceRecipientBuilder WithOrderId(string id)
        {
            orderId = id;
            return this;
        }

        internal ServiceRecipientBuilder WithName(string recipientName)
        {
            name = recipientName;
            return this;
        }

        internal ServiceRecipient Build() =>
            new()
            {
                OdsCode = odsCode,
                Name = name,
                Order = OrderBuilder
                    .Create()
                    .WithOrderId(orderId)
                    .Build(),
            };
    }
}
