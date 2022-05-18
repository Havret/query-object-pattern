using BasketService.Infrastructure.Db;
using Dapper;
using MediatR;

namespace BasketService.Features.OpenShoppingCart;

public class OpenShoppingCartCommandHandler : IRequestHandler<OpenShoppingCartCommand, int>
{
    private readonly SqlConnectionFactory _sqlConnectionFactory;

    public OpenShoppingCartCommandHandler(SqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<int> Handle(OpenShoppingCartCommand request, CancellationToken cancellationToken)
    {
        await using var connection = await _sqlConnectionFactory.GetOpenConnection(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        const string sql = @"
INSERT INTO ShoppingCarts
	(ClientId, ShoppingCartStatus)
VALUES
	(@ClientId, @PendingShoppingCartStatus);

SELECT SCOPE_IDENTITY();
";

        var command = new CommandDefinition(
            commandText: sql,
            parameters: new { request.ClientId, PendingShoppingCartStatus = (int) ShoppingCartStatus.Pending },
            transaction: transaction,
            cancellationToken: cancellationToken
        );
        var shoppingCartId = await connection.ExecuteScalarAsync<int>(command);

        await transaction.CommitAsync(cancellationToken);

        return shoppingCartId;
    }
}