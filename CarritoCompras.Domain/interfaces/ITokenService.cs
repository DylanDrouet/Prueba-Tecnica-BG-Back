using CarritoCompras.Domain.Entities;

namespace CarritoCompras.Domain.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}