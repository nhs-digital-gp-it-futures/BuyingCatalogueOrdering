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
                LastUpdatedByName
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
                @LastUpdatedByName
            )";

        public static async Task<string> FetchOrderDescriptionFromOrderId(string connectionString, string orderId)
        {
            return (await SqlRunner.QueryFirstAsync<string>(connectionString, $@"SELECT
                         [Description]
                         FROM [Order]
                         WHERE [OrderId] = @orderId", new { orderId }));
        }

        public static async Task<string> FetchLastUpdatedByNameFromOrderId(string connectionString, string orderId)
        {
            return (await SqlRunner.QueryFirstAsync<string>(connectionString, $@"SELECT
                         [LastUpdatedByName]
                         FROM [Order]
                         WHERE [OrderId] = @orderId", new { orderId }));
        }
    }
}
