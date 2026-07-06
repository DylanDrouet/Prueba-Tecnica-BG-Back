using CarritoCompras.Domain.Entities;

namespace CarritoCompras.Domain.Interfaces;

public interface ICartRepository
{
    Task<Cart> GetOrCreateCartAsync(int userId);
    Task<Cart?> GetCartWithItemsAsync(int userId);
    CartItem? FindItem(Cart cart, int productId);
    Task SaveChangesAsync();
}