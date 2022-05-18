using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace BasketService.Infrastructure.Db;

public abstract class BaseDbContext : IDbContext, IAsyncDisposable
{
    private readonly string _connectionString;
    private DbConnection? _dbConnection;
    private DbTransaction? _dbTransaction;

    protected BaseDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    async ValueTask<DbConnection> IDbContext.GetDbConnectionAsync(CancellationToken cancellationToken)
    {
        if (_dbConnection != null)
        {
            return _dbConnection;
        }

        _dbConnection = new SqlConnection(_connectionString);
        await _dbConnection.OpenAsync(cancellationToken);
        return _dbConnection;
    }

    async ValueTask<DbTransaction> IDbContext.GetTransactionAsync(CancellationToken cancellationToken)
    {
        if (_dbTransaction != null)
        {
            return _dbTransaction;
        }

        _dbTransaction = await _dbConnection!.BeginTransactionAsync(cancellationToken);
        return _dbTransaction;
    }

    public  Task CommitAsync(CancellationToken cancellationToken)
    {
        return _dbTransaction!.CommitAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_dbTransaction != null)
        {
            await _dbTransaction.DisposeAsync();
        }

        if (_dbConnection != null)
        {
            await _dbConnection.DisposeAsync();
        }
    }
}