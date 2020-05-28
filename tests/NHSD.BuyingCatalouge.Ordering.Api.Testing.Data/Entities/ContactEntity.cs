using System.Threading.Tasks;

namespace NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities
{
    public sealed class ContactEntity : EntityBase
    {
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

        public static async Task<ContactEntity> FetchContactByContactId(string connectionString, int? contactId)
        {
            if (contactId == null)
            {
                return null;
            }

            return (await SqlRunner.QueryFirstAsync<ContactEntity>(connectionString, @"SELECT
                        ContactId,
                        FirstName,
                        LastName,
                        Email,
                        Phone 
                        FROM dbo.[Contact]
                        WHERE ContactId = @ContactId;", new { contactId }));
        }
    }
}
