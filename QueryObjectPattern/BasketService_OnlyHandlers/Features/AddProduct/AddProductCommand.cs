using CSharpFunctionalExtensions;
using MediatR;

namespace BasketService.Features.AddProduct;

public record AddProductCommand : IRequest<Result>
{
    internal int ShoppingCartId { get; init; }
    public int ProductId { get; init; }
    public int Quantity { get; init; }
}