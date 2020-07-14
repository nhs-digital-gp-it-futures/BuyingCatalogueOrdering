using System;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class GetOrderItemModel
    {
        public string ItemId { get; set; }

        public ServiceRecipientModel ServiceRecipient { get; set; }

        public string CataloguePriceType { get; set; }

        public string CatalogueItemType { get; set; }

        public string CatalogueItemName { get; set; }

        public string CatalogueItemId { get; set; }
    }
}
