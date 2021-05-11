using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class CreateOrderItemModel
    {
        public string CatalogueItemName { get; init; }

        public string CatalogueItemType { get; set; }

        public string CatalogueSolutionId { get; set; }

        public string CurrencyCode { get; init; }

        public string EstimationPeriod { get; set; }

        public ItemUnitModel ItemUnit { get; init; }

        public int? PriceId { get; init; }

        public decimal? Price { get; init; }

        public string ProvisioningType { get; set; }

        [SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "Model binding will bind as list")]
        [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Set/init required for binding")]
        public List<OrderItemRecipientModel> ServiceRecipients { get; set; }

        public TimeUnitModel TimeUnit { get; set; }

        public string Type { get; init; }
    }
}
