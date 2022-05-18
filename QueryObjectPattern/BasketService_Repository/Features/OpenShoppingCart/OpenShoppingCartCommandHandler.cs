using MediatR;

namespace BasketService.Features.OpenShoppingCart;

public class OpenShoppingCartCommandHandler : IRequestHandler<OpenShoppingCartCommand, int>
{
    public Task<int> Handle(OpenShoppingCartCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}