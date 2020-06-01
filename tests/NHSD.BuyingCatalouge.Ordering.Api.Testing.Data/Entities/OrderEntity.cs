using System;
using System.Threading.Tasks;

namespace NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities
{
    public sealed class OrderEntity : EntityBase
    {
        public string OrderId { get; set; }

        public string Description { get; set; }

        public Guid OrganisationId { get; set; }

        public string OrganisationName { get; set; }

        public string OrganisationOdsCode { get; set; }

        public int? OrganisationAddressId { get; set; }

        public int? OrganisationBillingAddressId { get; set; }

        public int? OrganisationContactId { get; set; }

        public int OrderStatusId { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastUpdated { get; set; }

        public Guid LastUpdatedBy { get; set; }

        public DateTime? CommencementDate { get; set; }

        public string LastUpdatedByName { get; set; }

        public string SupplierId { get; set; }

        public string SupplierName { get; set; }

        public int? SupplierAddressId { get; set; }

        public int? SupplierContactId { get; set; }

        protected override string InsertSql => @"
            INSERT INTO dbo.[Order]
            (
                OrderId,
                Description,
                OrganisationId,
                OrganisationName,
                OrganisationOdsCode,
                OrganisationAddressId,
                OrganisationBillingAddressId,
                OrganisationContactId,
                OrderStatusId,
                Created,
                LastUpdated,
                LastUpdatedBy,
                LastUpdatedByName,
                SupplierId,
                SupplierName,
                CommencementDate,
                SupplierAddressId,
                SupplierContactId
            )
            VALUES
            (
                @OrderId,
                @Description,
                @OrganisationId,
                @OrganisationName,
                @OrganisationOdsCode,
                @OrganisationAddressId,
                @OrganisationBillingAddressId,
                @OrganisationContactId,
                @OrderStatusId,
                @Created,
                @LastUpdated,
                @LastUpdatedBy,
                @LastUpdatedByName,
                @SupplierId,
                @SupplierName,
                @CommencementDate,
                @SupplierAddressId,
                @SupplierContactId
            );";

        public static async Task<OrderEntity> FetchOrderByOrderId(string connectionString, string orderId)
        {
            return (await SqlRunner.QueryFirstAsync<OrderEntity>(connectionString, @"SELECT
                          OrderId,
                          Description,
                          OrganisationId,
                          OrganisationName,
                          OrganisationOdsCode,
                          OrganisationAddressId,
                          OrganisationContactId,
                          OrderStatusId,
                          Created,
                          SupplierId,
                          SupplierName,
                          SupplierAddressId,
                          SupplierContactId,
                          LastUpdated,
                          LastUpdatedBy,
                          CommencementDate,
                          LastUpdatedByName,
                          SupplierId,
                          SupplierName
                         FROM dbo.[Order]
                         WHERE OrderId = @orderId;", new { orderId }));
        }
    }
}
