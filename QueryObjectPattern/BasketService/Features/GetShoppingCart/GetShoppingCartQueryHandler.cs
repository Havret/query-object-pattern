using BasketService.DAL;
using BasketService.Infrastructure.Db;
using CSharpFunctionalExtensions;
using MediatR;

namespace BasketService.Features.GetShoppingCart;

public class GetShoppingCartQueryHandler : IRequestHandler<GetShoppingCartQuery, Result<ShoppingCartDto>>
{
    private readonly IDbQueryExecutor _dbQueryExecutor;

    public GetShoppingCartQueryHandler(IDbQueryExecutor dbQueryExecutor)
    {
        _dbQueryExecutor = dbQueryExecutor;
    }

    public async Task<Result<ShoppingCartDto>> Handle(GetShoppingCartQuery request, CancellationToken cancellationToken)
    {
        var shoppingCartResult = await _dbQueryExecutor.ExecuteAsync(new GetShoppingCartDbQuery
        {
            ShoppingCartId = request.ShoppingCartId
        }, cancellationToken);
        if (shoppingCartResult.IsFailure)
        {
            return shoppingCartResult.ConvertFailure<ShoppingCartDto>();
        }

        var productItems = await _dbQueryExecutor.ExecuteAsync(new GetProductItemsDbQuery(request.ShoppingCartId), cancellationToken);
        var shoppingCart = shoppingCartResult.Value;

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