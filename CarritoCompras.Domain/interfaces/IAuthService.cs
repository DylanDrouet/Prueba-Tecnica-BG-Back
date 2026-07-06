using CarritoCompras.Domain.DTOs.Auth;

namespace CarritoCompras.Domain.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
}