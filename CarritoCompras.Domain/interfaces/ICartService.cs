using CarritoCompras.Domain.Common;
using CarritoCompras.Domain.DTOs.Cart;

namespace CarritoCompras.Domain.Interfaces;

public interface ICartService
{
    Task<CartDto> GetCartAsync(int userId);
    Task<ServiceResult<CartDto>> AddItemAsync(int userId, AddCartItemDto dto);
    Task<ServiceResult<CartDto>> UpdateItemQuantityAsync(int userId, int productId, UpdateCartItemDto dto);
    Task<ServiceResult<CartDto>> RemoveItemAsync(int userId, int productId);
    Task<CartDto> ClearCartAsync(int userId);
}