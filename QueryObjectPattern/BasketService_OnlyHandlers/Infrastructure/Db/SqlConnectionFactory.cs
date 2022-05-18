using Microsoft.Data.SqlClient;

namespace BasketService.Infrastructure.Db;

public class SqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(string connectionString)
    {
        this._connectionString = connectionString;
    }

    public async Task<SqlConnection> GetOpenConnection(CancellationToken cancellationToken)
    {
        var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        return connection;
    }
}