using CarritoCompras.Domain.DTOs.Products;
using CarritoCompras.Domain.Entities;
using CarritoCompras.Domain.Interfaces;
using CarritoCompras.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CarritoCompras.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(List<Product> Items, int TotalCount)> GetFilteredAsync(ProductQueryParams query)
    {
        var productsQuery = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            productsQuery = productsQuery.Where(p =>
                p.Name.ToLower().Contains(search) ||
                p.Code.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(query.Category))
        {
            productsQuery = productsQuery.Where(p => p.Category.ToLower() == query.Category.ToLower());
        }

        if (query.MinPrice.HasValue)
        {
            productsQuery = productsQuery.Where(p => p.Price >= query.MinPrice.Value);
        }

        if (query.MaxPrice.HasValue)
        {
            productsQuery = productsQuery.Where(p => p.Price <= query.MaxPrice.Value);
        }

        if (query.InStockOnly == true)
        {
            productsQuery = productsQuery.Where(p => p.Stock > 0);
        }

        var totalCount = await productsQuery.CountAsync();

        var items = await productsQuery
            .OrderBy(p => p.Name)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products.FindAsync(id);
    }
}