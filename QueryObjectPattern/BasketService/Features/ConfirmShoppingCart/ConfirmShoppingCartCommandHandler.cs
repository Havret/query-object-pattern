using BasketService.DAL;
using BasketService.Infrastructure;
using BasketService.Infrastructure.Db;
using CSharpFunctionalExtensions;
using MediatR;

namespace BasketService.Features.ConfirmShoppingCart;

public class ConfirmShoppingCartCommandHandler : IRequestHandler<ConfirmShoppingCartCommand, Result>
{
    private readonly IDbQueryExecutor _dbQueryExecutor;
    private readonly BasketDbContext _basketDbContext;

    public ConfirmShoppingCartCommandHandler(IDbQueryExecutor dbQueryExecutor, BasketDbContext basketDbContext)
    {
        _dbQueryExecutor = dbQueryExecutor;
        _basketDbContext = basketDbContext;
    }

    public async Task<Result> Handle(ConfirmShoppingCartCommand request, CancellationToken cancellationToken)
    {
        var shoppingCartResult = await _dbQueryExecutor.ExecuteAsync(new GetShoppingCartDbQuery { ShoppingCartId = request.ShoppingCartId },
            cancellationToken);
        if (shoppingCartResult.IsFailure)
            return shoppingCartResult;

        var shoppingCart = shoppingCartResult.Value;

        if (shoppingCart.ShoppingCartStatus != ShoppingCartStatus.Pending)
        {
            return Result.Failure($"Confirming cart in '{shoppingCart.ShoppingCartStatus}' status is not allowed.");
        }

        var confirmShoppingCartResult = await _dbQueryExecutor.ExecuteAsync(new ConfirmShoppingCartDbQuery
        {
            ShoppingCartId = request.ShoppingCartId
        }, cancellationToken);

        if (confirmShoppingCartResult.IsFailure)
        {
            return confirmShoppingCartResult;
        }

        await _basketDbContext.CommitAsync(cancellationToken);

        return Result.Success();
    }
}