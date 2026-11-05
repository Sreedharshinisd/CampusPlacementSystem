using CampusPlacementSystem.DTOs.Auth;

namespace CampusPlacementSystem.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginRequestDto request);

    Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken);

    Task<bool> LogoutAsync(string refreshToken);
}