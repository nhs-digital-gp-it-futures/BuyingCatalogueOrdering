using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.EntityBuilder
{
    public sealed class ServiceRecipientBuilder
    {
        private string odsCode;
        private string name;
        private string orderId;

        private ServiceRecipientBuilder()
        {
        }

        public static ServiceRecipientBuilder Create() => new();

        public ServiceRecipientBuilder WithOdsCode(string code)
        {
            odsCode = code;
            return this;
        }

        public ServiceRecipientBuilder WithName(string recipientName)
        {
            name = recipientName;
            return this;
        }

        public ServiceRecipientBuilder WithOrderId(string id)
        {
            orderId = id;
            return this;
        }

        public ServiceRecipientEntity Build()
        {
            return new()
            {
                OdsCode = odsCode,
                Name = name,
                OrderId = orderId,
            };
        }
    }
}
