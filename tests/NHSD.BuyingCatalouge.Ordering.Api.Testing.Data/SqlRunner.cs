using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;

namespace NHSD.BuyingCatalouge.Ordering.Api.Testing.Data
{
    internal static class SqlRunner
    {
        internal static async Task ExecuteAsync<T>(string connectionString, string sql, T instance)
        {
            using IDbConnection databaseConnection = new SqlConnection(connectionString);
            await databaseConnection.ExecuteAsync(sql, instance);
        }

        internal static async Task ExecuteAsync<T>(string connectionString, string sql, IEnumerable<T> items)
        {
            await using var databaseConnection = new SqlConnection(connectionString);
            await databaseConnection.ExecuteAsync(sql, items);
        }

        internal static async Task<T> QueryFirstAsync<T>(string connectionString, string sql, object parameters = null)
        {
            await using var databaseConnection = new SqlConnection(connectionString);
            return await databaseConnection.QueryFirstAsync<T>(sql, parameters);
        }
    }
}
