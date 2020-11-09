using System;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class CreateOrderItemModel : UpdateOrderItemModel
    {
        public string CatalogueItemId { get; set; }

        public string CatalogueItemName { get; set; }

        public string CatalogueItemType { get; set; }

        public string CatalogueSolutionId { get; set; }

        public string CurrencyCode { get; set; }

        public ItemUnitModel ItemUnit { get; set; }

        public string ProvisioningType { get; set; }

        public ServiceRecipientModel ServiceRecipient { get; set; }

        public TimeUnitModel TimeUnit { get; set; }

        public string Type { get; set; }

        internal CreateOrderItemRequest ToRequest(Order order)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            if (Quantity is null)
                throw new InvalidOperationException($"{nameof(CreateOrderItemModel)}.{nameof(Quantity)} should never be null at this point.");

            var itemType = OrderingEnums.Parse<CatalogueItemType>(CatalogueItemType);

            return itemType switch
            {
                Domain.CatalogueItemType.Solution => new CreateOrderItemSolutionRequest(order, this),
                Domain.CatalogueItemType.AdditionalService => new CreateOrderItemAdditionalServiceRequest(order, this),
                Domain.CatalogueItemType.AssociatedService => new CreateOrderItemAssociatedServiceRequest(order, this),
                _ => throw new InvalidOperationException("Invalid catalogue item type."),
            };
        }
    }
}
