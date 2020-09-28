using System;
using System.Threading.Tasks;

namespace NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities
{
    public sealed class DefaultDeliveryDateEntity : EntityBase
    {
        public string OrderId { get; set; }

        public string CatalogueItemId { get; set; }

        public int PriceId { get; set; }

        public DateTime DeliveryDate { get; set; }

        protected override string InsertSql =>
            "INSERT INTO dbo.DefaultDeliveryDate VALUES(@OrderId, @CatalogueItemId, @PriceId, @DeliveryDate);";

        public static async Task<DefaultDeliveryDateEntity> Fetch(string connectionString, object parameters)
        {
            const string sql = @"SELECT OrderId, CatalogueItemId, PriceId, DeliveryDate
FROM dbo.DefaultDeliveryDate
WHERE OrderId = @orderId
	AND CatalogueItemId = @catalogueItemId
	AND PriceId = @priceId;";

            return await SqlRunner.QueryFirstAsync<DefaultDeliveryDateEntity>(connectionString, sql, parameters);
        }
    }
}
