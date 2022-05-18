using System.Data;
using BasketService.Infrastructure;
using BasketService.Infrastructure.Db;
using CSharpFunctionalExtensions;
using Dapper;

namespace BasketService.Features.RemoveProduct;

public class GetProductItemDbQuery : IDbQuery<Result<ProductItem>, BasketDbContext>
{
    public int ShoppingCartId { get; init; }
    public int ProductId { get; init; }

    public async Task<Result<ProductItem>> ExecuteAsync(IDbConnection connection, IDbTransaction transaction,
        CancellationToken cancellationToken)
    {
        const string sql = $@"
SELECT
        [ProductItemId] AS [{nameof(ProductItem.ProductItemId)}]
      , [ProductId]     AS [{nameof(ProductItem.ProductId)}]
      , [Quantity]      AS [{nameof(ProductItem.Quantity)}]
FROM [ProductItems]
WHERE [ShoppingCartId] = @{nameof(ShoppingCartId)} AND [ProductId] = @{nameof(ProductId)};
";

        var command = new CommandDefinition(
            commandText: sql,
            parameters: this,
            transaction: transaction,
            cancellationToken: cancellationToken
        );
        var productItem = await connection.QueryFirstOrDefaultAsync<ProductItem?>(command);
        if (productItem == null)
        {
            return Result.Failure<ProductItem>("Product '{ProductId}' is not in the cart '{ShoppingCartId}'.");
        }

        return productItem;
    }
}