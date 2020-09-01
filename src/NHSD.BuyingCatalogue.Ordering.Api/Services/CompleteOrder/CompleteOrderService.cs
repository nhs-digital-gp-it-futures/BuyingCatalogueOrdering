using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.EmailClient;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreatePurchasingDocument;
using NHSD.BuyingCatalogue.Ordering.Api.Settings;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Application.Services;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CompleteOrder
{
    public sealed class CompleteOrderService : ICompleteOrderService
    {
        private readonly IIdentityService _identityService;
        private readonly IOrderRepository _orderRepository;
        private readonly IEmailService _emailService;
        private readonly ICreatePurchasingDocumentService _createPurchasingDocumentService;
        private readonly PurchasingSettings _purchasingSettings;

        public CompleteOrderService(
            IIdentityService identityService,
            IOrderRepository orderRepository,
            IEmailService emailService,
            ICreatePurchasingDocumentService createPurchasingDocumentService,
            PurchasingSettings purchasingSettings)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _createPurchasingDocumentService = createPurchasingDocumentService ?? throw new ArgumentNullException(nameof(createPurchasingDocumentService));
            _purchasingSettings = purchasingSettings ?? throw new ArgumentNullException(nameof(purchasingSettings));
        }

        public async Task<Result> CompleteAsync(CompleteOrderRequest request)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var order = request.Order;

            var result = order.Complete(_identityService.GetUserIdentity(), _identityService.GetUserName());
            if (!result.IsSuccess)
                return result;

            await _orderRepository.UpdateOrderAsync(order);

            if (order.FundingSourceOnlyGMS.GetValueOrDefault())
            {
                var patientNumbers = order.OrderItems.Where(x =>
                    x.ProvisioningType.Equals(ProvisioningType.Patient) &&
                    !x.CatalogueItemType.Equals(CatalogueItemType.AssociatedService));

                if (order.OrderItems.Count.Equals(patientNumbers.Count()))
                {
                    await using var stream = new MemoryStream();
                    await _createPurchasingDocumentService.CreatePatientNumbersCsvAsync(stream, order);
                    stream.Position = 0;

                    var emailMessage = _purchasingSettings.EmailMessage;
                    emailMessage.Attachments.Add(new EmailAttachment("PatientNumbers.csv", stream));
                    await _emailService.SendEmailAsync(emailMessage);
                }
            }

            return Result.Success();
        }
    }
}
