using BasketService.Features.AddProduct;
using BasketService.Infrastructure.Db;
using CSharpFunctionalExtensions;
using Dapper;
using MediatR;

namespace BasketService.Features.GetShoppingCart;

public class GetShoppingCartQueryHandler : IRequestHandler<GetShoppingCartQuery, Result<ShoppingCartDto>>
{
    private readonly SqlConnectionFactory _sqlConnectionFactory;

    public GetShoppingCartQueryHandler(SqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<ShoppingCartDto>> Handle(GetShoppingCartQuery request, CancellationToken cancellationToken)
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
            return Result.Failure<ShoppingCartDto>($"Shopping Cart '{request.ShoppingCartId}' doesn't exist.");
        }

        const string getProductItemsSql = $@"
SELECT
        [ProductId] AS [{nameof(ProductItem.ProductId)}]
      , [Quantity]  AS [{nameof(ProductItem.Quantity)}]
FROM [ProductItems]
WHERE [ShoppingCartId] = @ShoppingCartId;
";

        var getProductItems = new CommandDefinition(
            commandText: getProductItemsSql,
            parameters: new { request.ShoppingCartId },
            transaction: transaction,
            cancellationToken: cancellationToken
        );

        var productItems = await connection.QueryAsync<ProductItem>(getProductItems);
        return new ShoppingCartDto
        {
            ShoppingCartId = shoppingCart.ShoppingCartId,
            ShoppingCartStatus = shoppingCart.ShoppingCartStatus,
            ProductItems = productItems.Select(x => new ProductItemDto
            {
                ProductId = x.ProductId,
                Quantity = x.Quantity
            }).ToList()
        };
    }
}