using CSharpFunctionalExtensions;
using MediatR;

namespace BasketService.Features.ConfirmShoppingCart;

public class ConfirmShoppingCartCommand : IRequest<Result>
{
    public int ShoppingCartId { get; init; }
}