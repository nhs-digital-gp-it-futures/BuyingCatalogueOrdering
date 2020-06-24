using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities
{
    public sealed class OrderItemEntity : EntityBase
    {
        public int OrderItemId { get; set; }

        public string OrderId { get; set; }

        public string CatalogueItemId { get; set; }

        public string CatalogueItemTypeId { get; set; }

        public string CatalogueItemName { get; set; }

        public string OdsCode { get; set; }

        public int ProvisioningTypeId { get; set; }

        public int CataloguePriceTypeId { get; set; }

        public string PricingUnitTierName { get; set; }

        public string PricingUnitDescription { get; set; }

        public int? TimeUnitId { get; set; }

        public string CurrencyCode { get; set; }

        public int Quantity { get; set; }

        public int? EstimationPeriodId { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public decimal? Price { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastUpdated { get; set; }

        protected override string InsertSql => @"
            INSERT INTO dbo.OrderItem
            (
                OrderId,
                CatalogueItemId,
                CatalogueItemTypeId,
                CatalogueItemName,
                OdsCode,
                ProvisioningTypeId,
                CataloguePriceTypeId,
                PricingUnitTierName,
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
                @CatalogueItemTypeId,
                @CatalogueItemName,
                @OdsCode,
                @ProvisioningTypeId,
                @CataloguePriceTypeId,
                @PricingUnitTierName,
                @PricingUnitDescription,
                @TimeUnitId,
                @CurrencyCode,
                @Quantity,
                @EstimationPeriodId,
                @DeliveryDate,
                @Price,
                @Created,
                @LastUpdated
            );";

        public static async Task<IEnumerable<OrderItemEntity>> FetchAllAsync(string connectionString)
        {
            return await SqlRunner.QueryAsync<OrderItemEntity>(connectionString, @"
                SELECT  OrderItemId,
                        OrderId,
                        CatalogueItemId,
                        CatalogueItemTypeId,
                        CatalogueItemName,
                        OdsCode,
                        ProvisioningTypeId,
                        CataloguePriceTypeId,
                        PricingUnitTierName,
                        PricingUnitDescription,
                        TimeUnitId,
                        CurrencyCode,
                        Quantity,
                        EstimationPeriodId,
                        DeliveryDate,
                        Price,
                        Created,
                        LastUpdated
                FROM    dbo.OrderItem;");
        }

        public static async Task<OrderItemEntity> FetchByOrderItemId(string connectionString, int orderItemId) => 
            (await FetchAllAsync(connectionString)).Single(item => orderItemId == item.OrderItemId);

        public static async Task<OrderItemEntity> FetchByCatalogueItemName(string connectionString, string catalogueItemName) => 
            (await FetchAllAsync(connectionString)).Single(item => 
                string.Equals(catalogueItemName, item.CatalogueItemName, StringComparison.OrdinalIgnoreCase));
    }
}
