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
        private readonly ICreatePurchasingDocumentService createPurchasingDocumentService;
        private readonly IEmailService emailService;
        private readonly IIdentityService identityService;
        private readonly IOrderRepository orderRepository;
        private readonly PurchasingSettings purchasingSettings;

        public CompleteOrderService(
            IIdentityService identityService,
            IOrderRepository orderRepository,
            IEmailService emailService,
            ICreatePurchasingDocumentService createPurchasingDocumentService,
            PurchasingSettings purchasingSettings)
        {
            this.identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            this.orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            this.createPurchasingDocumentService = createPurchasingDocumentService ?? throw new ArgumentNullException(nameof(createPurchasingDocumentService));
            this.purchasingSettings = purchasingSettings ?? throw new ArgumentNullException(nameof(purchasingSettings));
        }

        public async Task<Result> CompleteAsync(CompleteOrderRequest request)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var order = request.Order;

            var result = order.Complete(identityService.GetUserIdentity(), identityService.GetUserName());
            if (!result.IsSuccess)
                return result;

            await orderRepository.UpdateOrderAsync(order);

            if (!order.FundingSourceOnlyGMS.GetValueOrDefault())
                return Result.Success();

            await using var priceTypeStream = new MemoryStream();
            await using var patientNumbersStream = new MemoryStream();

            await createPurchasingDocumentService.CreateCsvAsync(priceTypeStream, order);
            priceTypeStream.Position = 0;

            var emailMessage = purchasingSettings.EmailMessage;
            var callOffAgreementId = order.OrderId;
            var orderingPartyId = order.OrganisationOdsCode;

            emailMessage.Subject = $"New Order {callOffAgreementId}_{orderingPartyId}";

            emailMessage.Attachments.Add(new EmailAttachment($"{callOffAgreementId}_{orderingPartyId}_Full.csv", priceTypeStream));

            var patientNumbers = order.OrderItems.Where(x =>
                x.ProvisioningType.Equals(ProvisioningType.Patient) &&
                !x.CatalogueItemType.Equals(CatalogueItemType.AssociatedService));

            if (order.OrderItems.Count.Equals(patientNumbers.Count()))
            {
                await createPurchasingDocumentService.CreatePatientNumbersCsvAsync(patientNumbersStream, order);
                patientNumbersStream.Position = 0;

                emailMessage.Attachments.Add(new EmailAttachment($"{callOffAgreementId}_{orderingPartyId}_Patients.csv", patientNumbersStream));
            }

            await emailService.SendEmailAsync(emailMessage);

            return Result.Success();
        }
    }
}
