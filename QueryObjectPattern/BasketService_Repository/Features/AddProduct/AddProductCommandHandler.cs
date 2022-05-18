using CSharpFunctionalExtensions;
using MediatR;

namespace BasketService.Features.AddProduct;

public class AddProductCommandHandler : IRequestHandler<AddProductCommand, Result>
{
    public Task<Result> Handle(AddProductCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}