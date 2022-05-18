using CSharpFunctionalExtensions;
using MediatR;

namespace BasketService.Features.ConfirmShoppingCart;

public class ConfirmShoppingCartCommandHandler : IRequestHandler<ConfirmShoppingCartCommand, Result>
{
    public Task<Result> Handle(ConfirmShoppingCartCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}