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

        public string LastUpdatedByName { get; set; }

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
                LastUpdatedByName
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
                @LastUpdatedByName
            );";

        public static async Task<string> FetchOrderDescriptionFromOrderId(string connectionString, string orderId)
        {
            return (await SqlRunner.QueryFirstAsync<string>(connectionString, @"SELECT
                         [Description]
                         FROM dbo.[Order]
                         WHERE OrderId = @orderId;", new { orderId }));
        }

        public static async Task<string> FetchLastUpdatedByNameFromOrderId(string connectionString, string orderId)
        {
            return (await SqlRunner.QueryFirstAsync<string>(connectionString, @"SELECT
                         LastUpdatedByName
                         FROM dbo.[Order]
                         WHERE OrderId = @orderId;", new { orderId }));
        }

        public static async Task<OrderEntity> FetchOrderByOrderId(string connectionString, string orderId)
        {
            return (await SqlRunner.QueryFirstAsync<OrderEntity>(connectionString, @"SELECT
                          OrderId,
                          [Description],
                          OrganisationId,
                          OrganisationName,
                          OrganisationOdsCode,
                          OrderStatusId,
                          Created,
                          LastUpdated,
                          LastUpdatedBy,
                          LastUpdatedByName
                         FROM [Order]
                         WHERE [OrderId] = @orderId", new { orderId }));
        }

        public static async Task<ContactEntity> FetchPrimaryContactByOrderId(string connectionString, string orderId)
        {
            return (await SqlRunner.QueryFirstAsync<ContactEntity>(connectionString, $@"SELECT
                        [ContactId],
                        [FirstName],
                        [LastName],
                        [Email],
                        [Phone] 
                        FROM [CatalogueOrdering].[dbo].[Order] od join [CatalogueOrdering].[dbo].[Contact] ct on od.organisationContactId = ct.ContactId
                        WHERE [OrderId] = @orderId", new { orderId }));
        }

        public static async Task<AddressEntity> FetchOrganisationAddressByOrderId(string connectionString, string orderId)
        {
            return (await SqlRunner.QueryFirstAsync<AddressEntity>(connectionString, $@"SELECT  
                        [AddressId],
                        [Line1],
                        [Line2],
                        [Line3],
                        [Line4],
                        [Line5],
                        [Town],
                        [County],
                        [Postcode], 
                        [Country]
                        FROM [CatalogueOrdering].[dbo].[Order] od join [CatalogueOrdering].[dbo].[Address] ad on od.organisationAddressId = ad.AddressId
                        WHERE [OrderId] = @orderId", new { orderId }));
        }
    }
}
