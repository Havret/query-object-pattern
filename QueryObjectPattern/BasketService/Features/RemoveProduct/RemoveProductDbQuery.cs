using System.Data;
using BasketService.Infrastructure;
using BasketService.Infrastructure.Db;
using CSharpFunctionalExtensions;
using Dapper;

namespace BasketService.Features.RemoveProduct;

public class RemoveProductDbQuery : IDbQuery<Result, BasketDbContext>
{
    public int ProductItemId { get; init; }
    public int Quantity { get; init; }
    
    public async Task<Result> ExecuteAsync(IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken)
    {
        const string sql = $@"
MERGE INTO ProductItems AS TARGET
USING (SELECT 
    @{nameof(ProductItemId)} AS [ProductItemId],
    @{nameof(Quantity)} AS [Quantity]) AS SOURCE
ON SOURCE.ProductItemId = TARGET.ProductItemId
WHEN MATCHED AND SOURCE.Quantity = TARGET.Quantity THEN
    DELETE
WHEN MATCHED THEN
    UPDATE
    SET TARGET.Quantity = TARGET.Quantity - SOURCE.Quantity;

SELECT @@ROWCOUNT;
";

        var command = new CommandDefinition(
            commandText: sql,
            parameters: this,
            transaction: transaction,
            cancellationToken: cancellationToken
        );
        var success = await connection.ExecuteScalarAsync<bool>(command);
        return Result.SuccessIf(success, "Failed to remove product from cart.");
    }
}