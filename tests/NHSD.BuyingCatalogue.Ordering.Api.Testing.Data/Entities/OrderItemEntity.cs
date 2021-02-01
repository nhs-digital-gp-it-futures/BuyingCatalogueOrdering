using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Data;

namespace NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities
{
    public sealed class OrderItemEntity : EntityBase
    {
        public int OrderItemId { get; set; }

        public string OrderId { get; set; }

        public string CatalogueItemId { get; set; }

        public string ParentCatalogueItemId { get; set; }

        public CatalogueItemType CatalogueItemType { get; set; }

        public string CatalogueItemName { get; set; }

        public string OdsCode { get; set; }

        public ProvisioningType ProvisioningType { get; set; }

        public CataloguePriceType CataloguePriceType { get; set; }

        public string PricingUnitTierName { get; set; }

        public string PricingUnitName { get; set; }

        public string PricingUnitDescription { get; set; }

        public TimeUnit? TimeUnit { get; set; }

        public string CurrencyCode { get; set; }

        public int Quantity { get; set; }

        public TimeUnit? EstimationPeriod { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public decimal? Price { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastUpdated { get; set; }

        protected override string InsertSql =>
            @"INSERT INTO dbo.OrderItem
            (
                OrderId,
                CatalogueItemId,
                ParentCatalogueItemId,
                CatalogueItemTypeId,
                CatalogueItemName,
                OdsCode,
                ProvisioningTypeId,
                CataloguePriceTypeId,
                PricingUnitTierName,
                PricingUnitName,
                PricingUnitDescription,
                TimeUnitId,
                CurrencyCode,
                Quantity,
                EstimationPeriodId,
                DeliveryDate,
                Price,
                Created,
                LastUpdated
            )
            VALUES
            (
                @OrderId,
                @CatalogueItemId,
                @ParentCatalogueItemId,
                @CatalogueItemType,
                @CatalogueItemName,
                @OdsCode,
                @ProvisioningType,
                @CataloguePriceType,
                @PricingUnitTierName,
                @PricingUnitName,
                @PricingUnitDescription,
                @TimeUnit,
                @CurrencyCode,
                @Quantity,
                @EstimationPeriod,
                @DeliveryDate,
                @Price,
                @Created,
                @LastUpdated
            );
            SELECT SCOPE_IDENTITY();";

        public static async Task<IEnumerable<OrderItemEntity>> FetchAllAsync(string connectionString)
        {
            const string sql =
                @"SELECT OrderItemId,
                         OrderId,
                         CatalogueItemId,
                         ParentCatalogueItemId,
                         CatalogueItemTypeId AS CatalogueItemType,
                         CatalogueItemName,
                         OdsCode,
                         ProvisioningTypeId AS ProvisioningType,
                         CataloguePriceTypeId AS CataloguePriceType,
                         PricingUnitTierName,
                         PricingUnitName,
                         PricingUnitDescription,
                         TimeUnitId AS TimeUnit,
                         CurrencyCode,
                         Quantity,
                         EstimationPeriodId AS EstimationPeriod,
                         DeliveryDate,
                         Price,
                         Created,
                         LastUpdated
                    FROM dbo.OrderItem;";

            return await SqlRunner.QueryAsync<OrderItemEntity>(connectionString, sql);
        }

        public static async Task<OrderItemEntity> FetchByOrderItemId(string connectionString, int orderItemId) =>
            (await FetchAllAsync(connectionString)).SingleOrDefault(item => orderItemId == item.OrderItemId);

        public static async Task<OrderItemEntity> FetchByCatalogueItemName(string connectionString, string catalogueItemName) =>
            (await FetchAllAsync(connectionString)).Single(item =>
                string.Equals(catalogueItemName, item.CatalogueItemName, StringComparison.OrdinalIgnoreCase));
    }
}
