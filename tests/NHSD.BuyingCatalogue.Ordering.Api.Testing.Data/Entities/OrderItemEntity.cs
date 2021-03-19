using System;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Data;

namespace NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities
{
    public sealed class OrderItemEntity : EntityBase
    {
        public int OrderId { get; set; }

        public string CatalogueItemId { get; set; }

        public ProvisioningType ProvisioningType { get; set; }

        public CataloguePriceType CataloguePriceType { get; set; }

        public string PricingUnitName { get; set; }

        public TimeUnit? TimeUnit { get; set; }

        public TimeUnit? EstimationPeriod { get; set; }

        public string CurrencyCode { get; set; }

        public decimal? Price { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow.Date;

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow.Date;

        protected override string InsertSql => @"
            INSERT INTO dbo.OrderItem
            (
                OrderId,
                CatalogueItemId,
                ProvisioningTypeId,
                CataloguePriceTypeId,
                PricingUnitName,
                TimeUnitId,
                EstimationPeriodId,
                CurrencyCode,
                Price,
                Created,
                LastUpdated
            )
            VALUES
            (
                @OrderId,
                @CatalogueItemId,
                @ProvisioningType,
                @CataloguePriceType,
                @PricingUnitName,
                @TimeUnit,
                @EstimationPeriod,
                @CurrencyCode,
                @Price,
                @Created,
                @LastUpdated
            );";

        public static async Task<OrderItemEntity> FetchByKey(
            string connectionString,
            int orderId,
            string catalogueItemId)
        {
            const string sql = @"
                SELECT OrderId,
                       CatalogueItemId,
                       ProvisioningTypeId AS ProvisioningType,
                       CataloguePriceTypeId AS CataloguePriceType,
                       PricingUnitName,
                       TimeUnitId AS TimeUnit,
                       CurrencyCode,
                       EstimationPeriodId AS EstimationPeriod,
                       Price,
                       Created,
                       LastUpdated
                  FROM dbo.OrderItem
                 WHERE OrderId = @orderId
                   AND CatalogueItemId = @catalogueItemId;";

            return await SqlRunner.QueryFirstAsync<OrderItemEntity>(connectionString, sql, new { orderId, catalogueItemId });
        }

        public static async Task<int> GetItemIdsForOrderId(
            string connectionString,
            int orderId)
        {
            const string sql = @"
                SELECT COUNT(1)
                  FROM dbo.OrderItem
                 WHERE OrderId = @orderId;";

            return await SqlRunner.QueryFirstAsync<int>(connectionString, sql, new { orderId });
        }
    }
}
