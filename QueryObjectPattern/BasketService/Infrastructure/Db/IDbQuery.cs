using System.Data;

namespace BasketService.Infrastructure.Db;

public interface IDbQuery<TResult, TDbContext>
    where TDbContext : IDbContext
{
    Task<TResult> ExecuteAsync(IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken
    );
}