using System;
using System.Threading.Tasks;

namespace NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities
{
    public sealed class DefaultDeliveryDateEntity : EntityBase
    {
        public int OrderId { get; set; }

        public string CatalogueItemId { get; set; }

        public DateTime? DeliveryDate { get; set; }

        protected override string InsertSql =>
            "INSERT INTO dbo.DefaultDeliveryDate VALUES(@OrderId, @CatalogueItemId, @DeliveryDate);";

        public static async Task<DefaultDeliveryDateEntity> Fetch(string connectionString, object parameters)
        {
            const string sql = @"
                SELECT OrderId, CatalogueItemId, DeliveryDate
                  FROM dbo.DefaultDeliveryDate
                 WHERE OrderId = @orderId
	               AND CatalogueItemId = @catalogueItemId;";

            return await SqlRunner.QueryFirstAsync<DefaultDeliveryDateEntity>(connectionString, sql, parameters);
        }
    }
}
