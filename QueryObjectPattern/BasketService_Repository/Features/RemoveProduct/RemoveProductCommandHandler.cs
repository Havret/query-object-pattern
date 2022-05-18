using CSharpFunctionalExtensions;
using MediatR;

namespace BasketService.Features.RemoveProduct;

public class RemoveProductCommandHandler : IRequestHandler<RemoveProductCommand, Result>
{
    public Task<Result> Handle(RemoveProductCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}