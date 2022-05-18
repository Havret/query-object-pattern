using CSharpFunctionalExtensions;
using MediatR;

namespace BasketService.Features.RemoveProduct;

public record RemoveProductCommand : IRequest<Result>
{
    internal int ShoppingCartId { get; init; }
    internal int ProductId { get; init; }
    public int Quantity { get; init; }
}