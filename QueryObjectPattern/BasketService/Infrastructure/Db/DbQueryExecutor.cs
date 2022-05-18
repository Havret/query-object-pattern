namespace BasketService.Infrastructure.Db;

public class DbQueryExecutor : IDbQueryExecutor
{
    private readonly IServiceProvider _serviceProvider;


    public DbQueryExecutor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResult> ExecuteAsync<TResult, TDbContext>(IDbQuery<TResult, TDbContext> query, CancellationToken cancellationToken)
        where TDbContext : IDbContext
    {
        var dbContext = _serviceProvider.GetRequiredService<TDbContext>();
        var connection = await dbContext.GetDbConnectionAsync(cancellationToken);
        var transaction = await dbContext.GetTransactionAsync(cancellationToken);
        return await query.ExecuteAsync(connection, transaction, cancellationToken);
    }
}