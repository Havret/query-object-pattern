using System.Data;
using BasketService.Infrastructure;
using BasketService.Infrastructure.Db;
using CSharpFunctionalExtensions;
using Dapper;

namespace BasketService.DAL;

public class GetShoppingCartDbQuery : IDbQuery<Result<ShoppingCart>, BasketDbContext>
{
    public int ShoppingCartId { get; init; }

    public async Task<Result<ShoppingCart>> ExecuteAsync(IDbConnection connection, IDbTransaction transaction,
        CancellationToken cancellationToken)
    {
        const string sql = $@"
SELECT TOP (1)
        [ShoppingCartId]        AS [{nameof(ShoppingCart.ShoppingCartId)}]
      , [ClientId]              AS [{nameof(ShoppingCart.ClientId)}]
      , [ShoppingCartStatus]    AS [{nameof(ShoppingCart.ShoppingCartStatus)}]
    
FROM [ShoppingCarts]
WHERE [ShoppingCartId] = @{nameof(ShoppingCartId)}
";

        var command = new CommandDefinition(
            commandText: sql,
            parameters: this,
            transaction: transaction,
            cancellationToken: cancellationToken
        );
        var shoppingCart = await connection.QueryFirstOrDefaultAsync<ShoppingCart?>(command);
        if (shoppingCart == null)
        {
            return Result.Failure<ShoppingCart>($"Shopping Cart '{ShoppingCartId}' doesn't exist.");
        }
        return shoppingCart;
    }
}