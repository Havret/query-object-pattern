using MediatR;

namespace BasketService.Features.OpenShoppingCart;

public class OpenShoppingCartCommand : IRequest<int>
{
    public int ClientId { get; set; }
}