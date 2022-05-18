using BasketService.Infrastructure.Db;
using CSharpFunctionalExtensions;
using Dapper;
using MediatR;

namespace BasketService.Features.AddProduct;

public class AddProductCommandHandler : IRequestHandler<AddProductCommand, Result>
{
    private readonly SqlConnectionFactory _sqlConnectionFactory;

    public AddProductCommandHandler(SqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result> Handle(AddProductCommand request, CancellationToken cancellationToken)
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
            parameters: new {request.ShoppingCartId},
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
            return Result.Failure($"Adding products to a cart in '{shoppingCart.ShoppingCartStatus}' status is not allowed.");
        }

        const string addProductSql = $@"
MERGE INTO ProductItems AS TARGET
USING (SELECT 
    @ShoppingCartId AS [ShoppingCartId],
    @ProductId AS [ProductId],
    @Quantity AS [Quantity]) AS SOURCE
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
            commandText: addProductSql,
            parameters: new { request.ShoppingCartId, request.ProductId, request.Quantity },
            transaction: transaction,
            cancellationToken: cancellationToken
        );
        var success = await connection.ExecuteScalarAsync<bool>(command);
        if (success)
        {
            await transaction.CommitAsync(cancellationToken);
        }

        return Result.SuccessIf(success, $"Failed to add product '{request.ProductId}' to shopping card '{request.ShoppingCartId}'.");
    }
}