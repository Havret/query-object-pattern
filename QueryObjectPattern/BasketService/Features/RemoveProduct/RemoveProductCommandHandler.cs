using BasketService.DAL;
using BasketService.Infrastructure;
using BasketService.Infrastructure.Db;
using CSharpFunctionalExtensions;
using MediatR;

namespace BasketService.Features.RemoveProduct;

public class RemoveProductCommandHandler : IRequestHandler<RemoveProductCommand, Result>
{
    private readonly IDbQueryExecutor _dbQueryExecutor;
    private readonly BasketDbContext _basketDbContext;

    public RemoveProductCommandHandler(IDbQueryExecutor dbQueryExecutor, BasketDbContext basketDbContext)
    {
        _dbQueryExecutor = dbQueryExecutor;
        _basketDbContext = basketDbContext;
    }

    public async Task<Result> Handle(RemoveProductCommand request, CancellationToken cancellationToken)
    {
        var shoppingCartResult = await _dbQueryExecutor.ExecuteAsync(new GetShoppingCartDbQuery { ShoppingCartId = request.ShoppingCartId },
            cancellationToken);
        if (shoppingCartResult.IsFailure)
            return shoppingCartResult;

        var shoppingCart = shoppingCartResult.Value;

        if (shoppingCart.ShoppingCartStatus != ShoppingCartStatus.Pending)
        {
            return Result.Failure($"Removing product from a cart in '{shoppingCart.ShoppingCartStatus}' status is not allowed.");
        }

        var productItemResult = await _dbQueryExecutor.ExecuteAsync(new GetProductItemDbQuery
        {
            ShoppingCartId = request.ShoppingCartId,
            ProductId = request.ProductId
        }, cancellationToken);
        if (productItemResult.IsFailure)
        {
            return productItemResult;
        }

        if (productItemResult.Value.Quantity < request.Quantity)
        {
            return Result.Failure($"Cannot remove {request.Quantity} items of Product  '{request.ProductId}' as there are only ${productItemResult.Value.Quantity} items in card.");
        }

        var result = await _dbQueryExecutor.ExecuteAsync(new RemoveProductDbQuery
        {
            ProductItemId = productItemResult.Value.ProductItemId,
            Quantity = request.Quantity
        }, cancellationToken);

        if (result.IsSuccess)
        {
            await _basketDbContext.CommitAsync(cancellationToken);
        }

        return result;
    }
}