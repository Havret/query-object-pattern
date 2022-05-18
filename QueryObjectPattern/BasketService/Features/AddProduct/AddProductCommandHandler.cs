using BasketService.DAL;
using BasketService.Infrastructure;
using BasketService.Infrastructure.Db;
using CSharpFunctionalExtensions;
using MediatR;

namespace BasketService.Features.AddProduct;

public class AddProductCommandHandler : IRequestHandler<AddProductCommand, Result>
{
    private readonly IDbQueryExecutor _dbQueryExecutor;
    private readonly BasketDbContext _dbContext;

    public AddProductCommandHandler(IDbQueryExecutor dbQueryExecutor, BasketDbContext dbContext)
    {
        _dbQueryExecutor = dbQueryExecutor;
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(AddProductCommand request, CancellationToken cancellationToken)
    {
        var shoppingCartResult = await _dbQueryExecutor.ExecuteAsync(new GetShoppingCartDbQuery { ShoppingCartId = request.ShoppingCartId },
            cancellationToken);
        if (shoppingCartResult.IsFailure)
            return shoppingCartResult;

        var shoppingCart = shoppingCartResult.Value;

        if (shoppingCart.ShoppingCartStatus != ShoppingCartStatus.Pending)
        {
            return Result.Failure($"Adding products to a cart in '{shoppingCart.ShoppingCartStatus}' status is not allowed.");
        }
        
        var addProductDbQuery = new AddProductDbQuery
        {
            ShoppingCartId = request.ShoppingCartId,
            ProductId = request.ProductId,
            Quantity = request.Quantity
        };
        var result = await _dbQueryExecutor.ExecuteAsync(addProductDbQuery, cancellationToken);
        if (result.IsSuccess)
        {
            await _dbContext.CommitAsync(cancellationToken);
        }
        
        return result;
    }
}