using BasketService.DAL;
using CSharpFunctionalExtensions;
using MediatR;

namespace BasketService.Features.GetShoppingCart;

public class GetShoppingCartQuery : IRequest<Result<ShoppingCartDto>>
{
    public int ShoppingCartId { get; init; }
}

public class ShoppingCartDto
{
    public int ShoppingCartId { get; init; }
    public ShoppingCartStatus ShoppingCartStatus { get; set; }
    public List<ProductItemDto> ProductItems { get; set; }
}

public class ProductItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}