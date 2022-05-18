using BasketService.Infrastructure;
using BasketService.Infrastructure.Db;
using MediatR;

namespace BasketService.Features.OpenShoppingCart;

public class OpenShoppingCartCommandHandler : IRequestHandler<OpenShoppingCartCommand, int>
{
    private readonly IDbQueryExecutor _dbQueryExecutor;
    private readonly BasketDbContext _dbContext;

    public OpenShoppingCartCommandHandler(IDbQueryExecutor dbQueryExecutor, BasketDbContext dbContext)
    {
        _dbQueryExecutor = dbQueryExecutor;
        _dbContext = dbContext;
    }

    public async Task<int> Handle(OpenShoppingCartCommand request, CancellationToken cancellationToken)
    {
        var openShoppingCartDbQuery = new OpenShoppingCartDbQuery
        {
            ClientId = request.ClientId
        };
        var shoppingCartId = await _dbQueryExecutor.ExecuteAsync(openShoppingCartDbQuery, cancellationToken);
        await _dbContext.CommitAsync(cancellationToken);
        return shoppingCartId;
    }
}