using System.Threading.Tasks;

namespace NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities
{
    public sealed class ContactEntity : EntityBase
    {
        public int ContactId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        protected override string InsertSql => @"
            INSERT INTO dbo.Contact
            (
                FirstName,
                LastName,
                Email,
                Phone
            )
            VALUES
            (
                @FirstName,
                @LastName,
                @Email,
                @Phone
            );
            SELECT SCOPE_IDENTITY();";

        public static async Task<ContactEntity> FetchContactById(string connectionString, int? contactId)
        {
            if (contactId == null)
            {
                return null;
            }

            return await SqlRunner.QueryFirstAsync<ContactEntity>(connectionString, @"SELECT
                         ContactId,
                         FirstName,
                         LastName,
                         Email,
                         Phone
                         FROM dbo.Contact
                         WHERE ContactId = @contactId;", new { contactId });
        }
    }
}
