namespace BasketService.Features.AddProduct;

public class ShoppingCart
{
    public int ShoppingCartId { get; init; }
    public int ClientId { get; init; }
    public ShoppingCartStatus ShoppingCartStatus { get; init; }
}