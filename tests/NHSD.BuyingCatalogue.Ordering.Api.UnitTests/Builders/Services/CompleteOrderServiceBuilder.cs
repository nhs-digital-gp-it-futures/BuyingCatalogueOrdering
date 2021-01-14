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
        private IIdentityService identityService;
        private IOrderRepository orderRepository;
        private IEmailService emailService;
        private ICreatePurchasingDocumentService createPurchasingDocumentService;

        private PurchasingSettings purchasingSettings;

        private CompleteOrderServiceBuilder()
        {
        }

        public static CompleteOrderServiceBuilder Create() => new();

        public CompleteOrderServiceBuilder WithIdentityService(IIdentityService service)
        {
            identityService = service;
            return this;
        }

        public CompleteOrderServiceBuilder WithOrderRepository(IOrderRepository repository)
        {
            orderRepository = repository;
            return this;
        }

        public CompleteOrderServiceBuilder WithEmailService(IEmailService service)
        {
            emailService = service;
            return this;
        }

        public CompleteOrderServiceBuilder WithCreatePurchasingDocumentService(
            ICreatePurchasingDocumentService service)
        {
            createPurchasingDocumentService = service;
            return this;
        }

        public CompleteOrderServiceBuilder WithPurchasingSettings(PurchasingSettings settings)
        {
            purchasingSettings = settings;
            return this;
        }

        public CompleteOrderService Build() => new(
            identityService,
            orderRepository,
            emailService,
            createPurchasingDocumentService,
            purchasingSettings);
    }
}
