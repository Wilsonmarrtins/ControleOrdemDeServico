using Microsoft.Data.SqlClient;
using System.Data;

namespace OsService.Infrastructure.Databases;

public sealed class SqlConnectionFactory(string connectionString) : IDefaultSqlConnectionFactory
{
    public IDbConnection Create()
        => new SqlConnection(connectionString);
}