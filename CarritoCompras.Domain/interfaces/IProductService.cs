using CarritoCompras.Domain.DTOs.Products;

namespace CarritoCompras.Domain.Interfaces;

public interface IProductService
{
    Task<PagedResultDto<ProductDto>> GetProductsAsync(ProductQueryParams query);
    Task<ProductDto?> GetProductByIdAsync(int id);
}