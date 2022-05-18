using System.Data.Common;

namespace BasketService.Infrastructure.Db;

public interface IDbContext
{
    ValueTask<DbConnection> GetDbConnectionAsync(CancellationToken cancellationToken);
    ValueTask<DbTransaction> GetTransactionAsync(CancellationToken cancellationToken);
}