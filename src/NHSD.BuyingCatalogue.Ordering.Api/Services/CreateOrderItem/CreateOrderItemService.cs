using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Validation;
using NHSD.BuyingCatalogue.Ordering.Contracts;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem
{
    internal sealed class CreateOrderItemService : ICreateOrderItemService
    {
        private readonly ApplicationDbContext context;
        private readonly ICreateOrderItemValidator orderItemValidator;
        private readonly IServiceRecipientService serviceRecipientService;

        public CreateOrderItemService(
            ApplicationDbContext context,
            ICreateOrderItemValidator orderItemValidator,
            IServiceRecipientService serviceRecipientService)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.orderItemValidator = orderItemValidator ?? throw new ArgumentNullException(nameof(orderItemValidator));
            this.serviceRecipientService = serviceRecipientService ?? throw new ArgumentNullException(nameof(serviceRecipientService));
        }

        public async Task<AggregateValidationResult> CreateAsync(Order order, CatalogueItemId catalogueItemId, CreateOrderItemModel model)
        {
            var catalogueItemType = Enum.Parse<CatalogueItemType>(model.CatalogueItemType, true);

            var aggregateValidationResult = orderItemValidator.Validate(order, model, catalogueItemType);
            if (!aggregateValidationResult.Success)
                return aggregateValidationResult;

            var catalogueItem = await AddOrUpdateCatalogueItem(catalogueItemId, model, catalogueItemType);
            var serviceRecipients = await AddOrUpdateServiceRecipients(model);
            var pricingUnit = await AddOrUpdatePricingUnit(model);

            var defaultDeliveryDate = order.DefaultDeliveryDates.SingleOrDefault(d => d.CatalogueItemId == catalogueItemId);
            var provisioningType = Enum.Parse<ProvisioningType>(model.ProvisioningType, true);
            var estimationPeriod = catalogueItemType.InferEstimationPeriod(
                provisioningType,
                OrderingEnums.ParseTimeUnit(model.EstimationPeriod));

            var item = order.AddOrUpdateOrderItem(new OrderItem
            {
                CatalogueItem = catalogueItem,
                CataloguePriceType = Enum.Parse<CataloguePriceType>(model.Type, true),
                CurrencyCode = model.CurrencyCode,
                DefaultDeliveryDate = defaultDeliveryDate?.DeliveryDate,
                EstimationPeriod = estimationPeriod,
                OrderId = order.Id,
                Price = model.Price,
                PricingUnit = pricingUnit,
                PriceTimeUnit = model.TimeUnit?.ToTimeUnit(),
                ProvisioningType = Enum.Parse<ProvisioningType>(model.ProvisioningType, true),
            });

            item.SetRecipients(model.ServiceRecipients.Select(r => new OrderItemRecipient
            {
                DeliveryDate = r.DeliveryDate,
                Quantity = r.Quantity.GetValueOrDefault(),
                Recipient = serviceRecipients[r.OdsCode],
            }));

            if (defaultDeliveryDate is not null)
                context.DefaultDeliveryDate.Remove(defaultDeliveryDate);

            await context.SaveChangesAsync();

            return aggregateValidationResult;
        }

        private async Task<CatalogueItem> AddOrUpdateCatalogueItem(
            CatalogueItemId catalogueItemId,
            CreateOrderItemModel model,
            CatalogueItemType catalogueItemType)
        {
            CatalogueItem parentCatalogueItem = null;
            var catalogueSolutionId = model.CatalogueSolutionId;

            if (catalogueSolutionId is not null)
                parentCatalogueItem = await context.FindAsync<CatalogueItem>(CatalogueItemId.Parse(catalogueSolutionId).Id);

            var catalogueItem = await context.FindAsync<CatalogueItem>(catalogueItemId) ?? new CatalogueItem
            {
                Id = catalogueItemId,
                CatalogueItemType = catalogueItemType,
            };

            catalogueItem.Name = model.CatalogueItemName;
            catalogueItem.ParentCatalogueItemId = parentCatalogueItem?.Id;

            return catalogueItem;
        }

        private async Task<PricingUnit> AddOrUpdatePricingUnit(CreateOrderItemModel model)
        {
            var pricingUnit = await context.FindAsync<PricingUnit>(model.ItemUnit.Name) ?? new PricingUnit
            {
                Name = model.ItemUnit.Name,
            };

            pricingUnit.Description = model.ItemUnit.Description;

            return pricingUnit;
        }

        private async Task<IReadOnlyDictionary<string, ServiceRecipient>> AddOrUpdateServiceRecipients(CreateOrderItemModel model)
        {
            var serviceRecipients = model.ServiceRecipients.Select(s => new ServiceRecipient(s.OdsCode, s.Name));

            return await serviceRecipientService.AddOrUpdateServiceRecipients(serviceRecipients);
        }
    }
}
