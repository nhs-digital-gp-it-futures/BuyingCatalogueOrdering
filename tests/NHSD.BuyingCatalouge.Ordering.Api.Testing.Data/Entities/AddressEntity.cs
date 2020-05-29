using System.Threading.Tasks;

namespace NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities
{
    public sealed class AddressEntity : EntityBase
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Line3 { get; set; }
        public string Line4 { get; set; }
        public string Line5 { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }

        protected override string InsertSql => @"
            INSERT INTO dbo.[Address]
            (
                Line1,
                Line2,
                Line3,
                Line4,
                Line5,
                Town,
                County,
                Postcode,
                Country
            )            
            VALUES
            (
                @Line1,
                @Line2,
                @Line3,
                @Line4,
                @Line5,
                @Town,
                @County,
                @Postcode,
                @Country
            );
            SELECT SCOPE_IDENTITY();";

        public static async Task<AddressEntity> FetchAddressById(string connectionString, int addressId)
        {
            return (await SqlRunner.QueryFirstAsync<AddressEntity>(connectionString, @"SELECT
                          Line1,
                          Line2,
                          Line3,
                          Line4,
                          Line5,
                          Town,
                          County,
                          Postcode,
                          Country
                         FROM dbo.[Address]
                         WHERE AddressId = @addressId;", new { addressId }));
        }
    }
}
