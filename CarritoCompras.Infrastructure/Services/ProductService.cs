using CarritoCompras.Domain.DTOs.Products;
using CarritoCompras.Domain.Entities;
using CarritoCompras.Domain.Interfaces;

namespace CarritoCompras.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResultDto<ProductDto>> GetProductsAsync(ProductQueryParams query)
    {
        var (items, totalCount) = await _repository.GetFilteredAsync(query);

        return new PagedResultDto<ProductDto>
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        return product is null ? null : MapToDto(product);
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Code = product.Code,
            Name = product.Name,
            Category = product.Category,
            Price = product.Price,
            Stock = product.Stock
        };
    }
}