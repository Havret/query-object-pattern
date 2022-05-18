using System.Data;
using BasketService.Infrastructure;
using BasketService.Infrastructure.Db;
using Dapper;

namespace BasketService.Features.GetShoppingCart;

public class GetProductItemsDbQuery : IDbQuery<IReadOnlyList<ProductItem>, BasketDbContext>
{
    public GetProductItemsDbQuery(int shoppingCartId)
    {
        ShoppingCartId = shoppingCartId;
    }

    public int ShoppingCartId { get; }

    public async Task<IReadOnlyList<ProductItem>> ExecuteAsync(IDbConnection connection, IDbTransaction transaction,
        CancellationToken cancellationToken)
    {
        const string sql = $@"
SELECT
        [ProductId] AS [{nameof(ProductItem.ProductId)}]
      , [Quantity]  AS [{nameof(ProductItem.Quantity)}]
FROM [ProductItems]
WHERE [ShoppingCartId] = @{nameof(ShoppingCartId)}
";

        var command = new CommandDefinition(
            commandText: sql,
            parameters: this,
            transaction: transaction,
            cancellationToken: cancellationToken
        );
        
        var productItems = await connection.QueryAsync<ProductItem>(command);
        return productItems.ToList();
    }
}