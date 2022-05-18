using System.Data;
using BasketService.DAL;
using BasketService.Infrastructure;
using BasketService.Infrastructure.Db;
using Dapper;

namespace BasketService.Features.OpenShoppingCart;

public class OpenShoppingCartDbQuery : IDbQuery<int, BasketDbContext>
{
    public int ClientId { get; init; }
    public int PendingShoppingCartStatus => (int) ShoppingCartStatus.Pending;

    public Task<int> ExecuteAsync(IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken)
    {
        const string sql = $@"
INSERT INTO ShoppingCarts
	(ClientId, ShoppingCartStatus)
VALUES
	(@{nameof(ClientId)}, @{nameof(PendingShoppingCartStatus)});

SELECT SCOPE_IDENTITY();
";

        var command = new CommandDefinition(
            commandText: sql,
            parameters: this,
            transaction: transaction,
            cancellationToken: cancellationToken
        );
        return connection.ExecuteScalarAsync<int>(command);
    }
}