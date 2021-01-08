using NHSD.BuyingCatalogue.EmailClient;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CompleteOrder;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreatePurchasingDocument;
using NHSD.BuyingCatalogue.Ordering.Api.Settings;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Application.Services;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders.Services
{
    internal sealed class CompleteOrderServiceBuilder
    {
        private IIdentityService _identityService;
        private IOrderRepository _orderRepository;
        private IEmailService _emailService;
        private ICreatePurchasingDocumentService _createPurchasingDocumentService;

        private PurchasingSettings _purchasingSettings;

        private CompleteOrderServiceBuilder()
        {
        }

        public static CompleteOrderServiceBuilder Create() => new();

        public CompleteOrderServiceBuilder WithIdentityService(IIdentityService identityService)
        {
            _identityService = identityService;
            return this;
        }

        public CompleteOrderServiceBuilder WithOrderRepository(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
            return this;
        }

        public CompleteOrderServiceBuilder WithEmailService(IEmailService emailService)
        {
            _emailService = emailService;
            return this;
        }

        public CompleteOrderServiceBuilder WithCreatePurchasingDocumentService(
            ICreatePurchasingDocumentService createPurchasingDocumentService)
        {
            _createPurchasingDocumentService = createPurchasingDocumentService;
            return this;
        }

        public CompleteOrderServiceBuilder WithPurchasingSettings(PurchasingSettings purchasingSettings)
        {
            _purchasingSettings = purchasingSettings;
            return this;
        }

        public CompleteOrderService Build() => new(
            _identityService,
            _orderRepository,
            _emailService,
            _createPurchasingDocumentService,
            _purchasingSettings);
    }
}
