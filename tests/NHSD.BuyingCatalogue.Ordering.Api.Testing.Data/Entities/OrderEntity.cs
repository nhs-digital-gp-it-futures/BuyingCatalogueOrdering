using System;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Data;

namespace NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities
{
    public sealed class OrderEntity : EntityBase
    {
        public int Id { get; set; }

        public byte Revision { get; set; } = 1;

        public string CallOffId { get; set; }

        public string Description { get; set; }

        public Guid OrderingPartyId { get; set; }

        public int? OrderingPartyContactId { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Completed { get; set; }

        public DateTime LastUpdated { get; set; }

        public Guid LastUpdatedBy { get; set; }

        public DateTime? CommencementDate { get; set; }

        public string LastUpdatedByName { get; set; }

        public string SupplierId { get; set; }

        public int? SupplierContactId { get; set; }

        public bool? FundingSourceOnlyGms { get; set; }

        public bool IsDeleted { get; set; }

        protected override string InsertSql => @"
            SET IDENTITY_INSERT dbo.[Order] ON;

            INSERT INTO dbo.[Order]
            (
                Id,
                Description,
                OrderingPartyId,
                OrderingPartyContactId,
                OrderStatusId,
                Created,
                LastUpdated,
                LastUpdatedBy,
                LastUpdatedByName,
                SupplierId,
                CommencementDate,
                SupplierContactId,
                FundingSourceOnlyGMS,
                IsDeleted,
                Completed
            )
            VALUES
            (
                @Id,
                @Description,
                @OrderingPartyId,
                @OrderingPartyContactId,
                @OrderStatus,
                @Created,
                @LastUpdated,
                @LastUpdatedBy,
                @LastUpdatedByName,
                @SupplierId,
                @CommencementDate,
                @SupplierContactId,
                @FundingSourceOnlyGMS,
                @IsDeleted,
                @Completed
            );

            SET IDENTITY_INSERT dbo.[Order] OFF;";

        public static async Task<OrderEntity> FetchOrderByOrderId(string connectionString, int id)
        {
            const string sql =
                @"SELECT Id,
                         Revision,
                         CallOffId,
                         [Description],
                         OrderingPartyId,
                         OrderingPartyContactId,
                         OrderStatusId AS OrderStatus,
                         Created,
                         SupplierId,
                         SupplierContactId,
                         LastUpdated,
                         LastUpdatedBy,
                         CommencementDate,
                         LastUpdatedByName,
                         FundingSourceOnlyGMS,
                         IsDeleted,
                         Completed
                    FROM dbo.[Order]
                   WHERE Id = @id;";

            return await SqlRunner.QueryFirstAsync<OrderEntity>(connectionString, sql, new { id });
        }
    }
}
