using BasketService.Features.AddProduct;
using BasketService.Features.ConfirmShoppingCart;
using BasketService.Features.GetShoppingCart;
using BasketService.Features.OpenShoppingCart;
using BasketService.Features.RemoveProduct;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BasketService.Api;

[ApiController]
[Route("api/shopping-carts")]
public class ShoppingCartsApi : ControllerBase
{
    private readonly IMediator _mediator;

    public ShoppingCartsApi(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> OpenCart([FromBody] OpenShoppingCartCommand command)
    {
        var cartId = await _mediator.Send(command, HttpContext.RequestAborted);
        return Created("api/shopping-carts", cartId);
    }

    [HttpPost("{shoppingCartId}/products")]
    public async Task<IActionResult> AddProduct([FromRoute] int shoppingCartId, [FromBody] AddProductCommand command)
    {
        var result = await _mediator.Send(command with { ShoppingCartId = shoppingCartId }, HttpContext.RequestAborted);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [HttpDelete("{shoppingCartId}/products/{productId}")]
    public async Task<IActionResult> RemoveProduct(
        [FromRoute] int shoppingCartId,
        [FromRoute] int productId,
        [FromBody] RemoveProductCommand command)
    {
        var result = await _mediator.Send(command with
        {
            ShoppingCartId = shoppingCartId,
            ProductId = productId
        }, HttpContext.RequestAborted);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [HttpPost("{shoppingCartId}/confirm")]
    public async Task<IActionResult> ConfirmShoppingCart([FromRoute] int shoppingCartId)
    {
        var result = await _mediator.Send(new ConfirmShoppingCartCommand
        {
            ShoppingCartId = shoppingCartId
        }, HttpContext.RequestAborted);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [HttpGet("{shoppingCartId}")]
    public async Task<ActionResult<ShoppingCartDto>> GetShoppingCart([FromRoute] int shoppingCartId)
    {
        var result = await _mediator.Send(new GetShoppingCartQuery { ShoppingCartId = shoppingCartId }, HttpContext.RequestAborted);
        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }
}