using System.Threading.Tasks;

namespace NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities
{
    public abstract class EntityBase
    {
        protected abstract string InsertSql { get; }

        public async Task InsertAsync(string connectionString) =>
            await SqlRunner.ExecuteAsync(connectionString, InsertSql, this);

        public async Task<T> InsertAsync<T>(string connectionString)
        {
            return await SqlRunner.QueryFirstAsync<T>(connectionString, InsertSql, this);
        }
    }
}
