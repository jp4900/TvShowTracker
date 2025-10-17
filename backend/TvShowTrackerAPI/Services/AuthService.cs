using Microsoft.EntityFrameworkCore;
using TvShowTrackerAPI.Data;
using TvShowTrackerAPI.DTOs.Auth;
using TvShowTrackerAPI.Models;
using BC = BCrypt.Net.BCrypt;

namespace TvShowTrackerAPI.Services;

/// <summary>
/// Authentication service implementation
/// Handles user registration, login, token refresh, and token management
/// </summary>
public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        AppDbContext context,
        ITokenService tokenService,
        ILogger<AuthService> logger)
    {
        _context = context;
        _tokenService = tokenService;
        _logger = logger;
    }

    /// <summary>
    /// Register new user
    /// </summary>
    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        // Check if user already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower());

        if (existingUser != null)
            throw new InvalidOperationException("User with this email already exists");

        // Create new user
        var user = new User
        {
            Email = dto.Email.ToLower(),
            PasswordHash = BC.HashPassword(dto.Password),
            DataProcessingConsent = dto.DataProcessingConsent,
            ConsentDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("New user registered: {Email}", user.Email);

        // Generate tokens and return response
        return await GenerateAuthResponse(user);
    }

    /// <summary>
    /// Login user
    /// </summary>
    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        // Get user by email
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower());

        // Verify password
        if (user == null || !BC.Verify(dto.Password, user.PasswordHash))
        {
            _logger.LogWarning("Failed login attempt for email: {Email}", dto.Email);
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Check if account is deleted
        if (user.IsDeleted)
            throw new UnauthorizedAccessException("Account has been deleted");

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("User logged in: {Email}", user.Email);

        // Generate tokens and return response
        return await GenerateAuthResponse(user);
    }

    /// <summary>
    /// Refresh access token
    /// </summary>
    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        // Find refresh token in database
        var token = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        // Validate token
        if (token == null)
            throw new UnauthorizedAccessException("Invalid refresh token");

        if (token.IsRevoked)
        {
            _logger.LogWarning("Attempt to use revoked token for user: {UserId}", token.UserId);
            throw new UnauthorizedAccessException("Token has been revoked");
        }

        if (token.ExpiresAt < DateTime.UtcNow)
        {
            _logger.LogWarning("Attempt to use expired token for user: {UserId}", token.UserId);
            throw new UnauthorizedAccessException("Token has expired");
        }

        // Revoke old token (token rotation for security)
        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;

        // Generate new tokens
        var newAccessToken = _tokenService.GenerateAccessToken(token.User.Id, token.User.Email);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        // Save new refresh token
        var newToken = new RefreshToken
        {
            UserId = token.UserId,
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        token.ReplacedByToken = newRefreshToken;
        _context.RefreshTokens.Add(newToken);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Token refreshed for user: {UserId}", token.UserId);

        return new AuthResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            User = new UserDto
            {
                Id = token.User.Id,
                Email = token.User.Email,

                LastLoginAt = token.User.LastLoginAt
            }
        };
    }

    /// <summary>
    /// Revoke refresh token (logout)
    /// </summary>
    public async Task RevokeTokenAsync(string refreshToken)
    {
        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (token == null)
            throw new InvalidOperationException("Invalid token");

        if (token.IsRevoked)
            return;

        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Token revoked for user: {UserId}", token.UserId);
    }

    /// <summary>
    /// Prune expired tokens (background job)
    /// </summary>
    public async Task PruneExpiredTokensAsync()
    {
        var expiredTokens = await _context.RefreshTokens
            .Where(rt => rt.ExpiresAt < DateTime.UtcNow || rt.IsRevoked)
            .ToListAsync();

        if (expiredTokens.Any())
        {
            _context.RefreshTokens.RemoveRange(expiredTokens);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Pruned {Count} expired/revoked tokens", expiredTokens.Count);
        }
    }

    /// <summary>
    /// Helper method to generate authentication response
    /// </summary>
    private async Task<AuthResponseDto> GenerateAuthResponse(User user)
    {
        // Generate tokens
        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Save refresh token to database
        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7), // TTL: 7 days
            CreatedAt = DateTime.UtcNow
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(1), // TTL: 1 hour
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                LastLoginAt = user.LastLoginAt
            }
        };
    }
}