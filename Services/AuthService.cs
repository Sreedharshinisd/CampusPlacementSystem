using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CampusPlacementSystem.Data;
using CampusPlacementSystem.DTOs.Auth;
using CampusPlacementSystem.Enums;
using CampusPlacementSystem.Models;
using CampusPlacementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CampusPlacementSystem.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context,
        IOptions<JwtSettings> jwtOptions)
    {
        _userManager = userManager;
        _context = context;
        _jwtSettings = jwtOptions.Value;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return null;

        if (user.AccountStatus != AccountStatus.Approved)
            return null;

        bool passwordValid = await _userManager.CheckPasswordAsync(
            user, request.Password);

        if (!passwordValid)
            return null;

        return await GenerateTokensAsync(user);
    }

    public async Task<AuthResponseDto?> RefreshTokenAsync(
        string refreshToken)
    {
        string tokenHash = HashToken(refreshToken);

        var storedToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);

        if (storedToken == null ||
            storedToken.IsRevoked ||
            storedToken.ExpiresAt <= DateTime.UtcNow)
        {
            return null;
        }

        if (storedToken.User.AccountStatus != AccountStatus.Approved)
            return null;

        storedToken.RevokedAt = DateTime.UtcNow;

        var response = await GenerateTokensAsync(storedToken.User);

        await _context.SaveChangesAsync();

        return response;
    }

    public async Task<bool> LogoutAsync(string refreshToken)
    {
        string tokenHash = HashToken(refreshToken);

        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);

        if (storedToken == null ||
            storedToken.IsRevoked)
        {
            return false;
        }

        storedToken.RevokedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    private async Task<AuthResponseDto> GenerateTokensAsync(
        ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.FullName)
        };

        claims.AddRange(roles.Select(role =>
            new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

        var credentials = new SigningCredentials(
            key, SecurityAlgorithms.HmacSha256);

        var accessTokenExpiresAt = DateTime.UtcNow.AddMinutes(
            _jwtSettings.AccessTokenExpirationMinutes);

        var jwtToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: accessTokenExpiresAt,
            signingCredentials: credentials);

        string accessToken = new JwtSecurityTokenHandler()
            .WriteToken(jwtToken);

        string rawRefreshToken = Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(64));

        DateTime refreshTokenExpiresAt = DateTime.UtcNow.AddDays(
            _jwtSettings.RefreshTokenExpirationDays);

        var refreshToken = new RefreshToken
        {
            TokenHash = HashToken(rawRefreshToken),
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = refreshTokenExpiresAt
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = rawRefreshToken,
            AccessTokenExpiresAt = accessTokenExpiresAt,
            RefreshTokenExpiresAt = refreshTokenExpiresAt,
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            Roles = roles
        };
    }

    private static string HashToken(string token)
    {
        byte[] hash = SHA256.HashData(
            Encoding.UTF8.GetBytes(token));

        return Convert.ToBase64String(hash);
    }
}