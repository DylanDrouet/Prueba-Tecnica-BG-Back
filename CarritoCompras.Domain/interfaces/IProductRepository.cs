using CarritoCompras.Domain.DTOs.Products;
using CarritoCompras.Domain.Entities;

namespace CarritoCompras.Domain.Interfaces;

public interface IProductRepository
{
    Task<(List<Product> Items, int TotalCount)> GetFilteredAsync(ProductQueryParams query);
    Task<Product?> GetByIdAsync(int id);
}