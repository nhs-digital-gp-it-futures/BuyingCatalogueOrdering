using System;
using System.IO;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.EmailClient;
using NHSD.BuyingCatalogue.Ordering.Api.Services.DocumentService;
using NHSD.BuyingCatalogue.Ordering.Api.Settings;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Application.Services;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CompleteOrder
{
    public sealed class CompleteOrderService : ICompleteOrderService
    {
        private readonly IIdentityService _identityService;
        private readonly IOrderRepository _orderRepository;
        private readonly IEmailService _emailService;
        private readonly IPurchasingDocumentService _purchasingDocumentService;
        private readonly PurchasingSettings _purchasingSettings;

        public CompleteOrderService(
            IIdentityService identityService,
            IOrderRepository orderRepository,
            IEmailService emailService,
            IPurchasingDocumentService purchasingDocumentService,
            PurchasingSettings purchasingSettings)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _purchasingDocumentService = purchasingDocumentService ?? throw new ArgumentNullException(nameof(purchasingDocumentService));
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

            await using var stream = new MemoryStream();
            await _purchasingDocumentService.Create(stream, order);
            stream.Position = 0;

            var emailMessage = _purchasingSettings.EmailMessage;
            emailMessage.Attachments.Add(new EmailAttachment("CompletedOrder.csv", stream));
            await _emailService.SendEmailAsync(emailMessage);

            return Result.Success();
        }
    }
}
