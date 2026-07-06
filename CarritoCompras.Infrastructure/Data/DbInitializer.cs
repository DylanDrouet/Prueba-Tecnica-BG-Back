using CarritoCompras.Domain.Entities;
using CarritoCompras.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarritoCompras.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context, IPasswordHasher passwordHasher)
    {
        await context.Database.MigrateAsync();

        if (!await context.Users.AnyAsync())
        {
            context.Users.AddRange(
                new User
                {
                    Username = "Fernando Mendoza",
                    Email = "fmendoza@bgcom",
                    PasswordHash = passwordHasher.Hash("Admin*123"),
                    Role = "Admin"
                },
                new User
                {
                    Username = "Irving Zambrano",
                    Email = "izambrano@bg.com",
                    PasswordHash = passwordHasher.Hash("Cliente*123"),
                    Role = "Customer"
                }
            );
        }

        if (!await context.Products.AnyAsync())
        {
            context.Products.AddRange(
                new Product { Code = "P001", Name = "Laptop MacBook Air M5", Category = "Tecnología", Price = 1000.00m, Stock = 15 },
                new Product { Code = "P002", Name = "Mouse Inalámbrico", Category = "Tecnología", Price = 18.50m, Stock = 100 },
                new Product { Code = "P003", Name = "Teclado Mecánico", Category = "Tecnología", Price = 45.00m, Stock = 40 },
                new Product { Code = "P004", Name = "Monitor 24 pulgadas", Category = "Tecnología", Price = 120.00m, Stock = 25 },
                new Product { Code = "P005", Name = "Silla Ergonómica", Category = "Oficina", Price = 135.00m, Stock = 10 }
            );
        }

        await context.SaveChangesAsync();
    }
}