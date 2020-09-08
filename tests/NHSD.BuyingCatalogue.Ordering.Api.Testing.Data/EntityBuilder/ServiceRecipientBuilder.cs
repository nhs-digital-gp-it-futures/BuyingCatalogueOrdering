using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.EntityBuilder
{
    public sealed class ServiceRecipientBuilder
    {
        private string _odsCode;
        private string _name;
        private string _orderId;

        private ServiceRecipientBuilder()
        {
        }

        public static ServiceRecipientBuilder Create()
        {
            return new ServiceRecipientBuilder();
        }

        public ServiceRecipientBuilder WithOdsCode(string odsCode)
        {
            _odsCode = odsCode;
            return this;
        }

        public ServiceRecipientBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public ServiceRecipientBuilder WithOrderId(string orderId)
        {
            _orderId = orderId;
            return this;
        }

        public ServiceRecipientEntity Build()
        {
            return new ServiceRecipientEntity
            {
                OdsCode = _odsCode,
                Name = _name,
                OrderId = _orderId
            };
        }
    }
}
