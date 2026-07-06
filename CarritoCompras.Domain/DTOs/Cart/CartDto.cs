namespace CarritoCompras.Domain.DTOs.Cart;

public class CartDto
{
    public List<CartItemDto> Items { get; set; } = new();
    public decimal Subtotal => Items.Sum(i => i.Subtotal);
    public decimal DiscountAmount => Subtotal > 100 ? Math.Round(Subtotal * 0.10m, 2) : 0;
    public decimal Total => Subtotal - DiscountAmount;
}