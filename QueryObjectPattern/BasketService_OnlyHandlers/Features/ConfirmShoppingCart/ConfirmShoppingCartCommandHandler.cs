using BasketService.Infrastructure.Db;
using CSharpFunctionalExtensions;
using Dapper;
using MediatR;

namespace BasketService.Features.ConfirmShoppingCart;

public class ConfirmShoppingCartCommandHandler : IRequestHandler<ConfirmShoppingCartCommand, Result>
{
    private readonly SqlConnectionFactory _sqlConnectionFactory;

    public ConfirmShoppingCartCommandHandler(SqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result> Handle(ConfirmShoppingCartCommand request, CancellationToken cancellationToken)
    {
        await using var connection = await _sqlConnectionFactory.GetOpenConnection(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        const string getShoppingCartSql = $@"
SELECT TOP (1)
        [ShoppingCartId]        AS [{nameof(ShoppingCart.ShoppingCartId)}]
      , [ClientId]              AS [{nameof(ShoppingCart.ClientId)}]
      , [ShoppingCartStatus]    AS [{nameof(ShoppingCart.ShoppingCartStatus)}]
    
FROM [ShoppingCarts]
WHERE [ShoppingCartId] = @ShoppingCartId
";

        var getShoppingCartCommand = new CommandDefinition(
            commandText: getShoppingCartSql,
            parameters: new { request.ShoppingCartId },
            transaction: transaction,
            cancellationToken: cancellationToken
        );
        var shoppingCart = await connection.QueryFirstOrDefaultAsync<ShoppingCart?>(getShoppingCartCommand);
        if (shoppingCart == null)
        {
            return Result.Failure($"Shopping Cart '{request.ShoppingCartId}' doesn't exist.");
        }

        if (shoppingCart.ShoppingCartStatus != ShoppingCartStatus.Pending)
        {
            return Result.Failure($"Confirming cart in '{shoppingCart.ShoppingCartStatus}' status is not allowed.");
        }

        string confirmShoppingCartSql = @"
UPDATE ShoppingCarts 
SET ShoppingCartStatus = @ConfirmedShoppingCartStatus
WHERE ShoppingCartId = @ShoppingCartId;

SELECT @@ROWCOUNT;
";

        var command = new CommandDefinition(
            commandText: confirmShoppingCartSql,
            parameters: new { request.ShoppingCartId, @ConfirmedShoppingCartStatus = (int) ShoppingCartStatus.Confirmed },
            transaction: transaction,
            cancellationToken: cancellationToken
        );
        var result = await connection.ExecuteScalarAsync<bool>(command);
        if (result == false)
        {
            return Result.Failure($"Failed to confirm Shopping Cart '{request.ShoppingCartId}'.");
        }

        await transaction.CommitAsync(cancellationToken);

        return Result.Success();
    }
}