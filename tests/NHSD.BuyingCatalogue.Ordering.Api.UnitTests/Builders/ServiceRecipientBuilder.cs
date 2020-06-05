using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class ServiceRecipientBuilder
    {
        private readonly ServiceRecipient _serviceRecipient;

        private ServiceRecipientBuilder()
        {
            _serviceRecipient = new ServiceRecipient
            {
                Name = "Some name",
                Order = OrderBuilder.Create().Build()
            };
        }

        internal static ServiceRecipientBuilder Create() => new ServiceRecipientBuilder();

        internal ServiceRecipientBuilder WithOdsCode(string odsCode)
        {
            _serviceRecipient.OdsCode = odsCode;
            return this;
        }

        internal ServiceRecipientBuilder WithOrderId(string orderId)
        {
            _serviceRecipient.Order.OrderId = orderId;
            return this;
        }

        internal ServiceRecipient Build() => _serviceRecipient;
    }
}
