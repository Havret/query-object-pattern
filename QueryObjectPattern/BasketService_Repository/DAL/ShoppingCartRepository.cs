using BasketService.Infrastructure.Db;
using CSharpFunctionalExtensions;
using Dapper;

namespace BasketService.DAL;

public class ShoppingCartRepository
{
    private readonly SqlConnectionFactory _sqlConnectionFactory;

    public ShoppingCartRepository(SqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<int> OpenShoppingCart(int clientId, CancellationToken cancellationToken)
    {
        await using var connection = await _sqlConnectionFactory.GetOpenConnection(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        const string sql = $@"
INSERT INTO ShoppingCarts
	(ClientId, ShoppingCartStatus)
VALUES
	(@ClientId, @PendingShoppingCartStatus);

SELECT SCOPE_IDENTITY();
";

        var command = new CommandDefinition(
            commandText: sql,
            parameters: new { ClientId = clientId, PendingShoppingCartStatus = (int) ShoppingCartStatus.Pending },
            transaction: transaction,
            cancellationToken: cancellationToken
        );
        var shoppingCartId = await connection.ExecuteAsync(command);
        await transaction.CommitAsync(cancellationToken);
        return shoppingCartId;
    }

    public async Task<Result> AddProduct(int shoppingCartId, int productId, int quantity, CancellationToken cancellationToken)
    {
        Console.WriteLine(shoppingCartId+productId+quantity);
        await Task.Delay(0, cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ConfirmShoppingCart(int shoppingCartId, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        return Result.Success();
    }

    public async Task<Result<ShoppingCart>> GetShoppingCart(int shoppingCartId, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        return Result.Success(new ShoppingCart {ShoppingCartId = shoppingCartId});
    }
    
    public async Task<Result> RemoveProduct(int shoppingCartId, int productId, int quantity, CancellationToken cancellationToken)
    {
        Console.WriteLine(shoppingCartId+productId+quantity);
        await Task.Delay(0, cancellationToken);
        return Result.Success();
    }
}