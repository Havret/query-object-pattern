namespace BasketService.Infrastructure.Db;

public interface IDbQueryExecutor
{
    Task<TResult> ExecuteAsync<TResult, TDbContext>(
        IDbQuery<TResult, TDbContext> query,
        CancellationToken cancellationToken)
        where TDbContext : IDbContext;
}