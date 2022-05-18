using System.Data;
using BasketService.Infrastructure;
using BasketService.Infrastructure.Db;
using CSharpFunctionalExtensions;
using Dapper;

namespace BasketService.Features.AddProduct;

public class AddProductDbQuery : IDbQuery<Result, BasketDbContext>
{
    public int ShoppingCartId { get; init; }
    public int ProductId { get; init; }
    public int Quantity { get; init; }

    public async Task<Result> ExecuteAsync(IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken)
    {
        const string sql = $@"
MERGE INTO ProductItems AS TARGET
USING (SELECT 
    @{nameof(ShoppingCartId)} AS [ShoppingCartId],
    @{nameof(ProductId)} AS [ProductId],
    @{nameof(Quantity)} AS [Quantity]) AS SOURCE
ON SOURCE.ShoppingCartId = TARGET.ShoppingCartId AND SOURCE.ProductId = TARGET.ProductId
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([ShoppingCartId], [ProductId], [Quantity])
    VALUES (SOURCE.ShoppingCartId, SOURCE.ProductId, SOURCE.Quantity)
WHEN MATCHED THEN
    UPDATE
    SET TARGET.Quantity = TARGET.Quantity + SOURCE.Quantity;

SELECT @@ROWCOUNT;
";

        var command = new CommandDefinition(
            commandText: sql,
            parameters: this,
            transaction: transaction,
            cancellationToken: cancellationToken
        );
        var success = await connection.ExecuteScalarAsync<bool>(command);
        return Result.SuccessIf(success, $"Failed to add product '{ProductId}' to shopping card '{ShoppingCartId}'.");
    }
}