using CSharpFunctionalExtensions;
using MediatR;

namespace BasketService.Features.GetShoppingCart;

public class GetShoppingCartQueryHandler : IRequestHandler<GetShoppingCartQuery, Result<ShoppingCartDto>>
{
    public Task<Result<ShoppingCartDto>> Handle(GetShoppingCartQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

