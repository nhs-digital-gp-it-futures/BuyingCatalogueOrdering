using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.EmailClient;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreatePurchasingDocument;
using NHSD.BuyingCatalogue.Ordering.Api.Settings;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CompleteOrder
{
    internal sealed class CompleteOrderService : ICompleteOrderService
    {
        private readonly ApplicationDbContext context;
        private readonly ICreatePurchasingDocumentService createPurchasingDocumentService;
        private readonly IEmailService emailService;
        private readonly PurchasingSettings purchasingSettings;

        public CompleteOrderService(
            ApplicationDbContext context,
            IEmailService emailService,
            ICreatePurchasingDocumentService createPurchasingDocumentService,
            PurchasingSettings purchasingSettings)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            this.createPurchasingDocumentService = createPurchasingDocumentService ?? throw new ArgumentNullException(nameof(createPurchasingDocumentService));
            this.purchasingSettings = purchasingSettings ?? throw new ArgumentNullException(nameof(purchasingSettings));
        }

        public async Task<Result> CompleteAsync(Order order)
        {
            var result = order.Complete();
            if (!result.IsSuccess)
                return result;

            await context.SaveChangesAsync();

            if (!order.FundingSourceOnlyGms.GetValueOrDefault())
                return Result.Success();

            await using var priceTypeStream = new MemoryStream();
            await using var patientNumbersStream = new MemoryStream();

            await createPurchasingDocumentService.CreateCsvAsync(priceTypeStream, order);
            priceTypeStream.Position = 0;

            var callOffAgreementId = order.CallOffId;
            var orderingPartyId = order.OrderingParty.OdsCode;

            var messageTemplate = purchasingSettings.EmailMessage.Message with
            {
                Subject = $"New Order {callOffAgreementId}_{orderingPartyId}",
            };

            var attachments = new List<EmailAttachment>(2)
            {
                new($"{callOffAgreementId}_{orderingPartyId}_Full.csv", priceTypeStream),
            };

            var patientNumbers = order.OrderItems.Where(i => i.ProvisioningType.Equals(ProvisioningType.Patient)
                && !i.CatalogueItem.CatalogueItemType.Equals(CatalogueItemType.AssociatedService));

            if (order.OrderItems.Count.Equals(patientNumbers.Count()))
            {
                await createPurchasingDocumentService.CreatePatientNumbersCsvAsync(patientNumbersStream, order);
                patientNumbersStream.Position = 0;

                attachments.Add(new EmailAttachment($"{callOffAgreementId}_{orderingPartyId}_Patients.csv", patientNumbersStream));
            }

            var message = new EmailMessage(
                messageTemplate,
                new[] { new EmailAddress(purchasingSettings.EmailMessage.Recipient) },
                attachments);

            await emailService.SendEmailAsync(message);

            return Result.Success();
        }
    }
}
