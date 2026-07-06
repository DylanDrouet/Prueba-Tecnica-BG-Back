using CarritoCompras.Domain.Common;
using CarritoCompras.Domain.DTOs.Cart;
using CarritoCompras.Domain.Entities;
using CarritoCompras.Domain.Interfaces;

namespace CarritoCompras.Infrastructure.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;

    public CartService(ICartRepository cartRepository, IProductRepository productRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    public async Task<CartDto> GetCartAsync(int userId)
    {
        var cart = await _cartRepository.GetOrCreateCartAsync(userId);
        return MapToDto(cart);
    }

    public async Task<ServiceResult<CartDto>> AddItemAsync(int userId, AddCartItemDto dto)
    {
        if (dto.Quantity <= 0)
        {
            return ServiceResult<CartDto>.Fail("La cantidad debe ser mayor a 0.", ServiceErrorType.Validation);
        }

        var product = await _productRepository.GetByIdAsync(dto.ProductId);

        if (product is null)
        {
            return ServiceResult<CartDto>.Fail("El producto no existe.", ServiceErrorType.NotFound);
        }

        var cart = await _cartRepository.GetOrCreateCartAsync(userId);
        var existingItem = _cartRepository.FindItem(cart, dto.ProductId);

        var requestedQuantity = (existingItem?.Quantity ?? 0) + dto.Quantity;

        if (requestedQuantity > product.Stock)
        {
            return ServiceResult<CartDto>.Fail(
                $"Stock insuficiente. Disponible: {product.Stock}, solicitado: {requestedQuantity}.",
                ServiceErrorType.Conflict);
        }

        if (existingItem is not null)
        {
            existingItem.Quantity = requestedQuantity;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                CartId = cart.Id,
                ProductId = product.Id,
                Quantity = dto.Quantity
            });
        }

        await _cartRepository.SaveChangesAsync();

        var updatedCart = await _cartRepository.GetCartWithItemsAsync(userId);
        return ServiceResult<CartDto>.Ok(MapToDto(updatedCart!));
    }

    public async Task<ServiceResult<CartDto>> UpdateItemQuantityAsync(int userId, int productId, UpdateCartItemDto dto)
    {
        if (dto.Quantity <= 0)
        {
            return ServiceResult<CartDto>.Fail("La cantidad debe ser mayor a 0.", ServiceErrorType.Validation);
        }
        var cart = await _cartRepository.GetCartWithItemsAsync(userId);
        var item = cart is null ? null : _cartRepository.FindItem(cart, productId);

        if (cart is null || item is null)
        {
            return ServiceResult<CartDto>.Fail("El producto no está en el carrito.", ServiceErrorType.NotFound);
        }

        var product = await _productRepository.GetByIdAsync(productId);

        if (product is null || dto.Quantity > product.Stock)
        {
            return ServiceResult<CartDto>.Fail(
                $"Stock insuficiente. Disponible: {product?.Stock ?? 0}.",
                ServiceErrorType.Conflict);
        }

        item.Quantity = dto.Quantity;
        await _cartRepository.SaveChangesAsync();

        var updatedCart = await _cartRepository.GetCartWithItemsAsync(userId);
        return ServiceResult<CartDto>.Ok(MapToDto(updatedCart!));
    }

    public async Task<ServiceResult<CartDto>> RemoveItemAsync(int userId, int productId)
    {
        var cart = await _cartRepository.GetCartWithItemsAsync(userId);
        var item = cart is null ? null : _cartRepository.FindItem(cart, productId);

        if (cart is null || item is null)
        {
            return ServiceResult<CartDto>.Fail("El producto no está en el carrito.", ServiceErrorType.NotFound);
        }

        cart.Items.Remove(item);
        await _cartRepository.SaveChangesAsync();

        return ServiceResult<CartDto>.Ok(MapToDto(cart));
    }

    public async Task<CartDto> ClearCartAsync(int userId)
    {
        var cart = await _cartRepository.GetOrCreateCartAsync(userId);
        cart.Items.Clear();
        await _cartRepository.SaveChangesAsync();

        return MapToDto(cart);
    }

    private static CartDto MapToDto(Cart cart)
    {
        return new CartDto
        {
            Items = cart.Items.Select(i => new CartItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                ProductCode = i.Product.Code,
                UnitPrice = i.Product.Price,
                Quantity = i.Quantity,
                AvailableStock = i.Product.Stock
            }).ToList()
        };
    }
}