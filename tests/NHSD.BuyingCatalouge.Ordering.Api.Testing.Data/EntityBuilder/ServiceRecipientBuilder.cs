using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.EntityBuilder
{
    public sealed class ServiceRecipientBuilder
    {
        private readonly ServiceRecipientEntity _serviceRecipientEntity;

        private ServiceRecipientBuilder()
        {
            _serviceRecipientEntity = new ServiceRecipientEntity();
        }

        public static ServiceRecipientBuilder Create()
        {
            return new ServiceRecipientBuilder();
        }

        public ServiceRecipientBuilder WithOdsCode(string odsCode)
        {
            _serviceRecipientEntity.OdsCode = odsCode;
            return this;
        }

        public ServiceRecipientBuilder WithName(string name)
        {
            _serviceRecipientEntity.Name = name;
            return this;
        }

        public ServiceRecipientBuilder WithOrderId(string orderId)
        {
            _serviceRecipientEntity.OrderId = orderId;
            return this;
        }

        public ServiceRecipientEntity Build()
        {
            return _serviceRecipientEntity;
        }
    }
}
