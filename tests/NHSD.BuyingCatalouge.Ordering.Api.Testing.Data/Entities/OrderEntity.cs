using System;
using System.Threading.Tasks;

namespace NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities
{
    public sealed class OrderEntity : EntityBase
    {
        public string OrderId { get; set; }

        public string Description { get; set; }

        public Guid OrganisationId { get; set; }

        public int OrderStatusId { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastUpdated { get; set; }

        public Guid LastUpdatedBy { get; set; }

        public string LastUpdatedByName { get; set; }

        public string SupplierId { get; set; }

        public string SupplierName { get; set; }

        protected override string InsertSql => $@"
            INSERT INTO [dbo].[Order]
            (
                OrderId,
                Description,
                OrganisationId,
                OrderStatusId,
                Created,
                LastUpdated,
                LastUpdatedBy,
                LastUpdatedByName,
                SupplierId,
                SupplierName
            )
            VALUES
            (
                @OrderId,
                @Description,
                @OrganisationId,
                @OrderStatusId,
                @Created,
                @LastUpdated,
                @LastUpdatedBy,
                @LastUpdatedByName,
                @SupplierId,
                @SupplierName
            )";

        public static async Task<OrderEntity> FetchOrderByOrderId(string connectionString, string orderId)
        {
            return (await SqlRunner.QueryFirstAsync<OrderEntity>(connectionString, $@"SELECT
                          [OrderId],
                          [Description],
                          [OrganisationId],
                          [OrderStatusId],
                          [Created],
                          [LastUpdated],
                          [LastUpdatedBy],
                          [LastUpdatedByName],
                          [SupplierId],
                          [SupplierName],
                         FROM [Order]
                         WHERE [OrderId] = @orderId", new { orderId }));
        }
    }
}
