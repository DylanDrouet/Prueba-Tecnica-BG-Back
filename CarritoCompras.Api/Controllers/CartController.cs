using System.Security.Claims;
using CarritoCompras.Domain.Common;
using CarritoCompras.Domain.DTOs.Cart;
using CarritoCompras.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarritoCompras.Api.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    private int GetUserId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.Parse(idClaim!);
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var cart = await _cartService.GetCartAsync(GetUserId());
        return Ok(cart);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem(AddCartItemDto dto)
    {
        var result = await _cartService.AddItemAsync(GetUserId(), dto);
        return HandleResult(result);
    }

    [HttpPut("items/{productId:int}")]
    public async Task<IActionResult> UpdateItem(int productId, UpdateCartItemDto dto)
    {
        var result = await _cartService.UpdateItemQuantityAsync(GetUserId(), productId, dto);
        return HandleResult(result);
    }

    [HttpDelete("items/{productId:int}")]
    public async Task<IActionResult> RemoveItem(int productId)
    {
        var result = await _cartService.RemoveItemAsync(GetUserId(), productId);
        return HandleResult(result);
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        var cart = await _cartService.ClearCartAsync(GetUserId());
        return Ok(cart);
    }

    private IActionResult HandleResult(ServiceResult<CartDto> result)
    {
        if (result.Success)
        {
            return Ok(result.Data);
        }

        return result.ErrorType switch
        {
            ServiceErrorType.NotFound => NotFound(new { message = result.ErrorMessage }),
            ServiceErrorType.Conflict => Conflict(new { message = result.ErrorMessage }),
            _ => BadRequest(new { message = result.ErrorMessage })
        };
    }
}