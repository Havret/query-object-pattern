using System.Data;
using BasketService.DAL;
using BasketService.Infrastructure;
using BasketService.Infrastructure.Db;
using CSharpFunctionalExtensions;
using Dapper;

namespace BasketService.Features.ConfirmShoppingCart;

public class ConfirmShoppingCartDbQuery : IDbQuery<Result, BasketDbContext>
{
    public int ConfirmedShoppingCartStatus => (int) ShoppingCartStatus.Confirmed;

    public int ShoppingCartId { get; init; }

    public async Task<Result> ExecuteAsync(IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken)
    {
        string sql = $@"
UPDATE ShoppingCarts 
SET ShoppingCartStatus = @{nameof(ConfirmedShoppingCartStatus)}
WHERE ShoppingCartId = @{nameof(ShoppingCartId)};

SELECT @@ROWCOUNT;
";

        var command = new CommandDefinition(
            commandText: sql,
            parameters: this,
            transaction: transaction,
            cancellationToken: cancellationToken
        );
        var result = await connection.ExecuteScalarAsync<bool>(command);
        if (result == false)
        {
            return Result.Failure($"Failed to confirm Shopping Cart '{ShoppingCartId}'.");
        }

        return Result.Success();
    }
}